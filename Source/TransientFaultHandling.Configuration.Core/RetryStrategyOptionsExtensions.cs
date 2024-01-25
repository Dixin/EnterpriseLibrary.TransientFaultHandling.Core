namespace Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling;

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
    public static FixedInterval ToFixedInterval(this FixedIntervalOptions options, string name) => 
        new (name, options.ThrowIfNull().RetryCount, options.RetryInterval, options.FastFirstRetry);

    /// <summary>
    /// Converts <see cref="Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling.IncrementalOptions"/> instance to <see cref="Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling.Incremental"/> retry strategy.
    /// </summary>
    /// <param name="options">The <see cref="Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling.IncrementalOptions"/> instance to convert.</param>
    /// <param name="name">The name of the retry strategy.</param>
    /// <returns>The converted <see cref="Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling.Incremental"/> retry strategy.</returns>
    public static Incremental ToIncremental(this IncrementalOptions options, string name) => 
        new (name, options.ThrowIfNull().RetryCount, options.InitialInterval, options.Increment, options.FastFirstRetry);

    /// <summary>
    /// Converts <see cref="Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling.ExponentialBackoffOptions"/> instance to <see cref="Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling.ExponentialBackoff"/> retry strategy.
    /// </summary>
    /// <param name="options">The <see cref="Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling.ExponentialBackoffOptions"/> instance to convert.</param>
    /// <param name="name">The name of the retry strategy.</param>
    /// <returns>The converted <see cref="Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling.ExponentialBackoff"/> retry strategy.</returns>
    public static ExponentialBackoff ToExponentialBackoff(this ExponentialBackoffOptions options, string name) => 
        new (name, options.ThrowIfNull().RetryCount, options.MinBackOff, options.MaxBackOff, options.DeltaBackOff, options.FastFirstRetry);

    /// <summary>
    /// Converts <see cref="Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling.FixedIntervalOptions"/> instance to <see cref="Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling.FixedInterval"/> retry strategy.
    /// </summary>
    /// <param name="options">The <see cref="Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling.FixedIntervalOptions"/> instance to convert.</param>
    /// <param name="name">The name of the retry strategy.</param>
    /// <returns>The converted <see cref="Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling.FixedInterval"/> retry strategy.</returns>
    [Obsolete("Use Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling.RetryStrategyOptionsExtensions.ToFixedInterval")]
    public static FixedInterval Strategy(this FixedIntervalOptions options, string name) => 
        ToFixedInterval(options, name);

    /// <summary>
    /// Converts <see cref="Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling.IncrementalOptions"/> instance to <see cref="Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling.Incremental"/> retry strategy.
    /// </summary>
    /// <param name="options">The <see cref="Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling.IncrementalOptions"/> instance to convert.</param>
    /// <param name="name">The name of the retry strategy.</param>
    /// <returns>The converted <see cref="Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling.Incremental"/> retry strategy.</returns>
    [Obsolete("Use Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling.RetryStrategyOptionsExtensions.ToIncremental")]
    public static Incremental Strategy(this IncrementalOptions options, string name) => 
        ToIncremental(options, name);

    /// <summary>
    /// Converts <see cref="Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling.ExponentialBackoffOptions"/> instance to <see cref="Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling.ExponentialBackoff"/> retry strategy.
    /// </summary>
    /// <param name="options">The <see cref="Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling.ExponentialBackoffOptions"/> instance to convert.</param>
    /// <param name="name">The name of the retry strategy.</param>
    /// <returns>The converted <see cref="Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling.ExponentialBackoff"/> retry strategy.</returns>
    [Obsolete("Use Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling.RetryStrategyOptionsExtensions.ToExponentialBackoff")]
    public static ExponentialBackoff Strategy(this ExponentialBackoffOptions options, string name) => 
        ToExponentialBackoff(options, name);
}