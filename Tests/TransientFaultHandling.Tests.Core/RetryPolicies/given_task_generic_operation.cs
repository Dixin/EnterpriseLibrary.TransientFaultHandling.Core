﻿namespace Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling.Tests.RetryPolicies.given_task_generic_operation;

public class Context : ArrangeActAssert
{
    protected Mock<ITransientErrorDetectionStrategy> detectionStrategyMock;
    protected RetryPolicy retryPolicy;

    protected override void Arrange()
    {
        base.Arrange();

        this.detectionStrategyMock = new Mock<ITransientErrorDetectionStrategy>();
        this.detectionStrategyMock.Setup(ds => ds.IsTransient(It.Is<Exception>(e => e is InvalidOperationException))).Returns(true);

        this.retryPolicy = new RetryPolicy(this.detectionStrategyMock.Object, 2);
    }
}

[TestClass]
public class when_invoking_successful_operation : Context
{
    private Task<int> task;
    private int timesStarted;

    protected override void Act()
    {
        this.task = this.retryPolicy
            .ExecuteAsync(() =>
            {
                int result = ++this.timesStarted;
                return Task.Factory.StartNew(() => result, TaskCreationOptions.LongRunning);
            });

        Assert.IsTrue(this.task.Wait(TimeSpan.FromSeconds(20)));
    }

    [TestMethod]
    public void then_ran_to_completion()
    {
        Assert.AreEqual(TaskStatus.RanToCompletion, this.task.Status);
    }

    [TestMethod]
    public void then_gets_value()
    {
        Assert.AreEqual(this.timesStarted, this.task.Result);
    }

    [TestMethod]
    public void then_starts_task_once()
    {
        Assert.AreEqual(1, this.timesStarted);
    }

    [TestMethod]
    public void then_no_exceptions_are_tested_for_transient()
    {
        this.detectionStrategyMock.Verify(ds => ds.IsTransient(It.IsAny<Exception>()), Times.Never());
    }
}


[TestClass]
public class when_invoking_successful_operation_that_executes_fast : Context
{
    private Task<int> task;
    private int timesStarted;

    protected override void Act()
    {
        this.task = this.retryPolicy
            .ExecuteAsync(() =>
            {
                int result = ++this.timesStarted;
                Task<int> t = Task.Factory.StartNew(() => result, TaskCreationOptions.LongRunning);
                t.Wait();
                return t;
            });

        Assert.IsTrue(this.task.Wait(TimeSpan.FromSeconds(20)));
    }

    [TestMethod]
    public void then_ran_to_completion()
    {
        Assert.AreEqual(TaskStatus.RanToCompletion, this.task.Status);
    }

    [TestMethod]
    public void then_starts_task_once()
    {
        Assert.AreEqual(1, this.timesStarted);
    }

    [TestMethod]
    public void then_no_exceptions_are_tested_for_transient()
    {
        this.detectionStrategyMock.Verify(ds => ds.IsTransient(It.IsAny<Exception>()), Times.Never());
    }
}

[TestClass]
public class when_task_invocation_throws_synchronously_non_transient_exception : Context
{
    private Exception exception;
    private int timesStarted;

    protected override void Act()
    {
        try
        {
            this.retryPolicy
                .ExecuteAsync<int>(() =>
                {
                    ++this.timesStarted;
                    throw new Exception("non transient");
                });
        }
        catch (Exception ex)
        {
            this.exception = ex;
        }
    }

    [TestMethod]
    public void then_gets_synchronous_exception()
    {
        Assert.AreEqual("non transient", this.exception.Message);
    }

    [TestMethod]
    public void then_starts_task_once()
    {
        Assert.AreEqual(1, this.timesStarted);
    }
}
    
[TestClass]
public class when_task_fails_with_non_transient_exception : Context
{
    private Task<int> task;
    private int timesStarted;

    protected override void Act()
    {
        this.task = this.retryPolicy
            .ExecuteAsync(() =>
            {
                int result = ++this.timesStarted;
                return Task<int>.Factory.StartNew(() => throw new Exception("non transient"), TaskCreationOptions.LongRunning);
            });
            
        Assert.IsTrue(((IAsyncResult)this.task).AsyncWaitHandle.WaitOne(TimeSpan.FromSeconds(20)));
    }

    [TestMethod]
    public void then_gets_exception()
    {
        Assert.IsTrue(this.task.IsFaulted);
        Assert.AreEqual("non transient", this.task.Exception?.InnerException?.Message);
    }

