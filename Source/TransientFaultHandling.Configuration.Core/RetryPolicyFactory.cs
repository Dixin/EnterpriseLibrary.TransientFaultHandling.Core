﻿namespace Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling
{
    using System;
    using Microsoft.Extensions.Configuration;

    /// <summary>
    /// Provides a factory class for instantiating application-specific retry policies described in the application configuration.
    /// </summary>
    public static class RetryPolicyFactory
    {
        /// <summary>
        /// Sets the retry manager.
        /// </summary>
        /// <param name="retryManager">The retry manager.</param>
        /// <param name="throwIfSet">true to throw an exception if the writer is already set; otherwise, false. Defaults to <see langword="true"/>.</param>
        /// <exception cref="InvalidOperationException">The factory is already set and <paramref name="throwIfSet"/> is true.</exception>
        public static void SetRetryManager(RetryManager? retryManager, bool throwIfSet = true)
        {
            RetryManager.SetDefault(retryManager, throwIfSet);
        }

        /// <summary>
        /// Creates a retry manager from the system configuration.
        /// </summary>
        /// <returns></returns>
        public static RetryManager CreateDefault(
            string configurationFile = RetryConfiguration.DefaultConfigurationFile, string configurationKey = nameof(RetryManager), Func<IConfigurationSection, RetryStrategy>? getCustomRetryStrategy = null)
        {
            Argument.NotNullOrEmpty(configurationFile, nameof(configurationFile));
            Argument.NotNullOrEmpty(configurationKey, nameof(configurationKey));

            RetryManager manager = RetryConfiguration.GetRetryManager(configurationFile, configurationKey, getCustomRetryStrategy);
            RetryManager.SetDefault(manager);
            return manager;
        }

        /// <summary>
        /// Returns the default retry policy dedicated to handling transient conditions with SQL connections.
        /// </summary>
        /// <returns>The retry policy for SQL connections with the corresponding default strategy (or the default strategy if no retry strategy definition for SQL connections was found).</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "As designed")]
        public static RetryPolicy GetDefaultSqlConnectionRetryPolicy(
            string configurationFile = RetryConfiguration.DefaultConfigurationFile, string configurationKey = nameof(RetryManager), Func<IConfigurationSection, RetryStrategy>? getCustomRetryStrategy = null) =>
            GetOrCreateRetryManager(Argument.NotNullOrEmpty(Argument.NotNullOrEmpty(configurationKey, nameof(configurationKey)), nameof(configurationFile)), configurationKey, getCustomRetryStrategy)
                .GetDefaultSqlConnectionRetryPolicy();

        /// <summary>
        /// Returns the default retry policy dedicated to handling transient conditions with SQL commands.
        /// </summary>
        /// <returns>The retry policy for SQL commands with the corresponding default strategy (or the default strategy if no retry strategy definition for SQL commands was found).</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "As designed")]
        public static RetryPolicy GetDefaultSqlCommandRetryPolicy(
            string configurationFile = RetryConfiguration.DefaultConfigurationFile, string configurationKey = nameof(RetryManager), Func<IConfigurationSection, RetryStrategy>? getCustomRetryStrategy = null) =>
            GetOrCreateRetryManager(Argument.NotNullOrEmpty(configurationFile, nameof(configurationFile)), Argument.NotNullOrEmpty(configurationKey, nameof(configurationKey)), getCustomRetryStrategy)
                .GetDefaultSqlCommandRetryPolicy();

        // TODO.
        ///// <summary>
        ///// Returns the default retry policy dedicated to handling transient conditions with Windows Azure Service Bus.
        ///// </summary>
        ///// <returns>The retry policy for Windows Azure Service Bus with the corresponding default strategy (or the default strategy if no retry strategy definition for Windows Azure Service Bus was found).</returns>
        //[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "As designed")]
        //public static RetryPolicy GetDefaultAzureServiceBusRetryPolicy()
        //{
        //    return GetOrCreateRetryManager().GetDefaultAzureServiceBusRetryPolicy();
        //}

        ///// <summary>
        ///// Returns the default retry policy dedicated to handling transient conditions with Windows Azure Caching.
        ///// </summary>
        ///// <returns>The retry policy for Windows Azure Caching with the corresponding default strategy (or the default strategy if no retry strategy definition for Windows Azure Caching was found).</returns>
        //[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "As designed")]
        //public static RetryPolicy GetDefaultAzureCachingRetryPolicy()
        //{
        //    return GetOrCreateRetryManager().GetDefaultCachingRetryPolicy();
        //}

        ///// <summary>
        ///// Returns the default retry policy dedicated to handling transient conditions with Windows Azure Storage.
        ///// </summary>
        ///// <returns>The retry policy for Windows Azure Storage with the corresponding default strategy (or the default strategy if no retry strategy definition for Windows Azure Storage was found).</returns>
        //[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "As designed")]
        //public static RetryPolicy GetDefaultAzureStorageRetryPolicy()
        //{
        //    return GetOrCreateRetryManager().GetDefaultAzureStorageRetryPolicy();
        //}

        /// <summary>
        /// Returns a retry policy with the specified error detection strategy and the default retry strategy defined in the configuration. 
        /// </summary>
        /// <typeparam name="T">The type that implements the <see cref="ITransientErrorDetectionStrategy"/> interface that is responsible for detecting transient conditions.</typeparam>
        /// <returns>The retry policy that is initialized with the default retry strategy.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "As designed")]
        public static RetryPolicy GetRetryPolicy<T>(
            string configurationFile = RetryConfiguration.DefaultConfigurationFile, string configurationKey = nameof(RetryManager), Func<IConfigurationSection, RetryStrategy>? getCustomRetryStrategy = null) 
            where T : ITransientErrorDetectionStrategy, new() =>
            GetOrCreateRetryManager(Argument.NotNullOrEmpty(configurationFile, nameof(configurationFile)), Argument.NotNullOrEmpty(configurationKey, nameof(configurationKey)), getCustomRetryStrategy)
                .GetRetryPolicy<T>();

        /// <summary>
        /// Returns a retry policy with the specified error detection strategy and the default retry strategy defined in the configuration. 
        /// </summary>
        /// <param name="errorDetectionStrategy">The <see cref="T:Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling.ITransientErrorDetectionStrategy" /> that is responsible for detecting transient conditions.</param>
        /// <param name="configurationFile">The specified configuration file.</param>
        /// <param name="configurationKey">The specified configuration key.</param>
        /// <param name="getCustomRetryStrategy">The function to ger custom retry strategy from options.</param>
        /// <returns>The retry policy that is initialized with the default retry strategy.</returns>
        public static RetryPolicy GetRetryPolicy(
            ITransientErrorDetectionStrategy errorDetectionStrategy, 
            string configurationFile = RetryConfiguration.DefaultConfigurationFile, 
            string configurationKey = nameof(RetryManager), 
            Func<IConfigurationSection, RetryStrategy>? getCustomRetryStrategy = null)
        {
            Argument.NotNull(errorDetectionStrategy, nameof(errorDetectionStrategy));
            Argument.NotNullOrEmpty(configurationFile, nameof(configurationFile));
            Argument.NotNullOrEmpty(configurationKey, nameof(configurationKey));

            return GetOrCreateRetryManager(configurationFile, configurationKey, getCustomRetryStrategy).GetRetryPolicy(errorDetectionStrategy);
        }

        /// <summary>
        /// Returns an instance of the <see cref="RetryPolicy"/> object for a given error detection strategy and retry strategy.
        /// </summary>
        /// <typeparam name="T">The type that implements the <see cref="ITransientErrorDetectionStrategy"/> interface that is responsible for detecting transient conditions.</typeparam>
        /// <param name="retryStrategyName">The name under which a retry strategy definition is registered in the application configuration.</param>
        /// <param name="configurationFile">The specified configuration file.</param>
        /// <param name="configurationKey">The specified configuration key.</param>
        /// <param name="getCustomRetryStrategy">The function to ger custom retry strategy from options.</param>
        /// <returns>The retry policy that is initialized with the retry strategy matching the provided name.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "As designed")]
        public static RetryPolicy GetRetryPolicy<T>(
            string retryStrategyName, 
            string configurationFile = RetryConfiguration.DefaultConfigurationFile, 
            string configurationKey = nameof(RetryManager), 
            Func<IConfigurationSection, RetryStrategy>? getCustomRetryStrategy = null) where T : ITransientErrorDetectionStrategy, new()
        {
            Argument.NotNullOrEmpty(retryStrategyName, nameof(retryStrategyName));
            Argument.NotNullOrEmpty(configurationFile, nameof(configurationFile));
            Argument.NotNullOrEmpty(configurationKey, nameof(configurationKey));

            return GetOrCreateRetryManager(configurationFile, configurationKey, getCustomRetryStrategy).GetRetryPolicy<T>(retryStrategyName);
        }

        /// <summary>
        /// Returns an instance of the <see cref="RetryPolicy"/> object for a given error detection strategy and retry strategy.
        /// </summary>
        /// <param name="errorDetectionStrategy">The <see cref="T:Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling.ITransientErrorDetectionStrategy" /> that is responsible for detecting transient conditions.</param>
        /// <param name="retryStrategyName">The name under which a retry strategy definition is registered in the application configuration.</param>
        /// <param name="configurationFile">The specified configuration file.</param>
        /// <param name="configurationKey">The specified configuration key.</param>
        /// <param name="getCustomRetryStrategy">The function to ger custom retry strategy from options.</param>
        /// <returns>The retry policy that is initialized with the retry strategy matching the provided name.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "As designed")]
        public static RetryPolicy GetRetryPolicy(
            string retryStrategyName, 
            ITransientErrorDetectionStrategy errorDetectionStrategy, 
            string configurationFile = RetryConfiguration.DefaultConfigurationFile, 
            string configurationKey = nameof(RetryManager), 
            Func<IConfigurationSection, RetryStrategy>? getCustomRetryStrategy = null)
        {
            Argument.NotNull(errorDetectionStrategy, nameof(errorDetectionStrategy));
            Argument.NotNullOrEmpty(retryStrategyName, nameof(retryStrategyName));
            Argument.NotNullOrEmpty(configurationFile, nameof(configurationFile));
            Argument.NotNullOrEmpty(configurationKey, nameof(configurationKey));

            return GetOrCreateRetryManager(configurationFile, configurationKey, getCustomRetryStrategy).GetRetryPolicy(retryStrategyName, errorDetectionStrategy);
        }

        private static RetryManager GetOrCreateRetryManager(
            string configurationFile, string configurationKey, Func<IConfigurationSection, RetryStrategy>? getCustomRetryStrategy = null)
        {
            try
            {
                return RetryManager.Instance;
            }
            catch (InvalidOperationException)
            {
                CreateDefault(configurationFile, configurationKey, getCustomRetryStrategy);
            }

            return RetryManager.Instance;
        }
    }
}
