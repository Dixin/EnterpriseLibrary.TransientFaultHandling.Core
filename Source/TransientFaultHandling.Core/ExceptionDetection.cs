namespace Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling;

/// <summary>
/// Detects specific transient conditions.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="ExceptionDetection"/> class.
/// </remarks>
/// <param name="isTransient">The predicate function to detect whether the specified exception is transient.</param>
[Obsolete("Use Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling.ErrorDetectionStrategy.")]
public class ExceptionDetection(Func<Exception, bool>? isTransient = null) : ITransientErrorDetectionStrategy
{
    private readonly Func<Exception, bool> isTransient = isTransient ?? (_ => true);

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
[Obsolete("Use Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling.ErrorDetectionStrategy<TException>.")]
public class TransientDetection<TException> : ExceptionDetection
    where TException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TransientDetection{TException}"/> class.
    /// </summary>
    public TransientDetection() : base(exception => exception is TException)
    {
    }
}