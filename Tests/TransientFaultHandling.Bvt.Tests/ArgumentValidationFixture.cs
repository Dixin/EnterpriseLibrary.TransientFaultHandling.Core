namespace Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling.Tests;

[TestClass]
public class ArgumentValidationFixture
{
    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void ExceptionIsThrownWhenExecutingANullAction()
    {
        RetryPolicy policy = new(new MockErrorDetectionStrategy(), 10);
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
        policy.ExecuteAsync(null);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void ExceptionIsThrownWhenExecutingANullTask()
    {
        RetryPolicy policy = new(new MockErrorDetectionStrategy(), 10);
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
        policy.ExecuteAsync<int>(null);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
    }
}