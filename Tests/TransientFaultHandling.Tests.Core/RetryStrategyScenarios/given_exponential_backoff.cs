﻿namespace Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling.Tests.RetryStrategyScenarios.given_exponential_backoff
{
    using System;
    using Microsoft.Practices.EnterpriseLibrary.Common.TestSupport.ContextBase;
    using Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling;
    using VisualStudio.TestTools.UnitTesting;

    public abstract class Context : ArrangeActAssert
    {
        protected override void Arrange()
        {
        }
    }

    [TestClass]
    public class when_using_default_values : Context
    {
        protected RetryStrategy retryStrategy;
        protected ShouldRetry shouldRetry;

        protected override void Act()
        {
            this.retryStrategy = new ExponentialBackoff();
            this.shouldRetry = this.retryStrategy.GetShouldRetry();
        }

        [TestMethod]
        public void then_default_values_are_used()
        {
            Assert.IsNull(this.retryStrategy.Name);

            TimeSpan delay;
            Assert.IsTrue(this.shouldRetry(0, null, out delay));
            Assert.AreEqual(TimeSpan.FromSeconds(1), delay);

            Assert.IsTrue(this.shouldRetry(5, null, out delay));
            Assert.IsTrue(delay >= TimeSpan.FromSeconds(1) && delay <= TimeSpan.FromSeconds(30));

            Assert.IsTrue(this.shouldRetry(9, null, out delay));
            Assert.AreEqual(TimeSpan.FromSeconds(30), delay);

            Assert.IsFalse(this.shouldRetry(10, null, out delay));
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
            this.retryStrategy = new ExponentialBackoff("name", 5, TimeSpan.FromSeconds(5), TimeSpan.FromMinutes(5), TimeSpan.FromSeconds(10));
            this.shouldRetry = this.retryStrategy.GetShouldRetry();
        }

        [TestMethod]
        public void then_default_values_are_used()
        {
            Assert.AreEqual("name", this.retryStrategy.Name);

            TimeSpan delay;
            Assert.IsTrue(this.shouldRetry(0, null, out delay));
            Assert.AreEqual(TimeSpan.FromSeconds(5), delay);

            Assert.IsTrue(this.shouldRetry(4, null, out delay));
            Assert.IsTrue(delay >= TimeSpan.FromSeconds(5) && delay <= TimeSpan.FromMinutes(30));

            Assert.IsFalse(this.shouldRetry(5, null, out delay));
            Assert.AreEqual(TimeSpan.Zero, delay);
        }
    }
}
