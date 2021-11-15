namespace Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling.Tests.ReliableConnections;

public abstract class Context : ArrangeActAssert
{
    protected ReliableSqlConnection reliableConnection;
    protected TestRetryStrategy connectionStrategy;
    protected TestRetryStrategy commandStrategy;
    protected SqlCommand command;

    protected override void Arrange()
    {
        this.command = new SqlCommand(TestSqlSupport.ValidForXmlSqlQuery);

        this.connectionStrategy = new TestRetryStrategy();

        this.commandStrategy = new TestRetryStrategy();

        this.reliableConnection = new ReliableSqlConnection(
            TestSqlSupport.SqlDatabaseConnectionString,
            new RetryPolicy(ErrorDetectionStrategy.AlwaysTransient, this.connectionStrategy),
            new RetryPolicy(ErrorDetectionStrategy.AlwaysTransient, this.commandStrategy));
    }
}