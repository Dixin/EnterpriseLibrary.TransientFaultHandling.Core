namespace Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling.Bvt.Tests.Sql
{
    using System;
    using System.Data;
    using System.Xml;
    using Microsoft.Data.SqlClient;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling.Bvt.Tests.TestObjects;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ReliableSqlConnectionFixture
    {
        private static readonly string ConnectionString = RetryConfiguration.GetConfiguration().GetConnectionString("TestDatabase");
        
        private RetryManager retryManager;

        [TestInitialize]
        public void Initialize()
        {
            this.retryManager = RetryConfiguration.GetRetryManager(getCustomRetryStrategy: section =>
                section.Get<TestRetryStrategyOptions>().ToTestRetryStrategy(section.Key));
            RetryManager.SetDefault(this.retryManager, false);
        }

        [TestCleanup]
        public void Cleanup()
        {
        }

        [TestMethod]
        public void OpensConnectionWithRetryPolicy()
        {
            using ReliableSqlConnection reliableConnection = new(ConnectionString);

            try
            {
                RetryPolicy<SqlDatabaseTransientErrorDetectionStrategy> retryPolicy = this.retryManager.GetRetryPolicy<SqlDatabaseTransientErrorDetectionStrategy>("Retry 5 times");

                retryPolicy.ExecuteAction(() => reliableConnection.Open(retryPolicy));
            }
            catch (SqlException)
            { }
            catch (Exception)
            {
                Assert.Fail();
            }
        }

        [TestMethod]
        public void ExecutesNonQuerySqlCommandWithConnectionRetryPolicyAndSqlCommandRetryPolicy()
        {
            using ReliableSqlConnection reliableConnection = new(ConnectionString);
            int count = 0;
            try
            {
                RetryPolicy<SqlDatabaseTransientErrorDetectionStrategy> retryPolicy = this.retryManager.GetRetryPolicy<SqlDatabaseTransientErrorDetectionStrategy>("Retry 5 times");

                retryPolicy.ExecuteAction(() =>
                {
                    SqlCommand command = new("SELECT 1", reliableConnection.Current);
                    count = command.ExecuteNonQueryWithRetry(retryPolicy, retryPolicy);
                });

                Assert.AreEqual(-1, count);
            }
            catch (SqlException)
            { }
            catch (Exception)
            {
                Assert.Fail();
            }
        }

        [TestMethod]
        public void ExecutesReaderWithRetryPolicy()
        {
            using ReliableSqlConnection reliableConnection = new(ConnectionString);

            try
            {
                RetryPolicy<SqlDatabaseTransientErrorDetectionStrategy> retryPolicy = this.retryManager.GetRetryPolicy<SqlDatabaseTransientErrorDetectionStrategy>("Retry 5 times");

                retryPolicy.ExecuteAction(() =>
                {
                    SqlCommand command = new("SELECT 1", reliableConnection.Current);
                    command.ExecuteReaderWithRetry(retryPolicy);
                });
            }
            catch (SqlException)
            { }
            catch (Exception)
            {
                Assert.Fail();
            }
        }

        [TestMethod]
        public void ExecutesXmlReaderWithRetryPolicy()
        {
            using ReliableSqlConnection reliableConnection = new(ConnectionString);

            XmlReader reader;
            int count = 0;
            try
            {
                RetryPolicy<SqlDatabaseTransientErrorDetectionStrategy> retryPolicy = this.retryManager.GetRetryPolicy<SqlDatabaseTransientErrorDetectionStrategy>("Retry 5 times");

                retryPolicy.ExecuteAction(() =>
                {
                    SqlCommand command = new("SELECT 1 FOR XML AUTO", reliableConnection.Current);
                    reader = command.ExecuteXmlReaderWithRetry(retryPolicy);

                    while (reader.Read())
                    {
                        reader.MoveToFirstAttribute();
                        reader.ReadAttributeValue();
                        count = reader.ReadContentAsInt();
                    }
                });

                Assert.AreEqual(1, count);
            }
            catch (SqlException)
            { }
            catch (Exception)
            {
                Assert.Fail();
            }
        }

        [TestMethod]
        public void ExecutesSqlCommand()
        {
            using ReliableSqlConnection reliableConnection = new(ConnectionString);

            int count = 0;
            try
            {
                RetryPolicy<SqlDatabaseTransientErrorDetectionStrategy> retryPolicy = this.retryManager.GetRetryPolicy<SqlDatabaseTransientErrorDetectionStrategy>("Retry 5 times");

                retryPolicy.ExecuteAction(() =>
                {
                    SqlCommand command = new("SELECT 1");
                    count = reliableConnection.ExecuteCommand(command);
                });

                Assert.AreEqual(-1, count);
            }
            catch (SqlException)
            { }
            catch (Exception)
            {
                Assert.Fail();
            }
        }

        [TestMethod]
        public void RetriesToExecuteActionWhenSqlExceptionDuringCommandExecution()
        {
            using ReliableSqlConnection reliableConnection = new(ConnectionString);

            int count = 0;
            try
            {
                RetryPolicy<SqlDatabaseTransientErrorDetectionStrategy> retryPolicy = this.retryManager.GetRetryPolicy<SqlDatabaseTransientErrorDetectionStrategy>("Retry 5 times");

                retryPolicy.ExecuteAction(() =>
                {
                    SqlCommand command = new("FAIL");
                    count = reliableConnection.ExecuteCommand(command);
                });

                Assert.AreEqual(-1, count);
            }
            catch (SqlException)
            {
                Assert.AreEqual<ConnectionState>(ConnectionState.Closed, reliableConnection.Current.State);
            }
            catch (Exception)
            {
                Assert.Fail();
            }
        }

        [TestMethod]
        public void ExecutesCommandWithRetryPolicyWhenSomeRetriesFailAndThenSucceeds()
        {
            using ReliableSqlConnection reliableConnection = new(ConnectionString);

            RetryPolicy<FakeSqlAzureTransientErrorDetectionStrategy> policy = this.retryManager.GetRetryPolicy<FakeSqlAzureTransientErrorDetectionStrategy>("Retry 5 times");
            int count = 0;
            policy.Retrying += (s, args) => count = args.CurrentRetryCount;

            SqlCommand command = new();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "ErrorRaisingReader";
            command.Parameters.Add(new SqlParameter("rowId", SqlDbType.UniqueIdentifier) { Value = Guid.NewGuid() });
            command.Parameters.Add(new SqlParameter("maxCountToRaiseErrors", SqlDbType.Int) { Value = 4 });
            command.Parameters.Add(new SqlParameter("error", SqlDbType.Int) { Value = 60000 });
            reliableConnection.Open();
            int rowCount = reliableConnection.ExecuteCommand(command, policy);
            reliableConnection.Close();

            Assert.AreEqual<int>(3, count);
            Assert.AreEqual(1, rowCount);
        }

        [TestMethod]
        [ExpectedException(typeof(SqlException))]
        public void ThrowsExceptionWhenAllRetriesFailAndCommandExecutedWithRetryPolicy()
        {
            using ReliableSqlConnection reliableConnection = new(ConnectionString);

            RetryPolicy<FakeSqlAzureTransientErrorDetectionStrategy> policy = this.retryManager.GetRetryPolicy<FakeSqlAzureTransientErrorDetectionStrategy>("Retry 5 times");
            int count = 0;
            policy.Retrying += (s, args) => count = args.CurrentRetryCount;

            int rowCount = 0;
            try
            {
                SqlCommand command = new();
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "ErrorRaisingReader";
                command.Parameters.Add(new SqlParameter("rowId", SqlDbType.UniqueIdentifier) { Value = Guid.NewGuid() });
                command.Parameters.Add(new SqlParameter("maxCountToRaiseErrors", SqlDbType.Int) { Value = 7 });
                command.Parameters.Add(new SqlParameter("error", SqlDbType.Int) { Value = 60000 });
                reliableConnection.Open();
                rowCount = reliableConnection.ExecuteCommand(command, policy);
            }
            catch (Exception)
            {
                reliableConnection.Close();
                Assert.AreEqual<int>(5, count);
                Assert.AreEqual(0, rowCount);
                throw;
            }

            Assert.Fail("test should throw");
        }

        [TestMethod]
        public void ExecutesCommandWithoutRetryPolicyWhenSomeRetriesFailAndThenSucceeds()
        {
            RetryPolicy<FakeSqlAzureTransientErrorDetectionStrategy> policy = this.retryManager.GetRetryPolicy<FakeSqlAzureTransientErrorDetectionStrategy>("Retry 5 times");
            using ReliableSqlConnection reliableConnection = new(ConnectionString, policy, policy);

            int count = 0;
            policy.Retrying += (s, args) => count = args.CurrentRetryCount;

            SqlCommand command = new();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "ErrorRaisingReader";
            command.Parameters.Add(new SqlParameter("rowId", SqlDbType.UniqueIdentifier) { Value = Guid.NewGuid() });
            command.Parameters.Add(new SqlParameter("maxCountToRaiseErrors", SqlDbType.Int) { Value = 4 });
            command.Parameters.Add(new SqlParameter("error", SqlDbType.Int) { Value = 60000 });
            reliableConnection.Open();
            int rowCount = reliableConnection.ExecuteCommand(command);
            reliableConnection.Close();

            Assert.AreEqual<int>(3, count);
            Assert.AreEqual(1, rowCount);
        }

        [TestMethod]
        [ExpectedException(typeof(SqlException))]
        public void ThrowsExceptionWhenAllRetriesFailAndCommandExecutedWithoutRetryPolicy()
        {
            RetryPolicy<FakeSqlAzureTransientErrorDetectionStrategy> policy = this.retryManager.GetRetryPolicy<FakeSqlAzureTransientErrorDetectionStrategy>("Retry 5 times");
            using ReliableSqlConnection reliableConnection = new(ConnectionString, policy, policy);

            int count = 0;
            policy.Retrying += (s, args) => count = args.CurrentRetryCount;

            int rowCount = 0;
            try
            {
                SqlCommand command = new();
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "ErrorRaisingReader";
                command.Parameters.Add(new SqlParameter("rowId", SqlDbType.UniqueIdentifier) { Value = Guid.NewGuid() });
                command.Parameters.Add(new SqlParameter("maxCountToRaiseErrors", SqlDbType.Int) { Value = 7 });
                command.Parameters.Add(new SqlParameter("error", SqlDbType.Int) { Value = 60000 });
                reliableConnection.Open();
                rowCount = reliableConnection.ExecuteCommand(command);
            }
            catch (Exception)
            {
                reliableConnection.Close();
                Assert.AreEqual<int>(5, count);
                Assert.AreEqual(0, rowCount);
                throw;
            }

            Assert.Fail("test should throw");
        }
    }
}
