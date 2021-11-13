namespace Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling;

using Microsoft.Data.SqlClient;

/// <summary>
/// Provides a set of extension methods that add retry capabilities to the standard <see cref="System.Data.SqlClient.SqlConnection"/> implementation.
/// </summary>
public static partial class SqlConnectionExtensions
{
    /// <summary>
    /// Opens a database connection with the connection settings specified in the ConnectionString property of the connection object.
    /// Uses the default retry policy when opening the connection.
    /// </summary>
    /// <param name="connection">The connection object that is required for the extension method declaration.</param>
    public static void OpenWithRetry(this SqlConnection connection) =>
        OpenWithRetry(connection, RetryManager.Instance.GetDefaultSqlConnectionRetryPolicy());

    /// <summary>
    /// Opens a database connection with the connection settings specified in the ConnectionString property of the connection object.
    /// Uses the specified retry policy when opening the connection.
    /// </summary>
    /// <param name="connection">The connection object that is required for the extension method declaration.</param>
    /// <param name="retryPolicy">The retry policy that defines whether to retry a request if the connection fails.</param>
    public static void OpenWithRetry(this SqlConnection connection, RetryPolicy? retryPolicy)
    {
        Argument.NotNull(connection, nameof(connection));

        (retryPolicy ?? RetryPolicy.NoRetry).ExecuteAction(connection.Open);
    }
}