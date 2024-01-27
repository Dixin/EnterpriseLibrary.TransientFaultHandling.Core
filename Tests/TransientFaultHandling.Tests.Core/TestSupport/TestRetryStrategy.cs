namespace Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling.Tests.TestSupport;

public class TestRetryStrategy() : RetryStrategy(nameof(TestRetryStrategy), true)
{
    public int CustomProperty { get; } = 1;

    public int ShouldRetryCount { get; private set; }

    public override ShouldRetry GetShouldRetry()
    {
        return delegate(int currentRetryCount, Exception lastException, out TimeSpan interval)
        {
            if (this.CustomProperty == currentRetryCount)
            {
                interval = TimeSpan.Zero;
                return false;
            }

            this.ShouldRetryCount++;

            interval = TimeSpan.FromMilliseconds(1);
            return true;
        };
    }
}