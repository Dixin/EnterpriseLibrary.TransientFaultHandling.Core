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
    public class SqlCommandExtensionsFixture
    {
        private static readonly string connectionString = RetryConfiguration.GetConfiguration().GetConnectionString("TestDatabase");
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
        [ExpectedException(typeof(SqlException))]
        public void ThrowsExceptionWhenAllRetriesFailDuringReaderExecutionWithRetryPolicy()
        {
            using SqlConnection connection = new(connectionString);
            RetryPolicy<FakeSqlAzureTransientErrorDetectionStrategy> policy = this.retryManager.GetRetryPolicy<FakeSqlAzureTransientErrorDetectionStrategy>("Retry 5 times");
            connection.Open();
            SqlCommand command = new();
            int count = 0;
            policy.Retrying += (sender, args) => count = args.CurrentRetryCount;

            try
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "ErrorRaisingReader";
                command.Connection = connection;
                command.Parameters.Add(new SqlParameter("rowId", SqlDbType.UniqueIdentifier) { Value = Guid.NewGuid() });
                command.Parameters.Add(new SqlParameter("maxCountToRaiseErrors", SqlDbType.Int) { Value = 7 });
                command.Parameters.Add(new SqlParameter("error", SqlDbType.Int) { Value = 60000 });
                using SqlDataReader reader = command.ExecuteReaderWithRetry(policy);
                while (reader.Read())
                {
                }
            }
            catch (Exception)
            {
                Assert.AreEqual(5, count);
                connection.Close();
                Assert.AreEqual(ConnectionState.Closed, command.Connection.State);
                throw;
            }

            Assert.Fail("Test should throw");
        }

        [TestMethod]
        public void ExecutesReaderWithRetryPolicyWhenSomeRetriesFailAndThenSucceeds()
        {
            using SqlConnection connection = new(connectionString);
            RetryPolicy<FakeSqlAzureTransientErrorDetectionStrategy> policy = this.retryManager.GetRetryPolicy<FakeSqlAzureTransientErrorDetectionStrategy>("Retry 5 times");
            connection.Open();
            SqlCommand command = new();
            int count = 0;
            int rowCount = 0;
            policy.Retrying += (sender, args) => count = args.CurrentRetryCount;

            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "ErrorRaisingReader";
            command.Connection = connection;
            command.Parameters.Add(new SqlParameter("rowId", SqlDbType.UniqueIdentifier) { Value = Guid.NewGuid() });
            command.Parameters.Add(new SqlParameter("maxCountToRaiseErrors", SqlDbType.Int) { Value = 4 });
            command.Parameters.Add(new SqlParameter("error", SqlDbType.Int) { Value = 60000 });
            using SqlDataReader reader = command.ExecuteReaderWithRetry(policy);
            while (reader.Read())
            {
                rowCount++;
            }

            Assert.AreEqual(3, count);
            connection.Close();
            Assert.AreEqual(ConnectionState.Closed, command.Connection.State);
            Assert.AreEqual(10, rowCount);
        }

        [TestMethod]
        [ExpectedException(typeof(SqlException))]
        public void ThrowsExceptionWhenAllRetriesFailDuringReaderExecutionWithRetryPolicyAndConnectionRetryPolicy()
        {
            using SqlConnection connection = new(connectionString);
            RetryPolicy policy = RetryPolicyFactory.GetRetryPolicy<FakeSqlAzureTransientErrorDetectionStrategy>(
                retryStrategyName: "Retry 5 times",
                getCustomRetryStrategy: section => section.Get<TestRetryStrategyOptions>().ToTestRetryStrategy(section.Key));
            connection.Open();
            using SqlCommand command = new();
            int count = 0;
            policy.Retrying += (sender, args) => count = args.CurrentRetryCount;

            try
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "ErrorRaisingReader";
                command.Connection = connection;
                command.Parameters.Add(new SqlParameter("rowId", SqlDbType.UniqueIdentifier) { Value = Guid.NewGuid() });
                command.Parameters.Add(new SqlParameter("maxCountToRaiseErrors", SqlDbType.Int) { Value = 7 });
                command.Parameters.Add(new SqlParameter("error", SqlDbType.Int) { Value = 60000 });
                using SqlDataReader reader = command.ExecuteReaderWithRetry(policy, policy);
                while (reader.Read())
                {
                }
            }
            catch (Exception)
            {
                Assert.AreEqual(5, count);
                connection.Close();
                Assert.AreEqual(ConnectionState.Closed, command.Connection.State);
                throw;
            }

            Assert.Fail("Test should throw");
        }

        [TestMethod]
        public void ExecutesReaderWithRetryPolicyAndConnectionRetryPolicyWhenSomeRetriesFailAndThenSucceeds()
        {
            using SqlConnection connection = new(connectionString);
            RetryPolicy<FakeSqlAzureTransientErrorDetectionStrategy> policy = this.retryManager.GetRetryPolicy<FakeSqlAzureTransientErrorDetectionStrategy>("Retry 5 times");
            connection.Open();
            using SqlCommand command = new();
            int count = 0;
            int rowCount = 0;
            policy.Retrying += (sender, args) => count = args.CurrentRetryCount;

            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "ErrorRaisingReader";
            command.Connection = connection;
            command.Parameters.Add(new SqlParameter("rowId", SqlDbType.UniqueIdentifier) { Value = Guid.NewGuid() });
            command.Parameters.Add(new SqlParameter("maxCountToRaiseErrors", SqlDbType.Int) { Value = 3 });
            command.Parameters.Add(new SqlParameter("error", SqlDbType.Int) { Value = 60000 });
            using SqlDataReader reader = command.ExecuteReaderWithRetry(policy, policy);
            while (reader.Read())
            {
                rowCount++;
            }

            Assert.AreEqual(2, count);
            connection.Close();
            Assert.AreEqual(ConnectionState.Closed, command.Connection.State);
            Assert.AreEqual(10, rowCount);
        }

        [TestMethod]
        [ExpectedException(typeof(SqlException))]
        public void ThrowsExceptionWhenAllRetriesFailDuringReaderExecutionWithRetryPolicyAndSqlCommandBehavior()
        {
            using SqlConnection connection = new(connectionString);
            RetryPolicy<FakeSqlAzureTransientErrorDetectionStrategy> policy = this.retryManager.GetRetryPolicy<FakeSqlAzureTransientErrorDetectionStrategy>("Retry 5 times");
            connection.Open();
            using SqlCommand command = new();
            int count = 0;
            policy.Retrying += (sender, args) => count = args.CurrentRetryCount;

            try
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "ErrorRaisingReader";
                command.Connection = connection;
                command.Parameters.Add(new SqlParameter("rowId", SqlDbType.UniqueIdentifier) { Value = Guid.NewGuid() });
                command.Parameters.Add(new SqlParameter("maxCountToRaiseErrors", SqlDbType.Int) { Value = 7 });
                command.Parameters.Add(new SqlParameter("error", SqlDbType.Int) { Value = 60000 });
                using SqlDataReader reader = command.ExecuteReaderWithRetry(CommandBehavior.CloseConnection, policy);
                while (reader.Read())
                {
                }
            }
            catch (Exception)
            {
                Assert.AreEqual(5, count);
                Assert.AreEqual(ConnectionState.Closed, command.Connection.State);
                throw;
            }

            Assert.Fail("Test should throw");
        }

        [TestMethod]
        public void ExecutesReaderWithRetryPolicyAndSqlCommandBehaviorWhenSomeRetriesFailAndThenSucceeds()
        {
            using SqlConnection connection = new(connectionString);
            RetryPolicy<FakeSqlAzureTransientErrorDetectionStrategy> policy = this.retryManager.GetRetryPolicy<FakeSqlAzureTransientErrorDetectionStrategy>("Retry 5 times");
            connection.Open();
            using SqlCommand command = new();
            int count = 0;
            policy.Retrying += (sender, args) => count = args.CurrentRetryCount;

            int rowCount = 0;
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "ErrorRaisingReader";
            command.Connection = connection;
            command.Parameters.Add(new SqlParameter("rowId", SqlDbType.UniqueIdentifier) { Value = Guid.NewGuid() });
            command.Parameters.Add(new SqlParameter("maxCountToRaiseErrors", SqlDbType.Int) { Value = 4 });
            command.Parameters.Add(new SqlParameter("error", SqlDbType.Int) { Value = 60000 });
            using SqlDataReader reader = command.ExecuteReaderWithRetry(CommandBehavior.CloseConnection, policy);
            while (reader.Read())
            {
                rowCount++;
            }

            reader.Close();
            Assert.AreEqual(3, count);
            Assert.AreEqual(ConnectionState.Closed, command.Connection.State);
            Assert.AreEqual(10, rowCount);
        }

        [TestMethod]
        [ExpectedException(typeof(SqlException))]
        public void ThrowsExceptionWhenAllRetriesFailDuringReaderExecutionWithRetryPolicyAndSqlCommandBehaviorAndConnectionRetryPolicy()
        {
            using SqlConnection connection = new(connectionString);
            RetryPolicy<FakeSqlAzureTransientErrorDetectionStrategy> policy = this.retryManager.GetRetryPolicy<FakeSqlAzureTransientErrorDetectionStrategy>("Retry 5 times");
            connection.Open();
            using SqlCommand command = new();
            int count = 0;
            policy.Retrying += (sender, args) => count = args.CurrentRetryCount;

            try
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "ErrorRaisingReader";
                command.Connection = connection;
                command.Parameters.Add(new SqlParameter("rowId", SqlDbType.UniqueIdentifier) { Value = Guid.NewGuid() });
                command.Parameters.Add(new SqlParameter("maxCountToRaiseErrors", SqlDbType.Int) { Value = 7 });
                command.Parameters.Add(new SqlParameter("error", SqlDbType.Int) { Value = 60000 });
                using SqlDataReader reader = command.ExecuteReaderWithRetry(CommandBehavior.CloseConnection, policy, policy);
                while (reader.Read())
                {
                }
            }
            catch (Exception)
            {
                Assert.AreEqual(5, count);
                Assert.AreEqual(ConnectionState.Closed, command.Connection.State);
                throw;
            }

            Assert.Fail("Test should throw");
        }

        [TestMethod]
        public void ExecutesReaderWithRetryPolicyAndSqlCommandBehaviorAndConnectionRetryPolicyWhenSomeRetriesFailAndThenSucceeds()
        {
            using SqlConnection connection = new(connectionString);
            RetryPolicy<FakeSqlAzureTransientErrorDetectionStrategy> policy = this.retryManager.GetRetryPolicy<FakeSqlAzureTransientErrorDetectionStrategy>("Retry 5 times");
            connection.Open();
            using SqlCommand command = new();
            int count = 0;
            int rowCount = 0;
            policy.Retrying += (sender, args) => count = args.CurrentRetryCount;

            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "ErrorRaisingReader";
            command.Connection = connection;
            command.Parameters.Add(new SqlParameter("rowId", SqlDbType.UniqueIdentifier) { Value = Guid.NewGuid() });
            command.Parameters.Add(new SqlParameter("maxCountToRaiseErrors", SqlDbType.Int) { Value = 3 });
            command.Parameters.Add(new SqlParameter("error", SqlDbType.Int) { Value = 60000 });
            using SqlDataReader reader = command.ExecuteReaderWithRetry(CommandBehavior.CloseConnection, policy, policy);
            while (reader.Read())
            {
                rowCount++;
            }

            reader.Close();
            Assert.AreEqual(2, count);
            Assert.AreEqual(ConnectionState.Closed, command.Connection.State);
            Assert.AreEqual(10, rowCount);
        }

        [TestMethod]
        public void ExecutesNonQueryWithRetryPolicyWhenSomeRetriesFailAndThenSucceeds()
        {
            using SqlConnection connection = new(connectionString);
            RetryPolicy<FakeSqlAzureTransientErrorDetectionStrategy> policy = this.retryManager.GetRetryPolicy<FakeSqlAzureTransientErrorDetectionStrategy>("Retry 5 times");
            connection.Open();
            using SqlCommand command = new();
            int count = 0;
            int rowCount = 0;
            policy.Retrying += (sender, args) => count = args.CurrentRetryCount;

            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "ErrorRaisingForExecuteNonQuery";
            command.Connection = connection;
            command.Parameters.Add(new SqlParameter("rowId", SqlDbType.UniqueIdentifier) { Value = Guid.NewGuid() });
            command.Parameters.Add(new SqlParameter("maxCountToRaiseErrors", SqlDbType.Int) { Value = 4 });
            command.Parameters.Add(new SqlParameter("error", SqlDbType.Int) { Value = 60000 });

            rowCount = command.ExecuteNonQueryWithRetry(policy);

            connection.Close();

            Assert.AreEqual(1, rowCount);
            Assert.AreEqual(3, count);
            connection.Close();
            Assert.AreEqual(ConnectionState.Closed, command.Connection.State);
        }

        [TestMethod]
        [ExpectedException(typeof(SqlException))]
        public void ThrowsExceptionWhenAllRetriesFailDuringNonQueryExecutionWithRetryPolicy()
        {
            using SqlConnection connection = new(connectionString);
            RetryPolicy<FakeSqlAzureTransientErrorDetectionStrategy> policy = this.retryManager.GetRetryPolicy<FakeSqlAzureTransientErrorDetectionStrategy>("Retry 5 times");
            connection.Open();
            using SqlCommand command = new();
            int count = 0;
            int rowCount = 0;
            policy.Retrying += (sender, args) => count = args.CurrentRetryCount;

            try
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "ErrorRaisingForExecuteNonQuery";
                command.Connection = connection;
                command.Parameters.Add(new SqlParameter("rowId", SqlDbType.UniqueIdentifier) { Value = Guid.NewGuid() });
                command.Parameters.Add(new SqlParameter("maxCountToRaiseErrors", SqlDbType.Int) { Value = 7 });
                command.Parameters.Add(new SqlParameter("error", SqlDbType.Int) { Value = 60000 });

                rowCount = command.ExecuteNonQueryWithRetry(policy);

                connection.Close();

                Assert.AreEqual(1, rowCount);
            }
            catch (Exception)
            {
                Assert.AreEqual(5, count);

                connection.Close();
                Assert.AreEqual(ConnectionState.Closed, command.Connection.State);
                throw;
            }

            Assert.Fail("Test should throw");
        }

        [TestMethod]
        [ExpectedException(typeof(SqlException))]
        public void ThrowsExceptionWhenAllRetriesFailDuringScalarExecutionWithRetryPolicy()
        {
            using SqlConnection connection = new(connectionString);
            RetryPolicy<FakeSqlAzureTransientErrorDetectionStrategy> policy = this.retryManager.GetRetryPolicy<FakeSqlAzureTransientErrorDetectionStrategy>("Retry 5 times");
            connection.Open();
            using SqlCommand command = new();
            int count = 0;
            int rowCount = 0;
            policy.Retrying += (sender, args) => count = args.CurrentRetryCount;

            try
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "ErrorRaisingScalar";
                command.Connection = connection;
                command.Parameters.Add(new SqlParameter("rowId", SqlDbType.UniqueIdentifier) { Value = Guid.NewGuid() });
                command.Parameters.Add(new SqlParameter("maxCountToRaiseErrors", SqlDbType.Int) { Value = 7 });
                command.Parameters.Add(new SqlParameter("error", SqlDbType.Int) { Value = 60000 });
                rowCount = (int)command.ExecuteScalarWithRetry(policy);
            }
            catch (Exception)
            {
                Assert.AreEqual(5, count);
                connection.Close();
                Assert.AreEqual(ConnectionState.Closed, command.Connection.State);
                throw;
            }

            Assert.Fail("Test should throw");
        }

        [TestMethod]
        public void ExecutesScalarWithRetryPolicyWhenSomeRetriesFailAndThenSucceeds()
        {
            using SqlConnection connection = new(connectionString);
            RetryPolicy<FakeSqlAzureTransientErrorDetectionStrategy> policy = this.retryManager.GetRetryPolicy<FakeSqlAzureTransientErrorDetectionStrategy>("Retry 5 times");
            connection.Open();
            using SqlCommand command = new();
            int count = 0;
            int totalRowCount = 0;
            policy.Retrying += (sender, args) => count = args.CurrentRetryCount;

            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "ErrorRaisingScalar";
            command.Connection = connection;
            command.Parameters.Add(new SqlParameter("rowId", SqlDbType.UniqueIdentifier) { Value = Guid.NewGuid() });
            command.Parameters.Add(new SqlParameter("maxCountToRaiseErrors", SqlDbType.Int) { Value = 4 });
            command.Parameters.Add(new SqlParameter("error", SqlDbType.Int) { Value = 60000 });
            totalRowCount = (int)command.ExecuteScalarWithRetry(policy);

            Assert.AreEqual(3, count);
            connection.Close();
            Assert.AreEqual(ConnectionState.Closed, command.Connection.State);
            Assert.AreEqual(10, totalRowCount);
        }

        [TestMethod]
        [ExpectedException(typeof(SqlException))]
        public void ThrowsExceptionWhenAllRetriesFailDuringScalarExecutionWithRetryPolicyAndConnectionRetryPolicy()
        {
            using SqlConnection connection = new(connectionString);
            RetryPolicy<FakeSqlAzureTransientErrorDetectionStrategy> policy = this.retryManager.GetRetryPolicy<FakeSqlAzureTransientErrorDetectionStrategy>("Retry 5 times");
            connection.Open();
            using SqlCommand command = new();
            int count = 0;
            int rowCount = 0;
            policy.Retrying += (sender, args) => count = args.CurrentRetryCount;

            try
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "ErrorRaisingScalar";
                command.Connection = connection;
                command.Parameters.Add(new SqlParameter("rowId", SqlDbType.UniqueIdentifier) { Value = Guid.NewGuid() });
                command.Parameters.Add(new SqlParameter("maxCountToRaiseErrors", SqlDbType.Int) { Value = 7 });
                command.Parameters.Add(new SqlParameter("error", SqlDbType.Int) { Value = 60000 });
                rowCount = (int)command.ExecuteScalarWithRetry(policy, policy);
            }
            catch (Exception)
            {
                Assert.AreEqual(5, count);
                connection.Close();
                Assert.AreEqual(ConnectionState.Closed, command.Connection.State);
                throw;
            }

            Assert.Fail("Test should throw");
        }

        [TestMethod]
        public void ExecutesScalarWithRetryPolicyAndConnectionRetryPolicyWhenSomeRetriesFailAndThenSucceeds()
        {
            using SqlConnection connection = new(connectionString);
            RetryPolicy<FakeSqlAzureTransientErrorDetectionStrategy> policy = this.retryManager.GetRetryPolicy<FakeSqlAzureTransientErrorDetectionStrategy>("Retry 5 times");
            connection.Open();
            using SqlCommand command = new();
            int count = 0;
            int rowCount = 0;
            policy.Retrying += (sender, args) => count = args.CurrentRetryCount;

            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "ErrorRaisingScalar";
            command.Connection = connection;
            command.Parameters.Add(new SqlParameter("rowId", SqlDbType.UniqueIdentifier) { Value = Guid.NewGuid() });
            command.Parameters.Add(new SqlParameter("maxCountToRaiseErrors", SqlDbType.Int) { Value = 2 });
            command.Parameters.Add(new SqlParameter("error", SqlDbType.Int) { Value = 60000 });
            rowCount = (int)command.ExecuteScalarWithRetry(policy, policy);
            Assert.AreEqual(1, count);

            connection.Close();
            Assert.AreEqual(ConnectionState.Closed, command.Connection.State);
            Assert.AreEqual(10, rowCount);
        }

        [TestMethod]
        [ExpectedException(typeof(SqlException))]
        public void ThrowsExceptionWhenAllRetriesFailDuringXmlReaderExecutionWithRetryPolicy()
        {
            using SqlConnection connection = new(connectionString);
            RetryPolicy<FakeSqlAzureTransientErrorDetectionStrategy> policy = this.retryManager.GetRetryPolicy<FakeSqlAzureTransientErrorDetectionStrategy>("Retry 5 times");
            connection.Open();
            using SqlCommand command = new();
            int count = 0;
            policy.Retrying += (sender, args) => count = args.CurrentRetryCount;

            try
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "ErrorRaisingXMLReader";
                command.Connection = connection;
                command.Parameters.Add(new SqlParameter("rowId", SqlDbType.UniqueIdentifier) { Value = Guid.NewGuid() });
                command.Parameters.Add(new SqlParameter("maxCountToRaiseErrors", SqlDbType.Int) { Value = 7 });
                command.Parameters.Add(new SqlParameter("error", SqlDbType.Int) { Value = 60000 });
                using XmlReader reader = command.ExecuteXmlReaderWithRetry(policy);
                while (reader.Read())
                {
                }
            }
            catch (Exception)
            {
                Assert.AreEqual(5, count);

                connection.Close();
                Assert.AreEqual(ConnectionState.Closed, command.Connection.State);
                throw;
            }

            Assert.Fail("Test should throw");
        }

        [TestMethod]
        public void ExecutesXmlReaderWithRetryPolicyWhenSomeRetriesFailAndThenSucceeds()
        {
            using SqlConnection connection = new(connectionString);
            RetryPolicy<FakeSqlAzureTransientErrorDetectionStrategy> policy = this.retryManager.GetRetryPolicy<FakeSqlAzureTransientErrorDetectionStrategy>("Retry 5 times");
            connection.Open();
            using SqlCommand command = new();
            int count = 0;
            int rowCount = 0;
            policy.Retrying += (sender, args) => count = args.CurrentRetryCount;

            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "ErrorRaisingXMLReader";
            command.Connection = connection;
            command.Parameters.Add(new SqlParameter("rowId", SqlDbType.UniqueIdentifier) { Value = Guid.NewGuid() });
            command.Parameters.Add(new SqlParameter("maxCountToRaiseErrors", SqlDbType.Int) { Value = 4 });
            command.Parameters.Add(new SqlParameter("error", SqlDbType.Int) { Value = 60000 });
            using XmlReader reader = command.ExecuteXmlReaderWithRetry(policy);
            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        rowCount++;
                        break;
                }
            }

            reader.Close();
            Assert.AreEqual(3, count);
            connection.Close();
            Assert.AreEqual(ConnectionState.Closed, command.Connection.State);
            Assert.AreEqual(10, rowCount);
        }

        [TestMethod]
        [ExpectedException(typeof(SqlException))]
        public void ThrowsExceptionWhenAllRetriesFailDuringXmlReaderExecutionWithRetryPolicyAndConnectionRetryPolicy()
        {
            using SqlConnection connection = new(connectionString);
            RetryPolicy<FakeSqlAzureTransientErrorDetectionStrategy> policy = this.retryManager.GetRetryPolicy<FakeSqlAzureTransientErrorDetectionStrategy>("Retry 5 times");
            connection.Open();
            using SqlCommand command = new();
            int count = 0;
            policy.Retrying += (sender, args) => count = args.CurrentRetryCount;

            try
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "ErrorRaisingXMLReader";
                command.Connection = connection;
                command.Parameters.Add(new SqlParameter("rowId", SqlDbType.UniqueIdentifier) { Value = Guid.NewGuid() });
                command.Parameters.Add(new SqlParameter("maxCountToRaiseErrors", SqlDbType.Int) { Value = 7 });
                command.Parameters.Add(new SqlParameter("error", SqlDbType.Int) { Value = 60000 });
                using XmlReader reader = command.ExecuteXmlReaderWithRetry(policy, policy);
                while (reader.Read())
                {
                }
            }
            catch (Exception)
            {
                Assert.AreEqual(5, count);
                connection.Close();
                Assert.AreEqual(ConnectionState.Closed, command.Connection.State);
                throw;
            }

            Assert.Fail("Test should throw");
        }

        [TestMethod]
        public void ExecutesXmlReaderWithRetryPolicyAndConnectionRetryPolicyWhenSomeRetriesFailAndThenSucceeds()
        {
            using SqlConnection connection = new(connectionString);
            RetryPolicy<FakeSqlAzureTransientErrorDetectionStrategy> policy = this.retryManager.GetRetryPolicy<FakeSqlAzureTransientErrorDetectionStrategy>("Retry 5 times");
            connection.Open();
            using SqlCommand command = new();
            int count = 0;
            int rowCount = 0;
            policy.Retrying += (sender, args) => count = args.CurrentRetryCount;

            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "ErrorRaisingXMLReader";
            command.Connection = connection;
            command.Parameters.Add(new SqlParameter("rowId", SqlDbType.UniqueIdentifier) { Value = Guid.NewGuid() });
            command.Parameters.Add(new SqlParameter("maxCountToRaiseErrors", SqlDbType.Int) { Value = 3 });
            command.Parameters.Add(new SqlParameter("error", SqlDbType.Int) { Value = 60000 });
            using XmlReader reader = command.ExecuteXmlReaderWithRetry(policy, policy);
            while (reader.Read())
            {
                rowCount++;
            }

            reader.Close();
            Assert.AreEqual(2, count);
            connection.Close();
            Assert.AreEqual(ConnectionState.Closed, command.Connection.State);
            Assert.AreEqual(10, rowCount);
        }
    }
}
