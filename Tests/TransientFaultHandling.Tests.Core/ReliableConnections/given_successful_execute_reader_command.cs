namespace Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling.Tests.ReliableConnections.given_successful_execute_reader_command;

using Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling.Tests.ReliableConnections;

[TestClass]
public class when_executing_command_with_no_connection : Context
{
    private IDataReader reader;

    protected override void Act()
    {
        this.reader = this.reliableConnection.ExecuteCommand<IDataReader>(this.command);
    }

    [TestMethod]
    public void then_connection_is_opened()
    {
        Assert.IsTrue(this.command.Connection.State == ConnectionState.Open);
    }

    [TestMethod]
    public void then_can_read_results()
    {
        Assert.IsTrue(this.reader.Read());
        Assert.IsFalse(string.IsNullOrEmpty(this.reader.GetString(1)));
    }

    [TestMethod]
    public void then_retried()
    {
        Assert.AreEqual(0, this.connectionStrategy.ShouldRetryCount);
        Assert.AreEqual(0, this.commandStrategy.ShouldRetryCount);
    }
}

[TestClass]
public class when_executing_command_with_concrete_reader_with_no_connection : Context
{
    private SqlDataReader reader;

    protected override void Act()
    {
        this.reader = this.reliableConnection.ExecuteCommand<SqlDataReader>(this.command);
    }

    [TestMethod]
    public void then_connection_is_opened()
    {
        Assert.IsTrue(this.command.Connection.State == ConnectionState.Open);
    }

    [TestMethod]
    public void then_can_read_results()
    {
        Assert.IsTrue(this.reader.Read());
        Assert.IsFalse(string.IsNullOrEmpty(this.reader.GetString(1)));
    }

    [TestMethod]
    public void then_retried()
    {
        Assert.AreEqual(0, this.connectionStrategy.ShouldRetryCount);
        Assert.AreEqual(0, this.commandStrategy.ShouldRetryCount);
    }
}

[TestClass]
public class when_executing_command_with_closed_connection : Context
{
    private IDataReader reader;

    protected override void Act()
    {
        this.command.Connection = new SqlConnection(TestSqlSupport.SqlDatabaseConnectionString);

        this.reader = this.reliableConnection.ExecuteCommand<IDataReader>(this.command);
    }

    [TestMethod]
    public void then_connection_is_opened()
    {
        Assert.IsTrue(this.command.Connection.State == ConnectionState.Open);
    }

    [TestMethod]
    public void then_can_read_results()
    {
        Assert.IsTrue(this.reader.Read());
        Assert.IsFalse(string.IsNullOrEmpty(this.reader.GetString(1)));
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
    private IDataReader reader;

    protected override void Act()
    {
        this.command.Connection = new SqlConnection(TestSqlSupport.SqlDatabaseConnectionString);
        this.command.Connection.Open();

        this.reader = this.reliableConnection.ExecuteCommand<IDataReader>(this.command);
    }

    [TestMethod]
    public void then_connection_is_opened()
    {
        Assert.IsTrue(this.command.Connection.State == ConnectionState.Open);
    }

    [TestMethod]
    public void then_can_read_results()
    {
        Assert.IsTrue(this.reader.Read());
        Assert.IsFalse(string.IsNullOrEmpty(this.reader.GetString(1)));
    }

    [TestMethod]
    public void then_retried()
    {
        Assert.AreEqual(0, this.connectionStrategy.ShouldRetryCount);
        Assert.AreEqual(0, this.commandStrategy.ShouldRetryCount);
    }
}