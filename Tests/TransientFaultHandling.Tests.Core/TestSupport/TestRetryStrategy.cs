namespace Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling.TestSupport
{
    using System;
    using Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling;

    public class TestRetryStrategy : RetryStrategy
    {
        public TestRetryStrategy()
            : base(nameof(TestRetryStrategy), true) =>
            this.CustomProperty = 1;

        public int CustomProperty { get; }

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
}
