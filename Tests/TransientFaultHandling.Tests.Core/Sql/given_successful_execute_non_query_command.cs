namespace Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling.Tests.Sql.given_successful_execute_non_query_command;

[TestClass]
public class when_executing_command_with_closed_connection : Context
{
    private int resultsCount;

    protected override void Act()
    {
        this.command.Connection = new SqlConnection(TestSqlSupport.SqlDatabaseConnectionString);

        this.resultsCount = this.command.ExecuteNonQueryWithRetry(this.commandPolicy, this.connectionPolicy);
    }

    [TestMethod]
    public void then_connection_is_closed()
    {
        Assert.IsTrue(this.command.Connection.State == ConnectionState.Closed);
    }

    [TestMethod]
    public void then_can_results_count()
    {
        Assert.AreEqual(-1, this.resultsCount);
    }

    [TestMethod]
    public void then_retried()
    {
        Assert.AreEqual(0, this.connectionStrategy.ShouldRetryCount);
        Assert.AreEqual(0, this.commandStrategy.ShouldRetryCount);
    }
}

[TestClass]
public class when_executing_command_with_opened_connection : Context
{
    private int resultsCount;

    protected override void Act()
    {
        this.command.Connection = new SqlConnection(TestSqlSupport.SqlDatabaseConnectionString);
        this.command.Connection.Open();

        this.resultsCount = this.command.ExecuteNonQueryWithRetry(this.commandPolicy, this.connectionPolicy);
    }

    [TestMethod]
    public void then_connection_is_opened()
    {
        Assert.IsTrue(this.command.Connection.State == ConnectionState.Open);
    }

    [TestMethod]
    public void then_can_results_count()
    {
        Assert.AreEqual(-1, this.resultsCount);
    }

    [TestMethod]
    public void then_retried()
    {
        Assert.AreEqual(0, this.connectionStrategy.ShouldRetryCount);
        Assert.AreEqual(0, this.commandStrategy.ShouldRetryCount);
    }
}