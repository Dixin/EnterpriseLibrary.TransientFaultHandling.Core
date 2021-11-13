namespace Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling;

public static partial class Retry
{
    /// <summary>
    /// Create a new instance of the <see cref="T:Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling.RetryPolicy" /> class with the specified retry strategy.
    /// </summary>
    /// <param name="retryStrategy">The retry strategy.</param>
    /// <param name="isTransient">The predicate function to detect whether the specified exception is transient.</param>
    /// <param name="retryingHandler">The callback function that will be invoked whenever a retry condition is encountered.</param>
    /// <returns>A new instance of the <see cref="T:Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling.RetryPolicy" /> class with the specified number of retry attempts and parameters defining the progressive delay between retries.</returns>
    public static RetryPolicy CreateRetryPolicy(
        RetryStrategy? retryStrategy = null,
        Func<Exception, bool>? isTransient = null,
        EventHandler<RetryingEventArgs>? retryingHandler = null)
    {
        RetryPolicy retryPolicy = new(new ErrorDetectionStrategy(isTransient), retryStrategy ?? RetryStrategy.NoRetry);
        if (retryingHandler is not null)
        {
            retryPolicy.Retrying += retryingHandler;
        }

        return retryPolicy;
    }

    /// <summary>
    /// Create a new instance of the <see cref="T:Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling.RetryPolicy" /> class with the specified retry strategy.
    /// </summary>
    /// <param name="retryStrategy">The retry strategy.</param>
    /// <param name="isTransient">The predicate function to detect whether the specified exception is transient.</param>
    /// <returns>A new instance of the <see cref="T:Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling.RetryPolicy" /> class.</returns>
    public static RetryPolicy Catch(this RetryStrategy retryStrategy, Func<Exception, bool>? isTransient = null) => 
        CreateRetryPolicy(Argument.NotNull(retryStrategy, nameof(retryStrategy)), isTransient);

    /// <summary>
    /// Create a new instance of the <see cref="T:Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling.RetryPolicy" /> class with the specified retry policy.
    /// </summary>
    /// <param name="retryPolicy">The retry strategy.</param>
    /// <param name="isTransient">The predicate function to detect whether the specified exception is transient.</param>
    /// <returns>A new instance of the <see cref="T:Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling.RetryPolicy" /> class.</returns>
    public static RetryPolicy Catch(this RetryPolicy retryPolicy, Func<Exception, bool>? isTransient = null) => 
        Catch<Exception>(Argument.NotNull(retryPolicy, nameof(retryPolicy)), isTransient);

    /// <summary>
    /// Create a new instance of the <see cref="T:Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling.RetryPolicy" /> class with the specified retry strategy.
    /// </summary>
    /// <param name="retryStrategy">The retry strategy.</param>
    /// <param name="isTransient">The predicate function to detect whether the specified exception is transient.</param>
    /// <typeparam name="TException">The type of the transient exception.</typeparam>
    /// <returns>A new instance of the <see cref="T:Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling.RetryPolicy" /> class.</returns>
    public static RetryPolicy Catch<TException>(
        this RetryStrategy retryStrategy,
        Func<TException, bool>? isTransient = null) where TException : Exception =>
        CreateRetryPolicy(
            Argument.NotNull(retryStrategy, nameof(retryStrategy)),
            isTransient is null
                ? exception => exception is TException
                : exception => exception is TException specifiedException && isTransient(specifiedException));

    /// <summary>
    /// Create a new instance of the <see cref="T:Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling.RetryPolicy" /> class based on the specified retry policy.
    /// </summary>
    /// <param name="retryPolicy">The retry police.</param>
    /// <param name="isTransient">The predicate function to detect whether the specified exception is transient.</param>
    /// <typeparam name="TException">The type of the transient exception.</typeparam>
    /// <returns>A new instance of the <see cref="T:Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling.RetryPolicy" /> class.</returns>
    public static RetryPolicy Catch<TException>(
        this RetryPolicy retryPolicy,
        Func<TException, bool>? isTransient = null) where TException : Exception =>
        CreateRetryPolicy(
            Argument.NotNull(retryPolicy, nameof(retryPolicy)).RetryStrategy,
            isTransient is null
                ? exception => retryPolicy.ErrorDetectionStrategy.IsTransient(exception)
                    || exception is TException
                : exception => retryPolicy.ErrorDetectionStrategy.IsTransient(exception)
                    || exception is TException specifiedException && isTransient(specifiedException),
            retryPolicy.InvokeRetrying);

    /// <summary>
    /// Create a new instance of the <see cref="T:Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling.RetryPolicy" /> class based on the specified retry strategy.
    /// </summary>
    /// <param name="retryStrategy">The retry strategy.</param>
    /// <param name="retryingHandler">The callback function that will be invoked whenever a retry condition is encountered.</param>
    /// <returns>A new instance of the <see cref="T:Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling.RetryPolicy" /> class.</returns>
    public static RetryPolicy HandleWith(this RetryStrategy retryStrategy, EventHandler<RetryingEventArgs> retryingHandler)
    {
        Argument.NotNull(retryStrategy, nameof(retryStrategy));
        Argument.NotNull(retryingHandler, nameof(retryingHandler));

        return CreateRetryPolicy(retryStrategy).HandleWith(retryingHandler);
    }

    /// <summary>
    /// Create a new instance of the <see cref="T:Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling.RetryPolicy" /> class based on the specified retry policy.
    /// </summary>
    /// <param name="retryPolicy">The retry policy.</param>
    /// <param name="retryingHandler">The callback function that will be invoked whenever a retry condition is encountered.</param>
    /// <returns>A new instance of the <see cref="T:Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling.RetryPolicy" /> class.</returns>
    public static RetryPolicy HandleWith(this RetryPolicy retryPolicy, EventHandler<RetryingEventArgs> retryingHandler)
    {
        Argument.NotNull(retryPolicy, nameof(retryPolicy));
        Argument.NotNull(retryingHandler, nameof(retryingHandler));

        retryPolicy.Retrying += retryingHandler;
        return retryPolicy;
    }
}