namespace Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling;

using static ErrorDetectionStrategy;

/// <summary>
/// Provides the base implementation of the retry mechanism for unreliable actions and transient conditions.
/// </summary>
public class RetryPolicy
{
    /// <summary>
    /// Initializes a new instance of the <see cref="T:Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling.RetryPolicy" /> class with the specified number of retry attempts and parameters defining the progressive delay between retries.
    /// </summary>
    /// <param name="errorDetectionStrategy">The <see cref="T:Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling.ITransientErrorDetectionStrategy" /> that is responsible for detecting transient conditions.</param>
    /// <param name="retryStrategy">The strategy to use for this retry policy.</param>
    public RetryPolicy(ITransientErrorDetectionStrategy errorDetectionStrategy, RetryStrategy retryStrategy)
    {
        this.ErrorDetectionStrategy= errorDetectionStrategy.NotNull();
        this.RetryStrategy = retryStrategy.NotNull();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling.RetryPolicy" /> class with the specified number of retry attempts and default fixed time interval between retries.
    /// </summary>
    /// <param name="errorDetectionStrategy">The <see cref="T:Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling.ITransientErrorDetectionStrategy" /> that is responsible for detecting transient conditions.</param>
    /// <param name="retryCount">The number of retry attempts.</param>
    public RetryPolicy(ITransientErrorDetectionStrategy errorDetectionStrategy, int retryCount) : 
        this(errorDetectionStrategy, new FixedInterval(retryCount))
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling.RetryPolicy" /> class with the specified number of retry attempts and fixed time interval between retries.
    /// </summary>
    /// <param name="errorDetectionStrategy">The <see cref="T:Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling.ITransientErrorDetectionStrategy" /> that is responsible for detecting transient conditions.</param>
    /// <param name="retryCount">The number of retry attempts.</param>
    /// <param name="retryInterval">The interval between retries.</param>
    public RetryPolicy(ITransientErrorDetectionStrategy errorDetectionStrategy, int retryCount, TimeSpan retryInterval) : 
        this(errorDetectionStrategy, new FixedInterval(retryCount, retryInterval))
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling.RetryPolicy" /> class with the specified number of retry attempts and backoff parameters for calculating the exponential delay between retries.
    /// </summary>
    /// <param name="errorDetectionStrategy">The <see cref="T:Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling.ITransientErrorDetectionStrategy" /> that is responsible for detecting transient conditions.</param>
    /// <param name="retryCount">The number of retry attempts.</param>
    /// <param name="minBackoff">The minimum backoff time.</param>
    /// <param name="maxBackoff">The maximum backoff time.</param>
    /// <param name="deltaBackoff">The time value that will be used to calculate a random delta in the exponential delay between retries.</param>
    public RetryPolicy(ITransientErrorDetectionStrategy errorDetectionStrategy, int retryCount, TimeSpan minBackoff, TimeSpan maxBackoff, TimeSpan deltaBackoff) : 
        this(errorDetectionStrategy, new ExponentialBackoff(retryCount, minBackoff, maxBackoff, deltaBackoff))
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling.RetryPolicy" /> class with the specified number of retry attempts and parameters defining the progressive delay between retries.
    /// </summary>
    /// <param name="errorDetectionStrategy">The <see cref="T:Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling.ITransientErrorDetectionStrategy" /> that is responsible for detecting transient conditions.</param>
    /// <param name="retryCount">The number of retry attempts.</param>
    /// <param name="initialInterval">The initial interval that will apply for the first retry.</param>
    /// <param name="increment">The incremental time value that will be used to calculate the progressive delay between retries.</param>
    public RetryPolicy(ITransientErrorDetectionStrategy errorDetectionStrategy, int retryCount, TimeSpan initialInterval, TimeSpan increment) : 
        this(errorDetectionStrategy, new Incremental(retryCount, initialInterval, increment))
    {
    }

    /// <summary>
    /// An instance of a callback delegate that will be invoked whenever a retry condition is encountered.
    /// </summary>
    public event EventHandler<RetryingEventArgs>? Retrying;

    /// <summary>
    /// Returns a default policy that performs no retries, but invokes the action only once.
    /// </summary>
    public static RetryPolicy NoRetry { get; } = new(NeverTransient, RetryStrategy.NoRetry);

    /// <summary>
    /// Returns a default policy that implements a fixed retry interval configured with the default <see cref="T:Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling.FixedInterval" /> retry strategy.
    /// The default retry policy treats all caught exceptions as transient errors.
    /// </summary>
    public static RetryPolicy DefaultFixed { get; } = new(AlwaysTransient, RetryStrategy.DefaultFixed);

    /// <summary>
    /// Returns a default policy that implements a progressive retry interval configured with the default <see cref="T:Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling.Incremental" /> retry strategy.
    /// The default retry policy treats all caught exceptions as transient errors.
    /// </summary>
    public static RetryPolicy DefaultProgressive { get; } = new(AlwaysTransient, RetryStrategy.DefaultProgressive);

    /// <summary>
    /// Returns a default policy that implements a random exponential retry interval configured with the default <see cref="T:Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling.FixedInterval" /> retry strategy.
    /// The default retry policy treats all caught exceptions as transient errors.
    /// </summary>
    public static RetryPolicy DefaultExponential { get; } = new(AlwaysTransient, RetryStrategy.DefaultExponential);

    /// <summary>
    /// Gets the retry strategy.
    /// </summary>
    public RetryStrategy RetryStrategy { get; }

    /// <summary>
    /// Gets the instance of the error detection strategy.
    /// </summary>
    public ITransientErrorDetectionStrategy ErrorDetectionStrategy { get; }

    /// <summary>
    /// Repetitively executes the specified action while it satisfies the current retry policy.
    /// </summary>
    /// <param name="action">A delegate that represents the executable action that doesn't return any results.</param>
    public virtual void ExecuteAction(Action action)
    {
        action.NotNull();

        this.ExecuteAction<object?>(() =>
        {
            action();
            return null;
        });
    }

    /// <summary>
    /// Repetitively executes the specified action while it satisfies the current retry policy.
    /// </summary>
    /// <typeparam name="TResult">The type of result expected from the executable action.</typeparam>
    /// <param name="func">A delegate that represents the executable action that returns the result of type <typeparamref name="TResult" />.</param>
    /// <returns>The result from the action.</returns>
    public virtual TResult ExecuteAction<TResult>(Func<TResult> func)
    {
        func.NotNull();

        int retryCount = 0;
        ShouldRetry shouldRetry = this.RetryStrategy.GetShouldRetry();
        for (; ; )
        {
            Exception lastException;
            TimeSpan delay;
            try
            {
                return func();
            }
#pragma warning disable 0618
            catch (RetryLimitExceededException limitExceededEx)
#pragma warning restore 0618
            {
                // The user code can throw a RetryLimitExceededException to force the exit from the retry loop.
                // The RetryLimitExceeded exception can have an inner exception attached to it. This is the exception
                // which we will have to throw up the stack so that callers can handle it.
                if (limitExceededEx.InnerException is not null)
                {
                    throw limitExceededEx.InnerException;
                }

#pragma warning disable CS8603 // Possible null reference return.
                return default(TResult);
#pragma warning restore CS8603 // Possible null reference return.
            }
            catch (Exception ex)
            {
                lastException = ex;

                if (!(this.ErrorDetectionStrategy.IsTransient(lastException) && shouldRetry(retryCount++, lastException, out delay)))
                {
                    throw;
                }
            }

            // Perform an extra check in the delay interval. Should prevent from accidentally ending up with the value of -1 that will block a thread indefinitely. 
            // In addition, any other negative numbers will cause an ArgumentOutOfRangeException fault that will be thrown by Thread.Sleep.
            if (delay.TotalMilliseconds < 0)
            {
                delay = TimeSpan.Zero;
            }

            this.OnRetrying(retryCount, lastException, delay);

            if (retryCount > 1 || !this.RetryStrategy.FastFirstRetry)
            {
                Task.Delay(delay).Wait();
            }
        }
    }

    /// <summary>
    /// Repetitively executes the specified asynchronous task while it satisfies the current retry policy.
    /// </summary>
    /// <param name="taskAction">A function that returns a started task (also known as "hot" task).</param>
    /// <param name="cancellationToken">The token used to cancel the retry operation. This token does not cancel the execution of the asynchronous task.</param>
    /// <returns>
    /// Returns a task that will run to completion if the original task completes successfully (either the
    /// first time or after retrying transient failures). If the task fails with a non-transient error or
    /// the retry limit is reached, the returned task will transition to a faulted state and the exception must be observed.
    /// </returns>
    public Task ExecuteAsync(Func<Task> taskAction, CancellationToken cancellationToken = default) => 
        new AsyncExecution(taskAction.NotNull(), this.RetryStrategy.GetShouldRetry(), this.ErrorDetectionStrategy.IsTransient, this.OnRetrying, this.RetryStrategy.FastFirstRetry, cancellationToken).ExecuteAsync();

    /// <summary>
    /// Repeatedly executes the specified asynchronous task while it satisfies the current retry policy.
    /// </summary>
    /// <param name="taskFunc">A function that returns a started task (also known as "hot" task).</param>
    /// <param name="cancellationToken">The token used to cancel the retry operation. This token does not cancel the execution of the asynchronous task.</param>
    /// <returns>
    /// Returns a task that will run to completion if the original task completes successfully (either the
    /// first time or after retrying transient failures). If the task fails with a non-transient error or
    /// the retry limit is reached, the returned task will transition to a faulted state and the exception must be observed.
    /// </returns>
    public Task<TResult> ExecuteAsync<TResult>(Func<Task<TResult>> taskFunc, CancellationToken cancellationToken = default) => 
        new AsyncExecution<TResult>(taskFunc.NotNull(), this.RetryStrategy.GetShouldRetry(), this.ErrorDetectionStrategy.IsTransient, this.OnRetrying, this.RetryStrategy.FastFirstRetry, cancellationToken).ExecuteAsync();

    /// <summary>
    /// Notifies the subscribers whenever a retry condition is encountered.
    /// </summary>
    /// <param name="retryCount">The current retry attempt count.</param>
    /// <param name="lastError">The exception that caused the retry conditions to occur.</param>
    /// <param name="delay">The delay that indicates how long the current thread will be suspended before the next iteration is invoked.</param>
    protected virtual void OnRetrying(int retryCount, Exception lastError, TimeSpan delay) =>
        this.InvokeRetrying(this, new RetryingEventArgs(retryCount, delay, lastError));

    internal void InvokeRetrying(object? sender, RetryingEventArgs args)
    {
        EventHandler<RetryingEventArgs>? retrying = this.Retrying;
        retrying?.Invoke(sender, args);
    }
}