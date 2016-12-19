namespace Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;

    using Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling.Properties;

    /// <summary>
    /// Provides the entry point to the retry functionality.
    /// </summary>
    public class RetryManager
    {
        private static RetryManager defaultRetryManager;

        private readonly IDictionary<string, RetryStrategy> defaultRetryStrategiesMap;

        private readonly IDictionary<string, string> defaultRetryStrategyNamesMap;

        private readonly IDictionary<string, RetryStrategy> retryStrategies;

        private string defaultRetryStrategyName;

        private RetryStrategy defaultStrategy;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling.RetryManager" /> class.
        /// </summary>
        /// <param name="retryStrategies">The complete set of retry strategies.</param>
        public RetryManager(IEnumerable<RetryStrategy> retryStrategies) : this(retryStrategies, null, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling.RetryManager" /> class with the specified retry strategies and default retry strategy name.
        /// </summary>
        /// <param name="retryStrategies">The complete set of retry strategies.</param>
        /// <param name="defaultRetryStrategyName">The default retry strategy.</param>
        public RetryManager(IEnumerable<RetryStrategy> retryStrategies, string defaultRetryStrategyName) : this(retryStrategies, defaultRetryStrategyName, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling.RetryManager" /> class with the specified retry strategies and defaults.
        /// </summary>
        /// <param name="retryStrategies">The complete set of retry strategies.</param>
        /// <param name="defaultRetryStrategyName">The default retry strategy.</param>
        /// <param name="defaultRetryStrategyNamesMap">The names of the default strategies for different technologies.</param>
        public RetryManager(IEnumerable<RetryStrategy> retryStrategies, string defaultRetryStrategyName, IDictionary<string, string> defaultRetryStrategyNamesMap)
        {
            this.retryStrategies = retryStrategies.ToDictionary(p => p.Name);
            this.defaultRetryStrategyNamesMap = defaultRetryStrategyNamesMap;
            this.DefaultRetryStrategyName = defaultRetryStrategyName;
            this.defaultRetryStrategiesMap = new Dictionary<string, RetryStrategy>();
            if (this.defaultRetryStrategyNamesMap != null)
            {
                foreach (KeyValuePair<string, string> current in from x in this.defaultRetryStrategyNamesMap
                                                                 where !string.IsNullOrWhiteSpace(x.Value)
                                                                 select x)
                {
                    RetryStrategy value;
                    if (!this.retryStrategies.TryGetValue(current.Value, out value))
                    {
                        throw new ArgumentOutOfRangeException(
                            nameof(defaultRetryStrategyNamesMap),
                            string.Format(CultureInfo.CurrentCulture, Resources.DefaultRetryStrategyMappingNotFound, new object[] { current.Key, current.Value }));
                    }

                    this.defaultRetryStrategiesMap.Add(current.Key, value);
                }
            }
        }

        /// <summary>
        /// Gets the default <see cref="T:Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling.RetryManager" /> for the application.
        /// </summary>
        /// <remarks>You can update the default retry manager by calling the <see cref="M:Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling.RetryManager.SetDefault(Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling.RetryManager,System.Boolean)" /> method.</remarks>
        public static RetryManager Instance
        {
            get
            {
                RetryManager retryManager = defaultRetryManager;
                if (retryManager == null)
                {
                    throw new InvalidOperationException(Resources.ExceptionRetryManagerNotSet);
                }

                return retryManager;
            }
        }

        /// <summary>
        /// Gets or sets the default retry strategy name.
        /// </summary>
        public string DefaultRetryStrategyName
        {
            get
            {
                return this.defaultRetryStrategyName;
            }

            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    this.defaultRetryStrategyName = null;
                    return;
                }

                RetryStrategy retryStrategy;
                if (this.retryStrategies.TryGetValue(value, out retryStrategy))
                {
                    this.defaultRetryStrategyName = value;
                    this.defaultStrategy = retryStrategy;
                    return;
                }

                throw new ArgumentOutOfRangeException(
                    nameof(value),
                    string.Format(CultureInfo.CurrentCulture, Resources.RetryStrategyNotFound, new object[] { value }));
            }
        }

        /// <summary>
        /// Sets the specified retry manager as the default retry manager.
        /// </summary>
        /// <param name="retryManager">The retry manager.</param>
        /// <param name="throwIfSet">true to throw an exception if the manager is already set; otherwise, false. Defaults to <see langword="true" />.</param>
        /// <exception cref="T:System.InvalidOperationException">The singleton is already set and <paramref name="throwIfSet" /> is true.</exception>
        public static void SetDefault(RetryManager retryManager, bool throwIfSet = true)
        {
            if (defaultRetryManager != null && throwIfSet && retryManager != defaultRetryManager)
            {
                throw new InvalidOperationException(Resources.ExceptionRetryManagerAlreadySet);
            }

            defaultRetryManager = retryManager;
        }

        /// <summary>
        /// Returns a retry policy with the specified error detection strategy and the default retry strategy defined in the configuration. 
        /// </summary>
        /// <typeparam name="T">The type that implements the <see cref="T:Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling.ITransientErrorDetectionStrategy" /> interface that is responsible for detecting transient conditions.</typeparam>
        /// <returns>A new retry policy with the specified error detection strategy and the default retry strategy defined in the configuration.</returns>
        public virtual RetryPolicy<T> GetRetryPolicy<T>() where T : ITransientErrorDetectionStrategy, new()
        {
            return new RetryPolicy<T>(this.GetRetryStrategy());
        }

        /// <summary>
        /// Returns a retry policy with the specified error detection strategy and retry strategy.
        /// </summary>
        /// <typeparam name="T">The type that implements the <see cref="T:Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling.ITransientErrorDetectionStrategy" /> interface that is responsible for detecting transient conditions.</typeparam>
        /// <param name="retryStrategyName">The retry strategy name, as defined in the configuration.</param>
        /// <returns>A new retry policy with the specified error detection strategy and the default retry strategy defined in the configuration.</returns>
        public virtual RetryPolicy<T> GetRetryPolicy<T>(string retryStrategyName) where T : ITransientErrorDetectionStrategy, new()
        {
            return new RetryPolicy<T>(this.GetRetryStrategy(retryStrategyName));
        }

        /// <summary>
        /// Returns the default retry strategy defined in the configuration.
        /// </summary>
        /// <returns>The retry strategy that matches the default strategy.</returns>
        public virtual RetryStrategy GetRetryStrategy()
        {
            return this.defaultStrategy;
        }

        /// <summary>
        /// Returns the retry strategy that matches the specified name.
        /// </summary>
        /// <param name="retryStrategyName">The retry strategy name.</param>
        /// <returns>The retry strategy that matches the specified name.</returns>
        public virtual RetryStrategy GetRetryStrategy(string retryStrategyName)
        {
            Guard.ArgumentNotNullOrEmptyString(retryStrategyName, "retryStrategyName");
            RetryStrategy result;
            if (!this.retryStrategies.TryGetValue(retryStrategyName, out result))
            {
                throw new ArgumentOutOfRangeException(
                    string.Format(CultureInfo.CurrentCulture, Resources.RetryStrategyNotFound, new object[] { retryStrategyName }));
            }

            return result;
        }

        /// <summary>
        /// Returns the retry strategy for the specified technology.
        /// </summary>
        /// <param name="technology">The technology to get the default retry strategy for.</param>
        /// <returns>The retry strategy for the specified technology.</returns>
        public virtual RetryStrategy GetDefaultRetryStrategy(string technology)
        {
            Guard.ArgumentNotNullOrEmptyString(technology, nameof(technology));
            RetryStrategy retryStrategy;
            if (!this.defaultRetryStrategiesMap.TryGetValue(technology, out retryStrategy))
            {
                retryStrategy = this.defaultStrategy;
            }

            if (retryStrategy == null)
            {
                throw new ArgumentOutOfRangeException(
                    string.Format(CultureInfo.CurrentCulture, Resources.DefaultRetryStrategyNotFound, new object[] { technology }));
            }

            return retryStrategy;
        }
    }
}