    [TestMethod]
    public void then_starts_task_once()
    {
        Assert.AreEqual(1, this.timesStarted);
    }

    [TestMethod]
    public void then_one_exception_is_tested_for_transient()
    {
        this.detectionStrategyMock.Verify(ds => ds.IsTransient(It.IsAny<Exception>()), Times.Exactly(1));
    }
}

[TestClass]
public class when_task_invocation_throws_synchronously_with_transient_exception_in_first_attempt : Context
{
    private Task<int> task;
    private int timesStarted;

    protected override void Act()
    {
        this.task = this.retryPolicy
            .ExecuteAsync(() =>
            {
                int result = ++this.timesStarted;
                if (result == 1)
                {
                    throw new InvalidOperationException("transient");
                }

                return Task.Factory.StartNew(() => result, TaskCreationOptions.LongRunning);
            });

        Assert.IsTrue(((IAsyncResult)this.task).AsyncWaitHandle.WaitOne(TimeSpan.FromSeconds(20)));
    }

    [TestMethod]
    public void then_ran_to_completion()
    {
        Assert.AreEqual(TaskStatus.RanToCompletion, this.task.Status);
    }

    [TestMethod]
    public void then_gets_value()
    {
        Assert.AreEqual(this.timesStarted, this.task.Result);
    }

    [TestMethod]
    public void then_starts_task_twice()
    {
        Assert.AreEqual(2, this.timesStarted);
    }

    [TestMethod]
    public void then_one_exception_is_tested_for_transient()
    {
        this.detectionStrategyMock.Verify(ds => ds.IsTransient(It.IsAny<Exception>()), Times.AtLeast(1));
    }
}

[TestClass]
public class when_task_invocation_throws_synchronously_with_transient_exception_in_first_and_second_attempts : Context
{
    private Task<int> task;
    private int timesStarted;

    protected override void Act()
    {
        this.task = this.retryPolicy
            .ExecuteAsync(() =>
            {
                int result = ++this.timesStarted;
                if (result <= 2)
                {
                    throw new InvalidOperationException("transient");
                }

                return Task.Factory.StartNew(() => result, TaskCreationOptions.LongRunning);
            });

        Assert.IsTrue(((IAsyncResult)this.task).AsyncWaitHandle.WaitOne(TimeSpan.FromSeconds(20)));
    }

    [TestMethod]
    public void then_ran_to_completion()
    {
        Assert.AreEqual(TaskStatus.RanToCompletion, this.task.Status);
    }

    [TestMethod]
    public void then_gets_value()
    {
        Assert.AreEqual(this.timesStarted, this.task.Result);
    }

    [TestMethod]
    public void then_starts_task_three_times()
    {
        Assert.AreEqual(3, this.timesStarted);
    }

    [TestMethod]
    public void then_two_exceptions_are_tested_for_transient()
    {
        this.detectionStrategyMock.Verify(ds => ds.IsTransient(It.IsAny<Exception>()), Times.AtLeast(2));
    }
}

[TestClass]
public class when_task_invocation_throws_synchronously_with_transient_exception_in_all_attempts : Context
{
    private Task<int> task;
    private int timesStarted;

    protected override void Act()
    {
        this.task = this.retryPolicy
            .ExecuteAsync(() =>
            {
                int result = ++this.timesStarted;
                if (result <= 3)
                {
                    throw new InvalidOperationException("transient");
                }

                return Task.Factory.StartNew(() => result, TaskCreationOptions.LongRunning);
            });

        Assert.IsTrue(((IAsyncResult)this.task).AsyncWaitHandle.WaitOne(TimeSpan.FromSeconds(20)));
    }

    [TestMethod]
    public void then_gets_exception()
    {
        Assert.IsTrue(this.task.IsFaulted);
        Assert.AreEqual("transient", this.task.Exception?.InnerException?.Message);
    }

    [TestMethod]
    public void then_starts_task_three_times()
    {
        Assert.AreEqual(3, this.timesStarted);
    }

    [TestMethod]
    public void then_three_exceptions_are_tested_for_transient()
    {
        this.detectionStrategyMock.Verify(ds => ds.IsTransient(It.IsAny<Exception>()), Times.AtLeast(3));
    }
}

[TestClass]
public class when_operation_fails_with_transient_exception_in_first_attempt : Context
{
    private Task<int> task;
    private int timesStarted;

