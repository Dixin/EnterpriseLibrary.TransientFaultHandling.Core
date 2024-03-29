﻿namespace Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling;

/// <summary>
/// A retry strategy with backoff parameters for calculating the exponential delay between retries.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="T:Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling.ExponentialBackoff" /> class with the specified name, retry settings, and fast retry option.
/// </remarks>
/// <param name="name">The name of the retry strategy.</param>
/// <param name="retryCount">The maximum number of retry attempts.</param>
/// <param name="minBackoff">The minimum backoff time</param>
/// <param name="maxBackoff">The maximum backoff time.</param>
/// <param name="deltaBackoff">The value that will be used to calculate a random delta in the exponential delay between retries.</param>
/// <param name="firstFastRetry">true to immediately retry in the first attempt; otherwise, false. The subsequent retries will remain subject to the configured retry interval.</param>
public class ExponentialBackoff(string? name, int retryCount, TimeSpan minBackoff, TimeSpan maxBackoff, TimeSpan deltaBackoff, bool firstFastRetry) : RetryStrategy(name, firstFastRetry)
{
    private readonly int retryCount = retryCount.ThrowIfNegative();

    private readonly TimeSpan minBackoff = minBackoff.ThrowIfOutOfRange(TimeSpan.Zero, maxBackoff);

    private readonly TimeSpan maxBackoff = maxBackoff.ThrowIfNegative();

    private readonly TimeSpan deltaBackoff = deltaBackoff.ThrowIfNegative();

    /// <summary>
    /// Initializes a new instance of the <see cref="T:Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling.ExponentialBackoff" /> class. 
    /// </summary>
    public ExponentialBackoff() : 
        this(DefaultClientRetryCount, DefaultMinBackoff, DefaultMaxBackoff, DefaultClientBackoff)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling.ExponentialBackoff" /> class with the specified retry settings.
    /// </summary>
    /// <param name="retryCount">The maximum number of retry attempts.</param>
    /// <param name="minBackoff">The minimum backoff time.</param>
    /// <param name="maxBackoff">The maximum backoff time.</param>
    /// <param name="deltaBackoff">The value that will be used to calculate a random delta in the exponential delay between retries.</param>
    public ExponentialBackoff(int retryCount, TimeSpan minBackoff, TimeSpan maxBackoff, TimeSpan deltaBackoff) : 
        this(null, retryCount, minBackoff, maxBackoff, deltaBackoff, DefaultFirstFastRetry)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling.ExponentialBackoff" /> class with the specified name and retry settings.
    /// </summary>
    /// <param name="name">The name of the retry strategy.</param>
    /// <param name="retryCount">The maximum number of retry attempts.</param>
    /// <param name="minBackoff">The minimum backoff time</param>
    /// <param name="maxBackoff">The maximum backoff time.</param>
    /// <param name="deltaBackoff">The value that will be used to calculate a random delta in the exponential delay between retries.</param>
    public ExponentialBackoff(string? name, int retryCount, TimeSpan minBackoff, TimeSpan maxBackoff, TimeSpan deltaBackoff) : 
        this(name, retryCount, minBackoff, maxBackoff, deltaBackoff, DefaultFirstFastRetry)
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
                Random random = new();
                int backoffMillisecond = (int)((Math.Pow(2.0, currentRetryCount) - 1.0) * random.Next((int)(this.deltaBackoff.TotalMilliseconds * 0.8), (int)(this.deltaBackoff.TotalMilliseconds * 1.2)));
                int retryIntervalMillisecond = (int)Math.Min(this.minBackoff.TotalMilliseconds + backoffMillisecond, this.maxBackoff.TotalMilliseconds);
                retryInterval = TimeSpan.FromMilliseconds(retryIntervalMillisecond);
                return true;
            }

            retryInterval = TimeSpan.Zero;
            return false;
        };
}