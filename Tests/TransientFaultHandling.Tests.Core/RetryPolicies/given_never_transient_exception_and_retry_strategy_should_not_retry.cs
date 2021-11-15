#pragma warning disable 618

namespace Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling.Tests.RetryPolicies.given_never_transient_exception_and_retry_strategy_should_not_retry;

[TestClass]
public class when_executing_action : Context
{
    private int execCount;

    protected override void Act()
    {
        try
        {
            this.retryPolicy.ExecuteAction(() =>
            {
                this.execCount++;
                throw new Exception();
            });

            Assert.Fail();
        }
        catch
        {
        }
    }

    [TestMethod]
    public void then_does_not_retry()
    {
        Assert.AreEqual(1, this.execCount);
    }
}

[TestClass]
public class when_executing_func : Context
{
    private int execCount;

    protected override void Act()
    {
        try
        {
            this.retryPolicy.ExecuteAction<int>(() =>
            {
                this.execCount++;
                throw new Exception();
            });
        }
        catch
        {
        }
    }

    [TestMethod]
    public void then_does_not_retry()
    {
        Assert.AreEqual(1, this.execCount);
    }
}

[TestClass]
public class when_executing_async : Context
{
    private int timesStarted;
    private Task task;
    private Exception exception;

    protected override void Act()
    {
        this.task = this.retryPolicy.ExecuteAsync(() =>
        {
            int result = ++this.timesStarted;
            return Task.Run((Func<int>)(() => throw new Exception()));
        });

        try
        {
            this.task.Wait(TimeSpan.FromSeconds(2));
        }
        catch (Exception e)
        {
            this.exception = e;
        }
    }

    [TestMethod]
    public void then_does_not_retry()
    {
        Assert.AreEqual(1, this.timesStarted);
    }

    [TestMethod]
    public void then_is_faulted()
    {
        Assert.IsTrue(this.task.IsFaulted);
    }
}