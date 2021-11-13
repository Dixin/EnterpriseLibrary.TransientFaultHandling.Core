namespace Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling.Tests.Configuration;

[TestClass]
public class RetryStrategyXmlTests
{
    [TestMethod]
    public void RetryStrategyXmlTest()
    {
        IConfiguration configuration = new ConfigurationBuilder()
            .AddXmlFile("app.xml")
            .Build();

        IDictionary<string, RetryStrategy> retryStrategies = configuration.GetRetryStrategies();
        Assert.AreEqual(configuration.GetSection(nameof(RetryStrategy)).GetChildren().Count(), retryStrategies.Count);

        string property;

        IConfigurationSection options1 = configuration.GetSection(nameof(RetryStrategy)).GetChildren().ElementAt(0);
        Assert.IsInstanceOfType(retryStrategies[options1.Key], typeof(FixedInterval));
        FixedInterval strategy1 = (FixedInterval)retryStrategies[options1.Key];
        Assert.AreEqual(options1.Key, strategy1.Name);
        property = nameof(RetryStrategy.FastFirstRetry);
        Assert.AreEqual(options1.GetValue<bool>(property), strategy1.FastFirstRetry);
        property = nameof(FixedIntervalOptions.RetryCount);
        Assert.AreEqual(options1.GetValue<int>(property), strategy1.GetInstanceNonPublicFieldValue("retryCount"));
        property = nameof(FixedIntervalOptions.RetryInterval);
        Assert.AreEqual(options1.GetValue<TimeSpan>(property), strategy1.GetInstanceNonPublicFieldValue("retryInterval"));

        IConfigurationSection options2 = configuration.GetSection(nameof(RetryStrategy)).GetChildren().ElementAt(1);
        Assert.IsInstanceOfType(retryStrategies[options2.Key], typeof(Incremental));
        Incremental strategy2 = (Incremental)retryStrategies[options2.Key];
        Assert.AreEqual(options2.Key, strategy2.Name);
        property = nameof(RetryStrategy.FastFirstRetry);
        Assert.AreEqual(options2.GetValue<bool>(property), strategy2.FastFirstRetry);
        property = nameof(IncrementalOptions.RetryCount);
        Assert.AreEqual(options2.GetValue<int>(property), strategy2.GetInstanceNonPublicFieldValue("retryCount"));
        property = nameof(IncrementalOptions.InitialInterval);
        Assert.AreEqual(options2.GetValue<TimeSpan>(property), strategy2.GetInstanceNonPublicFieldValue("initialInterval"));
        property = nameof(IncrementalOptions.Increment);
        Assert.AreEqual(options2.GetValue<TimeSpan>(property), strategy2.GetInstanceNonPublicFieldValue("increment"));

        IConfigurationSection options3 = configuration.GetSection(nameof(RetryStrategy)).GetChildren().ElementAt(2);
        Assert.IsInstanceOfType(retryStrategies[options3.Key], typeof(ExponentialBackoff));
        ExponentialBackoff strategy3 = (ExponentialBackoff)retryStrategies[options3.Key];
        Assert.AreEqual(options3.Key, strategy3.Name);
        property = nameof(RetryStrategy.FastFirstRetry);
        Assert.AreEqual(options3.GetValue<bool>(property), strategy3.FastFirstRetry);
        property = nameof(ExponentialBackoffOptions.RetryCount);
        Assert.AreEqual(options3.GetValue<int>(property), strategy3.GetInstanceNonPublicFieldValue("retryCount"));
        property = nameof(ExponentialBackoffOptions.MinBackOff);
        Assert.AreEqual(options3.GetValue<TimeSpan>(property), strategy3.GetInstanceNonPublicFieldValue("minBackoff"));
        property = nameof(ExponentialBackoffOptions.MaxBackOff);
        Assert.AreEqual(options3.GetValue<TimeSpan>(property), strategy3.GetInstanceNonPublicFieldValue("maxBackoff"));
        property = nameof(ExponentialBackoffOptions.DeltaBackOff);
        Assert.AreEqual(options3.GetValue<TimeSpan>(property), strategy3.GetInstanceNonPublicFieldValue("deltaBackoff"));

        ExponentialBackoff strategy = configuration.GetRetryStrategies<ExponentialBackoff>().Single().Value;

        Assert.AreEqual(options3.Key, strategy.Name);
        property = nameof(RetryStrategy.FastFirstRetry);
        Assert.AreEqual(options3.GetValue<bool>(property), strategy.FastFirstRetry);
        property = nameof(ExponentialBackoffOptions.RetryCount);
        Assert.AreEqual(options3.GetValue<int>(property), strategy.GetInstanceNonPublicFieldValue("retryCount"));
        property = nameof(ExponentialBackoffOptions.MinBackOff);
        Assert.AreEqual(options3.GetValue<TimeSpan>(property), strategy.GetInstanceNonPublicFieldValue("minBackoff"));
        property = nameof(ExponentialBackoffOptions.MaxBackOff);
        Assert.AreEqual(options3.GetValue<TimeSpan>(property), strategy.GetInstanceNonPublicFieldValue("maxBackoff"));
        property = nameof(ExponentialBackoffOptions.DeltaBackOff);
        Assert.AreEqual(options3.GetValue<TimeSpan>(property), strategy.GetInstanceNonPublicFieldValue("deltaBackoff"));
    }
}