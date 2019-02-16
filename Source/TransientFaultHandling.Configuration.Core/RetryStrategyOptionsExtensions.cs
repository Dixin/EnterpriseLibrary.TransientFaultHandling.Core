namespace Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling
{
    public static class RetryStrategyOptionsExtensions
    {
        public static FixedInterval Strategy(this FixedIntervalOptions options, string name)
        {
            Guard.ArgumentNotNull(options, nameof(options));

            return new FixedInterval(name, options.RetryCount, options.RetryInterval, options.FastFirstRetry);
        }

        public static Incremental Strategy(this IncrementalOptions options, string name)
        {
            Guard.ArgumentNotNull(options, nameof(options));

            return new Incremental(name, options.RetryCount, options.InitialInterval, options.Increment, options.FastFirstRetry);
        }

        public static ExponentialBackoff Strategy(this ExponentialBackoffOptions options, string name)
        {
            Guard.ArgumentNotNull(options, nameof(options));

            return new ExponentialBackoff(name, options.RetryCount, options.MinBackOff, options.MaxBackOff, options.DeltaBackOff, options.FastFirstRetry);
        }
    }
}
