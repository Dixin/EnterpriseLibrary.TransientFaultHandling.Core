namespace Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling.Tests.ReliableConnections.given_invalid_connection_string;

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
        catch (SqlException ex)
        {
            if (!ex.Message.StartsWith("A network-related or instance-specific error occurred while establishing a connection to SQL Server."))
            {
                Assert.Fail();
            }
        }
    }

    [TestMethod]
    public void then_connection_is_null()
    {
        Assert.IsNull(this.command.Connection);
    }

    [TestMethod]
    // TODO: this is a bug, and we need to fix it!
    [Ignore]
    public void then_retried()
    {
        Assert.AreEqual(2, this.connectionStrategy.ShouldRetryCount);
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
            this.command.Connection = new SqlConnection(TestSqlSupport.InvalidConnectionString);
            this.reliableConnection.ExecuteCommand<IDataReader>(this.command);
            Assert.Fail();
        }
        catch (SqlException ex)
        {
            if (!ex.Message.StartsWith("A network-related or instance-specific error occurred while establishing a connection to SQL Server."))
            {
                Assert.Fail();
            }
        }
    }

    [TestMethod]
    public void then_connection_is_closed()
    {
        Assert.IsTrue(this.command.Connection.State == ConnectionState.Closed);
    }

    [TestMethod]
    public void then_retried()
    {
        Assert.AreEqual(0, this.connectionStrategy.ShouldRetryCount);
        Assert.AreEqual(1, this.commandStrategy.ShouldRetryCount);
    }
}