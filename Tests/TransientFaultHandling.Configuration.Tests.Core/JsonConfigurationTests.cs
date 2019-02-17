namespace Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class JsonConfigurationTests
    {
        [TestMethod]
        public void JsonRetryStrategyTest()
        {
            IConfiguration configuration = new ConfigurationBuilder()
                .AddJsonFile("app.json")
                .Build();

            Dictionary<string, RetryStrategy> retryStrategies = configuration.GetRetryStrategies(nameof(RetryStrategy));
            Assert.AreEqual(configuration.GetSection(nameof(RetryStrategy)).GetChildren().Count(), retryStrategies.Count);

            string property;

            IConfigurationSection options1 = configuration.GetSection(nameof(RetryStrategy)).GetChildren().ElementAt(0);
            Assert.IsInstanceOfType(retryStrategies[options1.Key], typeof(FixedInterval));
            FixedInterval strategy1 = (FixedInterval)retryStrategies[options1.Key];
            Assert.AreEqual(options1.Key, strategy1.Name);
            property = nameof(RetryStrategy.FastFirstRetry);
            Assert.AreEqual(options1.GetValue<bool>(property.First().ToString().ToLower() + property.Substring(1)), strategy1.FastFirstRetry);
            property = nameof(FixedIntervalOptions.RetryCount);
            Assert.AreEqual(options1.GetValue<int>(property.First().ToString().ToLower() + property.Substring(1)), strategy1.GetType().GetField("retryCount", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(strategy1));
            property = nameof(FixedIntervalOptions.RetryInterval);
            Assert.AreEqual(options1.GetValue<TimeSpan>(property.First().ToString().ToLower() + property.Substring(1)), strategy1.GetType().GetField("retryInterval", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(strategy1));

            IConfigurationSection options2 = configuration.GetSection(nameof(RetryStrategy)).GetChildren().ElementAt(1);
            Assert.IsInstanceOfType(retryStrategies[options2.Key], typeof(Incremental));
            Incremental strategy2 = (Incremental)retryStrategies[options2.Key];
            Assert.AreEqual(options2.Key, strategy2.Name);
            property = nameof(RetryStrategy.FastFirstRetry);
            Assert.AreEqual(options2.GetValue<bool>(property.First().ToString().ToLower() + property.Substring(1)), strategy2.FastFirstRetry);
            property = nameof(IncrementalOptions.RetryCount);
            Assert.AreEqual(options2.GetValue<int>(property.First().ToString().ToLower() + property.Substring(1)), strategy2.GetType().GetField("retryCount", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(strategy2));
            property = nameof(IncrementalOptions.InitialInterval);
            Assert.AreEqual(options2.GetValue<TimeSpan>(property.First().ToString().ToLower() + property.Substring(1)), strategy2.GetType().GetField("initialInterval", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(strategy2));
            property = nameof(IncrementalOptions.Increment);
            Assert.AreEqual(options2.GetValue<TimeSpan>(property.First().ToString().ToLower() + property.Substring(1)), strategy2.GetType().GetField("increment", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(strategy2));

            IConfigurationSection options3 = configuration.GetSection(nameof(RetryStrategy)).GetChildren().ElementAt(2);
            Assert.IsInstanceOfType(retryStrategies[options3.Key], typeof(ExponentialBackoff));
            ExponentialBackoff strategy3 = (ExponentialBackoff)retryStrategies[options3.Key];
            Assert.AreEqual(options3.Key, strategy3.Name);
            property = nameof(RetryStrategy.FastFirstRetry);
            Assert.AreEqual(options3.GetValue<bool>(property.First().ToString().ToLower() + property.Substring(1)), strategy3.FastFirstRetry);
            property = nameof(ExponentialBackoffOptions.RetryCount);
            Assert.AreEqual(options3.GetValue<int>(property.First().ToString().ToLower() + property.Substring(1)), strategy3.GetType().GetField("retryCount", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(strategy3));
            property = nameof(ExponentialBackoffOptions.MinBackOff);
            Assert.AreEqual(options3.GetValue<TimeSpan>(property.First().ToString().ToLower() + property.Substring(1)), strategy3.GetType().GetField("minBackoff", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(strategy3));
            property = nameof(ExponentialBackoffOptions.MaxBackOff);
            Assert.AreEqual(options3.GetValue<TimeSpan>(property.First().ToString().ToLower() + property.Substring(1)), strategy3.GetType().GetField("maxBackoff", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(strategy3));
            property = nameof(ExponentialBackoffOptions.DeltaBackOff);
            Assert.AreEqual(options3.GetValue<TimeSpan>(property.First().ToString().ToLower() + property.Substring(1)), strategy3.GetType().GetField("deltaBackoff", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(strategy3));

            ExponentialBackoff strategy = configuration.GetRetryStrategies<ExponentialBackoff>(nameof(RetryStrategy)).Single().Value;

            Assert.AreEqual(options3.Key, strategy.Name);
            property = nameof(RetryStrategy.FastFirstRetry);
            Assert.AreEqual(options3.GetValue<bool>(property.First().ToString().ToLower() + property.Substring(1)), strategy.FastFirstRetry);
            property = nameof(ExponentialBackoffOptions.RetryCount);
            Assert.AreEqual(options3.GetValue<int>(property.First().ToString().ToLower() + property.Substring(1)), strategy.GetType().GetField("retryCount", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(strategy));
            property = nameof(ExponentialBackoffOptions.MinBackOff);
            Assert.AreEqual(options3.GetValue<TimeSpan>(property.First().ToString().ToLower() + property.Substring(1)), strategy.GetType().GetField("minBackoff", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(strategy));
            property = nameof(ExponentialBackoffOptions.MaxBackOff);
            Assert.AreEqual(options3.GetValue<TimeSpan>(property.First().ToString().ToLower() + property.Substring(1)), strategy.GetType().GetField("maxBackoff", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(strategy));
            property = nameof(ExponentialBackoffOptions.DeltaBackOff);
            Assert.AreEqual(options3.GetValue<TimeSpan>(property.First().ToString().ToLower() + property.Substring(1)), strategy.GetType().GetField("deltaBackoff", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(strategy));
        }
    }
}
