#pragma warning disable 618

namespace Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling.Tests.RetryPolicyScenarios.given_retry_limit_exceeded_exception_without_inner_exception;

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
    private Exception exception;

    protected override void Act()
    {
        try
        {
            this.retryPolicy.ExecuteAction(() =>
            {
                this.execCount++;
                throw new RetryLimitExceededException(new Exception("my exception"));
            });
        }
        catch (Exception e)
        {
            this.exception = e;
        }
    }

    [TestMethod]
    public void then_does_not_retry()
    {
        Assert.AreEqual(1, this.execCount);
    }

    [TestMethod]
    public void then_exception_is_not_retry_limit_exceeded()
    {
        Assert.IsNotInstanceOfType(this.exception, typeof(RetryLimitExceededException));
        Assert.AreEqual("my exception", this.exception.Message);
    }
}

[TestClass]
public class when_executing_func : Context
{
    private int execCount;
    private Exception exception;

    protected override void Act()
    {
        try
        {
            this.retryPolicy.ExecuteAction<int>(() =>
            {
                this.execCount++;
                throw new RetryLimitExceededException(new Exception("my exception"));
            });
        }
        catch (Exception e)
        {
            this.exception = e;
        }
    }

    [TestMethod]
    public void then_does_not_retry()
    {
        Assert.AreEqual(1, this.execCount);
    }

    [TestMethod]
    public void then_exception_is_not_retry_limit_exceeded()
    {
        Assert.IsNotInstanceOfType(this.exception, typeof(RetryLimitExceededException));
        Assert.AreEqual("my exception", this.exception.Message);
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
            return Task.Run((Func<int>)(() => throw new RetryLimitExceededException(new Exception("my exception"))));
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
    public void then_is_faulted()
    {
        Assert.IsTrue(this.task.IsFaulted);
    }

    [TestMethod]
    public void then_exception_is_not_retry_limit_exceeded()
    {
        Assert.IsNotInstanceOfType(this.exception.InnerException, typeof(RetryLimitExceededException));
        Assert.AreEqual("my exception", this.exception.InnerException?.Message);
    }
}