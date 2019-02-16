namespace Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling
{
    /// <summary>
    /// Provides the extension methods for retry strategy options.
    /// </summary>
    public static class RetryStrategyOptionsExtensions
    {
        /// <summary>
        /// Converts <see cref="Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling.FixedIntervalOptions"/> instance to <see cref="Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling.FixedInterval"/> retry strategy.
        /// </summary>
        /// <param name="options">The <see cref="Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling.FixedIntervalOptions"/> instance to convert.</param>
        /// <param name="name">The name of the retry strategy.</param>
        /// <returns>The converted <see cref="Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling.FixedInterval"/> retry strategy.</returns>
        public static FixedInterval Strategy(this FixedIntervalOptions options, string name)
        {
            Guard.ArgumentNotNull(options, nameof(options));

            return new FixedInterval(name, options.RetryCount, options.RetryInterval, options.FastFirstRetry);
        }

        /// <summary>
        /// Converts <see cref="Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling.IncrementalOptions"/> instance to <see cref="Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling.Incremental"/> retry strategy.
        /// </summary>
        /// <param name="options">The <see cref="Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling.IncrementalOptions"/> instance to convert.</param>
        /// <param name="name">The name of the retry strategy.</param>
        /// <returns>The converted <see cref="Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling.Incremental"/> retry strategy.</returns>
        public static Incremental Strategy(this IncrementalOptions options, string name)
        {
            Guard.ArgumentNotNull(options, nameof(options));

            return new Incremental(name, options.RetryCount, options.InitialInterval, options.Increment, options.FastFirstRetry);
        }

        /// <summary>
        /// Converts <see cref="Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling.ExponentialBackoffOptions"/> instance to <see cref="Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling.ExponentialBackoff"/> retry strategy.
        /// </summary>
        /// <param name="options">The <see cref="Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling.ExponentialBackoffOptions"/> instance to convert.</param>
        /// <param name="name">The name of the retry strategy.</param>
        /// <returns>The converted <see cref="Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling.ExponentialBackoff"/> retry strategy.</returns>
        public static ExponentialBackoff Strategy(this ExponentialBackoffOptions options, string name)
        {
            Guard.ArgumentNotNull(options, nameof(options));

            return new ExponentialBackoff(name, options.RetryCount, options.MinBackOff, options.MaxBackOff, options.DeltaBackOff, options.FastFirstRetry);
        }
    }
}
