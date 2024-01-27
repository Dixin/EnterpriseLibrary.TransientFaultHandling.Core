namespace Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling;

/// <summary>
/// A retry strategy with a specified number of retry attempts and an incremental time interval between retries.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="T:Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling.Incremental" /> class with the specified number of retry attempts, time interval, retry strategy, and fast start option. 
/// </remarks>
/// <param name="name">The retry strategy name.</param>
/// <param name="retryCount">The number of retry attempts.</param>
/// <param name="initialInterval">The initial interval that will apply for the first retry.</param>
/// <param name="increment">The incremental time value that will be used to calculate the progressive delay between retries.</param>
/// <param name="firstFastRetry">true to immediately retry in the first attempt; otherwise, false. The subsequent retries will remain subject to the configured retry interval.</param>
public class Incremental(string? name, int retryCount, TimeSpan initialInterval, TimeSpan increment, bool firstFastRetry) : RetryStrategy(name, firstFastRetry)
{
    private readonly int retryCount = retryCount.ThrowIfNegative();

    private readonly TimeSpan initialInterval = initialInterval.ThrowIfNegative();

    private readonly TimeSpan increment = increment.ThrowIfNegative();

    /// <summary>
    /// Initializes a new instance of the <see cref="T:Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling.Incremental" /> class. 
    /// </summary>
    public Incremental() : this(DefaultClientRetryCount, DefaultRetryInterval, DefaultRetryIncrement)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling.Incremental" /> class with the specified retry settings.
    /// </summary>
    /// <param name="retryCount">The number of retry attempts.</param>
    /// <param name="initialInterval">The initial interval that will apply for the first retry.</param>
    /// <param name="increment">The incremental time value that will be used to calculate the progressive delay between retries.</param>
    public Incremental(int retryCount, TimeSpan initialInterval, TimeSpan increment) : this(null, retryCount, initialInterval, increment)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling.Incremental" /> class with the specified name and retry settings.
    /// </summary>
    /// <param name="name">The retry strategy name.</param>
    /// <param name="retryCount">The number of retry attempts.</param>
    /// <param name="initialInterval">The initial interval that will apply for the first retry.</param>
    /// <param name="increment">The incremental time value that will be used to calculate the progressive delay between retries.</param>
    public Incremental(string? name, int retryCount, TimeSpan initialInterval, TimeSpan increment) : this(name, retryCount, initialInterval, increment, DefaultFirstFastRetry)
    {
    }

    /// <summary>
    /// Returns the corresponding ShouldRetry delegate.
    /// </summary>
    /// <returns>The ShouldRetry delegate.</returns>
    public override ShouldRetry GetShouldRetry() =>
        (int currentRetryCount, Exception lastException, out TimeSpan retryInterval) =>
        {
            if (currentRetryCount < this.retryCount)
            {
                retryInterval = TimeSpan.FromMilliseconds(
                    this.initialInterval.TotalMilliseconds + this.increment.TotalMilliseconds * currentRetryCount);
                return true;
            }

            retryInterval = TimeSpan.Zero;
            return false;
        };
}