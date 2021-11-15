namespace Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling.Tests.ReliableConnections.given_failing_execute_reader_command;

using Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling.Tests.ReliableConnections;

[TestClass]
public class when_executing_command_with_no_connection : Context
{
    protected override void Act()
    {
        try
        {
            this.reliableConnection.ExecuteCommand<IDataReader>(this.command);
            Assert.Fail();
        }
        catch (SqlException)
        {
        }
        catch (Exception)
        {
            Assert.Fail();
        }
    }

    [TestMethod]
    public void then_connection_is_closed()
    {
        Assert.IsNotNull(this.command.Connection);
        Assert.IsTrue(this.command.Connection.State == ConnectionState.Closed);
    }

    [TestMethod]
    public void then_retried()
    {
        Assert.AreEqual(0, this.connectionStrategy.ShouldRetryCount);
        Assert.AreEqual(1, this.commandStrategy.ShouldRetryCount);
    }
}

[TestClass]
public class when_executing_command_with_closed_connection : Context
{
    protected override void Act()
    {
        try
        {
            this.command.Connection = new SqlConnection(TestSqlSupport.SqlDatabaseConnectionString);
            this.reliableConnection.ExecuteCommand<IDataReader>(this.command);
            Assert.Fail();
        }
        catch (SqlException)
        {
        }
        catch (Exception)
        {
            Assert.Fail();
        }
    }

    [TestMethod]
    public void then_connection_is_closed()
    {
        Assert.IsNotNull(this.command.Connection);
        Assert.IsTrue(this.command.Connection.State == ConnectionState.Closed);
    }

    [TestMethod]
    public void then_retried()
    {
        Assert.AreEqual(0, this.connectionStrategy.ShouldRetryCount);
        Assert.AreEqual(1, this.commandStrategy.ShouldRetryCount);
    }
}

[TestClass]
public class when_executing_command_with_opened_connection : Context
{
    protected override void Act()
    {
        try
        {
            this.command.Connection = new SqlConnection(TestSqlSupport.SqlDatabaseConnectionString);
            this.command.Connection.Open();
            this.reliableConnection.ExecuteCommand<IDataReader>(this.command);
            Assert.Fail();
        }
        catch (SqlException)
        {
        }
        catch (Exception)
        {
            Assert.Fail();
        }
    }

    [TestMethod]
    public void then_connection_is_opened()
    {
        Assert.IsNotNull(this.command.Connection);
        Assert.IsTrue(this.command.Connection.State == ConnectionState.Open);
    }

    [TestMethod]
    public void then_retried()
    {
        Assert.AreEqual(0, this.connectionStrategy.ShouldRetryCount);
        Assert.AreEqual(1, this.commandStrategy.ShouldRetryCount);
    }
}