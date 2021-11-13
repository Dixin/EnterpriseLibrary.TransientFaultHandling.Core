namespace Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling.Bvt.Tests;

[TestClass]
public class RetryPolicyProgrammaticallyFixture
{
    [TestMethod]
    public void CreateFixedIntervalRetryStrategyWithCountAndInterval()
    {
        try
        {
            RetryPolicy<MockErrorDetectionStrategy> retryPolicy = new(new FixedInterval(3, TimeSpan.FromSeconds(1)));
            retryPolicy.ExecuteAction(() => throw new InvalidCastException());
        }
        catch (InvalidCastException)
        {
        }
        catch (Exception)
        {
            Assert.Fail();
        }
    }
}