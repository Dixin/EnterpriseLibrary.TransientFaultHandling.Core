namespace Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling.Tests
{
    using System;
    using System.Data;
    using System.Diagnostics;
    using System.Globalization;
    using Microsoft.Data.SqlClient;
    using Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling.TestSupport;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class TransactionRetryScopeTest
    {
        private string connectionString;

        [TestInitialize]
        public void Setup()
        {
            this.connectionString = TestSqlSupport.SqlDatabaseConnectionString;
            RetryPolicyFactory.CreateDefault();
        }

        [TestCleanup]
        public void Cleanup()
        {
            RetryPolicyFactory.SetRetryManager(null, false);
        }

        private ReliableSqlConnection InitializeConnection()
        {
            ReliableSqlConnection connection = new(this.connectionString);

            connection.ConnectionRetryPolicy.Retrying += (sender, args) =>
                Trace.WriteLine(string.Format(CultureInfo.CurrentCulture, "[Connection Retry] Current Retry Count: {0}, Last Exception: {1}, Delay (ms): {2}", args.CurrentRetryCount, args.LastException.Message, args.Delay.TotalMilliseconds));

            connection.CommandRetryPolicy.Retrying += (sender, args) =>
                Trace.WriteLine(string.Format(CultureInfo.CurrentCulture, "[Command Retry] Current Retry Count: {0}, Last Exception: {1}, Delay (ms): {2}", args.CurrentRetryCount, args.LastException.Message, args.Delay.TotalMilliseconds));

            connection.Current.StateChange += (sender, e) =>
                Trace.WriteLine(string.Format(CultureInfo.CurrentCulture, "OriginalState: {0}, CurrentState {1}", e.OriginalState, e.CurrentState));

            return connection;
        }

        [Description("F4.3.1")]
        [Priority(1)]
        [TestMethod]
        public void TestSuccessfulTransaction()
        {
            int customerId = 0;
            int addressId = 0;

            Action action = () =>
            {
                using ReliableSqlConnection connection = this.InitializeConnection();
                using SqlCommand command1 = connection.CreateCommand();
                command1.CommandText = "SELECT TOP 1 [CustomerID] FROM [SalesLT].[Customer] ORDER BY [CustomerID]";
                customerId = (int)command1.ExecuteScalarWithRetry();

                using SqlCommand command2 = connection.CreateCommand();
                command2.CommandText = "SELECT TOP 1 [AddressID] FROM [SalesLT].[Address] ORDER BY [AddressID]";
                addressId = (int)command2.ExecuteScalarWithRetry();

                using SqlCommand command3 = connection.CreateCommand();
                command3.CommandText = "INSERT INTO [SalesLT].[CustomerAddress] ([CustomerID], [AddressID], [AddressType]) VALUES (@CustomerID, @AddressID, @AddressType)";
                command3.Parameters.Add("@CustomerID", SqlDbType.Int).Value = customerId;
                command3.Parameters.Add("@AddressID", SqlDbType.Int).Value = addressId;
                command3.Parameters.Add("@AddressType", SqlDbType.NVarChar, 100).Value = "Custom Address";
                command3.ExecuteNonQueryWithRetry();
            };

            using TransactionRetryScope scope = new(RetryPolicy.NoRetry, action);
            try
            {
                scope.InvokeUnitOfWork();
                scope.Complete();
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message);
            }

            Assert.IsTrue(this.VerifyCustomerAddress(customerId, addressId));
            this.DeleteCustomerAddress(customerId, addressId);
        }

        [Description("F4.3.2")]
        [Priority(1)]
        [TestMethod]
        public void TestSuccessfulTransactionWithRetryableError()
        {
            int customerId = 0;
            int addressId = 0;

            int executeCount = 0;

            Action action = () =>
            {
                using ReliableSqlConnection connection = this.InitializeConnection();
                using SqlCommand command1 = connection.CreateCommand();
                command1.CommandText = "SELECT TOP 1 [CustomerID] FROM [SalesLT].[Customer] ORDER BY [CustomerID]";
                customerId = (int)command1.ExecuteScalarWithRetry();

                if (executeCount == 0)
                {
                    executeCount++;
                    throw new TimeoutException();
                }

                using SqlCommand command2 = connection.CreateCommand();
                command2.CommandText = "SELECT TOP 1 [AddressID] FROM [SalesLT].[Address] ORDER BY [AddressID]";
                addressId = (int)command2.ExecuteScalarWithRetry();

                using SqlCommand command3 = connection.CreateCommand();
                command3.CommandText = "INSERT INTO [SalesLT].[CustomerAddress] ([CustomerID], [AddressID], [AddressType]) VALUES (@CustomerID, @AddressID, @AddressType)";
                command3.Parameters.Add("@CustomerID", SqlDbType.Int).Value = customerId;
                command3.Parameters.Add("@AddressID", SqlDbType.Int).Value = addressId;
                command3.Parameters.Add("@AddressType", SqlDbType.NVarChar, 100).Value = "Custom Address";
                command3.ExecuteNonQueryWithRetry();
            };

            RetryPolicy retryPolicy = RetryPolicyFactory.GetRetryPolicy<SqlDatabaseTransientErrorDetectionStrategy>(retryStrategyName: "FixedIntervalDefault");

            using TransactionRetryScope scope = new(retryPolicy, action);
            try
            {
                scope.InvokeUnitOfWork();
                scope.Complete();
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message);
            }

            Assert.IsTrue(this.VerifyCustomerAddress(customerId, addressId));
            this.DeleteCustomerAddress(customerId, addressId);
        }

        [Description("F4.3.3")]
        [Priority(1)]
        [TestMethod]
        public void TestFailedTransaction()
        {
            int expectedCount = this.RetrieveCountOfTable();

            int customerId = 0;
            int addressId = 0;

            Action action = () =>
            {
                using ReliableSqlConnection connection = this.InitializeConnection();
                using SqlCommand command1 = connection.CreateCommand();
                command1.CommandText = "SELECT TOP 1 [CustomerID] FROM [SalesLT].[Customer] ORDER BY [CustomerID]";
                customerId = (int)command1.ExecuteScalarWithRetry();

                using SqlCommand command2 = connection.CreateCommand();
                command2.CommandText = "SELECT TOP 1 [AddressID] FROM [SalesLT].[Address] ORDER BY [AddressID]";
                addressId = (int)command2.ExecuteScalarWithRetry();

                using SqlCommand command3 = connection.CreateCommand();
                command3.CommandText = "INSERT INTO [SalesLT].[CustomerAddress] ([CustomerID], [AddressID], [AddressType]) VALUES (@CustomerID, @AddressID, @AddressType)";
                command3.Parameters.Add("@CustomerID", SqlDbType.Int).Value = customerId;
                command3.Parameters.Add("@AddressID", SqlDbType.Int).Value = addressId;
                command3.Parameters.Add("@AddressType", SqlDbType.NVarChar, 100).Value = "Custom Address";
                command3.ExecuteNonQueryWithRetry();

                using SqlCommand command4 = connection.CreateCommand();
                command4.CommandText = "RAISEERROR('ERROR', 16, 1)";
                command4.ExecuteNonQueryWithRetry();
            };

            using TransactionRetryScope scope = new(action);
            try
            {
                scope.InvokeUnitOfWork();
                scope.Complete();
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message);
            }

            int actualCount = this.RetrieveCountOfTable();

            Assert.AreEqual(expectedCount, actualCount, "Rollback failed");
        }

        private int RetrieveCountOfTable()
        {
            int count = 0;

            using ReliableSqlConnection connection = new(this.connectionString);
            using SqlCommand command = connection.CreateCommand();
            command.CommandText = "SELECT COUNT(*) FROM [SalesLT].[CustomerAddress]";
            count = (int)command.ExecuteScalarWithRetry();

            connection.Close();

            return count;
        }

        private bool VerifyCustomerAddress(int customerId, int addressId)
        {
            int count = 0;

            using ReliableSqlConnection connection = new(this.connectionString);
            using SqlCommand command = connection.CreateCommand();
            command.CommandText = "SELECT COUNT(*) FROM [SalesLT].[CustomerAddress] WHERE [CustomerID] = @CustomerID AND [AddressID] = @AddressID";
            command.Parameters.Add("@CustomerID", SqlDbType.Int).Value = customerId;
            command.Parameters.Add("@AddressID", SqlDbType.Int).Value = addressId;
            count = (int)command.ExecuteScalarWithRetry();

            connection.Close();

            return count > 0;
        }

        private void DeleteCustomerAddress(int customerId, int addressId)
        {
            using ReliableSqlConnection connection = new(this.connectionString);
            using SqlCommand command = connection.CreateCommand();
            command.CommandText = "DELETE FROM [SalesLT].[CustomerAddress] WHERE [CustomerID] = @CustomerID AND [AddressID] = @AddressID";
            command.Parameters.Add("@CustomerID", SqlDbType.Int).Value = customerId;
            command.Parameters.Add("@AddressID", SqlDbType.Int).Value = addressId;
            command.ExecuteNonQueryWithRetry();

            connection.Close();
        }
    }
}
