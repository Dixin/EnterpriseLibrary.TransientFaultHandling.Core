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
        private static RetryManager? defaultRetryManager;

        private readonly IDictionary<string, RetryStrategy> defaultRetryStrategiesMap;

        private readonly IDictionary<string, RetryStrategy> retryStrategies;

        private string? defaultRetryStrategyName;

        private RetryStrategy? defaultStrategy;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling.RetryManager" /> class with the specified retry strategies and defaults.
        /// </summary>
        /// <param name="retryStrategies">The complete set of retry strategies.</param>
        /// <param name="defaultRetryStrategyName">The default retry strategy.</param>
        /// <param name="defaultRetryStrategyNamesMap">The names of the default strategies for different technologies.</param>
        public RetryManager(IEnumerable<RetryStrategy> retryStrategies, string? defaultRetryStrategyName = null, IDictionary<string, string>? defaultRetryStrategyNamesMap = null)
        {
            Guard.ArgumentNotNull(retryStrategies, nameof(retryStrategies));

            this.retryStrategies = retryStrategies.ToDictionary(retryStrategy => 
                retryStrategy.Name 
                ?? throw new ArgumentException(Resources.RetryStrategyNameCannotBeEmpty, nameof(retryStrategies)));
            this.DefaultRetryStrategyName = defaultRetryStrategyName;
            this.defaultRetryStrategiesMap = new Dictionary<string, RetryStrategy>();
            if (defaultRetryStrategyNamesMap is not null)
            {
                foreach (KeyValuePair<string, string> mapping in defaultRetryStrategyNamesMap)
                {
                    if (this.retryStrategies.TryGetValue(mapping.Value, out RetryStrategy? retryStrategy))
                    {
                        this.defaultRetryStrategiesMap.Add(mapping.Key, retryStrategy);
                    }
                    else
                    {
                        throw new ArgumentOutOfRangeException(
                            nameof(defaultRetryStrategyNamesMap),
                            string.Format(CultureInfo.CurrentCulture, Resources.DefaultRetryStrategyMappingNotFound, mapping.Key, mapping.Value));
                    }
                }
            }
        }

        /// <summary>
        /// Gets the default <see cref="T:Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling.RetryManager" /> for the application.
        /// </summary>
        /// <remarks>You can update the default retry manager by calling the <see cref="M:Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling.RetryManager.SetDefault(Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling.RetryManager,System.Boolean)" /> method.</remarks>
        public static RetryManager Instance => 
            defaultRetryManager ?? throw new InvalidOperationException(Resources.ExceptionRetryManagerNotSet);

        /// <summary>
        /// Gets or sets the default retry strategy name.
        /// </summary>
        public string? DefaultRetryStrategyName
        {
            get => this.defaultRetryStrategyName;

            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    this.defaultRetryStrategyName = null;
                    this.defaultStrategy = null;
                    return;
                }

                if (this.retryStrategies.TryGetValue(value, out RetryStrategy? retryStrategy))
                {
                    this.defaultRetryStrategyName = value;
                    this.defaultStrategy = retryStrategy;
                    return;
                }

                throw new ArgumentOutOfRangeException(
                    nameof(value),
                    string.Format(CultureInfo.CurrentCulture, Resources.RetryStrategyNotFound, value));
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
            if (defaultRetryManager is not null && throwIfSet && retryManager != defaultRetryManager)
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
        public virtual RetryPolicy<T> GetRetryPolicy<T>() where T : ITransientErrorDetectionStrategy, new() =>
            new(
                this.GetRetryStrategy()
                ?? throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, Resources.DefaultRetryStrategyNotFound, string.Empty)));

        /// <summary>
        /// Returns a retry policy with the specified error detection strategy and retry strategy.
        /// </summary>
        /// <typeparam name="T">The type that implements the <see cref="T:Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling.ITransientErrorDetectionStrategy" /> interface that is responsible for detecting transient conditions.</typeparam>
        /// <param name="retryStrategyName">The retry strategy name, as defined in the configuration.</param>
        /// <returns>A new retry policy with the specified error detection strategy and the default retry strategy defined in the configuration.</returns>
        public virtual RetryPolicy<T> GetRetryPolicy<T>(string retryStrategyName) where T : ITransientErrorDetectionStrategy, new() =>
            new(this.GetRetryStrategy(retryStrategyName));

        /// <summary>
        /// Returns the default retry strategy defined in the configuration.
        /// </summary>
        /// <returns>The retry strategy that matches the default strategy.</returns>
        public virtual RetryStrategy? GetRetryStrategy() => this.defaultStrategy;

        /// <summary>
        /// Returns the retry strategy that matches the specified name.
        /// </summary>
        /// <param name="retryStrategyName">The retry strategy name.</param>
        /// <returns>The retry strategy that matches the specified name.</returns>
        public virtual RetryStrategy GetRetryStrategy(string retryStrategyName)
        {
            Guard.ArgumentNotNullOrEmptyString(retryStrategyName, nameof(retryStrategyName));

            if (!this.retryStrategies.TryGetValue(retryStrategyName, out RetryStrategy? result))
            {
                throw new ArgumentOutOfRangeException(
                    string.Format(CultureInfo.CurrentCulture, Resources.RetryStrategyNotFound, retryStrategyName));
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

            return this.defaultRetryStrategiesMap.TryGetValue(technology, out RetryStrategy? retryStrategy)
                ? retryStrategy
                : this.defaultStrategy ?? throw new ArgumentOutOfRangeException(
                    string.Format(CultureInfo.CurrentCulture, Resources.DefaultRetryStrategyNotFound, technology));
        }
    }
}