    protected override void Act()
    {
        this.task = this.retryPolicy
            .ExecuteAsync(() =>
            {
                int result = ++this.timesStarted;
                return Task<int>.Factory.StartNew(() =>
                    {
                        if (result == 1)
                        {
                            throw new InvalidOperationException("transient");
                        }

                        return result; 
                    },
                    TaskCreationOptions.LongRunning);
            });

        Assert.IsTrue(((IAsyncResult)this.task).AsyncWaitHandle.WaitOne(TimeSpan.FromSeconds(20)));
    }

    [TestMethod]
    public void then_ran_to_completion()
    {
        Assert.AreEqual(TaskStatus.RanToCompletion, this.task.Status);
    }

    [TestMethod]
    public void then_gets_value()
    {
        Assert.AreEqual(this.timesStarted, this.task.Result);
    }

    [TestMethod]
    public void then_starts_task_twice()
    {
        Assert.AreEqual(2, this.timesStarted);
    }

    [TestMethod]
    public void then_one_exception_is_tested_for_transient()
    {
        this.detectionStrategyMock.Verify(ds => ds.IsTransient(It.IsAny<Exception>()), Times.Exactly(1));
    }
}

[TestClass]
public class when_operation_fails_with_transient_exception_in_first_and_second_attempts : Context
{
    private Task<int> task;
    private int timesStarted;

    protected override void Act()
    {
        this.task = this.retryPolicy
            .ExecuteAsync(() =>
            {
                int result = ++this.timesStarted;
                return Task<int>.Factory.StartNew(() =>
                    {
                        if (result <= 2)
                        {
                            throw new InvalidOperationException("transient");
                        }

                        return result;
                    },
                    TaskCreationOptions.LongRunning);
            });

        Assert.IsTrue(((IAsyncResult)this.task).AsyncWaitHandle.WaitOne(TimeSpan.FromSeconds(20)));
    }

    [TestMethod]
    public void then_ran_to_completion()
    {
        Assert.AreEqual(TaskStatus.RanToCompletion, this.task.Status);
    }

    [TestMethod]
    public void then_gets_value()
    {
        Assert.AreEqual(this.timesStarted, this.task.Result);
    }

    [TestMethod]
    public void then_starts_task_three_times()
    {
        Assert.AreEqual(3, this.timesStarted);
    }

    [TestMethod]
    public void then_two_exceptions_are_tested_for_transient()
    {
        this.detectionStrategyMock.Verify(ds => ds.IsTransient(It.IsAny<Exception>()), Times.Exactly(2));
    }
}

[TestClass]
public class when_operation_fails_with_transient_exception_in_all_attempts : Context
{
    private Task<int> task;
    private int timesStarted;

    protected override void Act()
    {
        this.task = this.retryPolicy
            .ExecuteAsync(() =>
            {
                int result = ++this.timesStarted;
                return Task<int>.Factory.StartNew(
                    () => throw new InvalidOperationException("transient"),
                    TaskCreationOptions.LongRunning);
            });

        Assert.IsTrue(((IAsyncResult)this.task).AsyncWaitHandle.WaitOne(TimeSpan.FromSeconds(20)));
    }

    [TestMethod]
    public void then_gets_exception()
    {
        Assert.IsTrue(this.task.IsFaulted);
        Assert.AreEqual("transient", this.task.Exception?.InnerException?.Message);
    }

    [TestMethod]
    public void then_starts_task_three_times()
    {
        Assert.AreEqual(3, this.timesStarted);
    }

    [TestMethod]
    public void then_three_exceptions_are_tested_for_transient()
    {
        this.detectionStrategyMock.Verify(ds => ds.IsTransient(It.IsAny<Exception>()), Times.Exactly(3));
    }
}

[TestClass]
public class when_task_invocation_throws_synchronously_on_second_attempt_with_non_transient_exception : Context
{
    private Task<int> task;
    private int timesStarted;

    protected override void Act()
    {
        this.task = this.retryPolicy
            .ExecuteAsync(() =>
            {
                int result = ++this.timesStarted;
                if (result == 2)
                {
                    throw new Exception("non transient");
                }

                return Task<int>.Factory.StartNew(() =>
                    {
                        if (result == 1)
                        {
                            throw new InvalidOperationException("transient");
                        }

                        return result;
                    },
                    TaskCreationOptions.LongRunning);
            });

        Assert.IsTrue(((IAsyncResult)this.task).AsyncWaitHandle.WaitOne(TimeSpan.FromSeconds(20)));
    }

