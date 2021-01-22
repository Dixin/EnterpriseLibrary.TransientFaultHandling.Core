namespace Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling
{
    using System;

    /// <summary>
    /// Represents the options for <see cref="Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling.RetryStrategy" />.
    /// </summary>
    public abstract record RetryStrategyOptions(bool FastFirstRetry);

    /// <summary>
    /// Represents the options for <see cref="Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling.FixedInterval" /> retry strategy.
    /// </summary>
    public record FixedIntervalOptions(bool FastFirstRetry, int RetryCount, TimeSpan RetryInterval) : RetryStrategyOptions(FastFirstRetry)
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FixedIntervalOptions" /> record. 
        /// </summary>
        public FixedIntervalOptions() : this(true, default, default)
        {
        }
    }

    /// <summary>
    /// Represents the options for <see cref="Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling.Incremental" /> retry strategy.
    /// </summary>
    public record IncrementalOptions(bool FastFirstRetry, int RetryCount, TimeSpan InitialInterval, TimeSpan Increment) : RetryStrategyOptions(FastFirstRetry)
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IncrementalOptions" /> record. 
        /// </summary>
        public IncrementalOptions() : this(true, default, default, default)
        {
        }
    }

    /// <summary>
    /// Represents the options for <see cref="Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling.ExponentialBackoff" /> retry strategy.
    /// </summary>
    public record ExponentialBackoffOptions(bool FastFirstRetry, int RetryCount, TimeSpan MinBackOff, TimeSpan MaxBackOff, TimeSpan DeltaBackOff) : RetryStrategyOptions(FastFirstRetry)
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExponentialBackoffOptions" /> record. 
        /// </summary>
        public ExponentialBackoffOptions() : this(true, default, default, default, default)
        {
        }
    }
}
