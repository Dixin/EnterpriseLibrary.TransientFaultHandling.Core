namespace Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling.Properties;

    /// <summary>
    /// Provides the extension methods for <see cref="RetryManagerOptions"/> class.
    /// </summary>
    public static class RetryManagerOptionsExtensions
    {
        /// <summary>
        /// Converts options to retry manager.
        /// </summary>
        /// <param name="options">The options.</param>
        /// <param name="getCustomRetryStrategy">The function to get custom retry strategy from options.</param>
        /// <returns></returns>
        public static RetryManager ToRetryManager(this RetryManagerOptions options, Func<IConfigurationSection, RetryStrategy>? getCustomRetryStrategy = null)
        {
            Argument.NotNull(options, nameof(options));
            if (options.RetryStrategy is null || !options.RetryStrategy.Exists())
            {
                throw new ArgumentException(Resources.RetryStrategySectionNotFoundInRetryManager, nameof(options));
            }

            Dictionary<string, string>? defaultStrategies = new();
            if (!string.IsNullOrWhiteSpace(options.DefaultSqlCommandRetryStrategy))
            {
                defaultStrategies.Add(RetryManagerSqlExtensions.DefaultStrategyCommandTechnologyName, options.DefaultSqlCommandRetryStrategy);
            }

            if (!string.IsNullOrWhiteSpace(options.DefaultSqlConnectionRetryStrategy))
            {
                defaultStrategies.Add(RetryManagerSqlExtensions.DefaultStrategyConnectionTechnologyName, options.DefaultSqlConnectionRetryStrategy);
            }

            //if (!string.IsNullOrWhiteSpace(options.DefaultAzureServiceBusRetryStrategy))
            //{
            //    defaultStrategies.Add(RetryManagerServiceBusExtensions.DefaultStrategyTechnologyName, options.DefaultAzureServiceBusRetryStrategy);
            //}

            //if (!string.IsNullOrWhiteSpace(options.DefaultAzureStorageRetryStrategy))
            //{
            //    defaultStrategies.Add(RetryManagerWindowsAzureStorageExtensions.DefaultStrategyTechnologyName, options.DefaultAzureStorageRetryStrategy);
            //}

            //if (!string.IsNullOrWhiteSpace(options.DefaultAzureCachingRetryStrategy))
            //{
            //    defaultStrategies.Add(RetryManagerCachingExtensions.DefaultStrategyTechnologyName, options.DefaultAzureCachingRetryStrategy);
            //}

            ICollection<RetryStrategy> retryStrategies = options.RetryStrategy.GetRetryStrategies(getCustomRetryStrategy).Values;
            return new RetryManager(retryStrategies, options.DefaultRetryStrategy, defaultStrategies);

        }
    }
}
