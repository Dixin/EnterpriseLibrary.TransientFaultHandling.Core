namespace Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using Microsoft.Extensions.Configuration;

    /// <summary>
    /// Provides the extension methods for <see cref="Microsoft.Extensions.Configuration.IConfigurationSection" />.
    /// </summary>
    public static class ConfigurationExtensions
    {
        private static bool Has(this IConfigurationSection section, string key) =>
            section.GetSection(key).Exists();

        /// <summary>
        /// Gets the retry strategies from configuration section.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <param name="key">The key.</param>
        /// <param name="getCustomRetryStrategy">The function to ger custom retry strategy from options.</param>
        /// <returns>A dictionary where keys are the names of retry strategies and values are retry strategies.</returns>
        [CLSCompliant(false)]
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

                    throw new InvalidOperationException(string.Format(
                        CultureInfo.CurrentCulture, "Current retry strategy options in section {0}:{1} are invalid.", key, options.Key));
                });
        }

        /// <summary>
        /// Gets the retry strategies from configuration section.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <param name="key">The key.</param>
        /// <param name="getCustomRetryStrategy">The function to ger custom retry strategy from options.</param>
        /// <returns>A dictionary where keys are the names of retry strategies and values are retry strategies.</returns>
        [CLSCompliant(false)]
        public static Dictionary<string, TRetryStrategy> GetRetryStrategies<TRetryStrategy>(this IConfiguration configuration,
            string key, Func<IConfigurationSection, TRetryStrategy> getCustomRetryStrategy = null)
            where TRetryStrategy : RetryStrategy
        {
            Guard.ArgumentNotNull(configuration, nameof(configuration));

            Func<IConfigurationSection, RetryStrategy> covariant = getCustomRetryStrategy;
            return GetRetryStrategies(configuration, key, covariant)
                .Where(pair => pair.Value != null && pair.Value is TRetryStrategy).ToDictionary(pair => pair.Key, pair => (TRetryStrategy)pair.Value);
        }
    }
}
