﻿namespace Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling;

public static partial class Retry
{
    /// <summary>
    /// Repetitively executes the specified action while it satisfies the specified retry strategy.
    /// </summary>
    /// <typeparam name="TResult">The type of result expected from the executable action.</typeparam>
    /// <param name="func">A delegate that represents the executable action that returns the result of type <typeparamref name="TResult" />.</param>
    /// <param name="retryCount">The number of retry attempts.</param>
    /// <param name="isTransient">The predicate function to detect whether the specified exception is transient.</param>
    /// <param name="retryingHandler">The callback function that will be invoked whenever a retry condition is encountered.</param>
    /// <param name="retryInterval">The time interval between retries.</param>
    /// <param name="firstFastRetry">true to immediately retry in the first attempt; otherwise, false. The subsequent retries will remain subject to the configured retry interval.</param>
    /// <returns>The result from the action.</returns>
    /// <exception cref="ArgumentNullException">func</exception>
    public static TResult FixedInterval<TResult>(
        Func<TResult> func,
        int? retryCount = null,
        Func<Exception, bool>? isTransient = null,
        EventHandler<RetryingEventArgs>? retryingHandler = null,
        TimeSpan? retryInterval = null,
        bool? firstFastRetry = null) =>
        Execute(
            func.ThrowIfNull(),
            WithFixedInterval(retryCount, retryInterval, firstFastRetry),
            isTransient,
            retryingHandler);

    /// <summary>
    /// Repetitively executes the specified action while it satisfies the specified retry strategy.
    /// </summary>
    /// <param name="action">A delegate that represents the executable action that doesn't return any results.</param>
    /// <param name="retryCount">The number of retry attempts.</param>
    /// <param name="isTransient">The predicate function to detect whether the specified exception is transient.</param>
    /// <param name="retryingHandler">The callback function that will be invoked whenever a retry condition is encountered.</param>
    /// <param name="retryInterval">The time interval between retries.</param>
    /// <param name="firstFastRetry">true to immediately retry in the first attempt; otherwise, false. The subsequent retries will remain subject to the configured retry interval.</param>
    /// <exception cref="ArgumentNullException">action</exception>
    public static void FixedInterval(
        Action action,
        int? retryCount = null,
        Func<Exception, bool>? isTransient = null,
        EventHandler<RetryingEventArgs>? retryingHandler = null,
        TimeSpan? retryInterval = null,
        bool? firstFastRetry = null) =>
        Execute(
            action.ThrowIfNull(),
            WithFixedInterval(retryCount, retryInterval, firstFastRetry),
            isTransient,
            retryingHandler);

    /// <summary>
    /// Repeatedly executes the specified asynchronous function while it satisfies the current retry policy.
    /// </summary>
    /// <typeparam name="TResult">The type of result expected from the executable asynchronous function.</typeparam>
    /// <param name="func">A asynchronous function that returns a started task (also known as "hot" task).</param>
    /// <param name="retryCount">The number of retry attempts.</param>
    /// <param name="isTransient">The predicate function to detect whether the specified exception is transient.</param>
    /// <param name="retryingHandler">The callback function that will be invoked whenever a retry condition is encountered.</param>
    /// <param name="retryInterval">The time interval between retries.</param>
    /// <param name="firstFastRetry">true to immediately retry in the first attempt; otherwise, false. The subsequent retries will remain subject to the configured retry interval.</param>
    /// <param name="cancellationToken">The token used to cancel the retry operation. This token does not cancel the execution of the asynchronous task.</param>
    /// <returns>Returns a task that will run to completion if the original task completes successfully (either the first time or after retrying transient failures). If the task fails with a non-transient error or the retry limit is reached, the returned task will transition to a faulted state and the exception must be observed.</returns>
    /// <exception cref="ArgumentNullException">func</exception>
    public static Task<TResult> FixedIntervalAsync<TResult>(
        Func<Task<TResult>> func,
        int? retryCount = null,
        Func<Exception, bool>? isTransient = null,
        EventHandler<RetryingEventArgs>? retryingHandler = null,
        TimeSpan? retryInterval = null,
        bool? firstFastRetry = null,
        CancellationToken cancellationToken = default) =>
        ExecuteAsync(
            func.ThrowIfNull(),
            WithFixedInterval(retryCount, retryInterval, firstFastRetry),
            isTransient,
            retryingHandler,
            cancellationToken);

    /// <summary>
    /// Repeatedly executes the specified asynchronous function while it satisfies the current retry policy.
    /// </summary>
    /// <param name="func">A asynchronous function that returns a started task (also known as "hot" task).</param>
    /// <param name="retryCount">The number of retry attempts.</param>
    /// <param name="isTransient">The predicate function to detect whether the specified exception is transient.</param>
    /// <param name="retryingHandler">The callback function that will be invoked whenever a retry condition is encountered.</param>
    /// <param name="retryInterval">The time interval between retries.</param>
    /// <param name="firstFastRetry">true to immediately retry in the first attempt; otherwise, false. The subsequent retries will remain subject to the configured retry interval.</param>
    /// <param name="cancellationToken">The token used to cancel the retry operation. This token does not cancel the execution of the asynchronous task.</param>
    /// <returns>Returns a task that will run to completion if the original task completes successfully (either the first time or after retrying transient failures). If the task fails with a non-transient error or the retry limit is reached, the returned task will transition to a faulted state and the exception must be observed.</returns>
    /// <exception cref="ArgumentNullException">func</exception>
    public static Task FixedIntervalAsync(
        Func<Task> func,
        int? retryCount = null,
        Func<Exception, bool>? isTransient = null,
        EventHandler<RetryingEventArgs>? retryingHandler = null,
        TimeSpan? retryInterval = null,
        bool? firstFastRetry = null,
        CancellationToken cancellationToken = default) =>
        ExecuteAsync(
            func.ThrowIfNull(),
            WithFixedInterval(retryCount, retryInterval, firstFastRetry),
            isTransient,
            retryingHandler,
            cancellationToken);

    /// <summary>
    /// Create a new instance of the <see cref="T:Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling.FixedInterval" /> class. 
    /// </summary>
    /// <param name="retryCount">The number of retry attempts.</param>
    /// <param name="retryInterval">The time interval between retries.</param>
    /// <param name="firstFastRetry">true to immediately retry in the first attempt; otherwise, false. The subsequent retries will remain subject to the configured retry interval.</param>
    /// <param name="name">The name of the retry strategy.</param>
    /// <returns>A new instance of the <see cref="T:Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling.FixedInterval" /> class.</returns>
    public static FixedInterval WithFixedInterval(
        int? retryCount = null,
        TimeSpan? retryInterval = null,
        bool? firstFastRetry = null,
        string? name = null) => new(
        name,
        retryCount ?? RetryStrategy.DefaultClientRetryCount,
        retryInterval ?? RetryStrategy.DefaultRetryInterval,
        firstFastRetry ?? RetryStrategy.DefaultFirstFastRetry);
}