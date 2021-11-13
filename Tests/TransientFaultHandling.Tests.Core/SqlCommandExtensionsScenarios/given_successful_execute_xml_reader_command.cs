namespace Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling.Tests.SqlCommandExtensionsScenarios.given_successful_execute_xml_reader_command;

public abstract class Context : ArrangeActAssert
{
    protected TestRetryStrategy connectionStrategy;
    protected TestRetryStrategy commandStrategy;
    protected SqlCommand command;
    protected RetryPolicy connectionPolicy;
    protected RetryPolicy commandPolicy;

    protected override void Arrange()
    {
        this.command = new SqlCommand(TestSqlSupport.ValidForXmlSqlQuery);

        this.connectionStrategy = new TestRetryStrategy();

        this.commandStrategy = new TestRetryStrategy();

        this.connectionPolicy = new RetryPolicy(ErrorDetectionStrategy.AlwaysTransient, this.connectionStrategy);

        this.commandPolicy = new RetryPolicy(ErrorDetectionStrategy.AlwaysTransient, this.commandStrategy);
    }
}

[TestClass]
public class when_executing_command_with_closed_connection : Context
{
    private XmlReader reader;

    protected override void Act()
    {
        this.command.Connection = new SqlConnection(TestSqlSupport.SqlDatabaseConnectionString);

        this.reader = this.command.ExecuteXmlReaderWithRetry(this.commandPolicy, this.connectionPolicy);
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
    private XmlReader reader;

    protected override void Act()
    {
        this.command.Connection = new SqlConnection(TestSqlSupport.SqlDatabaseConnectionString);
        this.command.Connection.Open();

        this.reader = this.command.ExecuteXmlReaderWithRetry(this.commandPolicy, this.connectionPolicy);
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
    }

    [TestMethod]
    public void then_retried()
    {
        Assert.AreEqual(0, this.connectionStrategy.ShouldRetryCount);
        Assert.AreEqual(0, this.commandStrategy.ShouldRetryCount);
    }
}