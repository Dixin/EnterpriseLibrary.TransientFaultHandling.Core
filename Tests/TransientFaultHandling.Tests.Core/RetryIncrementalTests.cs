namespace Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling.Tests
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class RetryIncrementalTests
    {
        [TestMethod]
        public void IncrementalWithoutResultTest()
        {
            const int retryCount = 5;
            TimeSpan retryInterval = TimeSpan.FromSeconds(1);
            TimeSpan incremental = TimeSpan.FromSeconds(1);
            Counter<InvalidOperationException> counter = new Counter<InvalidOperationException>(retryCount);
            int retryFuncCount = 0;
            int retryHandlerCount = 0;
            Retry.Incremental(
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
                    Assert.AreEqual(retryInterval + TimeSpan.FromTicks(incremental.Ticks * retryHandlerCount), e.Delay);
                    Assert.AreEqual(counter.Time.Count, e.CurrentRetryCount);
                    retryHandlerCount++;
                },
                retryInterval,
                incremental,
                false);
            Assert.AreEqual(retryCount, retryFuncCount);
            Assert.AreEqual(retryCount - 1, retryHandlerCount);
            Assert.AreEqual(retryCount, counter.Time.Count);
            TimeSpan[] intervals = counter.Time.Take(counter.Time.Count - 1).Zip(counter.Time.Skip(1), (a, b) => b - a).ToArray();
            Assert.AreEqual(retryCount - 1, intervals.Length);
            intervals.ForEach((interval, index) => Assert.IsTrue(TimeSpanHelper.AlmostEquals(interval.Ticks, retryInterval.Ticks + incremental.Ticks * index)));
        }

        [TestMethod]
        public void IncrementalWithResultTest()
        {
            const int retryCount = 5;
            const int result = 1;
            TimeSpan retryInterval = TimeSpan.FromSeconds(1);
            TimeSpan incremental = TimeSpan.FromSeconds(1);
            Counter<InvalidOperationException> counter = new Counter<InvalidOperationException>(retryCount);
            int retryFuncCount = 0;
            int retryHandlerCount = 0;
            Assert.AreEqual(
                result, 
                Retry.Incremental(
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
                        Assert.AreEqual(retryInterval + TimeSpan.FromTicks(incremental.Ticks * retryHandlerCount), e.Delay);
                        Assert.AreEqual(counter.Time.Count, e.CurrentRetryCount);
                        retryHandlerCount++;
                    },
                    retryInterval,
                    incremental,
                    false));
            Assert.AreEqual(retryCount, retryFuncCount);
            Assert.AreEqual(retryCount - 1, retryHandlerCount);
            Assert.AreEqual(retryCount, counter.Time.Count);
            TimeSpan[] intervals = counter.Time.Take(counter.Time.Count - 1).Zip(counter.Time.Skip(1), (a, b) => b - a).ToArray();
            Assert.AreEqual(retryCount - 1, intervals.Length);
            intervals.ForEach((interval, index) => Assert.IsTrue(TimeSpanHelper.AlmostEquals(interval.Ticks, retryInterval.Ticks + incremental.Ticks * index)));
        }

        [TestMethod]
        public async Task IncrementalAsyncWithoutResultTest()
        {
            const int retryCount = 5;
            TimeSpan retryInterval = TimeSpan.FromSeconds(1);
            TimeSpan incremental = TimeSpan.FromSeconds(1);
            Counter<InvalidOperationException> counter = new Counter<InvalidOperationException>(retryCount);
            int retryFuncCount = 0;
            int retryHandlerCount = 0;
            await Retry.IncrementalAsync(
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
                    Assert.AreEqual(retryInterval + TimeSpan.FromTicks(incremental.Ticks * retryHandlerCount), e.Delay);
                    Assert.AreEqual(counter.Time.Count, e.CurrentRetryCount);
                    retryHandlerCount++;
                },
                retryInterval,
                incremental,
                false);
            Assert.AreEqual(retryCount, retryFuncCount);
            Assert.AreEqual(retryCount - 1, retryHandlerCount);
            Assert.AreEqual(retryCount, counter.Time.Count);
            TimeSpan[] intervals = counter.Time.Take(counter.Time.Count - 1).Zip(counter.Time.Skip(1), (a, b) => b - a).ToArray();
            Assert.AreEqual(retryCount - 1, intervals.Length);
            intervals.ForEach((interval, index) => Assert.IsTrue(TimeSpanHelper.AlmostEquals(interval.Ticks, retryInterval.Ticks + incremental.Ticks * index)));
        }

        [TestMethod]
        public async Task IncrementalAsyncWithResultTest()
        {
            const int retryCount = 5;
            const int result = 1;
            TimeSpan retryInterval = TimeSpan.FromSeconds(1);
            TimeSpan incremental = TimeSpan.FromSeconds(1);
            Counter<InvalidOperationException> counter = new Counter<InvalidOperationException>(retryCount);
            int retryFuncCount = 0;
            int retryHandlerCount = 0;
            Assert.AreEqual(
                result,
                await Retry.IncrementalAsync(
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
                        Assert.AreEqual(retryInterval + TimeSpan.FromTicks(incremental.Ticks * retryHandlerCount), e.Delay);
                        Assert.AreEqual(counter.Time.Count, e.CurrentRetryCount);
                        retryHandlerCount++;
                    },
                    retryInterval,
                    incremental,
                    false));
            Assert.AreEqual(retryCount, retryFuncCount);
            Assert.AreEqual(retryCount - 1, retryHandlerCount);
            Assert.AreEqual(retryCount, counter.Time.Count);
            TimeSpan[] intervals = counter.Time.Take(counter.Time.Count - 1).Zip(counter.Time.Skip(1), (a, b) => b - a).ToArray();
            Assert.AreEqual(retryCount - 1, intervals.Length);
            intervals.ForEach((interval, index) => Assert.IsTrue(TimeSpanHelper.AlmostEquals(interval.Ticks, retryInterval.Ticks + incremental.Ticks * index)));
        }
    }
}
