namespace Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling.Tests.Configuration;

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