    [TestMethod]
    public void then_gets_exception()
    {
        Assert.IsTrue(this.task.IsFaulted);
        Assert.AreEqual("non transient", this.task.Exception?.InnerException?.Message);
    }

    [TestMethod]
    public void then_starts_task_twice()
    {
        Assert.AreEqual(2, this.timesStarted);
    }

    [TestMethod]
    public void then_two_exceptions_are_tested_for_transient()
    {
        this.detectionStrategyMock.Verify(ds => ds.IsTransient(It.IsAny<Exception>()), Times.Exactly(2));
    }
}

[TestClass]
public class when_operation_fails_on_second_attempt_with_non_transient_exception : Context
{
    private Task<int> task;
    private int timesStarted;

    protected override void Act()
    {
        this.task = this.retryPolicy
            .ExecuteAsync(() =>
            {
                int result = ++this.timesStarted;
                return Task<int>.Factory.StartNew(() =>
                    {
                        if (result == 1)
                        {
                            throw new InvalidOperationException("transient");
                        }

                        if (result == 2)
                        {
                            throw new Exception("non transient");
                        }

                        return result;
                    },
                    TaskCreationOptions.LongRunning);
            });

        Assert.IsTrue(((IAsyncResult)this.task).AsyncWaitHandle.WaitOne(TimeSpan.FromSeconds(20)));
    }

    [TestMethod]
    public void then_gets_exception()
    {
        Assert.IsTrue(this.task.IsFaulted);
        Assert.AreEqual("non transient", this.task.Exception?.InnerException?.Message);
    }

    [TestMethod]
    public void then_starts_task_twice()
    {
        Assert.AreEqual(2, this.timesStarted);
    }

    [TestMethod]
    public void then_two_exceptions_are_tested_for_transient()
    {
        this.detectionStrategyMock.Verify(ds => ds.IsTransient(It.IsAny<Exception>()), Times.Exactly(2));
    }
}

[TestClass]
public class when_invoking_operation_without_task : Context
{
    private ArgumentNullException exception;

    protected override void Act()
    {
        this.exception = null;

        try
        {
            this.retryPolicy.ExecuteAsync((Func<Task<object>>?)null);

            Assert.Fail("Should have thrown");
        }
        catch (ArgumentNullException e)
        {
            this.exception = e;
        }
    }

    [TestMethod]
    public void then_throws_exception()
    {
        Assert.IsNotNull(this.exception);
        Assert.AreEqual("taskFunc", this.exception.ParamName);
    }
}

[TestClass]
public class when_invoking_operation_with_function_that_returns_null : Context
{
    private ArgumentException exception;

    protected override void Act()
    {
        this.exception = null;

        try
        {
            this.retryPolicy.ExecuteAsync(() => (Task<object>)null);

            Assert.Fail("Should have thrown");
        }
        catch (ArgumentException e)
        {
            this.exception = e;
        }
    }

    [TestMethod]
    public void then_throws_exception()
    {
        Assert.IsNotNull(this.exception);
        Assert.AreEqual("taskFunc", this.exception.ParamName);
    }
}

[TestClass]
public class when_invoking_operation_with_function_that_returns_a_cold_task : Context
{
    private ArgumentException exception;

    protected override void Act()
    {
        try
        {
            this.retryPolicy.ExecuteAsync(() => new Task<int>(() => 5));
        }
        catch (ArgumentException ex)
        {
            this.exception = ex;
        }
    }

    [TestMethod]
    public void then_gets_synchronous_exception()
    {
        Assert.IsNotNull(this.exception);
        Assert.AreEqual("taskFunc", this.exception.ParamName);
    }
}

[TestClass]
public class when_task_fails_synchronously_on_second_attempt_then_asynchronous_task_fails : Context
{
    private Task<int> task;
    private int timesStarted;

    protected override void Act()
    {
        this.task = this.retryPolicy
            .ExecuteAsync(() =>
            {
                int result = ++this.timesStarted;
                if (result == 2)
                {
                    throw new Exception("not checked");
                }

                return Task<int>.Factory.StartNew(
                    () => throw new InvalidOperationException("transient"),
                    TaskCreationOptions.LongRunning);
            });

        Assert.IsTrue(((IAsyncResult)this.task).AsyncWaitHandle.WaitOne(TimeSpan.FromSeconds(20)));
    }

