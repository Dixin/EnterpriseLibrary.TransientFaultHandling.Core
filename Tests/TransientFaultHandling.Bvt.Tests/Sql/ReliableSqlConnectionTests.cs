namespace Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling.Bvt.Tests.Sql;

[TestClass]
public class ReliableSqlConnectionTests
{
    private static readonly RetryManager RetryManager = RetryConfiguration.GetRetryManager(getCustomRetryStrategy: section =>
        section.Get<TestRetryStrategyOptions>().ToTestRetryStrategy(section.Key));

    [TestInitialize]
    public void Initialize()
    {
        RetryManager.SetDefault(RetryManager, false);
    }

    [TestCleanup]
    public void Cleanup()
    {
    }

    [TestMethod]
    public void OpensConnectionWithRetryPolicy()
    {
        using ReliableSqlConnection reliableConnection = new(TestDatabase.TransientFaultHandlingTestDatabase);

        try
        {
            RetryPolicy<SqlDatabaseTransientErrorDetectionStrategy> retryPolicy = RetryManager.GetRetryPolicy<SqlDatabaseTransientErrorDetectionStrategy>("Retry 5 times");
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
        using ReliableSqlConnection reliableConnection = new(TestDatabase.TransientFaultHandlingTestDatabase);
        int count = 0;
        try
        {
            RetryPolicy<SqlDatabaseTransientErrorDetectionStrategy> retryPolicy = RetryManager.GetRetryPolicy<SqlDatabaseTransientErrorDetectionStrategy>("Retry 5 times");

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
        using ReliableSqlConnection reliableConnection = new(TestDatabase.TransientFaultHandlingTestDatabase);

        try
        {
            RetryPolicy<SqlDatabaseTransientErrorDetectionStrategy> retryPolicy = RetryManager.GetRetryPolicy<SqlDatabaseTransientErrorDetectionStrategy>("Retry 5 times");

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
        using ReliableSqlConnection reliableConnection = new(TestDatabase.TransientFaultHandlingTestDatabase);

        XmlReader reader;
        int count = 0;
        try
        {
            RetryPolicy<SqlDatabaseTransientErrorDetectionStrategy> retryPolicy = RetryManager.GetRetryPolicy<SqlDatabaseTransientErrorDetectionStrategy>("Retry 5 times");

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
        using ReliableSqlConnection reliableConnection = new(TestDatabase.TransientFaultHandlingTestDatabase);

        int count = 0;
        try
        {
            RetryPolicy<SqlDatabaseTransientErrorDetectionStrategy> retryPolicy = RetryManager.GetRetryPolicy<SqlDatabaseTransientErrorDetectionStrategy>("Retry 5 times");

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
        using ReliableSqlConnection reliableConnection = new(TestDatabase.TransientFaultHandlingTestDatabase);

        int count = 0;
        try
        {
            RetryPolicy<SqlDatabaseTransientErrorDetectionStrategy> retryPolicy = RetryManager.GetRetryPolicy<SqlDatabaseTransientErrorDetectionStrategy>("Retry 5 times");

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
        using ReliableSqlConnection reliableConnection = new(TestDatabase.TransientFaultHandlingTestDatabase);

        RetryPolicy<FakeSqlAzureTransientErrorDetectionStrategy> policy = RetryManager.GetRetryPolicy<FakeSqlAzureTransientErrorDetectionStrategy>("Retry 5 times");
        int count = 0;
        policy.Retrying += (_, args) => count = args.CurrentRetryCount;

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
        using ReliableSqlConnection reliableConnection = new(TestDatabase.TransientFaultHandlingTestDatabase);

        RetryPolicy<FakeSqlAzureTransientErrorDetectionStrategy> policy = RetryManager.GetRetryPolicy<FakeSqlAzureTransientErrorDetectionStrategy>("Retry 5 times");
        int count = 0;
        policy.Retrying += (_, args) => count = args.CurrentRetryCount;

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
        RetryPolicy<FakeSqlAzureTransientErrorDetectionStrategy> policy = RetryManager.GetRetryPolicy<FakeSqlAzureTransientErrorDetectionStrategy>("Retry 5 times");
        using ReliableSqlConnection reliableConnection = new(TestDatabase.TransientFaultHandlingTestDatabase, policy, policy);

        int count = 0;
        policy.Retrying += (_, args) => count = args.CurrentRetryCount;

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
        RetryPolicy<FakeSqlAzureTransientErrorDetectionStrategy> policy = RetryManager.GetRetryPolicy<FakeSqlAzureTransientErrorDetectionStrategy>("Retry 5 times");
        using ReliableSqlConnection reliableConnection = new(TestDatabase.TransientFaultHandlingTestDatabase, policy, policy);

        int count = 0;
        policy.Retrying += (_, args) => count = args.CurrentRetryCount;

        int rowCount = 0;
        try
        {
            SqlCommand command = new ()
            {
                CommandType = CommandType.StoredProcedure,
                CommandText = "ErrorRaisingReader"
            };
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