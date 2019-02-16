namespace Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Extensions.Configuration;

    public class RetryStrategyOptions
    {
        public bool FastFirstRetry { get; private set; }
    }

    public class FixedIntervalOptions : RetryStrategyOptions
    {
        public int RetryCount { get; private set; }

        public TimeSpan RetryInterval { get; private set; }
    }

    public class IncrementalOptions : RetryStrategyOptions
    {
        public int RetryCount { get; private set; }

        public TimeSpan InitialInterval { get; private set; }

        public TimeSpan Increment { get; private set; }
    }

    public class ExponentialBackoffOptions : RetryStrategyOptions
    {
        public int RetryCount { get; private set; }

        public TimeSpan MinBackOff { get; private set; }

        public TimeSpan MaxBackOff { get; private set; }

        public TimeSpan DeltaBackOff { get; private set; }
    }

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

    public static class OptionsExtensions
    {
        public static FixedInterval Strategy(this FixedIntervalOptions options, string name)
        {
            Guard.ArgumentNotNull(options, nameof(options));

            return new FixedInterval(name, options.RetryCount, options.RetryInterval, options.FastFirstRetry);
        }

        public static Incremental Strategy(this IncrementalOptions options, string name)
        {
            Guard.ArgumentNotNull(options, nameof(options));

            return new Incremental(name, options.RetryCount, options.InitialInterval, options.Increment, options.FastFirstRetry);
        }

        public static ExponentialBackoff Strategy(this ExponentialBackoffOptions options, string name)
        {
            Guard.ArgumentNotNull(options, nameof(options));

            return new ExponentialBackoff(name, options.RetryCount, options.MinBackOff, options.MaxBackOff, options.DeltaBackOff, options.FastFirstRetry);
        }
    }
}
