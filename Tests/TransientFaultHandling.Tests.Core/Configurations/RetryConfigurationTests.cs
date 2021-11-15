namespace Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling.Tests.Configurations;

[TestClass]
public class RetryConfigurationTests
{
    [TestMethod]
    public void RetryConfigurationTest()
    {
        RetryManager retryManager = RetryConfiguration.GetRetryManager();
        Assert.IsNotNull(retryManager);

        try
        {
            retryManager = RetryConfiguration.GetRetryManager("app.ini");
            Assert.Fail();
        }
        catch (ArgumentException exception)
        {
            Trace.WriteLine(exception);
        }

        try
        {
            retryManager = RetryConfiguration.GetRetryManager("app.json");
            Assert.Fail();
        }
        catch (ArgumentException exception)
        {
            Trace.WriteLine(exception);
        }

        try
        {
            retryManager = RetryConfiguration.GetRetryManager("app.xml");
            Assert.Fail();
        }
        catch (ArgumentException exception)
        {
            Trace.WriteLine(exception);
        }

        IDictionary<string, RetryStrategy> retryStrategies;
        try
        {
            retryStrategies = RetryConfiguration.GetRetryStrategies();
            Assert.Fail();
        }
        catch (ArgumentException exception)
        {
            Trace.WriteLine(exception);
        }

        retryStrategies = RetryConfiguration.GetRetryStrategies("app.ini");
        Assert.IsNotNull(retryStrategies);
        Assert.IsTrue(retryStrategies.Count > 0);

        retryStrategies = RetryConfiguration.GetRetryStrategies("app.json");
        Assert.IsNotNull(retryStrategies);
        Assert.IsTrue(retryStrategies.Count > 0);

        retryStrategies = RetryConfiguration.GetRetryStrategies("app.xml");
        Assert.IsNotNull(retryStrategies);
        Assert.IsTrue(retryStrategies.Count > 0);
    }
}