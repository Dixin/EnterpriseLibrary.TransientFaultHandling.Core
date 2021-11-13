#pragma warning disable 618

namespace Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling.Tests.RetryPolicyScenarios.given_retry_limit_exceeded_exception_with_inner_exception;

public abstract class Context : ArrangeActAssert
{
    protected RetryPolicy retryPolicy;
    protected Mock<RetryStrategy> retryStrategyMock;

    protected override void Arrange()
    {
        this.retryStrategyMock = new Mock<RetryStrategy>("name", false);
        this.retryPolicy = new RetryPolicy(ErrorDetectionStrategy.AlwaysTransient, this.retryStrategyMock.Object);
    }
}

[TestClass]
public class when_executing_action : Context
{
    private int execCount;

    protected override void Act()
    {
        this.retryPolicy.ExecuteAction(() =>
        {
            this.execCount++;
            throw new RetryLimitExceededException();
        });
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
        this.retryPolicy.ExecuteAction<int>(() =>
        {
            this.execCount++;
            throw new RetryLimitExceededException();
        });
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
    private AggregateException exception;

    protected override void Act()
    {
        this.task = this.retryPolicy.ExecuteAsync(() =>
        {
            int result = ++this.timesStarted;
            return Task.Run((Func<int>)(() => throw new RetryLimitExceededException()));
        });

        try
        {
            this.task.Wait(TimeSpan.FromSeconds(2));
        }
        catch (AggregateException e)
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
    public void then_is_canceled()
    {
        Assert.IsTrue(this.task.IsCanceled);
    }

    [TestMethod]
    public void then_exception_is_task_canceled_exception()
    {
        Assert.IsInstanceOfType(this.exception.InnerException, typeof(TaskCanceledException));
    }
}