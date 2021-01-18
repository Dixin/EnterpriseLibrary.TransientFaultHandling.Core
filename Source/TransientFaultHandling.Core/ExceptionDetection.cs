namespace Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling
{
    using System;

    /// <summary>
    /// Detects specific transient conditions.
    /// </summary>
    public class ExceptionDetection : ITransientErrorDetectionStrategy
    {
        private readonly Func<Exception, bool> isTransient;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExceptionDetection"/> class.
        /// </summary>
        /// <param name="isTransient">The predicate function to detect whether the specified exception is transient.</param>
        public ExceptionDetection(Func<Exception, bool>? isTransient = null) =>
            this.isTransient = isTransient ?? (_ => true);

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
}
