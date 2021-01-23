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
        private readonly int retryCount;

        private readonly TimeSpan retryInterval;

        /// <summary>
        /// Initializes a new instance of the <see cref="FixedIntervalOptions" /> record. 
        /// </summary>
        public FixedIntervalOptions() : this(true, default, default)
        {
        }

        /// <summary>
        /// Gets or sets the retry count.
        /// </summary>
        public int RetryCount
        {
            get => this.retryCount;
            init => this.retryCount = Argument.NotNegative(value, nameof(this.RetryCount));
        }

        /// <summary>
        /// Gets the time interval between retries.
        /// </summary>
        public TimeSpan RetryInterval
        {
            get => this.retryInterval;
            init => this.retryInterval = Argument.NotNegative(value, nameof(this.RetryInterval));
        }
    }

    /// <summary>
    /// Represents the options for <see cref="Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling.Incremental" /> retry strategy.
    /// </summary>
    public record IncrementalOptions(bool FastFirstRetry, int RetryCount, TimeSpan InitialInterval, TimeSpan Increment) : RetryStrategyOptions(FastFirstRetry)
    {
        private readonly int retryCount;

        private readonly TimeSpan initialInterval;

        private readonly TimeSpan increment;

        /// <summary>
        /// Initializes a new instance of the <see cref="IncrementalOptions" /> record. 
        /// </summary>
        public IncrementalOptions() : this(true, default, default, default)
        {
        }

        /// <summary>
        /// Gets the maximum number of retry attempts.
        /// </summary>
        public int RetryCount
        {
            get => this.retryCount;
            init => this.retryCount = Argument.NotNegative(value, nameof(this.RetryCount));
        }

        /// <summary>
        /// Gets the initial interval that will apply for the first retry.
        /// </summary>
        public TimeSpan InitialInterval
        {
            get => this.initialInterval;
            init => this.initialInterval = Argument.NotNegative(value, nameof(this.InitialInterval));
        }

        /// <summary>
        /// Gets the incremental time value that will be used to calculate the progressive delay between retries..
        /// </summary>
        public TimeSpan Increment
        {
            get => this.increment;
            init => this.increment = Argument.NotNegative(value, nameof(this.Increment));
        }
    }

    /// <summary>
    /// Represents the options for <see cref="Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling.ExponentialBackoff" /> retry strategy.
    /// </summary>
    public record ExponentialBackoffOptions(bool FastFirstRetry, int RetryCount, TimeSpan MinBackOff, TimeSpan MaxBackOff, TimeSpan DeltaBackOff) : RetryStrategyOptions(FastFirstRetry)
    {

        private readonly int retryCount;

        private readonly TimeSpan minBackOff;

        private readonly TimeSpan maxBackOff;

        private readonly TimeSpan deltaBackOff;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExponentialBackoffOptions" /> record. 
        /// </summary>
        public ExponentialBackoffOptions() : this(true, default, default, default, default)
        {
        }

        /// <summary>
        /// Gets the maximum number of retry attempts.
        /// </summary>
        public int RetryCount
        {
            get => this.retryCount;
            init => this.retryCount = Argument.NotNegative(value, nameof(this.RetryCount));
        }

        /// <summary>
        /// Gets the minimum backoff time.
        /// </summary>
        public TimeSpan MinBackOff
        {
            get => this.minBackOff;
            init => this.minBackOff = Argument.NotNegative(value, nameof(this.MinBackOff));
        }

        /// <summary>
        /// Gets the maximum backoff time.
        /// </summary>
        public TimeSpan MaxBackOff
        {
            get => this.maxBackOff;
            init => this.maxBackOff = Argument.NotNegative(value, nameof(this.MaxBackOff));
        }

        /// <summary>
        /// Gets the value that will be used to calculate a random delta in the exponential delay between retries.
        /// </summary>
        public TimeSpan DeltaBackOff
        {
            get => this.deltaBackOff;
            init => this.deltaBackOff = Argument.NotNegative(value, nameof(this.DeltaBackOff));
        }
    }
}
