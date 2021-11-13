namespace Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling.Bvt.Tests.RetryStrategies;

[TestClass]
public class IncrementalRetryStrategyFixture
{
    private readonly RetryManager RetryManager = RetryConfiguration.GetRetryManager(getCustomRetryStrategy: section =>
        section.Get<TestRetryStrategyOptions>().ToTestRetryStrategy(section.Key));

    [TestMethod]
    [ExpectedException(typeof(InvalidCastException))]
    public void ExceptionIsThrownWhenAllRetriesFail()
    {
        RetryPolicy<MockErrorDetectionStrategy> policy = this.RetryManager.GetRetryPolicy<MockErrorDetectionStrategy>("Incremental, 5 retries");

        int count = 0;
        int count1 = 0;
        policy.Retrying += (sender, args) =>
        {
            Assert.AreEqual(count, args.CurrentRetryCount);
            Assert.IsInstanceOfType(args.LastException, typeof(InvalidCastException));
            count1 = args.CurrentRetryCount;
        };

        try
        {
            policy.ExecuteAction(() =>
            {
                ++count;
                throw new InvalidCastException();
            });
        }
        catch (Exception)
        {
            Assert.AreEqual(6, count);
            Assert.AreEqual(5, count1);
            throw;
        }

        Assert.Fail("Test should throw");
    }

    [TestMethod]
    public void ActionIsExecutedWhenSomeRetriesFailAndThenSucceeds()
    {
        RetryPolicy<MockErrorDetectionStrategy> policy = this.RetryManager.GetRetryPolicy<MockErrorDetectionStrategy>("Incremental, 5 retries");

        int count = 0;
        int count1 = 0;
        policy.Retrying += (sender, args) =>
        {
            Assert.AreEqual(count, args.CurrentRetryCount);
            Assert.IsInstanceOfType(args.LastException, typeof(InvalidCastException));
            count1 = args.CurrentRetryCount;
        };

        policy.ExecuteAction(() =>
        {
            ++count;
            if (count1 < 2)
            {
                throw new InvalidCastException();
            }
        });

        Assert.AreEqual(3, count);
        Assert.AreEqual(2, count1);
    }

