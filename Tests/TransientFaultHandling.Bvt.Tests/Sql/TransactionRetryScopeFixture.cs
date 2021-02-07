namespace Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling.Bvt.Tests.Sql
{
    using System;
    using System.Data;
    using Microsoft.Data.SqlClient;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling.Bvt.Tests.TestObjects;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class TransactionRetryScopeFixture
    {
        private static readonly string ConnectionString = RetryConfiguration.GetConfiguration().GetConnectionString("TestDatabase");
        private RetryManager retryManager;

        [TestInitialize]
        public void Initialize()
        {
            this.retryManager = RetryConfiguration.GetRetryManager(getCustomRetryStrategy: section =>
                section.Get<TestRetryStrategyOptions>().ToTestRetryStrategy(section.Key));
        }

        [TestCleanup]
        public void Cleanup()
        {
        }

        [TestMethod]
        public void TransactionIsCommittedWhenSomeRetriesFailAndThenSucceeds()
        {
            this.DeleteAllOnTransactionScopeTestTable();

            int retryTransactionCount = 0;
            RetryPolicy<FakeSqlAzureTransientErrorDetectionStrategy> policyForTransaction = this.retryManager.GetRetryPolicy<FakeSqlAzureTransientErrorDetectionStrategy>("Retry 5 times");
            policyForTransaction.Retrying += (sender, args) => retryTransactionCount++;

            int retrySqlCommandCount = 0;
            RetryPolicy<FakeSqlAzureTransientErrorDetectionStrategy> policyForSqlCommand = this.retryManager.GetRetryPolicy<FakeSqlAzureTransientErrorDetectionStrategy>("Retry 2 times, first retry is fast");
            policyForSqlCommand.Retrying += (sender, args) => retrySqlCommandCount++;

            int transactionActionExecutedCount = 0;
            Action action = () =>
            {
                transactionActionExecutedCount++;

                using SqlConnection connection = new(ConnectionString);
                using SqlCommand command1 = connection.CreateCommand();
                command1.CommandType = CommandType.Text;
                command1.CommandText = "Insert Into TranscationScopeTestTable (rowId) Values (@rowId);";
                command1.Connection = connection;
                command1.Parameters.Add(new SqlParameter("rowId", SqlDbType.UniqueIdentifier) { Value = Guid.NewGuid() });
                command1.ExecuteNonQueryWithRetry(policyForSqlCommand);

                if (retryTransactionCount < 4)
                {
                    using SqlCommand command2 = connection.CreateCommand();
                    command2.CommandType = CommandType.StoredProcedure;
                    command2.CommandText = "ErrorRaisingForExecuteNonQuery";
                    command2.Connection = connection;
                    command2.Parameters.Add(new SqlParameter("rowId", SqlDbType.UniqueIdentifier) { Value = Guid.NewGuid() });
                    command2.Parameters.Add(new SqlParameter("maxCountToRaiseErrors", SqlDbType.Int) { Value = 10 });
                    command2.Parameters.Add(new SqlParameter("error", SqlDbType.Int) { Value = 60000 });
                    command2.ExecuteNonQueryWithRetry(policyForSqlCommand);
                }
            };

            using (TransactionRetryScope scope = new(policyForTransaction, action))
            {
                try
                {
                    scope.InvokeUnitOfWork();
                    scope.Complete();
                }
                catch (Exception)
                {
                    Assert.Fail("Should not throw");
                }
            }

            Assert.AreEqual(1, this.GetCountOnTransactionScopeTestTable());
            Assert.AreEqual(5, transactionActionExecutedCount);
            Assert.AreEqual(4, retryTransactionCount);
            Assert.AreEqual(8, retrySqlCommandCount);
        }

        [TestMethod]
        public void TransactionIsRolledBackAndExceptionIsThrownWhenAllRetriesFail()
        {
            this.DeleteAllOnTransactionScopeTestTable();

            int retryTransactionCount = 0;
            RetryPolicy<FakeSqlAzureTransientErrorDetectionStrategy> policyForTransaction = this.retryManager.GetRetryPolicy<FakeSqlAzureTransientErrorDetectionStrategy>("Retry 5 times");
            policyForTransaction.Retrying += (sender, args) => retryTransactionCount++;

            int retrySqlCommandCount = 0;
            RetryPolicy<FakeSqlAzureTransientErrorDetectionStrategy> policyForSqlCommand = this.retryManager.GetRetryPolicy<FakeSqlAzureTransientErrorDetectionStrategy>("Retry 2 times, first retry is fast");
            policyForSqlCommand.Retrying += (sender, args) => retrySqlCommandCount++;

            int transactionActionExecutedCount = 0;
            Action action = () =>
            {
                transactionActionExecutedCount++;

                using SqlConnection connection = new(ConnectionString);
                using SqlCommand command1 = connection.CreateCommand();
                command1.CommandType = CommandType.Text;
                command1.CommandText = "Insert Into TranscationScopeTestTable (rowId) Values (@rowId);";
                command1.Connection = connection;
                command1.Parameters.Add(new SqlParameter("rowId", SqlDbType.UniqueIdentifier) { Value = Guid.NewGuid() });
                command1.ExecuteNonQueryWithRetry(policyForSqlCommand);

                using SqlCommand command2 = connection.CreateCommand();
                command2.CommandType = CommandType.StoredProcedure;
                command2.CommandText = "ErrorRaisingForExecuteNonQuery";
                command2.Connection = connection;
                command2.Parameters.Add(new SqlParameter("rowId", SqlDbType.UniqueIdentifier) { Value = Guid.NewGuid() });
                command2.Parameters.Add(new SqlParameter("maxCountToRaiseErrors", SqlDbType.Int) { Value = 10 });
                command2.Parameters.Add(new SqlParameter("error", SqlDbType.Int) { Value = 60000 });
                command2.ExecuteNonQueryWithRetry(policyForSqlCommand);
            };

            using (TransactionRetryScope scope = new(policyForTransaction, action))
            {
                try
                {
                    scope.InvokeUnitOfWork();
                    scope.Complete();

                    Assert.Fail("Should have thrown SqlException");
                }
                catch (SqlException)
                { }
                catch (Exception)
                {
                    Assert.Fail("Should have thrown SqlException");
                }
            }

            Assert.AreEqual(0, this.GetCountOnTransactionScopeTestTable());
            Assert.AreEqual(6, transactionActionExecutedCount);
            Assert.AreEqual(5, retryTransactionCount);
            Assert.AreEqual(12, retrySqlCommandCount);
        }

        private int GetCountOnTransactionScopeTestTable()
        {
            using SqlConnection connection = new(ConnectionString);
            connection.Open();
            using SqlCommand command = connection.CreateCommand();
            command.CommandText = "Select Count(*) From TranscationScopeTestTable";
            return (int)command.ExecuteScalar();
        }

        private void DeleteAllOnTransactionScopeTestTable()
        {
            using SqlConnection connection = new(ConnectionString);
            connection.Open();
            using SqlCommand command = connection.CreateCommand();
            command.CommandText = "Delete From TranscationScopeTestTable";
            command.ExecuteNonQuery();
        }
    }
}
