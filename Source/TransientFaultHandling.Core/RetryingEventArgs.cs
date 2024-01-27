namespace Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling;

/// <summary>
/// Contains information that is required for the <see cref="E:Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling.RetryPolicy.Retrying" /> event.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="T:Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling.RetryingEventArgs" /> class.
/// </remarks>
/// <param name="currentRetryCount">The current retry attempt count.</param>
/// <param name="delay">The delay that indicates how long the current thread will be suspended before the next iteration is invoked.</param>
/// <param name="lastException">The exception that caused the retry conditions to occur.</param>
public class RetryingEventArgs(int currentRetryCount, TimeSpan delay, Exception lastException) : EventArgs
{
    /// <summary>
    /// Gets the current retry count.
    /// </summary>
    public int CurrentRetryCount { get; } = currentRetryCount;

    /// <summary>
    /// Gets the delay that indicates how long the current thread will be suspended before the next iteration is invoked.
    /// </summary>
    public TimeSpan Delay { get; } = delay;

    /// <summary>
    /// Gets the exception that caused the retry conditions to occur.
    /// </summary>
    public Exception LastException { get; } = lastException.ThrowIfNull();
}