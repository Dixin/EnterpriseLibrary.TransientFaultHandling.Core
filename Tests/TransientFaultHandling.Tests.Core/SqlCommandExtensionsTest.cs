namespace Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling.Tests
{
    using System.Data;
    using System.Diagnostics;
    using System.Xml;
    using Microsoft.Data.SqlClient;
    using Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling.TestSupport;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class SqlCommandExtensionsTest
    {
        private string connectionString;
        private ReliableSqlConnection connection;

        [TestInitialize]
        public void Initialize()
        {
            this.connectionString = TestSqlSupport.SqlDatabaseConnectionString;
            RetryPolicyFactory.CreateDefault();

            this.connection = new ReliableSqlConnection(this.connectionString);

            this.connection.ConnectionRetryPolicy.Retrying += (sender, args) =>
                Trace.WriteLine(string.Format("[Connection Retry] Current Retry Count: {0}, Last Exception: {0}, Delay (ms): {0}", args.CurrentRetryCount, args.LastException.Message, args.Delay.TotalMilliseconds));

            this.connection.CommandRetryPolicy.Retrying += (sender, args) =>
                Trace.WriteLine(string.Format("[Command Retry] Current Retry Count: {0}, Last Exception: {0}, Delay (ms): {0}", args.CurrentRetryCount, args.LastException.Message, args.Delay.TotalMilliseconds));
        }

        [TestCleanup]
        public void Cleanup()
        {
            RetryPolicyFactory.SetRetryManager(null, false);

            // Work around, close the connection manually.
            if (this.connection?.State == ConnectionState.Open)
            {
                this.connection.Close();
            }

            this.connection?.Dispose();
        }

        [Description("F5.2.1")]
        [Priority(1)]
        [TestMethod]
        public void TextExecuteSimpleCommand()
        {
            SqlCommand command = this.connection.CreateCommand();
            command.CommandText = "SELECT 1";
            command.ExecuteNonQueryWithRetry();
        }

        [Description("F5.2.2")]
        [Priority(1)]
        [TestMethod]
        public void TestExecuteSimpleCommandWithResult()
        {
            SqlCommand command = this.connection.CreateCommand();
            command.CommandText = "SELECT 1";
            int result = (int)command.ExecuteScalarWithRetry();

            Assert.AreEqual(1, result, "Unexpected result");
        }

        [Description("F5.2.3")]
        [Priority(1)]
        [TestMethod]
        public void TextExecuteSelectCommandToGetDataReader()
        {
            SqlCommand command = this.connection.CreateCommand();
            command.CommandText = "SELECT [ProductCategoryID], [Name] FROM [SalesLT].[ProductCategory]";

            using IDataReader reader = command.ExecuteReaderWithRetry();
            while (reader.Read())
            {
                int id = reader.GetInt32(0);
                string name = reader.GetString(1);

                Trace.WriteLine($"{id}: {name}");
            }

            reader.Close();
        }

        [Description("F5.2.4")]
        [Priority(1)]
        [TestMethod]
        public void TextExecuteSelectCommandToGetXmlReader()
        {
            SqlCommand command = this.connection.CreateCommand();
            command.CommandText = "SELECT [ProductCategoryID], [Name] FROM [SalesLT].[ProductCategory] FOR XML AUTO";

            using XmlReader reader = command.ExecuteXmlReaderWithRetry();
            while (reader.Read())
            {
                reader.MoveToFirstAttribute();
                reader.ReadAttributeValue();
                int id = reader.ReadContentAsInt();

                reader.MoveToNextAttribute();
                reader.ReadAttributeValue();
                string name = reader.ReadContentAsString();

                reader.MoveToElement();

                Trace.WriteLine($"{id}: {name}");
            }

            reader.Close();
        }

        [Description("F5.2.5")]
        [Priority(1)]
        [TestMethod]
        public void TestSuccessfulTransaction()
        {
            this.connection.Open();
            IDbTransaction transaction = this.connection.BeginTransaction();

            SqlCommand command = this.connection.CreateCommand();
            command.CommandText = "SELECT TOP 1 [CustomerID] FROM [SalesLT].[Customer] ORDER BY [CustomerID]";
            command.Transaction = (SqlTransaction)transaction;
            int customerId = (int)command.ExecuteScalarWithRetry();

            command = this.connection.CreateCommand();
            command.CommandText = "SELECT TOP 1 [AddressID] FROM [SalesLT].[Address] ORDER BY [AddressID]";
            command.Transaction = (SqlTransaction)transaction;
            int addressId = (int)command.ExecuteScalarWithRetry();

            command = this.connection.CreateCommand();
            command.CommandText = "INSERT INTO [SalesLT].[CustomerAddress] ([CustomerID], [AddressID], [AddressType]) VALUES (@CustomerID, @AddressID, @AddressType)";
            command.Parameters.Add("@CustomerID", SqlDbType.Int).Value = customerId;
            command.Parameters.Add("@AddressID", SqlDbType.Int).Value = addressId;
            command.Parameters.Add("@AddressType", SqlDbType.NVarChar, 100).Value = "Custom Address";
            command.Transaction = (SqlTransaction)transaction;

            command.ExecuteNonQueryWithRetry();
            transaction.Commit();

            Assert.IsTrue(this.VerifyCustomerAddress(customerId, addressId), "Insert was failed");
            this.DeleteCustomerAddress(customerId, addressId);

            this.connection.Close();
        }

        [Description("F5.2.6")]
        [Priority(1)]
        [TestMethod]
        public void TestFailedTransaction()
        {
            this.connection.Open();
            IDbTransaction transaction = this.connection.BeginTransaction(IsolationLevel.Serializable);

            SqlCommand command = this.connection.CreateCommand();
            command.CommandText = "SELECT COUNT(*) FROM [SalesLT].[CustomerAddress]";
            command.Transaction = (SqlTransaction)transaction;
            int expectedCount = this.connection.ExecuteCommand(command);

            command = this.connection.CreateCommand();
            command.CommandText = "SELECT TOP 1 [CustomerID] FROM [SalesLT].[Customer] ORDER BY [CustomerID]";
            command.Transaction = (SqlTransaction)transaction;
            int customerId = (int)command.ExecuteScalarWithRetry();

            command = this.connection.CreateCommand();
            command.CommandText = "SELECT TOP 1 [AddressID] FROM [SalesLT].[Address] ORDER BY [AddressID]";
            command.Transaction = (SqlTransaction)transaction;
            int addressId = (int)command.ExecuteScalarWithRetry();

            command = this.connection.CreateCommand();
            command.CommandText = "INSERT INTO [SalesLT].[CustomerAddress] ([CustomerID], [AddressID], [AddressType]) VALUES (@CustomerID, @AddressID, @AddressType)";
            command.Parameters.Add("@CustomerID", SqlDbType.Int).Value = customerId;
            command.Parameters.Add("@AddressID", SqlDbType.Int).Value = addressId;
            command.Parameters.Add("@AddressType", SqlDbType.NVarChar, 100).Value = "Custom Address";
            command.Transaction = (SqlTransaction)transaction;

            command.ExecuteNonQueryWithRetry();
            transaction.Rollback();

            command = this.connection.CreateCommand();
            command.CommandText = "SELECT COUNT(*) FROM [SalesLT].[CustomerAddress]";
            command.Transaction = (SqlTransaction)transaction;
            int actualCount = this.connection.ExecuteCommand(command);

            this.connection.Close();

            Assert.AreEqual(expectedCount, actualCount, "Rollback failed");
        }

        private bool VerifyCustomerAddress(int customerId, int addressId)
        {
            SqlCommand command = this.connection.CreateCommand();
            command.CommandText = "SELECT COUNT(*) FROM [SalesLT].[CustomerAddress] WHERE [CustomerID] = @CustomerID AND [AddressID] = @AddressID";
            command.Parameters.Add("@CustomerID", SqlDbType.Int).Value = customerId;
            command.Parameters.Add("@AddressID", SqlDbType.Int).Value = addressId;

            int count = (int)command.ExecuteScalarWithRetry();
            return count > 0;
        }

        private void DeleteCustomerAddress(int customerId, int addressId)
        {
            SqlCommand command = this.connection.CreateCommand();
            command.CommandText = "DELETE FROM [SalesLT].[CustomerAddress] WHERE [CustomerID] = @CustomerID AND [AddressID] = @AddressID";
            command.Parameters.Add("@CustomerID", SqlDbType.Int).Value = customerId;
            command.Parameters.Add("@AddressID", SqlDbType.Int).Value = addressId;

            command.ExecuteNonQueryWithRetry();
        }
    }
}
