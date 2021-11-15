namespace Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling.Tests.RetryPolicies;

public abstract class Context : ArrangeActAssert
{
    protected RetryPolicy retryPolicy;
    protected Mock<RetryStrategy> retryStrategyMock;

    protected override void Arrange()
    {
        this.retryStrategyMock = new Mock<RetryStrategy>("name", false);
        this.retryStrategyMock.Setup(x => x.GetShouldRetry())
            .Returns(() => (int currentRetryCount, Exception lastException, out TimeSpan interval) =>
            {
                interval = TimeSpan.Zero;
                return false;
            });

        this.retryPolicy = new RetryPolicy(ErrorDetectionStrategy.AlwaysTransient, this.retryStrategyMock.Object);
    }
}
