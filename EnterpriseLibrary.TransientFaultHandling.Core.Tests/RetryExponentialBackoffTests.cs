namespace Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling.Tests
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class RetryExponentialBackoffTests
    {
        [TestMethod]
        public void ExponentialBackoffWithoutResultTest()
        {
            const int retryCount = 5;
            TimeSpan maxBackoff = TimeSpan.FromSeconds(5);
            Counter<InvalidOperationException> counter = new Counter<InvalidOperationException>(retryCount);
            int retryFuncCount = 0;
            int retryHandlerCount = 0;
            Retry.ExponentialBackoff(
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
                    Assert.IsTrue(e.Delay >= RetryStrategy.DefaultMinBackoff && e.Delay <= RetryStrategy.DefaultMaxBackoff);
                    Assert.AreEqual(counter.Time.Count, e.CurrentRetryCount);
                    retryHandlerCount++;
                },
                maxBackoff: maxBackoff,
                firstFastRetry: false);
            Assert.AreEqual(retryCount, retryFuncCount);
            Assert.AreEqual(retryCount - 1, retryHandlerCount);
            Assert.AreEqual(retryCount, counter.Time.Count);
            TimeSpan[] intervals = counter.Time.Take(counter.Time.Count - 1).Zip(counter.Time.Skip(1), (a, b) => b - a).ToArray();
            Assert.AreEqual(retryCount - 1, intervals.Length);
            intervals.ForEach((interval, index) => Assert.IsTrue(interval >= RetryStrategy.DefaultMinBackoff && interval <= maxBackoff + TimeSpan.FromTicks(TimeSpanHelper.Delta)));
        }

        [TestMethod]
        public void ExponentialBackoffWithResultTest()
        {
            const int retryCount = 5;
            const int result = 1;
            TimeSpan maxBackoff = TimeSpan.FromSeconds(5);
            Counter<InvalidOperationException> counter = new Counter<InvalidOperationException>(retryCount);
            int retryFuncCount = 0;
            int retryHandlerCount = 0;
            Assert.AreEqual(
                result,
                Retry.ExponentialBackoff(
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
                        Assert.IsTrue(e.Delay >= RetryStrategy.DefaultMinBackoff && e.Delay <= RetryStrategy.DefaultMaxBackoff);
                        Assert.AreEqual(counter.Time.Count, e.CurrentRetryCount);
                        retryHandlerCount++;
                    },
                    maxBackoff: maxBackoff,
                    firstFastRetry: false));
            Assert.AreEqual(retryCount, retryFuncCount);
            Assert.AreEqual(retryCount - 1, retryHandlerCount);
            Assert.AreEqual(retryCount, counter.Time.Count);
            TimeSpan[] intervals = counter.Time.Take(counter.Time.Count - 1).Zip(counter.Time.Skip(1), (a, b) => b - a).ToArray();
            Assert.AreEqual(retryCount - 1, intervals.Length);
            intervals.ForEach((interval, index) => Assert.IsTrue(interval >= RetryStrategy.DefaultMinBackoff && interval <= maxBackoff + TimeSpan.FromTicks(TimeSpanHelper.Delta)));
        }

        [TestMethod]
        public async Task ExponentialBackoffAsyncWithoutResultTest()
        {
            const int retryCount = 5;
            TimeSpan maxBackoff = TimeSpan.FromSeconds(5);
            Counter<InvalidOperationException> counter = new Counter<InvalidOperationException>(retryCount);
            int retryFuncCount = 0;
            int retryHandlerCount = 0;
            await Retry.ExponentialBackoffAsync(
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
                    Assert.IsTrue(e.Delay >= RetryStrategy.DefaultMinBackoff && e.Delay <= RetryStrategy.DefaultMaxBackoff);
                    Assert.AreEqual(counter.Time.Count, e.CurrentRetryCount);
                    retryHandlerCount++;
                },
                maxBackoff: maxBackoff,
                firstFastRetry: false);
            Assert.AreEqual(retryCount, retryFuncCount);
            Assert.AreEqual(retryCount - 1, retryHandlerCount);
            Assert.AreEqual(retryCount, counter.Time.Count);
            TimeSpan[] intervals = counter.Time.Take(counter.Time.Count - 1).Zip(counter.Time.Skip(1), (a, b) => b - a).ToArray();
            Assert.AreEqual(retryCount - 1, intervals.Length);
            intervals.ForEach((interval, index) => Assert.IsTrue(interval >= RetryStrategy.DefaultMinBackoff && interval <= maxBackoff + TimeSpan.FromTicks(TimeSpanHelper.Delta)));
        }

        [TestMethod]
        public async Task ExponentialBackoffAsyncWithResultTest()
        {
            const int retryCount = 5;
            const int result = 1;
            TimeSpan maxBackoff = TimeSpan.FromSeconds(5);
            Counter<InvalidOperationException> counter = new Counter<InvalidOperationException>(retryCount);
            int retryFuncCount = 0;
            int retryHandlerCount = 0;
            Assert.AreEqual(
                result,
                await Retry.ExponentialBackoffAsync(
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
                        Assert.IsTrue(e.Delay >= RetryStrategy.DefaultMinBackoff && e.Delay <= RetryStrategy.DefaultMaxBackoff);
                        Assert.AreEqual(counter.Time.Count, e.CurrentRetryCount);
                        retryHandlerCount++;
                    },
                    maxBackoff: maxBackoff,
                    firstFastRetry: false));
            Assert.AreEqual(retryCount, retryFuncCount);
            Assert.AreEqual(retryCount - 1, retryHandlerCount);
            Assert.AreEqual(retryCount, counter.Time.Count);
            TimeSpan[] intervals = counter.Time.Take(counter.Time.Count - 1).Zip(counter.Time.Skip(1), (a, b) => b - a).ToArray();
            Assert.AreEqual(retryCount - 1, intervals.Length);
            intervals.ForEach((interval, index) => Assert.IsTrue(interval >= RetryStrategy.DefaultMinBackoff && interval <= maxBackoff + TimeSpan.FromTicks(TimeSpanHelper.Delta)));
        }
    }
}
