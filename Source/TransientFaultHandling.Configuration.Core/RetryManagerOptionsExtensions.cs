namespace Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Extensions.Configuration;

    public static class RetryManagerOptionsExtensions
    {
        public static RetryManager ToRetryManager(this RetryManagerOptions options, Func<IConfigurationSection, RetryStrategy>? getCustomRetryStrategy = null)
        {
            Guard.ArgumentNotNull(options, nameof(options));
            if (options.RetryStrategy is null || !options.RetryStrategy.Exists())
            {
                throw new ArgumentException("The retry manager options does not have the section for retry strategies", nameof(options));
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
