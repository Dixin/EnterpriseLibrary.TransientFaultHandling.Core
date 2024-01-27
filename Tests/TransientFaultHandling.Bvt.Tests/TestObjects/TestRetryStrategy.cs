namespace Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling.Bvt.Tests.TestObjects;

public class TestRetryStrategy(string name, bool firstFastRetry, int customProperty) : RetryStrategy(name, firstFastRetry)
{
    public int CustomProperty { get; } = customProperty;

    public int ShouldRetryCount { get; private set; }

    public override ShouldRetry GetShouldRetry() =>
        (int currentRetryCount, Exception lastException, out TimeSpan interval) =>
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

public record TestRetryStrategyOptions(bool FastFirstRetry, int CustomProperty) : RetryStrategyOptions(FastFirstRetry)
{
    public TestRetryStrategyOptions() : this(true, default)
    {
    }

    public TestRetryStrategy ToTestRetryStrategy(string name) => 
        new (name, this.FastFirstRetry, this.CustomProperty);
}