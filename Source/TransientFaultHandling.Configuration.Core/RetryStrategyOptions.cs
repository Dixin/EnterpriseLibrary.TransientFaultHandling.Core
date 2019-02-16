namespace Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling
{
    using System;

    /// <summary>
    /// Represents the options for <see cref="Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling.RetryStrategy" />.
    /// </summary>
    public class RetryStrategyOptions
    {
        /// <summary>
        /// Gets or sets a value indicating whether the first retry attempt will be made immediately, whereas subsequent retries will remain subject to the retry interval.
        /// </summary>
        public bool FastFirstRetry { get; private set; }
    }

    /// <summary>
    /// Represents the options for <see cref="Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling.FixedInterval" /> retry strategy.
    /// </summary>
    public class FixedIntervalOptions : RetryStrategyOptions
    {
        /// <summary>
        /// Gets the maximum number of retry attempts.
        /// </summary>
        public int RetryCount { get; private set; }

        /// <summary>
        /// Gets the time interval between retries.
        /// </summary>
        public TimeSpan RetryInterval { get; private set; }
    }

    /// <summary>
    /// Represents the options for <see cref="Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling.Incremental" /> retry strategy.
    /// </summary>
    public class IncrementalOptions : RetryStrategyOptions
    {
        /// <summary>
        /// Gets the maximum number of retry attempts.
        /// </summary>
        public int RetryCount { get; private set; }

        /// <summary>
        /// Gets the initial interval that will apply for the first retry.
        /// </summary>
        public TimeSpan InitialInterval { get; private set; }

        /// <summary>
        /// Gets the incremental time value that will be used to calculate the progressive delay between retries..
        /// </summary>
        public TimeSpan Increment { get; private set; }
    }

    /// <summary>
    /// Represents the options for <see cref="Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling.ExponentialBackoff" /> retry strategy.
    /// </summary>
    public class ExponentialBackoffOptions : RetryStrategyOptions
    {
        /// <summary>
        /// Gets the maximum number of retry attempts.
        /// </summary>
        public int RetryCount { get; private set; }

        /// <summary>
        /// Gets the minimum backoff time.
        /// </summary>
        public TimeSpan MinBackOff { get; private set; }

        /// <summary>
        /// Gets the maximum backoff time.
        /// </summary>
        public TimeSpan MaxBackOff { get; private set; }

        /// <summary>
        /// Gets the value that will be used to calculate a random delta in the exponential delay between retries.
        /// </summary>
        public TimeSpan DeltaBackOff { get; private set; }
    }
}