    [TestMethod]
    [ExpectedException(typeof(FieldAccessException))]
    public void ExceptionIsThrownWhenNonTransientExceptionOccurs()
    {
        RetryPolicy<MockErrorDetectionStrategy> policy = this.RetryManager.GetRetryPolicy<MockErrorDetectionStrategy>("Incremental, 5 retries");

        int count = 0;
        int count1 = 0;
        policy.Retrying += (sender, args) =>
        {
            Assert.AreEqual(count, args.CurrentRetryCount);
            if (args.CurrentRetryCount <= 2)
            {
                Assert.IsInstanceOfType(args.LastException, typeof(InvalidCastException));
            }

            if (args.CurrentRetryCount > 2 && args.CurrentRetryCount <= 4)
            {
                Assert.IsInstanceOfType(args.LastException, typeof(InvalidOperationException));
            }

            count1 = args.CurrentRetryCount;
        };
        try
        {
            policy.ExecuteAction(() =>
            {
                ++count;
                if (count1 < 2)
                {
                    throw new InvalidCastException();
                }

                if (count1 < 4)
                {
                    throw new InvalidOperationException();
                }

                throw new FieldAccessException();
            });
        }
        catch (Exception)
        {
            Assert.AreEqual(5, count);
            Assert.AreEqual(4, count1);
            throw;
        }

        Assert.Fail("Test should throw");
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidCastException))]
    public void IntervalsAreCorrectWhenFastFirstRetryIsEnabled()
    {
        RetryPolicy<MockErrorDetectionStrategy> policy = this.RetryManager.GetRetryPolicy<MockErrorDetectionStrategy>("Incremental, 5 retries, first retry is fast");

        int count = 0;
        DateTime firstTryTime = default;
        DateTime startTime = DateTime.Now;
        TimeSpan[] elapsedTimes = new TimeSpan[5];
        policy.Retrying += (sender, args) =>
        {
            Assert.AreEqual(count, args.CurrentRetryCount);
            Assert.IsInstanceOfType(args.LastException, typeof(InvalidCastException));
            Assert.AreEqual(TimeSpan.FromSeconds(1) + TimeSpan.FromSeconds(1 * (args.CurrentRetryCount - 1)), args.Delay);
            elapsedTimes[args.CurrentRetryCount - 1] = DateTime.Now - startTime;
        };

        try
        {
            policy.ExecuteAction(() =>
            {
                if (count == 0)
                {
                    firstTryTime = DateTime.Now;
                }

                ++count;
                throw new InvalidCastException();
            });
        }
        catch (Exception)
        {
            Assert.AreEqual(0, (firstTryTime - startTime).Seconds);
            Assert.AreEqual(0, elapsedTimes[1].Seconds); // Fast Retry
            for (int i = 2; i < 5; i++)
            {
                int expectedIntervalInSeconds = i * TimeSpan.FromSeconds(1).Seconds; // Incremental Interval
                int actualIntervalInSeconds = elapsedTimes[i].Seconds - elapsedTimes[i - 1].Seconds;
                Assert.AreEqual(expectedIntervalInSeconds, actualIntervalInSeconds, "i = " + i.ToString());
            }

            throw;
        }

        Assert.Fail("Test should throw");
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidCastException))]
    public void IntervalsAreCorrectWhenFastFirstRetryIsDisabled()
    {
        RetryPolicy<MockErrorDetectionStrategy> policy = this.RetryManager.GetRetryPolicy<MockErrorDetectionStrategy>("Incremental, 5 retries, no fast retry");

        int count = 0;
        int count1 = 0;
        DateTime firstTryTime = default;
        DateTime startTime = DateTime.Now;
        TimeSpan[] elapsedTimes = new TimeSpan[5];
        policy.Retrying += (sender, args) =>
        {
            Assert.AreEqual(count, args.CurrentRetryCount);
            Assert.IsInstanceOfType(args.LastException, typeof(InvalidCastException));
            Assert.AreEqual(TimeSpan.FromSeconds(1) + TimeSpan.FromSeconds(1 * (args.CurrentRetryCount - 1)), args.Delay);
            count1 = args.CurrentRetryCount;
            elapsedTimes[args.CurrentRetryCount - 1] = DateTime.Now - startTime;
        };

        try
        {
            policy.ExecuteAction(() =>
            {
                if (count == 0)
                {
                    firstTryTime = DateTime.Now;
                }

                ++count; throw new InvalidCastException();
            });
        }
        catch (Exception)
        {
            Assert.AreEqual(0, (firstTryTime - startTime).Seconds);
            Assert.AreEqual(1, elapsedTimes[1].Seconds); // Initial Interval
            for (int i = 2; i < 5; i++)
            {
                int expectedIntervalInSeconds = i * TimeSpan.FromSeconds(1).Seconds; // Incremental Interval
                int actualIntervalInSeconds = elapsedTimes[i].Seconds - elapsedTimes[i - 1].Seconds;
                Assert.AreEqual(expectedIntervalInSeconds, actualIntervalInSeconds, "i = " + i.ToString());
            }

            throw;
        }

        Assert.Fail("Test should throw");
    }

    [TestMethod]
    [ExpectedException(typeof(Exception))]
    public void ExceptionIsThrownWhenFastFirstRetryIsEnabledAndNonTransientExceptionOccurs()
    {
        RetryPolicy<MockErrorDetectionStrategy> policy = this.RetryManager.GetRetryPolicy<MockErrorDetectionStrategy>("Incremental, 5 retries, first retry is fast");

        int count = 0;
        int count1 = 0;
        DateTime[] elapsedTimes = new DateTime[5];
        policy.Retrying += (sender, args) =>
        {
            Assert.AreEqual(count, args.CurrentRetryCount);
            if (args.CurrentRetryCount < 4)
            {
                Assert.IsInstanceOfType(args.LastException, typeof(InvalidCastException));
            }

            count1 = args.CurrentRetryCount;
            elapsedTimes[args.CurrentRetryCount - 1] = DateTime.Now;
        };

        DateTime firstTryTime = default;
        DateTime startTime = DateTime.Now;
        try
        {
            policy.ExecuteAction(() =>
            {
                if (count == 0)
                {
                    firstTryTime = DateTime.Now;
                }

                ++count;
                if (count < 4)
                {
                    throw new InvalidCastException();
                }

                throw new Exception();
            });
        }
        catch (Exception)
        {
            Assert.AreEqual(0, (firstTryTime - startTime).Seconds);
            Assert.AreEqual(0, (elapsedTimes[1] - startTime).Seconds);
            for (int i = 2; i < 3; i++)
            {
                int expectedIntervalInSeconds = i * TimeSpan.FromSeconds(1).Seconds; // Incremental Interval
                int actualIntervalInSeconds = (elapsedTimes[i] - startTime).Seconds;
                Assert.AreEqual(expectedIntervalInSeconds, actualIntervalInSeconds, "i = " + i.ToString());
            }

            throw;
        }

        Assert.Fail("Test should throw");
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidCastException))]
    public void ExceptionIsThrownWhenAllRetriesFailUsingMillisecondsIntervals()
    {
        RetryPolicy<MockErrorDetectionStrategy> policy = this.RetryManager.GetRetryPolicy<MockErrorDetectionStrategy>("Incremental, 5 retries in Milliseconds");

        int count = 0;
        int count1 = 0;
        policy.Retrying += (sender, args) =>
        {
            Assert.AreEqual(count, args.CurrentRetryCount);
            Assert.IsInstanceOfType(args.LastException, typeof(InvalidCastException));
            count1 = args.CurrentRetryCount;
        };

        try
        {
            policy.ExecuteAction(() =>
            {
                ++count;
                throw new InvalidCastException();
            });
        }
        catch (Exception)
        {
            Assert.AreEqual(6, count);
            Assert.AreEqual(5, count1);
            throw;
        }

        Assert.Fail("Test should throw");
    }

    [TestMethod]
    public void ActionIsExecutedWhenSomeRetriesFailAndThenSucceedsUsingMillisecondsIntervals()
    {
        RetryPolicy<MockErrorDetectionStrategy> policy = this.RetryManager.GetRetryPolicy<MockErrorDetectionStrategy>("Incremental, 5 retries in Milliseconds");

        int count = 0;
        int count1 = 0;
        policy.Retrying += (sender, args) =>
        {
            Assert.AreEqual(count, args.CurrentRetryCount);
            Assert.IsInstanceOfType(args.LastException, typeof(InvalidCastException));
            count1 = args.CurrentRetryCount;
        };

        policy.ExecuteAction(() =>
        {
            ++count;
            if (count1 < 2)
            {
                throw new InvalidCastException();
            }
        });

        Assert.AreEqual(3, count);
        Assert.AreEqual(2, count1);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidCastException))]
    public void ExceptionIsThrownWhenAllRetriesFailDuringAsyncBegin()
    {
        RetryPolicy<MockErrorDetectionStrategy> policy = this.RetryManager.GetRetryPolicy<MockErrorDetectionStrategy>("Incremental, 5 retries");

        int count1 = 0;
        policy.Retrying += (sender, args) =>
        {
            Assert.IsInstanceOfType(args.LastException, typeof(InvalidCastException));
            count1 = args.CurrentRetryCount;
        };

        TestAsyncOperation asyncInstance = new();
        asyncInstance.ExceptionToThrowAtBegin = new InvalidCastException();
        asyncInstance.CountToThrowAtBegin = 15;
        try
        {
            policy
                .ExecuteAsync(() =>
                    Task.Factory.FromAsync(asyncInstance.BeginMethod, asyncInstance.EndMethod, null))
                .Wait(TimeSpan.FromSeconds(5));
        }
        catch (AggregateException ex)
        {
            Assert.AreEqual(5, count1);
            Assert.AreEqual(1, ex.InnerExceptions.Count);

            throw ex.InnerException!;
        }

        Assert.Fail("Test should throw");
    }

    [TestMethod]
    public void ActionIsExecutedWhenSomeRetriesFailDuringAsyncBeginAndThenSucceeds()
    {
        RetryPolicy<MockErrorDetectionStrategy> policy = this.RetryManager.GetRetryPolicy<MockErrorDetectionStrategy>("Incremental, 5 retries");

        int count1 = 0;
        policy.Retrying += (sender, args) =>
        {
            Assert.IsInstanceOfType(args.LastException, typeof(InvalidCastException));
            count1 = args.CurrentRetryCount;
        };

        TestAsyncOperation asyncInstance = new();
        asyncInstance.CountToThrowAtBegin = 3;
        asyncInstance.ExceptionToThrowAtBegin = new InvalidCastException();
        policy
            .ExecuteAsync(() =>
                Task.Factory.FromAsync(asyncInstance.BeginMethod, asyncInstance.EndMethod, null))
            .Wait(TimeSpan.FromSeconds(5));

        Assert.AreEqual(2, count1);
    }

    [TestMethod]
    [ExpectedException(typeof(MissingFieldException))]
    public void ExceptionIsThrownWhenNonTransientExceptionOccursDuringAsyncBegin()
    {
        RetryPolicy<MockErrorDetectionStrategy> policy = this.RetryManager.GetRetryPolicy<MockErrorDetectionStrategy>("Incremental, 5 retries");

        int count1 = 0;
        policy.Retrying += (sender, args) =>
        {
            Assert.IsInstanceOfType(args.LastException, typeof(InvalidCastException));
            count1 = args.CurrentRetryCount;
        };

        TestAsyncOperation asyncInstance = new();
        asyncInstance.CountToThrowAtBegin = 3;
        asyncInstance.ExceptionToThrowAtBegin = new InvalidCastException();
        asyncInstance.FatalException = new MissingFieldException();
        asyncInstance.ThrowFatalExceptionAtBegin = true;
        try
        {
            policy
                .ExecuteAsync(() =>
                    Task.Factory.FromAsync(asyncInstance.BeginMethod, asyncInstance.EndMethod, null))
                .Wait(TimeSpan.FromSeconds(5));
        }
        catch (AggregateException ex)
        {
            Assert.AreEqual(2, count1);
            Assert.AreEqual(1, ex.InnerExceptions.Count);

            throw ex.InnerException!;
        }

        Assert.Fail("Test should throw");
    }

    [TestMethod]
    [ExpectedException(typeof(MissingFieldException))]
    public void ExceptionIsThrownWhenNonTransientExceptionOccursDuringAsyncEnd()
    {
        RetryPolicy<MockErrorDetectionStrategy> policy = this.RetryManager.GetRetryPolicy<MockErrorDetectionStrategy>("Incremental, 5 retries");

        int count1 = 0;
        policy.Retrying += (sender, args) =>
        {
            Assert.IsInstanceOfType(args.LastException, typeof(InvalidCastException));
            count1 = args.CurrentRetryCount;
        };

        TestAsyncOperation asyncInstance = new();
        asyncInstance.CountToThrowAtEnd = 3;
        asyncInstance.ExceptionToThrowAtEnd = new InvalidCastException();
        asyncInstance.FatalException = new MissingFieldException();
        asyncInstance.ThrowFatalExceptionAtBegin = false;
        try
        {
            policy
                .ExecuteAsync(() =>
                    Task.Factory.FromAsync(asyncInstance.BeginMethod, asyncInstance.EndMethod, null))
                .Wait(TimeSpan.FromSeconds(5));
        }
        catch (AggregateException ex)
        {
            Assert.AreEqual(2, count1);
            Assert.AreEqual(1, ex.InnerExceptions.Count);

            throw ex.InnerException!;
        }

        Assert.Fail("Test should throw");
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidCastException))]
    public void ExceptionIsThrownWhenAllRetriesFailInsideATask()
    {
        RetryPolicy<MockErrorDetectionStrategy> policy = this.RetryManager.GetRetryPolicy<MockErrorDetectionStrategy>("Incremental, 5 retries");

        int count1 = 0;
        policy.Retrying += (sender, args) =>
        {
            Assert.IsInstanceOfType(args.LastException, typeof(InvalidCastException));
            count1 = args.CurrentRetryCount;
        };

        int attempts = 0;
        Task executeTask = policy.ExecuteAsync(() => Task.Factory.StartNew(() =>
        {
            ++attempts;
            throw new InvalidCastException();
        }));

        try
        {
            executeTask.Wait(TimeSpan.FromSeconds(5));
        }
        catch (AggregateException)
        {
            Assert.AreEqual(5, count1);
            Assert.AreEqual(6, attempts);

            // should not be nesting the exceptions which were thrown on retry
            if (executeTask.Exception!.InnerExceptions.Count > 1)
            {
                Assert.Fail("More than one exception was thrown");
            }

            throw executeTask.Exception.InnerException!;
        }

        Assert.Fail("Test should throw");
    }

    [TestMethod]
    public void ActionWithTaskIsExecutedWhenSomeRetriesFailAndThenSucceeds()
    {
        RetryPolicy<MockErrorDetectionStrategy> policy = this.RetryManager.GetRetryPolicy<MockErrorDetectionStrategy>("Incremental, 5 retries");

        int count1 = 0;
        policy.Retrying += (sender, args) =>
        {
            Assert.IsInstanceOfType(args.LastException, typeof(InvalidCastException));
            count1 = args.CurrentRetryCount;
        };

        int attempts = 0;
        Task<int> executeTask = policy.ExecuteAsync(() => Task.Factory.StartNew(() =>
        {
            ++attempts;
            if (attempts < 3)
            {
                throw new InvalidCastException();
            }

            return 1;
        }));

        executeTask.Wait(TimeSpan.FromSeconds(5));
        Assert.AreEqual(1, executeTask.Result);
        Assert.AreEqual(2, count1);
        Assert.AreEqual(3, attempts);
    }

    [TestMethod]
    public void ActionWithLongRunningTaskIsExecutedWhenSomeRetriesFailAndThenSucceeds()
    {
        RetryPolicy<MockErrorDetectionStrategy> policy = this.RetryManager.GetRetryPolicy<MockErrorDetectionStrategy>("Incremental, 5 retries");

        int count1 = 0;
        policy.Retrying += (sender, args) =>
        {
            Assert.IsInstanceOfType(args.LastException, typeof(InvalidCastException));
            count1 = args.CurrentRetryCount;
        };

        int attempts = 0;
        Task<int> executeTask = policy.ExecuteAsync(() => Task.Factory.StartNew(() =>
            {
                ++attempts;
                if (attempts < 3)
                {
                    throw new InvalidCastException();
                }

                return 1;
            },
            TaskCreationOptions.LongRunning));

        executeTask.Wait(TimeSpan.FromSeconds(5));
        Assert.AreEqual(1, executeTask.Result);
        Assert.AreEqual(2, count1);
        Assert.AreEqual(3, attempts);
    }

    [TestMethod]
    public void ActionWithTaskAndAttachedTaskIsExecutedWhenSomeRetriesFailAndThenSucceeds()
    {
        RetryPolicy<MockErrorDetectionStrategy> policy = this.RetryManager.GetRetryPolicy<MockErrorDetectionStrategy>("Incremental, 5 retries");

        int count1 = 0;
        policy.Retrying += (sender, args) =>
        {
            Assert.IsInstanceOfType(args.LastException, typeof(AggregateException));
            count1 = args.CurrentRetryCount;
        };

        int attempts = 0;
        Task<int> executeTask = policy.ExecuteAsync(() => Task.Factory.StartNew(() =>
        {
            Task<int> inner = Task.Factory.StartNew(() =>
                {
                    ++attempts;
                    if (attempts < 3)
                    {
                        throw new InvalidCastException();
                    }

                    return 1;
                },
                TaskCreationOptions.AttachedToParent);

            return inner.Result;
        }));

        executeTask.Wait(TimeSpan.FromSeconds(5));
        Assert.AreEqual(1, executeTask.Result);
        Assert.AreEqual(2, count1);
        Assert.AreEqual(3, attempts);
    }

    [TestMethod]
    public void ActionWithNestedTasksIsExecutedWhenSomeRetriesFailAndThenSucceeds()
    {
        RetryPolicy<MockErrorDetectionStrategy> policy = this.RetryManager.GetRetryPolicy<MockErrorDetectionStrategy>("Incremental, 5 retries");

        int count1 = 0;
        policy.Retrying += (sender, args) =>
        {
            Assert.IsInstanceOfType(args.LastException, typeof(InvalidCastException));
            count1 = args.CurrentRetryCount;
        };

        int attempts = 0;
        Task executeTask = Task.Factory.StartNew(() =>
        {
            Task child = policy.ExecuteAsync(() => Task.Factory.StartNew(() =>
            {
                ++attempts;
                if (attempts < 3)
                {
                    throw new InvalidCastException();
                }
            }));

            child.Wait(TimeSpan.FromSeconds(5));
        });

        executeTask.Wait(TimeSpan.FromSeconds(5));
        Assert.AreEqual(2, count1);
        Assert.AreEqual(3, attempts);
    }

    [TestMethod]
    public void ActionWithTaskIsExecutedWhenFirstRetrySucceeds()
    {
        RetryPolicy<MockErrorDetectionStrategy> policy = this.RetryManager.GetRetryPolicy<MockErrorDetectionStrategy>("Incremental, 5 retries");

        int count1 = 0;
        policy.Retrying += (sender, args) =>
        {
            Assert.IsInstanceOfType(args.LastException, typeof(InvalidCastException));
            count1 = args.CurrentRetryCount;
        };

        int attempts = 0;
        Task<int> executeTask = policy.ExecuteAsync(() => Task.Factory.StartNew(() =>
        {
            ++attempts;
            return 1;
        }));

        Assert.AreEqual(1, executeTask.Result);
        Assert.AreEqual(0, count1);
        Assert.AreEqual(1, attempts);
    }

    [TestMethod]
    public void ActionWithTaskIsCancelledWhenCancellingBeforeItStarts()
    {
        RetryPolicy<MockErrorDetectionStrategy> policy = this.RetryManager.GetRetryPolicy<MockErrorDetectionStrategy>("Incremental, 5 retries");

        CancellationTokenSource cancellationToken = new();
        int count1 = 0;
        policy.Retrying += (sender, args) =>
        {
            Assert.IsInstanceOfType(args.LastException, typeof(InvalidCastException));
            count1 = args.CurrentRetryCount;
        };

        int attempts = 0;
        bool taskHasStarted = false;
        Task<int> executeTask = policy.ExecuteAsync(() => Task.Factory.StartNew(() =>
                {
                    taskHasStarted = true;
                    Task.Delay(TimeSpan.FromSeconds(5)).Wait(cancellationToken.Token); // Delay so task does not start before Cancel is called

                    ++attempts;
                    if (attempts < 3)
                    {
                        throw new InvalidCastException();
                    }

                    return 1;
                },
                cancellationToken.Token),
            cancellationToken.Token);

        Task.Delay(TimeSpan.FromMilliseconds(100)).Wait();
        cancellationToken.Cancel();
        while (!executeTask.IsCanceled && !executeTask.IsFaulted && !executeTask.IsCompleted)
        {
            Task.Delay(TimeSpan.FromMilliseconds(100)).Wait();
        }

        Assert.IsTrue(taskHasStarted, "executeTask has not started");
        Assert.AreEqual(TaskStatus.Canceled, executeTask.Status);
        Assert.AreEqual(0, count1);
        Assert.AreEqual(0, attempts);
    }

    [TestMethod]
    public void ActionWithTaskIsCancelledWhenCancellingAfterSomeRetriesFail()
    {
        RetryPolicy<MockErrorDetectionStrategy> policy = this.RetryManager.GetRetryPolicy<MockErrorDetectionStrategy>("Incremental, 5 retries");

        CancellationTokenSource cancellationToken = new();
        int count1 = 0;
        policy.Retrying += (sender, args) =>
        {
            Assert.IsInstanceOfType(args.LastException, typeof(InvalidCastException));
            count1 = args.CurrentRetryCount;
        };

        bool exceptionThrown = false;
        try
        {
            int attempts = 0;
            policy.ExecuteAsync(() => Task.Factory.StartNew(() =>
                {
                    if (attempts > 2)
                    {
                        cancellationToken.Cancel();
                    }

                    ++attempts;
                    throw new InvalidCastException();
                }),
                cancellationToken.Token).Wait(TimeSpan.FromSeconds(5));
        }
        catch (AggregateException exp)
        {
            exceptionThrown = true;
            Assert.AreEqual(1, exp.InnerExceptions.Count, "Expected only 1 exception");
            Assert.IsInstanceOfType(exp.InnerExceptions[0], typeof(InvalidCastException));
        }

        Assert.IsTrue(exceptionThrown, "An aggregate exception was expected and not thrown");
        Assert.IsTrue(count1 > 0);
        Assert.IsTrue(count1 < 5);
    }
}