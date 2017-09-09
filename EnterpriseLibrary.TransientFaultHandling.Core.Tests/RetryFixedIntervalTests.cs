namespace Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling.Tests
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class RetryFixedIntervalTests
    {
        [TestMethod]
        public void FixedIntervalWithoutResultTest()
        {
            const int retryCount = 5;
            TimeSpan retryInterval = TimeSpan.FromSeconds(1);
            Counter<InvalidOperationException> counter = new Counter<InvalidOperationException>(retryCount);
            int retryFuncCount = 0;
            int retryHandlerCount = 0;
            Retry.FixedInterval(
                () =>
                {
                    retryFuncCount++;
                    counter.Increase();
                },
                retryCount,
                exception => exception is InvalidOperationException,
                (sender, e) =>
                {
                    Assert.IsInstanceOfType(e.LastException, typeof(InvalidOperationException));
                    Assert.AreEqual(retryInterval, e.Delay);
                    Assert.AreEqual(counter.Time.Count, e.CurrentRetryCount);
                    retryHandlerCount++;
                },
                retryInterval,
                false);
            Assert.AreEqual(retryCount, retryFuncCount);
            Assert.AreEqual(retryCount - 1, retryHandlerCount);
            Assert.AreEqual(retryCount, counter.Time.Count);
            TimeSpan[] intervals = counter.Time.Take(counter.Time.Count - 1).Zip(counter.Time.Skip(1), (a, b) => b - a).ToArray();
            Assert.AreEqual(retryCount - 1, intervals.Length);
            Assert.IsTrue(intervals.All(interval => interval >= retryInterval));
        }

        [TestMethod]
        public void FixedIntervalWithResultTest()
        {
            const int retryCount = 5;
            const int result = 1;
            TimeSpan retryInterval = TimeSpan.FromSeconds(1);
            Counter<InvalidOperationException> counter = new Counter<InvalidOperationException>(retryCount);
            int retryFuncCount = 0;
            int retryHandlerCount = 0;
            Assert.AreEqual(
                result, 
                Retry.FixedInterval(
                    () =>
                    {
                        retryFuncCount++;
                        counter.Increase();
                        return result;
                    },
                    retryCount,
                    exception => exception is InvalidOperationException,
                    (sender, e) =>
                    {
                        Assert.IsInstanceOfType(e.LastException, typeof(InvalidOperationException));
                        Assert.AreEqual(retryInterval, e.Delay);
                        Assert.AreEqual(counter.Time.Count, e.CurrentRetryCount);
                        retryHandlerCount++;
                    },
                    retryInterval,
                    false));
            Assert.AreEqual(retryCount, retryFuncCount);
            Assert.AreEqual(retryCount - 1, retryHandlerCount);
            Assert.AreEqual(retryCount, counter.Time.Count);
            TimeSpan[] intervals = counter.Time.Take(counter.Time.Count - 1).Zip(counter.Time.Skip(1), (a, b) => b - a).ToArray();
            Assert.AreEqual(retryCount - 1, intervals.Length);
            Assert.IsTrue(intervals.All(interval => interval >= retryInterval));
        }

        [TestMethod]
        public async Task FixedIntervalAsyncWithoutResultTest()
        {
            const int retryCount = 5;
            TimeSpan retryInterval = TimeSpan.FromSeconds(1);
            Counter<InvalidOperationException> counter = new Counter<InvalidOperationException>(retryCount);
            int retryFuncCount = 0;
            int retryHandlerCount = 0;
            await Retry.FixedIntervalAsync(
                async () =>
                {
                    retryFuncCount++;
                    counter.Increase();
                    await Task.Yield();
                },
                retryCount,
                exception => exception is InvalidOperationException,
                (sender, e) =>
                {
                    Assert.IsInstanceOfType(e.LastException, typeof(InvalidOperationException));
                    Assert.AreEqual(retryInterval, e.Delay);
                    Assert.AreEqual(counter.Time.Count, e.CurrentRetryCount);
                    retryHandlerCount++;
                },
                retryInterval,
                false);
            Assert.AreEqual(retryCount, retryFuncCount);
            Assert.AreEqual(retryCount - 1, retryHandlerCount);
            Assert.AreEqual(retryCount, counter.Time.Count);
            TimeSpan[] intervals = counter.Time.Take(counter.Time.Count - 1).Zip(counter.Time.Skip(1), (a, b) => b - a).ToArray();
            Assert.AreEqual(retryCount - 1, intervals.Length);
            Assert.IsTrue(intervals.All(interval => interval >= retryInterval));
        }

        [TestMethod]
        public async Task FixedIntervalAsyncWithResultTest()
        {
            const int retryCount = 5;
            const int result = 1;
            TimeSpan retryInterval = TimeSpan.FromSeconds(1);
            Counter<InvalidOperationException> counter = new Counter<InvalidOperationException>(retryCount);
            int retryFuncCount = 0;
            int retryHandlerCount = 0;
            Assert.AreEqual(
                result,
                await Retry.FixedIntervalAsync(
                    async () =>
                    {
                        retryFuncCount++;
                        counter.Increase();
                        await Task.Yield();
                        return result;
                    },
                    retryCount,
                    exception => exception is InvalidOperationException,
                    (sender, e) =>
                    {
                        Assert.IsInstanceOfType(e.LastException, typeof(InvalidOperationException));
                        Assert.AreEqual(retryInterval, e.Delay);
                        Assert.AreEqual(counter.Time.Count, e.CurrentRetryCount);
                        retryHandlerCount++;
                    },
                    retryInterval,
                    false));
            Assert.AreEqual(retryCount, retryFuncCount);
            Assert.AreEqual(retryCount - 1, retryHandlerCount);
            Assert.AreEqual(retryCount, counter.Time.Count);
            TimeSpan[] intervals = counter.Time.Take(counter.Time.Count - 1).Zip(counter.Time.Skip(1), (a, b) => b - a).ToArray();
            Assert.AreEqual(retryCount - 1, intervals.Length);
            Assert.IsTrue(intervals.All(interval => interval >= retryInterval));
        }
    }
}
