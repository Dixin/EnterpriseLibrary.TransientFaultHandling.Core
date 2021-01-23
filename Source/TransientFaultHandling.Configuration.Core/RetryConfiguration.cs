namespace Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling.Properties;

    /// <summary>
    /// Provides the methods to load the configuration files.
    /// </summary>
    public static class RetryConfiguration
    {
        /// <summary>
        /// The default configuration file name.
        /// </summary>
        public const string DefaultConfigurationFile = "appsettings.json";

        /// <summary>
        /// The default section key for the retry strategies configuration,
        /// </summary>
        public const string DefaultConfigurationKeyRetryStrategy = nameof(RetryStrategy);

        /// <summary>
        /// The default section key for the retry manager configuration,
        /// </summary>
        public const string DefaultConfigurationKeyRetryManager = nameof(RetryManager);

        /// <summary>
        /// Gets the retry strategies from the specified configuration file and key.
        /// </summary>
        /// <param name="configurationFile">The specified configuration files.</param>
        /// <param name="configurationKey">The specified key in the configuration file.</param>
        /// <param name="getCustomRetryStrategy">The function to get custom retry strategy from options.</param>
        /// <returns>The retry strategies dictionary, where keys are the retry strategy names..</returns>
        public static IDictionary<string, RetryStrategy> GetRetryStrategies(
            string configurationFile = DefaultConfigurationFile, 
            string configurationKey = DefaultConfigurationKeyRetryStrategy, 
            Func<IConfigurationSection, RetryStrategy>? getCustomRetryStrategy = null)
        {
            Guard.ArgumentNotNullOrEmptyString(configurationFile, nameof(configurationFile));
            Guard.ArgumentNotNullOrEmptyString(configurationKey, nameof(configurationKey));

            return GetConfiguration(configurationFile).GetRetryStrategies(configurationKey, getCustomRetryStrategy);
        }

        /// <summary>
        /// ets the retry manager from the specified configuration file and key.
        /// </summary>
        /// <param name="configurationFile">The specified configuration files.</param>
        /// <param name="configurationKey">The specified key in the configuration file.</param>
        /// <param name="getCustomRetryStrategy">The function to get custom retry strategy from options.</param>
        /// <returns>The retry manager.</returns>
        public static RetryManager GetRetryManager(
            string configurationFile = DefaultConfigurationFile, 
            string configurationKey = DefaultConfigurationKeyRetryManager, 
            Func<IConfigurationSection, RetryStrategy>? getCustomRetryStrategy = null)
        {
            Guard.ArgumentNotNullOrEmptyString(configurationFile, nameof(configurationFile));
            Guard.ArgumentNotNullOrEmptyString(configurationKey, nameof(configurationKey));

            return GetConfiguration(configurationFile).GetRetryManager(configurationKey, getCustomRetryStrategy);
        }

        /// <summary>Gets the configuration from the specified file.</summary>
        /// <param name="configurationFile">The specified configuration file.</param>
        /// <returns>The configuration.</returns>
        public static IConfiguration GetConfiguration(string configurationFile = DefaultConfigurationFile)
        {
            Guard.ArgumentNotNullOrEmptyString(configurationFile, nameof(configurationFile));

            IConfigurationBuilder builder = new ConfigurationBuilder();
            builder = Path.GetExtension(configurationFile).ToUpperInvariant() switch
            {
                ".INI" => builder.AddIniFile(configurationFile),
                ".JSON" => builder.AddJsonFile(configurationFile),
                ".XML" => builder.AddXmlFile(configurationFile),
                _ => throw new ArgumentOutOfRangeException(nameof(configurationFile), string.Format(CultureInfo.InvariantCulture, Resources.ConfigurationFileNotSupported, configurationFile))
            };

            return builder.Build();
        }
    }
}
