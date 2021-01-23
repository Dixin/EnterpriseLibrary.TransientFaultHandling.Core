namespace Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling.Tests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class RetryManagerJsonTests
    {
        [TestMethod]
        public void RetryManagerJsonTest()
        {
            RetryManager retryManager = RetryConfiguration.GetRetryManager();
            Assert.IsNotNull(retryManager);
        }
    }
}
