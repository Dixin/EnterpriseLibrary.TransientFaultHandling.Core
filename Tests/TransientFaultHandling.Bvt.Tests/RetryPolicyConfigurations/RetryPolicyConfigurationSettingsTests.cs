namespace Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling.Bvt.Tests.RetryPolicyConfigurations;

[TestClass]
public class RetryPolicyConfigurationSettingsTests
{
    private RetryManagerOptions? retryManagerOptions;

    [TestInitialize]
    public void Initialize() =>
        this.retryManagerOptions = RetryConfiguration.GetConfiguration().GetSection(nameof(RetryManager)).Get<RetryManagerOptions>();

    [TestCleanup]
    public void Cleanup()
    {
    }

    [TestMethod]
    public void ReadsFixedIntervalRetryStrategyValuesFromConfiguration()
    {
        IConfigurationSection? section = this.retryManagerOptions?.RetryStrategy?.GetSection("Fixed Interval Non Default");
        FixedIntervalOptions data = section.Get<FixedIntervalOptions>();

        Assert.AreEqual("Fixed Interval Non Default", section?.Key);
        Assert.AreEqual(new TimeSpan(0, 0, 2), data.RetryInterval);
        Assert.AreEqual(2, data.RetryCount);
        Assert.AreEqual(false, data.FastFirstRetry);
    }

    [TestMethod]
    public void ReadsIncrementalRetryStrategyValuesFromConfiguration()
    {
        IConfigurationSection? section = this.retryManagerOptions?.RetryStrategy?.GetSection("Incremental Non Default");
        IncrementalOptions data = section.Get<IncrementalOptions>();

        Assert.AreEqual("Incremental Non Default", section?.Key);
        Assert.AreEqual(new TimeSpan(0, 0, 1), data.InitialInterval);
        Assert.AreEqual(new TimeSpan(0, 0, 2), data.Increment);
        Assert.AreEqual(3, data.RetryCount);
        Assert.AreEqual(false, data.FastFirstRetry);
    }

    [TestMethod]
    public void ReadsExponentialBackoffRetryStrategyValuesFromConfiguration()
    {
        IConfigurationSection? section = this.retryManagerOptions?.RetryStrategy?.GetSection("Exponential Backoff Non Default");
        ExponentialBackoffOptions data = section.Get<ExponentialBackoffOptions>();

        Assert.AreEqual("Exponential Backoff Non Default", section?.Key);
        Assert.AreEqual(new TimeSpan(0, 0, 1), data.MinBackOff);
        Assert.AreEqual(new TimeSpan(0, 0, 2), data.MaxBackOff);
        Assert.AreEqual(TimeSpan.FromMilliseconds(300), data.DeltaBackOff);
        Assert.AreEqual(4, data.RetryCount);
        Assert.AreEqual(false, data.FastFirstRetry);
    }
}