    [TestMethod]
    public void then_gets_exception()
    {
        Assert.IsTrue(this.task.IsFaulted);
        Assert.AreEqual("not checked", this.task.Exception?.InnerException?.Message);
    }

    [TestMethod]
    public void then_starts_task_twice()
    {
        Assert.AreEqual(2, this.timesStarted);
    }

    [TestMethod]
    public void then_one_exception_is_tested_for_transient()
    {
        this.detectionStrategyMock.Verify(ds => ds.IsTransient(It.IsAny<Exception>()), Times.AtLeast(1));
    }
}

[TestClass]
public class when_task_is_canceled : Context
{
    private Task<int> task;
    private int timesStarted;

    protected override void Act()
    {
        CancellationTokenSource cts = new();
        cts.Cancel();
        this.task = this.retryPolicy
            .ExecuteAsync(() =>
            {
                int result = ++this.timesStarted;
                return Task<int>.Factory.StartNew(
                    () => throw new OperationCanceledException(cts.Token),
                    cts.Token);
            });

        Assert.IsTrue(((IAsyncResult)this.task).AsyncWaitHandle.WaitOne(TimeSpan.FromSeconds(20)));
    }

    [TestMethod]
    public void then_task_is_canceled()
    {
        Assert.AreEqual(TaskStatus.Canceled, this.task.Status);
    }

    [TestMethod]
    public void then_starts_task_once()
    {
        Assert.AreEqual(1, this.timesStarted);
    }

    [TestMethod]
    public void then_no_exceptions_are_tested_for_transient()
    {
        this.detectionStrategyMock.Verify(ds => ds.IsTransient(It.IsAny<Exception>()), Times.Never());
    }
}

[TestClass]
public class when_using_cancellation_token_and_task_is_canceled_in_flight_and_succeeds : Context
{
    private Task<int> task;
    private int timesStarted;

    protected override void Act()
    {
        CancellationTokenSource cts = new();
        this.task = this.retryPolicy
            .ExecuteAsync(() =>
                {
                    int result = ++this.timesStarted;
                    return Task<int>.Factory.StartNew(() =>
                    {
                        if (!cts.IsCancellationRequested)
                        {
                            cts.Cancel();
                        }

                        return result;
                    });
                },
                cts.Token);

        Assert.IsTrue(((IAsyncResult)this.task).AsyncWaitHandle.WaitOne(TimeSpan.FromSeconds(20)));
    }

    [TestMethod]
    public void then_ran_to_completion()
    {
        Assert.AreEqual(TaskStatus.RanToCompletion, this.task.Status);
    }

    [TestMethod]
    public void then_gets_value()
    {
        Assert.AreEqual(this.timesStarted, this.task.Result);
    }

    [TestMethod]
    public void then_starts_task_once()
    {
        Assert.AreEqual(1, this.timesStarted);
    }

    [TestMethod]
    public void then_no_exceptions_are_tested_for_transient()
    {
        this.detectionStrategyMock.Verify(ds => ds.IsTransient(It.IsAny<Exception>()), Times.Never());
    }
}

[TestClass]
public class when_using_cancellation_token_and_is_canceled_even_before_starting : Context
{
    private Task<int> task;
    private int timesStarted;

    protected override void Act()
    {
        CancellationTokenSource cts = new();
        cts.Cancel();
        this.task = this.retryPolicy
            .ExecuteAsync(() =>
            {
                int result = ++this.timesStarted;
                return Task<int>.Factory.StartNew(() => result);
            }, cts.Token);

        Assert.IsTrue(((IAsyncResult)this.task).AsyncWaitHandle.WaitOne(TimeSpan.FromSeconds(20)));
    }

    [TestMethod]
    public void then_task_is_canceled()
    {
        Assert.AreEqual(TaskStatus.Canceled, this.task.Status);
    }

    [TestMethod]
    public void then_does_not_start_task()
    {
        Assert.AreEqual(0, this.timesStarted);
    }

    [TestMethod]
    public void then_no_exceptions_are_tested_for_transient()
    {
        this.detectionStrategyMock.Verify(ds => ds.IsTransient(It.IsAny<Exception>()), Times.Never());
    }
}

[TestClass]
public class when_using_cancellation_token_and_task_is_canceled_in_flight_and_fails_with_transient : Context
{
    private Task<int> task;
    private int timesStarted;

