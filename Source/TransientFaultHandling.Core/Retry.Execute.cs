namespace Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling
{
    using System;
    using System.Threading.Tasks;

    /// <summary>
    /// Provides the retry methods.
    /// </summary>
    public static partial class Retry
    {
        /// <summary>
        /// Repetitively executes the specified action while it satisfies the specified retry strategy.
        /// </summary>
        /// <typeparam name="TResult">he type of result expected from the executable action.</typeparam>
        /// <param name="func">A delegate that represents the executable action that returns the result of type <typeparamref name="TResult" />.</param>
        /// <param name="retryStrategy">The retry strategy.</param>
        /// <param name="isTransient">The predicate function to detect whether the specified exception is transient.</param>
        /// <param name="retryingHandler">The callback function that will be invoked whenever a retry condition is encountered.</param>
        /// <returns>The result from the action.</returns>
        /// <exception cref="ArgumentNullException">func</exception>
        public static TResult Execute<TResult>(
            Func<TResult> func,
            RetryStrategy retryStrategy = null,
            Func<Exception, bool> isTransient = null,
            EventHandler<RetryingEventArgs> retryingHandler = null)
        {
            Guard.ArgumentNotNull(func, nameof(func));

            return CreateRetryPolicy(retryStrategy, isTransient, retryingHandler).ExecuteAction(func);
        }

        /// <summary>
        /// Repetitively executes the specified action while it satisfies the specified retry strategy.
        /// </summary>
        /// <param name="action">A delegate that represents the executable action that doesn't return any results.</param>
        /// <param name="retryStrategy">The retry strategy.</param>
        /// <param name="isTransient">The predicate function to detect whether the specified exception is transient.</param>
        /// <param name="retryingHandler">The callback function that will be invoked whenever a retry condition is encountered.</param>
        /// <exception cref="ArgumentNullException">action</exception>
        public static void Execute(
            Action action,
            RetryStrategy retryStrategy = null,
            Func<Exception, bool> isTransient = null,
            EventHandler<RetryingEventArgs> retryingHandler = null)
        {
            Guard.ArgumentNotNull(action, nameof(action));

            CreateRetryPolicy(retryStrategy, isTransient, retryingHandler).ExecuteAction(action);
        }

        /// <summary>
        /// Repeatedly executes the specified asynchronous function while it satisfies the current retry policy.
        /// </summary>
        /// <typeparam name="TResult">he type of result expected from the executable asynchronous function.</typeparam>
        /// <param name="func">A asynchronous function that returns a started task (also known as "hot" task).</param>
        /// <param name="retryStrategy">The retry strategy.</param>
        /// <param name="isTransient">The predicate function to detect whether the specified exception is transient.</param>
        /// <param name="retryingHandler">The callback function that will be invoked whenever a retry condition is encountered.</param>
        /// <returns>Returns a task that will run to completion if the original task completes successfully (either the first time or after retrying transient failures). If the task fails with a non-transient error or the retry limit is reached, the returned task will transition to a faulted state and the exception must be observed.</returns>
        /// <exception cref="ArgumentNullException">func</exception>
        public static Task<TResult> ExecuteAsync<TResult>(
            Func<Task<TResult>> func,
            RetryStrategy retryStrategy = null,
            Func<Exception, bool> isTransient = null,
            EventHandler<RetryingEventArgs> retryingHandler = null)
        {
            Guard.ArgumentNotNull(func, nameof(func));

            return CreateRetryPolicy(retryStrategy, isTransient, retryingHandler).ExecuteAsync(func);
        }

        /// <summary>
        /// Repeatedly executes the specified asynchronous function while it satisfies the current retry policy.
        /// </summary>
        /// <param name="func">A asynchronous function that returns a started task (also known as "hot" task).</param>
        /// <param name="retryStrategy">The retry strategy.</param>
        /// <param name="isTransient">The predicate function to detect whether the specified exception is transient.</param>
        /// <param name="retryingHandler">The callback function that will be invoked whenever a retry condition is encountered.</param>
        /// <returns>Returns a task that will run to completion if the original task completes successfully (either the first time or after retrying transient failures). If the task fails with a non-transient error or the retry limit is reached, the returned task will transition to a faulted state and the exception must be observed.</returns>
        /// <exception cref="ArgumentNullException">func</exception>
        public static Task ExecuteAsync(
            Func<Task> func,
            RetryStrategy retryStrategy = null,
            Func<Exception, bool> isTransient = null,
            EventHandler<RetryingEventArgs> retryingHandler = null)
        {
            Guard.ArgumentNotNull(func, nameof(func));

            return CreateRetryPolicy(retryStrategy, isTransient, retryingHandler).ExecuteAsync(func);
        }
    }
}