namespace Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling.Tests;

[TestClass]
public class RetryIncrementalTests
{
    [TestMethod]
    public void IncrementalWithoutResultTest()
    {
        const int RetryCount = 5;
        TimeSpan retryInterval = TimeSpan.FromSeconds(1);
        TimeSpan incremental = TimeSpan.FromSeconds(1);
        Counter<InvalidOperationException> counter = new(RetryCount);
        int retryFuncCount = 0;
        int retryHandlerCount = 0;
        Retry.Incremental(
            () =>
            {
                retryFuncCount++;
                counter.Increase();
            },
            RetryCount,
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
        Assert.AreEqual(RetryCount, retryFuncCount);
        Assert.AreEqual(RetryCount - 1, retryHandlerCount);
        Assert.AreEqual(RetryCount, counter.Time.Count);
        TimeSpan[] intervals = counter.Time.Take(counter.Time.Count - 1).Zip(counter.Time.Skip(1), (a, b) => b - a).ToArray();
        Assert.AreEqual(RetryCount - 1, intervals.Length);
        intervals.ForEach((interval, index) => Assert.IsTrue(TimeSpanHelper.AlmostEquals(interval.Ticks, retryInterval.Ticks + incremental.Ticks * index)));
    }

    [TestMethod]
    public void IncrementalWithResultTest()
    {
        const int RetryCount = 5;
        const int Result = 1;
        TimeSpan retryInterval = TimeSpan.FromSeconds(1);
        TimeSpan incremental = TimeSpan.FromSeconds(1);
        Counter<InvalidOperationException> counter = new(RetryCount);
        int retryFuncCount = 0;
        int retryHandlerCount = 0;
        Assert.AreEqual(
            Result, 
            Retry.Incremental(
                () =>
                {
                    retryFuncCount++;
                    counter.Increase();
                    return Result;
                },
                RetryCount,
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
        Assert.AreEqual(RetryCount, retryFuncCount);
        Assert.AreEqual(RetryCount - 1, retryHandlerCount);
        Assert.AreEqual(RetryCount, counter.Time.Count);
        TimeSpan[] intervals = counter.Time.Take(counter.Time.Count - 1).Zip(counter.Time.Skip(1), (a, b) => b - a).ToArray();
        Assert.AreEqual(RetryCount - 1, intervals.Length);
        intervals.ForEach((interval, index) => Assert.IsTrue(TimeSpanHelper.AlmostEquals(interval.Ticks, retryInterval.Ticks + incremental.Ticks * index)));
    }

    [TestMethod]
    public async Task IncrementalAsyncWithoutResultTest()
    {
        const int RetryCount = 5;
        TimeSpan retryInterval = TimeSpan.FromSeconds(1);
        TimeSpan incremental = TimeSpan.FromSeconds(1);
        Counter<InvalidOperationException> counter = new(RetryCount);
        int retryFuncCount = 0;
        int retryHandlerCount = 0;
        await Retry.IncrementalAsync(
            async () =>
            {
                retryFuncCount++;
                counter.Increase();
                await Task.Yield();
            },
            RetryCount,
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
        Assert.AreEqual(RetryCount, retryFuncCount);
        Assert.AreEqual(RetryCount - 1, retryHandlerCount);
        Assert.AreEqual(RetryCount, counter.Time.Count);
        TimeSpan[] intervals = counter.Time.Take(counter.Time.Count - 1).Zip(counter.Time.Skip(1), (a, b) => b - a).ToArray();
        Assert.AreEqual(RetryCount - 1, intervals.Length);
        intervals.ForEach((interval, index) => Assert.IsTrue(TimeSpanHelper.AlmostEquals(interval.Ticks, retryInterval.Ticks + incremental.Ticks * index)));
    }

    [TestMethod]
    public async Task IncrementalAsyncWithResultTest()
    {
        const int RetryCount = 5;
        const int Result = 1;
        TimeSpan retryInterval = TimeSpan.FromSeconds(1);
        TimeSpan incremental = TimeSpan.FromSeconds(1);
        Counter<InvalidOperationException> counter = new(RetryCount);
        int retryFuncCount = 0;
        int retryHandlerCount = 0;
        Assert.AreEqual(
            Result,
            await Retry.IncrementalAsync(
                async () =>
                {
                    retryFuncCount++;
                    counter.Increase();
                    await Task.Yield();
                    return Result;
                },
                RetryCount,
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
        Assert.AreEqual(RetryCount, retryFuncCount);
        Assert.AreEqual(RetryCount - 1, retryHandlerCount);
        Assert.AreEqual(RetryCount, counter.Time.Count);
        TimeSpan[] intervals = counter.Time.Take(counter.Time.Count - 1).Zip(counter.Time.Skip(1), (a, b) => b - a).ToArray();
        Assert.AreEqual(RetryCount - 1, intervals.Length);
        intervals.ForEach((interval, index) => Assert.IsTrue(TimeSpanHelper.AlmostEquals(interval.Ticks, retryInterval.Ticks + incremental.Ticks * index)));
    }
}