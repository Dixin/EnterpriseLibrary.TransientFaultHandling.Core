namespace Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling.Bvt.Tests;

[TestClass]
public class RetryPolicyFactoryDefaultsFixture
{
    [TestInitialize]
    public void Initialize()
    {
        RetryPolicyFactory.SetRetryManager(RetryConfiguration.GetRetryManager(getCustomRetryStrategy: section =>
            section.Get<TestRetryStrategyOptions>().ToTestRetryStrategy(section.Key)), false);
    }

    [TestMethod]
    public void CreatesDefaultRetryPolicyFromConfiguration()
    {
        RetryPolicy retryPolicy = RetryPolicyFactory.GetRetryPolicy<MockErrorDetectionStrategy>();
        Incremental retryStrategy = retryPolicy.RetryStrategy as Incremental;

        Assert.AreEqual("Default Retry Strategy", retryStrategy.Name);
    }

    [TestMethod]
    public void CreatesDefaultSqlConnectionPolicyFromConfiguration()
    {
        RetryPolicy retryPolicy = RetryPolicyFactory.GetDefaultSqlConnectionRetryPolicy();
        Incremental retryStrategy = retryPolicy.RetryStrategy as Incremental;

        Assert.AreEqual("Default SqlConnection Retry Strategy", retryStrategy.Name);
    }

    [TestMethod]
    public void CreatesDefaultSqlCommandPolicyFromConfiguration()
    {
        RetryPolicy retryPolicy = RetryPolicyFactory.GetDefaultSqlCommandRetryPolicy();
        Incremental retryStrategy = retryPolicy.RetryStrategy as Incremental;

        Assert.AreEqual("Default SqlCommand Retry Strategy", retryStrategy.Name);
        Assert.IsInstanceOfType(retryPolicy.ErrorDetectionStrategy, typeof(SqlDatabaseTransientErrorDetectionStrategy));
    }

    // TODO.
    //[TestMethod]
    //public void CreatesDefaultServiceBusPolicyFromConfiguration()
    //{
    //    var retryPolicy = RetryPolicyFactory.GetDefaultAzureServiceBusRetryPolicy();
    //    var retryStrategy = retryPolicy.RetryStrategy as Incremental;

    //    Assert.AreEqual("Default Azure ServiceBus Retry Strategy", retryStrategy.Name);
    //    var busPolicy1 = RetryPolicyFactory.GetRetryPolicy<ServiceBusTransientErrorDetectionStrategy>();
    //    Assert.IsInstanceOfType(busPolicy1.RetryStrategy, typeof(Incremental));
    //    Assert.IsInstanceOfType(retryPolicy.ErrorDetectionStrategy, typeof(ServiceBusTransientErrorDetectionStrategy));
    //    Assert.IsInstanceOfType(busPolicy1.ErrorDetectionStrategy, typeof(ServiceBusTransientErrorDetectionStrategy));
    //}

    //[TestMethod]
    //public void CreatesDefaultAzureCachingPolicyFromConfiguration()
    //{
    //    var retryPolicy = RetryPolicyFactory.GetDefaultAzureCachingRetryPolicy();
    //    var retryStrategy = retryPolicy.RetryStrategy as Incremental;

    //    Assert.AreEqual("Default Azure Caching Retry Strategy", retryStrategy.Name);
    //    Assert.IsInstanceOfType(retryPolicy.ErrorDetectionStrategy, typeof(CacheTransientErrorDetectionStrategy));
    //    var cachePolicy1 = RetryPolicyFactory.GetRetryPolicy<CacheTransientErrorDetectionStrategy>();
    //    Assert.IsInstanceOfType(cachePolicy1.ErrorDetectionStrategy, typeof(CacheTransientErrorDetectionStrategy));
    //    Assert.IsInstanceOfType(cachePolicy1.RetryStrategy, typeof(Incremental));
    //}

    //[TestMethod]
    //public void CreatesDefaultAzureStoragePolicyFromConfiguration()
    //{
    //    var retryPolicy = RetryPolicyFactory.GetDefaultAzureStorageRetryPolicy();
    //    var retryStrategy = retryPolicy.RetryStrategy as Incremental;

    //    Assert.AreEqual("Default Azure Storage Retry Strategy", retryStrategy.Name);
    //    Assert.IsInstanceOfType(retryPolicy.ErrorDetectionStrategy, typeof(StorageTransientErrorDetectionStrategy));
    //    var storagePolicy1 = RetryPolicyFactory.GetRetryPolicy<StorageTransientErrorDetectionStrategy>();
    //    Assert.IsInstanceOfType(storagePolicy1.RetryStrategy, typeof(Incremental));
    //    Assert.IsInstanceOfType(storagePolicy1.ErrorDetectionStrategy, typeof(StorageTransientErrorDetectionStrategy));
    //}

    [TestMethod]
    public void PolicyInstancesAreNotSingletons()
    {
        RetryPolicy connPolicy = RetryPolicyFactory.GetDefaultSqlConnectionRetryPolicy();
        Incremental nonDefaultIncRetry = connPolicy.RetryStrategy as Incremental;

        RetryPolicy connPolicy1 = RetryPolicyFactory.GetDefaultSqlConnectionRetryPolicy();
        Incremental nonDefaultIncRetry1 = connPolicy1.RetryStrategy as Incremental;

        Assert.AreNotSame(connPolicy, connPolicy1);
    }

    [TestMethod]
    public void DefaultRetryPolicyIsNoRetry()
    {
        int count = 0;
        RetryPolicy connPolicy = RetryPolicyFactory.GetRetryPolicy<MockErrorDetectionStrategy>();
        try
        {
            connPolicy.ExecuteAction(() =>
            {
                count++;
                throw new ApplicationException();
            });
        }
        catch (ApplicationException)
        {
            Assert.AreEqual(1, count);
        }
        catch (Exception)
        {
            Assert.Fail();
        }
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void ExceptionIsThrownWhenRetryStrategyIsNotDefinedInConfiguration()
    {
        RetryPolicyFactory.GetRetryPolicy<MockErrorDetectionStrategy>(retryStrategyName: "someinstancewhichdoesnotexist");
    }
}