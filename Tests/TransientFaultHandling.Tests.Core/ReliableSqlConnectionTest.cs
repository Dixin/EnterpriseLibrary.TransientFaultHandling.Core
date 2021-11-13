namespace Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling.Tests;

[TestClass]
public class ReliableSqlConnectionTest
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

    [Description("F4.1.2")]
    [Priority(1)]
    [TestMethod]
    public void TestOpen()
    {
        this.connection.Open();
        this.connection.Close();
    }

    [Description("F3.2.2; F4.1.1")]
    [Priority(1)]
    [TestMethod]
    [Ignore]    // Unstable test
    public void TestConnectionString()
    {
        string str = this.connection.ConnectionString;
        StringAssert.StartsWith(this.connectionString, str, "Unexpected connection string");

        str = this.connection.Current.ConnectionString;
        StringAssert.StartsWith(this.connectionString, str, "Unexpected connection string");
    }

    [Description("F4.1.3; F4.2.1")]
    [Priority(2)]
    [TestMethod]
    public void TestSessionTracingId()
    {
        Guid guid = this.connection.SessionTracingId;
        Trace.WriteLine($"Session Tracing Id: {guid.ToString()}");
    }

    [Description("F4.1.4")]
    [Priority(1)]
    [TestMethod]
    public void TestExecuteSimpleCommand()
    {
        SqlCommand command = new("SELECT 1");
        this.connection.ExecuteCommand(command);
    }

    [Description("F4.1.5")]
    [Priority(1)]
    [TestMethod]
    public void TestExecuteSimpleCommandWithResult()
    {
        SqlCommand command = new("SELECT 1");
        int result = this.connection.ExecuteCommand<int>(command);

        Assert.AreEqual(1, result, "Unexpected result");
    }

    [Description("F4.1.6")]
    [Priority(1)]
    [TestMethod]
    public void TextExecuteSelectCommandToGetDataReader()
    {
        SqlCommand command = new("SELECT [ProductCategoryID], [Name] FROM [SalesLT].[ProductCategory]");
        using IDataReader reader = this.connection.ExecuteCommand<IDataReader>(command);
        while (reader.Read())
        {
            int id = reader.GetInt32(0);
            string name = reader.GetString(1);

            Trace.WriteLine($"{id}: {name}");
        }

        reader.Close();
    }

    [Description("F4.1.7")]
    [Priority(2)]
    [TestMethod]
    public void TextExecuteSelectCommandToGetXmlReader()
    {
        SqlCommand command = new("SELECT [ProductCategoryID], [Name] FROM [SalesLT].[ProductCategory] FOR XML AUTO");
        using XmlReader reader = this.connection.ExecuteCommand<XmlReader>(command);
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

    [Description("F4.1.8")]
    [Priority(2)]
    [TestMethod]
    public void TestConnectionTimeout()
    {
        int expectedTimeout = 30;

        this.connection.ConnectionString += $"Connection Timeout={expectedTimeout};";
        int timeout = this.connection.ConnectionTimeout;

        Assert.AreEqual(expectedTimeout, timeout, "Unexpected timeout");
        this.connection.ConnectionString = TestSqlSupport.SqlDatabaseConnectionString;
    }

    [Description("F4.1.9")]
    [Priority(1)]
    [TestMethod]
    public void TestDatabaseName()
    {
        this.connection.Open();
        string database = this.connection.Database;
        this.connection.Close();

        Assert.AreEqual("AdventureWorksLTAZ2008R2", database, "Unexpected database");
        this.connection.ConnectionString = TestSqlSupport.SqlDatabaseConnectionString;
    }

    [Description("F4.1.10")]
    [Priority(2)]
    [TestMethod]
    [ExpectedException(typeof(SqlException))]
    public void TestChangeDatabase()
    {
        string expectedDatabase = "master";

        this.connection.Open();
        this.connection.ChangeDatabase(expectedDatabase);
        string database = this.connection.Database;
        this.connection.Close();

        Assert.AreEqual(expectedDatabase, database, "Unexpected database");
        this.connection.ConnectionString = TestSqlSupport.SqlDatabaseConnectionString;
    }

    [Ignore]
    [TestMethod]
    public void TestSuccessfulTransaction()
    {
        this.connection.Open();
        IDbTransaction transaction = this.connection.BeginTransaction();

        SqlCommand command = new("SELECT TOP 1 [CustomerID] FROM [SalesLT].[Customer] ORDER BY [CustomerID]")
        {
            Transaction = (SqlTransaction)transaction
        };
        int customerId = this.connection.ExecuteCommand<int>(command);

        command = new SqlCommand("SELECT TOP 1 [AddressID] FROM [SalesLT].[Address] ORDER BY [AddressID]")
        {
            Transaction = (SqlTransaction)transaction
        };
        int addressId = this.connection.ExecuteCommand<int>(command);

        command = new SqlCommand("INSERT INTO [SalesLT].[CustomerAddress] ([CustomerID], [AddressID], [AddressType]) VALUES (@CustomerID, @AddressID, @AddressType)");
        command.Parameters.Add("@CustomerID", SqlDbType.Int).Value = customerId;
        command.Parameters.Add("@AddressID", SqlDbType.Int).Value = addressId;
        command.Parameters.Add("@AddressType", SqlDbType.NVarChar, 100).Value = "Custom Address";

        this.connection.ExecuteCommand(command);
        transaction.Commit();

        Assert.IsTrue(this.VerifyCustomerAddress(customerId, addressId), "Insert was failed");
        this.DeleteCustomerAddress(customerId, addressId);

        this.connection.Close();
    }

    [Description("F4.1.11")]
    [Priority(1)]
    [TestMethod]
    public void TestSuccessfulTransactionWithWorkaround()
    {
        this.connection.Open();
        IDbTransaction transaction = this.connection.BeginTransaction();

        SqlCommand command = new("SELECT TOP 1 [CustomerID] FROM [SalesLT].[Customer] ORDER BY [CustomerID]")
        {
            Connection = this.connection.Current,
            Transaction = (SqlTransaction)transaction
        };
        int customerId = this.connection.ExecuteCommand<int>(command);

        command = new SqlCommand("SELECT TOP 1 [AddressID] FROM [SalesLT].[Address] ORDER BY [AddressID]")
        {
            Connection = this.connection.Current,
            Transaction = (SqlTransaction)transaction
        };
        int addressId = this.connection.ExecuteCommand<int>(command);

        command = new SqlCommand("INSERT INTO [SalesLT].[CustomerAddress] ([CustomerID], [AddressID], [AddressType]) VALUES (@CustomerID, @AddressID, @AddressType)");
        command.Parameters.Add("@CustomerID", SqlDbType.Int).Value = customerId;
        command.Parameters.Add("@AddressID", SqlDbType.Int).Value = addressId;
        command.Parameters.Add("@AddressType", SqlDbType.NVarChar, 100).Value = "Custom Address";
        command.Connection = this.connection.Current;
        command.Transaction = (SqlTransaction)transaction;

        this.connection.ExecuteCommand(command);
        transaction.Commit();

        Assert.IsTrue(this.VerifyCustomerAddress(customerId, addressId), "Insert was failed");
        this.DeleteCustomerAddress(customerId, addressId);

        this.connection.Close();
    }

    [Description("F4.1.12")]
    [Priority(1)]
    [TestMethod]
    public void TestFailedTransaction()
    {
        this.connection.Open();
        IDbTransaction transaction = this.connection.BeginTransaction(IsolationLevel.Serializable);

        SqlCommand command = new("SELECT COUNT(*) FROM [SalesLT].[CustomerAddress]")
        {
            Connection = this.connection.Current,
            Transaction = (SqlTransaction)transaction
        };
        int expectedCount = this.connection.ExecuteCommand(command);

        command = new SqlCommand("SELECT TOP 1 [CustomerID] FROM [SalesLT].[Customer] ORDER BY [CustomerID]")
        {
            Connection = this.connection.Current,
            Transaction = (SqlTransaction)transaction
        };
        int customerId = this.connection.ExecuteCommand<int>(command);

        command = new SqlCommand("SELECT TOP 1 [AddressID] FROM [SalesLT].[Address] ORDER BY [AddressID]")
        {
            Connection = this.connection.Current,
            Transaction = (SqlTransaction)transaction
        };
        int addressId = this.connection.ExecuteCommand<int>(command);

        command = new SqlCommand("INSERT INTO [SalesLT].[CustomerAddress] ([CustomerID], [AddressID], [AddressType]) VALUES (@CustomerID, @AddressID, @AddressType)");
        command.Parameters.Add("@CustomerID", SqlDbType.Int).Value = customerId;
        command.Parameters.Add("@AddressID", SqlDbType.Int).Value = addressId;
        command.Parameters.Add("@AddressType", SqlDbType.NVarChar, 100).Value = "Custom Address";
        command.Connection = this.connection.Current;
        command.Transaction = (SqlTransaction)transaction;

        this.connection.ExecuteCommand(command);
        transaction.Rollback();

        command = new SqlCommand("SELECT COUNT(*) FROM [SalesLT].[CustomerAddress]")
        {
            Connection = this.connection.Current,
            Transaction = (SqlTransaction)transaction
        };
        int actualCount = this.connection.ExecuteCommand(command);

        this.connection.Close();

        Assert.AreEqual(expectedCount, actualCount, "Rollback failed");
    }

    [TestMethod]
    [Ignore]    // Unstable test
    public void TestServerNameSubstitutionWithIPAddress()
    {
        using ReliableSqlConnection conn1 = new(this.connectionString);
        Assert.AreEqual(this.connectionString, conn1.Current.ConnectionString, "Connection string managed by ReliableSqlConnection class must not be modified at this point.");
        Assert.AreNotEqual(conn1.SessionTracingId, Guid.Empty, "Unable to resolve the connection's session ID.");

        Thread.Sleep(1000);

        using ReliableSqlConnection conn2 = new(this.connectionString);
        Assert.AreNotEqual(this.connectionString, conn2.Current.ConnectionString, "Connection string managed by ReliableSqlConnection class has not been modified.");
        Assert.AreNotEqual(conn2.SessionTracingId, Guid.Empty, "Unable to resolve the connection's session ID.");

        IPAddress hostAddress = null;
        SqlConnectionStringBuilder conStringBuilder = new(conn2.Current.ConnectionString);
        string hostName = conStringBuilder.DataSource.StartsWith("tcp:") ? conStringBuilder.DataSource.Remove(0, "tcp:".Length) : conStringBuilder.DataSource;

        Assert.IsTrue(IPAddress.TryParse(hostName, out hostAddress), "The data source doesn't seem to be represented by an IP address.");

        conn2.Current.Close();
        conn2.Current.ConnectionString = conn2.Current.ConnectionString.Replace("0", "1").Replace("2", "3").Replace("4", "5").Replace("6", "7");

        conn2.Open();

        Assert.AreNotEqual(conn2.SessionTracingId, Guid.Empty, "Unable to resolve the connection's session ID.");
    }

    private bool VerifyCustomerAddress(int customerId, int addressId)
    {
        SqlCommand command = new("SELECT COUNT(*) FROM [SalesLT].[CustomerAddress] WHERE [CustomerID] = @CustomerID AND [AddressID] = @AddressID");
        command.Parameters.Add("@CustomerID", SqlDbType.Int).Value = customerId;
        command.Parameters.Add("@AddressID", SqlDbType.Int).Value = addressId;
        command.Connection = this.connection.Current;

        int count = this.connection.ExecuteCommand<int>(command);
        return count > 0;
    }

    private void DeleteCustomerAddress(int customerId, int addressId)
    {
        SqlCommand command = new("DELETE FROM [SalesLT].[CustomerAddress] WHERE [CustomerID] = @CustomerID AND [AddressID] = @AddressID");
        command.Parameters.Add("@CustomerID", SqlDbType.Int).Value = customerId;
        command.Parameters.Add("@AddressID", SqlDbType.Int).Value = addressId;
        command.Connection = this.connection.Current;

        this.connection.ExecuteCommand(command);
    }
}