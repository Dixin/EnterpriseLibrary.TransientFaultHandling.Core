namespace Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling.Tests.Sql;

public abstract class Context : ArrangeActAssert
{
    protected TestRetryStrategy connectionStrategy;
    protected TestRetryStrategy commandStrategy;
    protected SqlCommand command;
    protected RetryPolicy connectionPolicy;
    protected RetryPolicy commandPolicy;

    protected override void Arrange()
    {
        this.command = new SqlCommand(TestSqlSupport.InvalidSqlText);

        this.connectionStrategy = new TestRetryStrategy();

        this.commandStrategy = new TestRetryStrategy();

        this.connectionPolicy = new RetryPolicy(ErrorDetectionStrategy.AlwaysTransient, this.connectionStrategy);

        this.commandPolicy = new RetryPolicy(ErrorDetectionStrategy.AlwaysTransient, this.commandStrategy);
    }
}
