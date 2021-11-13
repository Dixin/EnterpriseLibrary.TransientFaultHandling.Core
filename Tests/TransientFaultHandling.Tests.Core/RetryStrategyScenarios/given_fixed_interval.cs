﻿namespace Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling.Tests.RetryStrategyScenarios.given_fixed_interval;

public abstract class Context : ArrangeActAssert
{
    protected override void Arrange()
    {
    }
}

[TestClass]
public class when_using_default_values : Context
{
    protected RetryStrategy? retryStrategy;
    protected ShouldRetry? shouldRetry;

    protected override void Act()
    {
        this.retryStrategy = new FixedInterval();
        this.shouldRetry = this.retryStrategy.GetShouldRetry();
    }

    [TestMethod]
    public void then_default_values_are_used()
    {
        Assert.IsNull(this.retryStrategy!.Name);

        Assert.IsTrue(this.shouldRetry!(0, null, out TimeSpan delay) == true);
        Assert.AreEqual(TimeSpan.FromSeconds(1), delay);

        Assert.IsTrue(this.shouldRetry(9, null, out delay));
        Assert.AreEqual(TimeSpan.FromSeconds(1), delay);

        Assert.IsFalse(this.shouldRetry(10, null, out delay));
        Assert.AreEqual(TimeSpan.Zero, delay);
    }
}

[TestClass]
public class when_using_default_interval : Context
{
    protected RetryStrategy retryStrategy;
    protected ShouldRetry shouldRetry;

    protected override void Act()
    {
        this.retryStrategy = new FixedInterval(5);
        this.shouldRetry = this.retryStrategy.GetShouldRetry();
    }

    [TestMethod]
    public void then_default_values_are_used()
    {
        Assert.IsNull(this.retryStrategy.Name);

        Assert.IsTrue(this.shouldRetry(0, null, out TimeSpan delay));
        Assert.AreEqual(TimeSpan.FromSeconds(1), delay);

        Assert.IsTrue(this.shouldRetry(4, null, out delay));
        Assert.AreEqual(TimeSpan.FromSeconds(1), delay);

        Assert.IsFalse(this.shouldRetry(5, null, out delay));
        Assert.AreEqual(TimeSpan.Zero, delay);
    }
}

[TestClass]
public class when_using_custom_values : Context
{
    protected RetryStrategy retryStrategy;
    protected ShouldRetry shouldRetry;

    protected override void Act()
    {
        this.retryStrategy = new FixedInterval("name", 5, TimeSpan.FromSeconds(5));
        this.shouldRetry = this.retryStrategy.GetShouldRetry();
    }

    [TestMethod]
    public void then_default_values_are_used()
    {
        Assert.AreEqual("name", this.retryStrategy.Name);

        Assert.IsTrue(this.shouldRetry(0, null, out TimeSpan delay));
        Assert.AreEqual(TimeSpan.FromSeconds(5), delay);

        Assert.IsTrue(this.shouldRetry(4, null, out delay));
        Assert.AreEqual(TimeSpan.FromSeconds(5), delay);

        Assert.IsFalse(this.shouldRetry(5, null, out delay));
        Assert.AreEqual(TimeSpan.Zero, delay);
    }
}