namespace Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling
{
    using System;
    using System.Globalization;
    using System.Threading;
    using System.Threading.Tasks;

    using Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling.Properties;

    /// <summary>
    /// Handles the execution and retries of the user-initiated task.
    /// </summary>
    /// <typeparam name="TResult">The result type of the user-initiated task.</typeparam>
    internal class AsyncExecution<TResult>
    {
        private readonly Func<Task<TResult>> taskFunc;

        private readonly ShouldRetry shouldRetry;

        private readonly Func<Exception, bool> isTransient;

        private readonly Action<int, Exception, TimeSpan> onRetrying;

        private readonly bool fastFirstRetry;

        private readonly CancellationToken cancellationToken;

        private Task<TResult>? previousTask;

        private int retryCount;

        public AsyncExecution(Func<Task<TResult>> taskFunc, ShouldRetry shouldRetry, Func<Exception, bool> isTransient, Action<int, Exception, TimeSpan> onRetrying, bool fastFirstRetry, CancellationToken cancellationToken)
        {
            this.taskFunc = taskFunc;
            this.shouldRetry = shouldRetry;
            this.isTransient = isTransient;
            this.onRetrying = onRetrying;
            this.fastFirstRetry = fastFirstRetry;
            this.cancellationToken = cancellationToken;
        }

        internal Task<TResult> ExecuteAsync() => this.ExecuteAsyncImpl(null);

        private Task<TResult> ExecuteAsyncImpl(Task? ignore)
        {
            if (this.cancellationToken.IsCancellationRequested)
            {
                if (this.previousTask is not null)
                {
                    return this.previousTask;
                }

                TaskCompletionSource<TResult> taskCompletionSource = new();
                taskCompletionSource.TrySetCanceled();
                return taskCompletionSource.Task;
            }

            Task<TResult> task;
            try
            {
                task = this.taskFunc();
            }
            catch (Exception ex)
            {
                if (!this.isTransient(ex))
                {
                    throw;
                }

                TaskCompletionSource<TResult> taskCompletionSource = new();
                taskCompletionSource.TrySetException(ex);
                task = taskCompletionSource.Task;
            }

            if (task is null)
            {
                throw new ArgumentException(
                    string.Format(CultureInfo.InvariantCulture, Resources.TaskCannotBeNull, nameof(this.taskFunc)), nameof(this.taskFunc));
            }

            return task.Status switch
            {
                TaskStatus.RanToCompletion => task,
                TaskStatus.Created => throw new ArgumentException(
                    string.Format(CultureInfo.InvariantCulture, Resources.TaskMustBeScheduled, nameof(this.taskFunc)), nameof(this.taskFunc)),
                _ => task
                    .ContinueWith(this.ExecuteAsyncContinueWith, CancellationToken.None, TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default)
                    .Unwrap()
            };
        }

        private Task<TResult> ExecuteAsyncContinueWith(Task<TResult> runningTask)
        {
            if (!runningTask.IsFaulted || this.cancellationToken.IsCancellationRequested)
            {
                return runningTask;
            }

            Exception innerException = runningTask.Exception!.InnerException!;
#pragma warning disable 618
            if (innerException is RetryLimitExceededException)
#pragma warning restore 618
            {
                TaskCompletionSource<TResult> taskCompletionSource = new();
                if (innerException.InnerException is not null)
                {
                    taskCompletionSource.TrySetException(innerException.InnerException);
                }
                else
                {
                    taskCompletionSource.TrySetCanceled();
                }

                return taskCompletionSource.Task;
            }

            if (!this.isTransient(innerException) || !this.shouldRetry(this.retryCount++, innerException, out TimeSpan zero))
            {
                return runningTask;
            }

            if (zero < TimeSpan.Zero)
            {
                zero = TimeSpan.Zero;
            }

            this.onRetrying(this.retryCount, innerException, zero);
            this.previousTask = runningTask;
            if (zero > TimeSpan.Zero && (this.retryCount > 1 || !this.fastFirstRetry))
            {
                return Task
                    .Delay(zero, this.cancellationToken)
                    .ContinueWith(this.ExecuteAsyncImpl, CancellationToken.None, TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default)
                    .Unwrap();
            }

            return this.ExecuteAsyncImpl(null);
        }
    }
}
