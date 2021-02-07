namespace Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling.Bvt.Tests.Extensibility
{
    using global::TransientFaultHandling.Tests.TestObjects;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling.Bvt.Tests.TestObjects;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ExtensibilityFixture
    {
        private RetryManager retryManager;

        [TestInitialize]
        public void Initialize() =>
            this.retryManager = RetryConfiguration.GetRetryManager(getCustomRetryStrategy: section => 
                section.Get<TestRetryStrategyOptions>().ToTestRetryStrategy(section.Key));

        [TestCleanup]
        public void Cleanup()
        {
        }

        [TestMethod]
        public void CreatesCustomRetryStrategyFromConfiguration()
        {
            RetryPolicy<MockErrorDetectionStrategy> mockCustomStrategy = this.retryManager.GetRetryPolicy<MockErrorDetectionStrategy>("Test Retry Strategy");
            Assert.IsInstanceOfType(mockCustomStrategy.RetryStrategy, typeof(TestRetryStrategy));
        }
    }
}
