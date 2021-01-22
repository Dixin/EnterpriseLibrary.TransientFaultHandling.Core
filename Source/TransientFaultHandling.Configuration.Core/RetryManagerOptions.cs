namespace Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling
{
    using Microsoft.Extensions.Configuration;

    /// <summary>
    /// Represents the options for <see cref="RetryManager"/>.
    /// </summary>
    public record RetryManagerOptions(
        string? DefaultRetryStrategy,
        string? DefaultSqlConnectionRetryStrategy,
        string? DefaultSqlCommandRetryStrategy,
        string? DefaultAzureServiceBusRetryStrategy,
        string? DefaultAzureCachingRetryStrategy,
        string? DefaultAzureStorageRetryStrategy,
        IConfigurationSection? RetryStrategy)
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RetryManagerOptions" /> record. 
        /// </summary>
        public RetryManagerOptions() : this(default, default, default, default, default, default, default)
        {
        }
    }
}
