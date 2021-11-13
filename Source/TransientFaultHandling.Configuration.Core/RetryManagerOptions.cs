namespace Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling;

/// <summary>
/// Represents the options for <see cref="RetryManager"/>.
/// </summary>
public record RetryManagerOptions(
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    string? DefaultRetryStrategy,
    string? DefaultSqlConnectionRetryStrategy,
    string? DefaultSqlCommandRetryStrategy,
    string? DefaultAzureServiceBusRetryStrategy,
    string? DefaultAzureCachingRetryStrategy,
    string? DefaultAzureStorageRetryStrategy,
    IConfigurationSection? RetryStrategy)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RetryManagerOptions" /> record. 
    /// </summary>
    public RetryManagerOptions() : this(default, default, default, default, default, default, default)
    {
    }
}