    protected override void Act()
    {
        CancellationTokenSource cts = new();
        this.task = this.retryPolicy
            .ExecuteAsync(() =>
                {
                    int result = ++this.timesStarted;
                    return Task<int>.Factory.StartNew(() =>
                    {
                        if (!cts.IsCancellationRequested)
                        {
                            cts.Cancel();
                        }

                        if (result == 1)
                        {
                            throw new InvalidOperationException("transient");
                        }

                        return result;
                    });
                },
                cts.Token);

        Assert.IsTrue(((IAsyncResult)this.task).AsyncWaitHandle.WaitOne(TimeSpan.FromSeconds(20)));
    }

    [TestMethod]
    public void then_gets_exception()
    {
        // TODO: should this be Canceled or Faulted?
        Assert.IsTrue(this.task.IsFaulted);
        Assert.AreEqual("transient", this.task.Exception?.InnerException?.Message);
    }

    [TestMethod]
    public void then_starts_task_once()
    {
        Assert.AreEqual(1, this.timesStarted);
    }

    [TestMethod]
    public void then_no_exceptions_are_tested_for_transient()
    {
        this.detectionStrategyMock.Verify(ds => ds.IsTransient(It.IsAny<Exception>()), Times.Never());
    }
}

[TestClass]
public class when_using_cancellation_token_and_task_is_canceled_in_flight_after_one_attempt_and_succeeds : Context
{
    private Task<int> task;
    private int timesStarted;

    protected override void Act()
    {
        CancellationTokenSource cts = new();
        this.task = this.retryPolicy
            .ExecuteAsync(() =>
                {
                    int result = ++this.timesStarted;
                    return Task<int>.Factory.StartNew(() =>
                    {
                        if (result == 1)
                        {
                            throw new InvalidOperationException("transient");
                        }

                        if (!cts.IsCancellationRequested)
                        {
                            cts.Cancel();
                        }

                        return result;
                    });
                },
                cts.Token);

        Assert.IsTrue(((IAsyncResult)this.task).AsyncWaitHandle.WaitOne(TimeSpan.FromSeconds(20)));
    }

    [TestMethod]
    public void then_ran_to_completion()
    {
        Assert.AreEqual(TaskStatus.RanToCompletion, this.task.Status);
    }

    [TestMethod]
    public void then_gets_value()
    {
        Assert.AreEqual(this.timesStarted, this.task.Result);
    }

    [TestMethod]
    public void then_starts_task_twice()
    {
        Assert.AreEqual(2, this.timesStarted);
    }

    [TestMethod]
    public void then_no_exceptions_are_tested_for_transient()
    {
        this.detectionStrategyMock.Verify(ds => ds.IsTransient(It.IsAny<Exception>()), Times.Exactly(1));
    }
}

[TestClass]
public class when_using_cancellation_token_and_task_is_canceled_while_waiting_to_retry : Context
{
    private Task<int> task;
    private int timesStarted;

    protected override void Arrange()
    {
        base.Arrange();
        this.retryPolicy.RetryStrategy.FastFirstRetry = false;
    }

    protected override void Act()
    {
        CancellationTokenSource cts = new();
        this.task = this.retryPolicy
            .ExecuteAsync(() =>
                {
                    int result = ++this.timesStarted;
                    return Task<int>.Factory.StartNew(() =>
                    {
                        if (result == 1)
                        {
                            Task.Factory.StartNew(
                                () => { Thread.Sleep(400); cts.Cancel(); },
                                TaskCreationOptions.LongRunning);

                            throw new InvalidOperationException("transient");
                        }

                        return result;
                    });
                },
                cts.Token);

        Assert.IsTrue(((IAsyncResult)this.task).AsyncWaitHandle.WaitOne(TimeSpan.FromSeconds(20)));
    }

    [TestMethod]
    public void then_gets_past_exception()
    {
        // TODO: should this be Canceled or Faulted?
        Assert.IsTrue(this.task.IsFaulted);
        Assert.AreEqual("transient", this.task.Exception?.InnerException?.Message);
    }

    [TestMethod]
    public void then_starts_task_once()
    {
        Assert.AreEqual(1, this.timesStarted);
    }

    [TestMethod]
    public void then_no_exceptions_are_tested_for_transient()
    {
        this.detectionStrategyMock.Verify(ds => ds.IsTransient(It.IsAny<Exception>()), Times.Exactly(1));
    }
}