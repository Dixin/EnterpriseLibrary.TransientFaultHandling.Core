namespace Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling
{
    using System;
    using System.Net.Sockets;
    using System.ServiceModel;
    using Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling.Caching;

    /// <summary>
    /// Provides the transient error detection logic that can recognize transient faults when dealing with Windows Azure Caching.
    /// </summary>
    public class CacheTransientErrorDetectionStrategy : ITransientErrorDetectionStrategy
    {
        /// <summary>
        /// Checks whether or not the specified exception belongs to a category of transient failures that can be compensated by a retry.
        /// </summary>
        /// <param name="ex">The exception object to be verified.</param>
        /// <returns>true if the specified exception belongs to the category of transient errors; otherwise, false.</returns>
        public bool IsTransient(Exception? ex) =>
            ex switch
            {
                null => false,
                ServerTooBusyException => true,
                SocketException socketFault => socketFault.SocketErrorCode == SocketError.TimedOut,
                _ => DataCacheExceptionChecker.IsTransientDataCacheException(ex)
                    // Some transient exceptions may be wrapped into an outer exception, hence we should also inspect any inner exceptions.
                    ?? ex.InnerException is not null && this.IsTransient(ex.InnerException)
            };
    }
}
