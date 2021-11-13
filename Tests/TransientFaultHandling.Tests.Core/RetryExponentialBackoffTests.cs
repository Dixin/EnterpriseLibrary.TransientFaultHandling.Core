namespace Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling.Tests;

[TestClass]
public class RetryExponentialBackoffTests
{
    [TestMethod]
    public void ExponentialBackoffWithoutResultTest()
    {
        const int RetryCount = 5;
        TimeSpan maxBackoff = TimeSpan.FromSeconds(5);
        Counter<InvalidOperationException> counter = new(RetryCount);
        int retryFuncCount = 0;
        int retryHandlerCount = 0;
        Retry.ExponentialBackoff(
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
                Assert.IsTrue(e.Delay >= RetryStrategy.DefaultMinBackoff && e.Delay <= RetryStrategy.DefaultMaxBackoff);
                Assert.AreEqual(counter.Time.Count, e.CurrentRetryCount);
                retryHandlerCount++;
            },
            maxBackoff: maxBackoff,
            firstFastRetry: false);
        Assert.AreEqual(RetryCount, retryFuncCount);
        Assert.AreEqual(RetryCount - 1, retryHandlerCount);
        Assert.AreEqual(RetryCount, counter.Time.Count);
        TimeSpan[] intervals = counter.Time.Take(counter.Time.Count - 1).Zip(counter.Time.Skip(1), (a, b) => b - a).ToArray();
        Assert.AreEqual(RetryCount - 1, intervals.Length);
        intervals.ForEach((interval, index) => Assert.IsTrue(interval >= RetryStrategy.DefaultMinBackoff && interval <= maxBackoff + TimeSpan.FromTicks(TimeSpanHelper.Delta)));
    }

    [TestMethod]
    public void ExponentialBackoffWithResultTest()
    {
        const int RetryCount = 5;
        const int Result = 1;
        TimeSpan maxBackoff = TimeSpan.FromSeconds(5);
        Counter<InvalidOperationException> counter = new(RetryCount);
        int retryFuncCount = 0;
        int retryHandlerCount = 0;
        Assert.AreEqual(
            Result,
            Retry.ExponentialBackoff(
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
                    Assert.IsTrue(e.Delay >= RetryStrategy.DefaultMinBackoff && e.Delay <= RetryStrategy.DefaultMaxBackoff);
                    Assert.AreEqual(counter.Time.Count, e.CurrentRetryCount);
                    retryHandlerCount++;
                },
                maxBackoff: maxBackoff,
                firstFastRetry: false));
        Assert.AreEqual(RetryCount, retryFuncCount);
        Assert.AreEqual(RetryCount - 1, retryHandlerCount);
        Assert.AreEqual(RetryCount, counter.Time.Count);
        TimeSpan[] intervals = counter.Time.Take(counter.Time.Count - 1).Zip(counter.Time.Skip(1), (a, b) => b - a).ToArray();
        Assert.AreEqual(RetryCount - 1, intervals.Length);
        intervals.ForEach((interval, index) => Assert.IsTrue(interval >= RetryStrategy.DefaultMinBackoff && interval <= maxBackoff + TimeSpan.FromTicks(TimeSpanHelper.Delta)));
    }

    [TestMethod]
    public async Task ExponentialBackoffAsyncWithoutResultTest()
    {
        const int RetryCount = 5;
        TimeSpan maxBackoff = TimeSpan.FromSeconds(5);
        Counter<InvalidOperationException> counter = new(RetryCount);
        int retryFuncCount = 0;
        int retryHandlerCount = 0;
        await Retry.ExponentialBackoffAsync(
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
                Assert.IsTrue(e.Delay >= RetryStrategy.DefaultMinBackoff && e.Delay <= RetryStrategy.DefaultMaxBackoff);
                Assert.AreEqual(counter.Time.Count, e.CurrentRetryCount);
                retryHandlerCount++;
            },
            maxBackoff: maxBackoff,
            firstFastRetry: false);
        Assert.AreEqual(RetryCount, retryFuncCount);
        Assert.AreEqual(RetryCount - 1, retryHandlerCount);
        Assert.AreEqual(RetryCount, counter.Time.Count);
        TimeSpan[] intervals = counter.Time.Take(counter.Time.Count - 1).Zip(counter.Time.Skip(1), (a, b) => b - a).ToArray();
        Assert.AreEqual(RetryCount - 1, intervals.Length);
        intervals.ForEach((interval, index) => Assert.IsTrue(interval >= RetryStrategy.DefaultMinBackoff && interval <= maxBackoff + TimeSpan.FromTicks(TimeSpanHelper.Delta)));
    }

    [TestMethod]
    public async Task ExponentialBackoffAsyncWithResultTest()
    {
        const int RetryCount = 5;
        const int Result = 1;
        TimeSpan maxBackoff = TimeSpan.FromSeconds(5);
        Counter<InvalidOperationException> counter = new(RetryCount);
        int retryFuncCount = 0;
        int retryHandlerCount = 0;
        Assert.AreEqual(
            Result,
            await Retry.ExponentialBackoffAsync(
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
                    Assert.IsTrue(e.Delay >= RetryStrategy.DefaultMinBackoff && e.Delay <= RetryStrategy.DefaultMaxBackoff);
                    Assert.AreEqual(counter.Time.Count, e.CurrentRetryCount);
                    retryHandlerCount++;
                },
                maxBackoff: maxBackoff,
                firstFastRetry: false));
        Assert.AreEqual(RetryCount, retryFuncCount);
        Assert.AreEqual(RetryCount - 1, retryHandlerCount);
        Assert.AreEqual(RetryCount, counter.Time.Count);
        TimeSpan[] intervals = counter.Time.Take(counter.Time.Count - 1).Zip(counter.Time.Skip(1), (a, b) => b - a).ToArray();
        Assert.AreEqual(RetryCount - 1, intervals.Length);
        intervals.ForEach((interval, index) => Assert.IsTrue(interval >= RetryStrategy.DefaultMinBackoff && interval <= maxBackoff + TimeSpan.FromTicks(TimeSpanHelper.Delta)));
    }

    [TestMethod]
    public void FluentExponentialBackoffWithoutResultTest()
    {
        const int RetryCount = 5;
        TimeSpan maxBackoff = TimeSpan.FromSeconds(5);
        Counter<InvalidOperationException> counter = new(RetryCount);
        int retryFuncCount = 0;
        int retryHandlerCount = 0;
        Retry
            .WithExponentialBackoff(RetryCount, maxBackoff: maxBackoff, firstFastRetry: false)
            .Catch(exception => exception is InvalidOperationException)
            .HandleWith((sender, e) =>
            {
                Assert.IsInstanceOfType(e.LastException, typeof(InvalidOperationException));
                Assert.IsTrue(e.Delay >= RetryStrategy.DefaultMinBackoff && e.Delay <= RetryStrategy.DefaultMaxBackoff);
                Assert.AreEqual(counter.Time.Count, e.CurrentRetryCount);
                retryHandlerCount++;
            })
            .ExecuteAction(() =>
            {
                retryFuncCount++;
                counter.Increase();
            });
        Assert.AreEqual(RetryCount, retryFuncCount);
        Assert.AreEqual(RetryCount - 1, retryHandlerCount);
        Assert.AreEqual(RetryCount, counter.Time.Count);
        TimeSpan[] intervals = counter.Time.Take(counter.Time.Count - 1).Zip(counter.Time.Skip(1), (a, b) => b - a).ToArray();
        Assert.AreEqual(RetryCount - 1, intervals.Length);
        intervals.ForEach((interval, index) => Assert.IsTrue(interval >= RetryStrategy.DefaultMinBackoff && interval <= maxBackoff + TimeSpan.FromTicks(TimeSpanHelper.Delta)));
    }

    [TestMethod]
    public void FluentExponentialBackoffWithResultTest()
    {
        const int RetryCount = 5;
        const int Result = 1;
        TimeSpan maxBackoff = TimeSpan.FromSeconds(5);
        Counter<InvalidOperationException> counter = new(RetryCount);
        int retryFuncCount = 0;
        int retryHandlerCount = 0;
        Assert.AreEqual(
            Result,
            Retry
                .WithExponentialBackoff(RetryCount, maxBackoff: maxBackoff, firstFastRetry: false)
                .Catch<InvalidOperationException>()
                .HandleWith((sender, e) =>
                {
                    Assert.IsInstanceOfType(e.LastException, typeof(InvalidOperationException));
                    Assert.IsTrue(e.Delay >= RetryStrategy.DefaultMinBackoff && e.Delay <= RetryStrategy.DefaultMaxBackoff);
                    Assert.AreEqual(counter.Time.Count, e.CurrentRetryCount);
                    retryHandlerCount++;
                })
                .ExecuteAction(() =>
                {
                    retryFuncCount++;
                    counter.Increase();
                    return Result;
                }));
        Assert.AreEqual(RetryCount, retryFuncCount);
        Assert.AreEqual(RetryCount - 1, retryHandlerCount);
        Assert.AreEqual(RetryCount, counter.Time.Count);
        TimeSpan[] intervals = counter.Time.Take(counter.Time.Count - 1).Zip(counter.Time.Skip(1), (a, b) => b - a).ToArray();
        Assert.AreEqual(RetryCount - 1, intervals.Length);
        intervals.ForEach((interval, index) => Assert.IsTrue(interval >= RetryStrategy.DefaultMinBackoff && interval <= maxBackoff + TimeSpan.FromTicks(TimeSpanHelper.Delta)));
    }

    [TestMethod]
    public async Task FluentExponentialBackoffAsyncWithoutResultTest()
    {
        const int RetryCount = 5;
        TimeSpan maxBackoff = TimeSpan.FromSeconds(5);
        Counter<InvalidOperationException> counter = new(RetryCount);
        int retryFuncCount = 0;
        int retryHandlerCount = 0;
        await Retry
            .WithExponentialBackoff(RetryCount, maxBackoff: maxBackoff, firstFastRetry: false)
            .HandleWith((sender, e) =>
            {
                Assert.IsInstanceOfType(e.LastException, typeof(InvalidOperationException));
                Assert.IsTrue(e.Delay >= RetryStrategy.DefaultMinBackoff && e.Delay <= RetryStrategy.DefaultMaxBackoff);
                Assert.AreEqual(counter.Time.Count, e.CurrentRetryCount);
                retryHandlerCount++;
            })
            .Catch(exception => exception is InvalidOperationException)
            .ExecuteAsync(async () =>
            {
                retryFuncCount++;
                counter.Increase();
                await Task.Yield();
            });
        Assert.AreEqual(RetryCount, retryFuncCount);
        Assert.AreEqual(RetryCount - 1, retryHandlerCount);
        Assert.AreEqual(RetryCount, counter.Time.Count);
        TimeSpan[] intervals = counter.Time.Take(counter.Time.Count - 1).Zip(counter.Time.Skip(1), (a, b) => b - a).ToArray();
        Assert.AreEqual(RetryCount - 1, intervals.Length);
        intervals.ForEach((interval, index) => Assert.IsTrue(interval >= RetryStrategy.DefaultMinBackoff && interval <= maxBackoff + TimeSpan.FromTicks(TimeSpanHelper.Delta)));
    }

    [TestMethod]
    public async Task FluentExponentialBackoffAsyncWithResultTest()
    {
        const int RetryCount = 5;
        const int Result = 1;
        TimeSpan maxBackoff = TimeSpan.FromSeconds(5);
        Counter<InvalidOperationException> counter = new(RetryCount);
        int retryFuncCount = 0;
        int retryHandlerCount = 0;
        Assert.AreEqual(
            Result,
            await Retry
                .WithExponentialBackoff(RetryCount, maxBackoff: maxBackoff, firstFastRetry: false)
                .Catch(exception => exception is InvalidOperationException)
                .HandleWith((sender, e) =>
                {
                    Assert.IsInstanceOfType(e.LastException, typeof(InvalidOperationException));
                    Assert.IsTrue(e.Delay >= RetryStrategy.DefaultMinBackoff && e.Delay <= RetryStrategy.DefaultMaxBackoff);
                    Assert.AreEqual(counter.Time.Count, e.CurrentRetryCount);
                    retryHandlerCount++;
                })
                .ExecuteAsync(async () =>
                {
                    retryFuncCount++;
                    counter.Increase();
                    await Task.Yield();
                    return Result;
                }));
        Assert.AreEqual(RetryCount, retryFuncCount);
        Assert.AreEqual(RetryCount - 1, retryHandlerCount);
        Assert.AreEqual(RetryCount, counter.Time.Count);
        TimeSpan[] intervals = counter.Time.Take(counter.Time.Count - 1).Zip(counter.Time.Skip(1), (a, b) => b - a).ToArray();
        Assert.AreEqual(RetryCount - 1, intervals.Length);
        intervals.ForEach((interval, index) => Assert.IsTrue(interval >= RetryStrategy.DefaultMinBackoff && interval <= maxBackoff + TimeSpan.FromTicks(TimeSpanHelper.Delta)));
    }
}