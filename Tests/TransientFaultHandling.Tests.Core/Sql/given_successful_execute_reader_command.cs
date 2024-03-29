﻿namespace Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling.Tests.Sql.given_successful_execute_reader_command;

[TestClass]
public class when_executing_command_with_closed_connection : Context
{
    private IDataReader reader;

    protected override void Act()
    {
        this.command.Connection = new SqlConnection(TestSqlSupport.SqlDatabaseConnectionString);

        this.reader = this.command.ExecuteReaderWithRetry(this.commandPolicy, this.connectionPolicy);
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

        this.reader = this.command.ExecuteReaderWithRetry(this.commandPolicy, this.connectionPolicy);
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