namespace Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling;

/// <summary>
/// Detects specific transient conditions.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="ErrorDetectionStrategy"/> class.
/// </remarks>
/// <param name="isTransient">The predicate function to detect whether the specified exception is transient. The default behavior is to catch all exceptions and retry.</param>
public class ErrorDetectionStrategy(Func<Exception, bool>? isTransient = null) : ITransientErrorDetectionStrategy
{
    private readonly Func<Exception, bool> isTransient = isTransient ?? (_ => true);

    /// <summary>
    /// Gets an instance of <see cref="ITransientErrorDetectionStrategy"/> that catches all exceptions as transient.
    /// </summary>
    public static ErrorDetectionStrategy AlwaysTransient { get; } = new();

    /// <summary>
    /// Gets an instance of <see cref="ITransientErrorDetectionStrategy"/> that detects any exception as not transient.
    /// </summary>
    public static ErrorDetectionStrategy NeverTransient { get; } = new(_ => false);

    /// <summary>
    /// Determines whether the specified exception is transient.
    /// </summary>
    /// <param name="exception">The exception.</param>
    /// <returns><c>true</c> if the specified exception is transient; otherwise, <c>false</c>.</returns>
    public bool IsTransient(Exception exception) => this.isTransient(exception);
}



/// <summary>
/// Detects specific transient exception.
/// </summary>
/// <typeparam name="TException">The type of the transient exception.</typeparam>
public class ErrorDetectionStrategy<TException> : ErrorDetectionStrategy
    where TException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ErrorDetectionStrategy{TException}"/> class.
    /// </summary>
    /// <param name="isTransient">The predicate function to detect whether the specified exception is transient.</param>
    public ErrorDetectionStrategy(Func<TException, bool>? isTransient = null) :
        base(exception => isTransient is null
            ? exception is TException
            : exception is TException transientException && isTransient(transientException))
    {
    }

    /// <summary>
    /// Gets an instance of <see cref="ITransientErrorDetectionStrategy"/> that catches all <c>TException</c> as transient.
    /// </summary>
    public new static ErrorDetectionStrategy<TException> AlwaysTransient { get; } = new();

    /// <summary>
    /// Gets an instance of <see cref="ITransientErrorDetectionStrategy"/> that detects any <c>TException</c> as not transient.
    /// </summary>
    public new static ErrorDetectionStrategy<TException> NeverTransient { get; } = new(_ => false);
}