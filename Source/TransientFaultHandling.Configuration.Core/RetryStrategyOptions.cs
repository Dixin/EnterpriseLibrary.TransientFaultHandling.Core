namespace Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling
{
    using System;

    public class RetryStrategyOptions
    {
        public bool FastFirstRetry { get; private set; }
    }

    public class FixedIntervalOptions : RetryStrategyOptions
    {
        public int RetryCount { get; private set; }

        public TimeSpan RetryInterval { get; private set; }
    }

    public class IncrementalOptions : RetryStrategyOptions
    {
        public int RetryCount { get; private set; }

        public TimeSpan InitialInterval { get; private set; }

        public TimeSpan Increment { get; private set; }
    }

    public class ExponentialBackoffOptions : RetryStrategyOptions
    {
        public int RetryCount { get; private set; }

        public TimeSpan MinBackOff { get; private set; }

        public TimeSpan MaxBackOff { get; private set; }

        public TimeSpan DeltaBackOff { get; private set; }
    }
}
