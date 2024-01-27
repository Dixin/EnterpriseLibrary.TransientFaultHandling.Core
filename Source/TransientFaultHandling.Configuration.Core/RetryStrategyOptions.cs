namespace Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling;

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
    public FixedIntervalOptions() : this(RetryStrategy.DefaultFirstFastRetry, default, default)
    {
    }

    /// <summary>
    /// Gets or sets the retry count.
    /// </summary>
    public int RetryCount { get; init; } = RetryCount.ThrowIfNegative();

    /// <summary>
    /// Gets the time interval between retries.
    /// </summary>
    public TimeSpan RetryInterval { get; init; } = RetryInterval.ThrowIfNegative();
}

/// <summary>
/// Represents the options for <see cref="Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling.Incremental" /> retry strategy.
/// </summary>
public record IncrementalOptions(bool FastFirstRetry, int RetryCount, TimeSpan InitialInterval, TimeSpan Increment) : RetryStrategyOptions(FastFirstRetry)
{
    /// <summary>
    /// Initializes a new instance of the <see cref="IncrementalOptions" /> record. 
    /// </summary>
    public IncrementalOptions() : this(RetryStrategy.DefaultFirstFastRetry, default, default, default)
    {
    }

    /// <summary>
    /// Gets the maximum number of retry attempts.
    /// </summary>
    public int RetryCount { get; init; } = RetryCount.ThrowIfNegative();

    /// <summary>
    /// Gets the initial interval that will apply for the first retry.
    /// </summary>
    public TimeSpan InitialInterval { get; init; } = InitialInterval.ThrowIfNegative();

    /// <summary>
    /// Gets the incremental time value that will be used to calculate the progressive delay between retries..
    /// </summary>
    public TimeSpan Increment { get; init; } = Increment.ThrowIfNegative();
}

/// <summary>
/// Represents the options for <see cref="Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling.ExponentialBackoff" /> retry strategy.
/// </summary>
public record ExponentialBackoffOptions(bool FastFirstRetry, int RetryCount, TimeSpan MinBackOff, TimeSpan MaxBackOff, TimeSpan DeltaBackOff) : RetryStrategyOptions(FastFirstRetry)
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ExponentialBackoffOptions" /> record. 
    /// </summary>
    public ExponentialBackoffOptions() : this(RetryStrategy.DefaultFirstFastRetry, default, default, default, default)
    {
    }

    /// <summary>
    /// Gets the maximum number of retry attempts.
    /// </summary>
    public int RetryCount { get; init; } = RetryCount.ThrowIfNegative();

    /// <summary>
    /// Gets the minimum backoff time.
    /// </summary>
    public TimeSpan MinBackOff { get; init; } = MinBackOff.ThrowIfNegative();

    /// <summary>
    /// Gets the maximum backoff time.
    /// </summary>
    public TimeSpan MaxBackOff { get; init; } = MaxBackOff.ThrowIfNegative();

    /// <summary>
    /// Gets the value that will be used to calculate a random delta in the exponential delay between retries.
    /// </summary>
    public TimeSpan DeltaBackOff { get; init; } = DeltaBackOff.ThrowIfNegative();
}