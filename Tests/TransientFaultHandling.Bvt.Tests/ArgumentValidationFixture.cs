namespace Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling.Tests
{
    using System;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using global::TransientFaultHandling.Tests.TestObjects;

    [TestClass]
    public class ArgumentValidationFixture
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ExceptionIsThrownWhenExecutingANullAction()
        {
            RetryPolicy policy = new(new MockErrorDetectionStrategy(), 10);
            policy.ExecuteAsync(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ExceptionIsThrownWhenExecutingANullTask()
        {
            RetryPolicy policy = new(new MockErrorDetectionStrategy(), 10);
            policy.ExecuteAsync<int>(null);
        }
    }
}
