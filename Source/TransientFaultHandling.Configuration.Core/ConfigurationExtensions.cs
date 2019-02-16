namespace Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Extensions.Configuration;

    public static class ConfigurationExtensions
    {
        private static bool Has(this IConfigurationSection section, string key) =>
            section.GetSection(key).Exists();

        public static Dictionary<string, RetryStrategy> GetRetryStrategies(this IConfiguration configuration, string key, Func<IConfigurationSection, RetryStrategy> getCustomRetryStrategy = null)
        {
            Guard.ArgumentNotNull(configuration, nameof(configuration));

            string[] fixedIntervalProperties = typeof(FixedIntervalOptions).GetProperties().Select(property => property.Name).ToArray();
            string[] incrementalProperties = typeof(IncrementalOptions).GetProperties().Select(property => property.Name).ToArray();
            string[] exponentialBackoffProperties = typeof(ExponentialBackoffOptions).GetProperties().Select(property => property.Name).ToArray();

            return configuration.GetSection(key).GetChildren().ToDictionary(
                options => options.Key,
                options =>
                {
                    if (fixedIntervalProperties.All(options.Has)
                        && !incrementalProperties.All(options.Has)
                        && !exponentialBackoffProperties.All(options.Has))
                    {
                        return options.Get<FixedIntervalOptions>(buildOptions => buildOptions.BindNonPublicProperties = true).Strategy(options.Key);
                    }

                    if (incrementalProperties.All(options.Has)
                        && !fixedIntervalProperties.All(options.Has)
                        && !exponentialBackoffProperties.All(options.Has))
                    {
                        return options.Get<IncrementalOptions>(buildOptions => buildOptions.BindNonPublicProperties = true).Strategy(options.Key);
                    }

                    if (exponentialBackoffProperties.All(options.Has)
                        && !incrementalProperties.All(options.Has)
                        && !fixedIntervalProperties.All(options.Has))
                    {
                        return options.Get<ExponentialBackoffOptions>(buildOptions => buildOptions.BindNonPublicProperties = true).Strategy(options.Key);
                    }

                    if (getCustomRetryStrategy != null)
                    {
                        return getCustomRetryStrategy(options);
                    }

                    throw new InvalidOperationException("Current retry strategy options are invalid.");
                });
        }
    }
}
