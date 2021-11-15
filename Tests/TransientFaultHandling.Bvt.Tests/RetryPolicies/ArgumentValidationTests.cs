namespace Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling.Bvt.Tests.RetryPolicies;

[TestClass]
public class ArgumentValidationTests
{
    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void ExceptionIsThrownWhenExecutingANullAction()
    {
        RetryPolicy policy = new(new MockErrorDetectionStrategy(), 10);
        policy.ExecuteAsync(null!);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void ExceptionIsThrownWhenExecutingANullTask()
    {
        RetryPolicy policy = new(new MockErrorDetectionStrategy(), 10);
        policy.ExecuteAsync<int>(null!);
    }
}