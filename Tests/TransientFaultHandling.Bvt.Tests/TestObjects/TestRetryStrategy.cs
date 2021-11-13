namespace Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling.Bvt.Tests.TestObjects;

public class TestRetryStrategy : RetryStrategy
{
    public TestRetryStrategy(string name, bool firstFastRetry, int customProperty)
        : base(name, firstFastRetry) =>
        this.CustomProperty = customProperty;

    public int CustomProperty { get; }

    public int ShouldRetryCount { get; private set; }

    public override ShouldRetry GetShouldRetry()
    {
        return (int currentRetryCount, Exception lastException, out TimeSpan interval) =>
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

public record TestRetryStrategyOptions(bool FastFirstRetry, int CustomProperty) : RetryStrategyOptions(FastFirstRetry)
{
    public TestRetryStrategyOptions() : this(true, default)
    {
    }

    public TestRetryStrategy ToTestRetryStrategy(string name) => 
        new (name, this.FastFirstRetry, this.CustomProperty);
}