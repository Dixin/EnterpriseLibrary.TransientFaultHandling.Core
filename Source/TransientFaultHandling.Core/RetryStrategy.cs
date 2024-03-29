﻿namespace Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling;

/// <summary>
/// Represents a retry strategy that determines the number of retry attempts and the interval between retries.
/// </summary>
public abstract class RetryStrategy
{
    /// <summary>
    /// Represents the default number of retry attempts.
    /// </summary>
    public static readonly int DefaultClientRetryCount = 10;

    /// <summary>
    /// Represents the default amount of time used when calculating a random delta in the exponential delay between retries.
    /// </summary>
    public static readonly TimeSpan DefaultClientBackoff = TimeSpan.FromSeconds(10.0);

    /// <summary>
    /// Represents the default maximum amount of time used when calculating the exponential delay between retries.
    /// </summary>
    public static readonly TimeSpan DefaultMaxBackoff = TimeSpan.FromSeconds(30.0);

    /// <summary>
    /// Represents the default minimum amount of time used when calculating the exponential delay between retries.
    /// </summary>
    public static readonly TimeSpan DefaultMinBackoff = TimeSpan.FromSeconds(1.0);

    /// <summary>
    /// Represents the default interval between retries.
    /// </summary>
    public static readonly TimeSpan DefaultRetryInterval = TimeSpan.FromSeconds(1.0);

    /// <summary>
    /// Represents the default time increment between retry attempts in the progressive delay policy.
    /// </summary>
    public static readonly TimeSpan DefaultRetryIncrement = TimeSpan.FromSeconds(1.0);

    /// <summary>
    /// Represents the default flag indicating whether the first retry attempt will be made immediately,
    /// whereas subsequent retries will remain subject to the retry interval.
    /// </summary>
    public static readonly bool DefaultFirstFastRetry = true;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling.RetryStrategy" /> class. 
    /// </summary>
    /// <param name="name">The name of the retry strategy.</param>
    /// <param name="firstFastRetry">true to immediately retry in the first attempt; otherwise, false. The subsequent retries will remain subject to the configured retry interval.</param>
    protected RetryStrategy(string? name, bool firstFastRetry)
    {
        this.Name = name;
        this.FastFirstRetry = firstFastRetry;
    }

    /// <summary>
    /// Returns a default policy that performs no retries, but invokes the action only once.
    /// </summary>
    public static RetryStrategy NoRetry { get; } = new FixedInterval(0, DefaultRetryInterval);

    /// <summary>
    /// Returns a default policy that implements a fixed retry interval configured with the <see cref="F:Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling.RetryStrategy.DefaultClientRetryCount" /> and <see cref="F:Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling.RetryStrategy.DefaultRetryInterval" /> parameters.
    /// The default retry policy treats all caught exceptions as transient errors.
    /// </summary>
    public static RetryStrategy DefaultFixed { get; } = new FixedInterval(DefaultClientRetryCount, DefaultRetryInterval);

    /// <summary>
    /// Returns a default policy that implements a progressive retry interval configured with the <see cref="F:Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling.RetryStrategy.DefaultClientRetryCount" />, <see cref="F:Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling.RetryStrategy.DefaultRetryInterval" />, and <see cref="F:Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling.RetryStrategy.DefaultRetryIncrement" /> parameters.
    /// The default retry policy treats all caught exceptions as transient errors.
    /// </summary>
    public static RetryStrategy DefaultProgressive { get; } = new Incremental(DefaultClientRetryCount, DefaultRetryInterval, DefaultRetryIncrement);

    /// <summary>
    /// Returns a default policy that implements a random exponential retry interval configured with the <see cref="F:Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling.RetryStrategy.DefaultClientRetryCount" />, <see cref="F:Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling.RetryStrategy.DefaultMinBackoff" />, <see cref="F:Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling.RetryStrategy.DefaultMaxBackoff" />, and <see cref="F:Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling.RetryStrategy.DefaultClientBackoff" /> parameters.
    /// The default retry policy treats all caught exceptions as transient errors.
    /// </summary>
    public static RetryStrategy DefaultExponential { get; } = new ExponentialBackoff(DefaultClientRetryCount, DefaultMinBackoff, DefaultMaxBackoff, DefaultClientBackoff);

    /// <summary>
    /// Gets or sets a value indicating whether the first retry attempt will be made immediately, whereas subsequent retries will remain subject to the retry interval.
    /// </summary>
    public bool FastFirstRetry { get; set; }

    /// <summary>
    /// Gets the name of the retry strategy.
    /// </summary>
    public string? Name { get; }

    /// <summary>
    /// Returns the corresponding ShouldRetry delegate.
    /// </summary>
    /// <returns>The ShouldRetry delegate.</returns>
    public abstract ShouldRetry GetShouldRetry();
}