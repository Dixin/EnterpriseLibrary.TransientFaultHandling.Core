namespace Microsoft.Practices.EnterpriseLibrary.WindowsAzure.TransientFaultHandling.SqlAzure;

using Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling;

/// <summary>
/// This class is obsolete. Please use <see cref="SqlDatabaseTransientErrorDetectionStrategy"/>.
/// Provides the transient error detection logic for transient faults that are specific to SQL Database.
/// </summary>
[Obsolete("Use SqlDatabaseTransientErrorDetectionStrategy for Microsoft.Data.SqlClient.", false)]
public class SqlAzureTransientErrorDetectionStrategyLegacy : ITransientErrorDetectionStrategy
{
    private readonly SqlDatabaseTransientErrorDetectionStrategyLegacy inner = new();

    /// <summary>
    /// Determines whether the specified exception represents a transient failure that can be compensated by a retry.
    /// </summary>
    /// <param name="ex">The exception object to be verified.</param>
    /// <returns>true if the specified exception is considered transient; otherwise, false.</returns>
    public bool IsTransient(Exception ex) => this.inner.IsTransient(ex);
}