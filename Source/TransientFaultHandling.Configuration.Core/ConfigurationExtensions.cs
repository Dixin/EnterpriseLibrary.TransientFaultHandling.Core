namespace Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling.Properties;

    /// <summary>
    /// Provides the extension methods for <see cref="IConfigurationSection" />.
    /// </summary>
    public static class ConfigurationExtensions
    {
        private static bool HasKey(this IConfigurationSection section, string key) =>
            section.GetSection(key).Exists();

        /// <summary>
        /// Gets the retry strategies from configuration.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <param name="key">The key to the retry strategy configuration section.</param>
        /// <param name="getCustomRetryStrategy">The function to get custom retry strategy from options.</param>
        /// <returns>A dictionary where keys are the names of retry strategies and values are retry strategies.</returns>
        [CLSCompliant(false)]
        public static IDictionary<string, RetryStrategy> GetRetryStrategies(
            this IConfiguration configuration,
            string key = RetryConfiguration.DefaultConfigurationKeyRetryStrategy,
            Func<IConfigurationSection, RetryStrategy>? getCustomRetryStrategy = null)
        {
            Argument.NotNull(configuration, nameof(configuration));
            Argument.NotNullOrEmpty(key, nameof(key));

            IConfigurationSection section = configuration.GetSection(key);
            return section.Exists()
                ? section.GetRetryStrategies(getCustomRetryStrategy)
                : throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, Resources.ConfigurationKeyNotFOund, key), nameof(key));
        }

        /// <summary>
        /// Gets the retry strategies from configuration section.
        /// </summary>
        /// <param name="configurationSection">The configuration section.</param>
        /// <param name="getCustomRetryStrategy">The function to ger custom retry strategy from options.</param>
        /// <returns>A dictionary where keys are the names of retry strategies and values are retry strategies.</returns>
        public static IDictionary<string, RetryStrategy> GetRetryStrategies(
            this IConfigurationSection configurationSection,
            Func<IConfigurationSection, RetryStrategy>? getCustomRetryStrategy = null)
        {
            Argument.NotNull(configurationSection, nameof(configurationSection));
            if (!configurationSection.Exists())
            {
                throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, Resources.ConfigurationSectionNotExist, configurationSection.Path), nameof(configurationSection));
            }

            string[] fixedIntervalProperties = typeof(FixedIntervalOptions).GetProperties().Select(property => property.Name).ToArray();
            string[] incrementalProperties = typeof(IncrementalOptions).GetProperties().Select(property => property.Name).ToArray();
            string[] exponentialBackoffProperties = typeof(ExponentialBackoffOptions).GetProperties().Select(property => property.Name).ToArray();

            return configurationSection.GetChildren().ToDictionary(
                options => options.Key,
                options =>
                {
                    if (fixedIntervalProperties.All(options.HasKey)
                        && !incrementalProperties.All(options.HasKey)
                        && !exponentialBackoffProperties.All(options.HasKey))
                    {
                        return options.Get<FixedIntervalOptions>(buildOptions => buildOptions.BindNonPublicProperties = true).ToFixedInterval(options.Key);
                    }

                    if (incrementalProperties.All(options.HasKey)
                        && !fixedIntervalProperties.All(options.HasKey)
                        && !exponentialBackoffProperties.All(options.HasKey))
                    {
                        return options.Get<IncrementalOptions>(buildOptions => buildOptions.BindNonPublicProperties = true).ToIncremental(options.Key);
                    }

                    if (exponentialBackoffProperties.All(options.HasKey)
                        && !incrementalProperties.All(options.HasKey)
                        && !fixedIntervalProperties.All(options.HasKey))
                    {
                        return options.Get<ExponentialBackoffOptions>(buildOptions => buildOptions.BindNonPublicProperties = true).ToExponentialBackoff(options.Key);
                    }

                    if (getCustomRetryStrategy is not null)
                    {
                        return getCustomRetryStrategy(options);
                    }

                    throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, "Current retry strategy options in section {0} are invalid.", options.Key));
                });
        }

        /// <summary>
        /// Gets the retry strategies from configuration section.
        /// </summary>
        /// <param name="configuration">The configuration section.</param>
        /// <param name="key">The key to the retry strategy configuration section.</param>
        /// <param name="getCustomRetryStrategy">The function to ger custom retry strategy from options.</param>
        /// <returns>A dictionary where keys are the names of retry strategies and values are retry strategies.</returns>
        [CLSCompliant(false)]
        public static IDictionary<string, TRetryStrategy> GetRetryStrategies<TRetryStrategy>(
            this IConfiguration configuration,
            string key = RetryConfiguration.DefaultConfigurationKeyRetryStrategy,
            Func<IConfigurationSection, TRetryStrategy>? getCustomRetryStrategy = null)
            where TRetryStrategy : RetryStrategy
        {
            Argument.NotNull(configuration, nameof(configuration));
            Argument.NotNullOrEmpty(key, nameof(key));

            Func<IConfigurationSection, RetryStrategy>? covariant = getCustomRetryStrategy;
            return GetRetryStrategies(configuration, key, covariant)
                .Where(pair => pair.Value is not null and TRetryStrategy).ToDictionary(pair => pair.Key, pair => (TRetryStrategy)pair.Value);
        }

        /// <summary>
        /// Gets the retry manager from configuration.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <param name="key">The key to the retry strategy configuration section.</param>
        /// <param name="getCustomRetryStrategy">The function to ger custom retry strategy from options.</param>
        /// <returns>An instance of <see cref="RetryManager"/> type.</returns>
        public static RetryManager GetRetryManager(
            this IConfiguration configuration,
            string key = RetryConfiguration.DefaultConfigurationKeyRetryManager,
            Func<IConfigurationSection, RetryStrategy>? getCustomRetryStrategy = null)
        {
            Argument.NotNull(configuration, nameof(configuration));
            Argument.NotNullOrEmpty(key, nameof(key));

            IConfigurationSection section = configuration.GetSection(key);
            return section.Exists()
                ? section.GetRetryManager(getCustomRetryStrategy)
                : throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, Resources.ConfigurationKeyNotFOund, key), nameof(key));
        }

        /// <summary>
        /// Gets the retry manager from configuration section.
        /// </summary>
        /// <param name="configurationSection">The configuration section.</param>
        /// <param name="getCustomRetryStrategy">The function to ger custom retry strategy from options.</param>
        /// <returns>An instance of <see cref="RetryManager"/> type.</returns>
        public static RetryManager GetRetryManager(
            this IConfigurationSection configurationSection,
            Func<IConfigurationSection, RetryStrategy>? getCustomRetryStrategy = null) =>
            Argument.NotNull(configurationSection, nameof(configurationSection)).Exists()
                ? configurationSection.Get<RetryManagerOptions>(buildOptions => buildOptions.BindNonPublicProperties = true).ToRetryManager(getCustomRetryStrategy)
                : throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, Resources.ConfigurationSectionNotExist, configurationSection.Path), nameof(configurationSection));
    }
}
