namespace Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling.Tests.RetryStrategies.given_constructors;

[TestClass]
public class when_creating_a_fixed_interval_strategy : Context
{
    [TestMethod]
    [ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void then_throws_if_retry_count_is_negative() => new FixedInterval("", -1, TimeSpan.FromSeconds(1), true);

    [TestMethod]
    [ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void then_throws_if_retry_interval_is_negative() => new FixedInterval("", 1, TimeSpan.FromSeconds(-1), true);
}