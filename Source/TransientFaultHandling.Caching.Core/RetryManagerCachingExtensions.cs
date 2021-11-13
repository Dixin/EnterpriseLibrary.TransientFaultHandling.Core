namespace Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling;

using System.ComponentModel;

/// <summary>
/// Provides extensions to the <see cref="RetryManager"/> class for using the Windows Azure Caching retry strategy.
/// </summary>
public static class RetryManagerCachingExtensions
{
    /// <summary>
    /// The technology name that can be used to get the default Caching retry strategy name.
    /// </summary>
    public const string DefaultStrategyTechnologyName = "Caching";

    /// <summary>
    /// Returns the default retry strategy for Windows Azure Caching.
    /// </summary>
    /// <returns>The default Windows Azure Caching retry strategy (or the default strategy if no default could be found for Windows Azure Caching).</returns>
    [Obsolete("Use GetDefaultCachingRetryStrategy instead.")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static RetryStrategy GetDefaultAzureCachingRetryStrategy(this RetryManager retryManager) => 
        retryManager.NotNull().GetDefaultRetryStrategy(DefaultStrategyTechnologyName);

    /// <summary>
    /// Returns the default retry policy dedicated to handling transient conditions with Windows Azure Caching.
    /// </summary>
    /// <returns>The retry policy for Windows Azure Caching with the corresponding default strategy (or the default strategy if no retry strategy definition assigned to Windows Azure Caching was found).</returns>
    [Obsolete("Use GetDefaultCachingRetryPolicy instead.")]
    public static RetryPolicy GetDefaultAzureCachingRetryPolicy(this RetryManager retryManager) => 
        new (new CacheTransientErrorDetectionStrategy(), retryManager.NotNull().GetDefaultCachingRetryStrategy());

    /// <summary>
    /// Returns the default retry strategy for Windows Azure Caching.
    /// </summary>
    /// <returns>The default Windows Azure Caching retry strategy (or the default strategy if no default could be found for Windows Azure Caching).</returns>
    public static RetryStrategy GetDefaultCachingRetryStrategy(this RetryManager retryManager) => 
        retryManager.NotNull().GetDefaultRetryStrategy(DefaultStrategyTechnologyName);

    /// <summary>
    /// Returns the default retry policy dedicated to handling transient conditions with Windows Azure Caching.
    /// </summary>
    /// <returns>The retry policy for Windows Azure Caching with the corresponding default strategy (or the default strategy if no retry strategy definition for Windows Azure Caching was found).</returns>
    public static RetryPolicy GetDefaultCachingRetryPolicy(this RetryManager retryManager) => 
        new (new CacheTransientErrorDetectionStrategy(), retryManager.NotNull().GetDefaultCachingRetryStrategy());
}