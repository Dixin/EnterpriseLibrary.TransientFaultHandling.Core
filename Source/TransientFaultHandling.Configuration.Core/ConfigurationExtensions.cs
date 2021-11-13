namespace Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling;

/// <summary>
/// Provides the extension methods for <see cref="IConfigurationSection" />.
/// </summary>
public static class ConfigurationExtensions
{
    private static readonly string[] FixedIntervalProperties = typeof(FixedIntervalOptions).GetProperties().Select(property => property.Name).ToArray();

    private static readonly string[] IncrementalProperties = typeof(IncrementalOptions).GetProperties().Select(property => property.Name).ToArray();

    private static readonly string[] ExponentialBackoffProperties = typeof(ExponentialBackoffOptions).GetProperties().Select(property => property.Name).ToArray();

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
        IConfigurationSection section = configuration.NotNull().GetSection(key.NotNullOrEmpty());
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
        Func<IConfigurationSection, RetryStrategy>? getCustomRetryStrategy = null) =>
        configurationSection.NotNull().Exists()
            ? configurationSection.GetChildren().ToDictionary(
                childSection => childSection.Key,
                childSection => childSection.GetRetryStrategy(getCustomRetryStrategy))
            : throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, Resources.ConfigurationSectionNotExist, configurationSection.Path), nameof(configurationSection));

    /// <summary>Gets the retry strategy from the specified configuration section.</summary>
    /// <param name="configurationSection">The configuration section.</param>
    /// <param name="getCustomRetryStrategy">The function to ger custom retry strategy from options.</param>
    /// <returns>The retry strategy. </returns>
    public static RetryStrategy GetRetryStrategy(this IConfigurationSection configurationSection, Func<IConfigurationSection, RetryStrategy>? getCustomRetryStrategy = null)
    {
        if (!configurationSection.NotNull().Exists())
        {
            throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, Resources.ConfigurationSectionNotExist, configurationSection.Path), nameof(configurationSection));
        }

        return (FixedIntervalProperties.All(configurationSection.HasKey), IncrementalProperties.All(configurationSection.HasKey), ExponentialBackoffProperties.All(configurationSection.HasKey)) switch
        {
            (true, false, false) => configurationSection.Get<FixedIntervalOptions>().ToFixedInterval(configurationSection.Key),
            (false, true, false) => configurationSection.Get<IncrementalOptions>().ToIncremental(configurationSection.Key),
            (false, false, true) => configurationSection.Get<ExponentialBackoffOptions>().ToExponentialBackoff(configurationSection.Key),
            _ => getCustomRetryStrategy?.Invoke(configurationSection)
                ?? throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, Resources.ConfigurationSectionHasInvalidRetryStrategy, configurationSection.Path), nameof(configurationSection))
        };
    }

    /// <summary>Gets the retry strategy from the specified configuration section.</summary>
    /// <param name="configurationSection">The configuration section.</param>
    /// <returns>The retry strategy. </returns>
    public static FixedInterval GetFixedInterval(this IConfigurationSection configurationSection)
    {
        if (!configurationSection.NotNull().Exists())
        {
            throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, Resources.ConfigurationSectionNotExist, configurationSection.Path), nameof(configurationSection));
        }

        if (!FixedIntervalProperties.All(configurationSection.HasKey) || IncrementalProperties.All(configurationSection.HasKey) || ExponentialBackoffProperties.All(configurationSection.HasKey))
        {
            throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, Resources.ConfigurationSectionHasInvalidRetryStrategy, configurationSection.Path), nameof(configurationSection));
        }

        return configurationSection.Get<FixedIntervalOptions>().ToFixedInterval(configurationSection.Key);
    }

    /// <summary>Gets the retry strategy from the specified configuration section.</summary>
    /// <param name="configurationSection">The configuration section.</param>
    /// <returns>The retry strategy. </returns>
    public static Incremental GetIncremental(this IConfigurationSection configurationSection)
    {
        if (!configurationSection.NotNull().Exists())
        {
            throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, Resources.ConfigurationSectionNotExist, configurationSection.Path), nameof(configurationSection));
        }

        if (FixedIntervalProperties.All(configurationSection.HasKey) || !IncrementalProperties.All(configurationSection.HasKey) || ExponentialBackoffProperties.All(configurationSection.HasKey))
        {
            throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, Resources.ConfigurationSectionHasInvalidRetryStrategy, configurationSection.Path), nameof(configurationSection));
        }

        return configurationSection.Get<IncrementalOptions>().ToIncremental(configurationSection.Key);
    }

    /// <summary>Gets the retry strategy from the specified configuration section.</summary>
    /// <param name="configurationSection">The configuration section.</param>
    /// <returns>The retry strategy. </returns>
    public static ExponentialBackoff GetExponentialBackoff(this IConfigurationSection configurationSection)
    {
        if (!configurationSection.NotNull().Exists())
        {
            throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, Resources.ConfigurationSectionNotExist, configurationSection.Path), nameof(configurationSection));
        }

        if (FixedIntervalProperties.All(configurationSection.HasKey) || IncrementalProperties.All(configurationSection.HasKey) || !ExponentialBackoffProperties.All(configurationSection.HasKey))
        {
            throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, Resources.ConfigurationSectionHasInvalidRetryStrategy, configurationSection.Path), nameof(configurationSection));
        }

        return configurationSection.Get<ExponentialBackoffOptions>().ToExponentialBackoff(configurationSection.Key);
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
        Func<IConfigurationSection, RetryStrategy>? covariance = getCustomRetryStrategy;
        return GetRetryStrategies(configuration.NotNull(), key.NotNullOrEmpty(), covariance)
            .Where(pair => pair.Value is not null and TRetryStrategy)
            .ToDictionary(pair => pair.Key, pair => (TRetryStrategy)pair.Value);
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
        IConfigurationSection section = configuration.NotNull().GetSection(key.NotNullOrEmpty());
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
        configurationSection.NotNull().Exists()
            ? configurationSection.Get<RetryManagerOptions>(buildOptions => buildOptions.BindNonPublicProperties = true).ToRetryManager(getCustomRetryStrategy)
            : throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, Resources.ConfigurationSectionNotExist, configurationSection.Path), nameof(configurationSection));
}