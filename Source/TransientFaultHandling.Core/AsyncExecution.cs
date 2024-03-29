﻿namespace Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling;

/// <summary>
/// Provides a wrapper for a non-generic <see cref="T:System.Threading.Tasks.Task" /> and calls into the pipeline
/// to retry only the generic version of the <see cref="T:System.Threading.Tasks.Task" />.
/// </summary>
internal class AsyncExecution(Func<Task> taskAction, ShouldRetry shouldRetry, Func<Exception, bool> isTransient, Action<int, Exception, TimeSpan> onRetrying, bool fastFirstRetry, CancellationToken cancellationToken)
    : AsyncExecution<bool>(() => StartAsGenericTask(taskAction), shouldRetry, isTransient, onRetrying, fastFirstRetry, cancellationToken)
{
    private static Task<bool>? cachedBoolTask;

    /// <summary>
    /// Wraps the non-generic <see cref="T:System.Threading.Tasks.Task" /> into a generic <see cref="T:System.Threading.Tasks.Task" />.
    /// </summary>
    /// <param name="taskAction">The task to wrap.</param>
    /// <returns>A <see cref="T:System.Threading.Tasks.Task" /> that wraps the non-generic <see cref="T:System.Threading.Tasks.Task" />.</returns>
    private static Task<bool> StartAsGenericTask(Func<Task> taskAction)
    {
        Task task = taskAction();
        if (task is null)
        {
            throw new ArgumentException(
                string.Format(CultureInfo.InvariantCulture, Resources.TaskCannotBeNull, nameof(taskAction)), nameof(taskAction));
        }

        if (task.Status == TaskStatus.RanToCompletion)
        {
            return GetCachedTask();
        }

        if (task.Status == TaskStatus.Created)
        {
            throw new ArgumentException(
                string.Format(CultureInfo.InvariantCulture, Resources.TaskMustBeScheduled, nameof(taskAction)), nameof(taskAction));
        }

        TaskCompletionSource<bool> taskCompletionSource = new();
        task.ContinueWith(
            result =>
            {
                if (result.IsFaulted)
                {
                    taskCompletionSource.TrySetException(result.Exception!.InnerExceptions);
                    return;
                }

                if (result.IsCanceled)
                {
                    taskCompletionSource.TrySetCanceled();
                    return;
                }

                taskCompletionSource.TrySetResult(true);
            },
            TaskContinuationOptions.ExecuteSynchronously);
        return taskCompletionSource.Task;
    }

    private static Task<bool> GetCachedTask()
    {
        if (cachedBoolTask is null)
        {
            TaskCompletionSource<bool> taskCompletionSource = new();
            taskCompletionSource.TrySetResult(true);
            cachedBoolTask = taskCompletionSource.Task;
        }

        return cachedBoolTask;
    }
}