namespace Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling;

using System.Data;
using System.Data.SqlClient;
using System.Xml;
using Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling.Properties;

/// <summary>
/// Provides a set of extension methods that add retry capabilities to the standard System.Data.SqlClient.SqlCommand implementation.
/// </summary>
public static partial class SqlCommandExtensions
{
    #region ExecuteNonQueryWithRetry method implementations

    /// <summary>
    /// Executes a Transact-SQL statement against the connection and returns the number of rows affected. Uses the default retry policy when executing the command.
    /// </summary>
    /// <param name="command">The command object that is required for the extension method declaration.</param>
    /// <returns>The number of rows affected.</returns>
    [Obsolete("Use ExecuteNonQueryWithRetry for Microsoft.Data.SqlClient.SqlCommand in Microsoft.Data.SqlClient.")]
    public static int ExecuteNonQueryWithRetry(this SqlCommand command) =>
        ExecuteNonQueryWithRetry(command, RetryManager.Instance.GetDefaultSqlCommandRetryPolicy());

    /// <summary>
    /// Executes a Transact-SQL statement against the connection and returns the number of rows affected. Uses the specified retry policies when executing the command
    /// and establishing a connection.
    /// </summary>
    /// <param name="command">The command object that is required for the extension method declaration.</param>
    /// <param name="cmdRetryPolicy">The command retry policy that determines whether to retry a command if it fails while executing.</param>
    /// <param name="conRetryPolicy">The connection retry policy that determines whether to re-establish a connection if it drops while executing the command.</param>
    /// <returns>The number of rows affected.</returns>
    [Obsolete("Use ExecuteNonQueryWithRetry for Microsoft.Data.SqlClient.SqlCommand in Microsoft.Data.SqlClient.")]
    public static int ExecuteNonQueryWithRetry(this SqlCommand command, RetryPolicy? cmdRetryPolicy, RetryPolicy? conRetryPolicy = null)
    {
        command.ThrowIfNull().ConnectionNotNull();

        // Check if retry policy was specified, if not, use the default retry policy.
        return (cmdRetryPolicy ?? RetryPolicy.NoRetry).ExecuteAction(() =>
        {
            bool hasOpenConnection = EnsureValidConnection(command, conRetryPolicy);

            try
            {
                return command.ExecuteNonQuery();
            }
            finally
            {
                if (hasOpenConnection && command.Connection is not null && command.Connection.State == ConnectionState.Open)
                {
                    command.Connection.Close();
                }
            }
        });
    }

    #endregion

    #region ExecuteReaderWithRetry method implementations

    /// <summary>
    /// Sends the specified command to the connection and builds a SqlDataReader object that contains the results.
    /// Uses the default retry policy when executing the command.
    /// </summary>
    /// <param name="command">The command object that is required for the extension method declaration.</param>
    /// <returns>A System.Data.SqlClient.SqlDataReader object.</returns>
    [Obsolete("Use ExecuteReaderWithRetry for Microsoft.Data.SqlClient.SqlCommand in Microsoft.Data.SqlClient.")]
    public static SqlDataReader ExecuteReaderWithRetry(this SqlCommand command) =>
        ExecuteReaderWithRetry(command, RetryManager.Instance.GetDefaultSqlCommandRetryPolicy());

    /// <summary>
    /// Sends the specified command to the connection and builds a SqlDataReader object that contains the results.
    /// Uses the specified retry policies when executing the command and
    /// establishing a connection.
    /// </summary>
    /// <param name="command">The command object that is required for the extension method declaration.</param>
    /// <param name="cmdRetryPolicy">The command retry policy that determines whether to retry a command if it fails while executing.</param>
    /// <param name="conRetryPolicy">The connection retry policy that determines whether to re-establish a connection if it drops while executing the command.</param>
    /// <returns>A System.Data.SqlClient.SqlDataReader object.</returns>
    [Obsolete("Use ExecuteReaderWithRetry for Microsoft.Data.SqlClient.SqlCommand in Microsoft.Data.SqlClient.")]
    public static SqlDataReader ExecuteReaderWithRetry(this SqlCommand command, RetryPolicy cmdRetryPolicy, RetryPolicy? conRetryPolicy = null)
    {
        return ExecuteReaderWithRetry(command.ThrowIfNull().ConnectionNotNull(), CommandBehavior.Default, cmdRetryPolicy, conRetryPolicy);
    }

    /// <summary>
    /// Sends the specified command to the connection and builds a SqlDataReader object by using the specified 
    /// command behavior. Uses the default retry policy when executing the command.
    /// </summary>
    /// <param name="command">The command object that is required for the extension method declaration.</param>
    /// <param name="behavior">One of the enumeration values that specifies the command behavior.</param>
    /// <returns>A System.Data.SqlClient.SqlDataReader object.</returns>
    [Obsolete("Use ExecuteReaderWithRetry for Microsoft.Data.SqlClient.SqlCommand in Microsoft.Data.SqlClient.")]
    public static SqlDataReader ExecuteReaderWithRetry(this SqlCommand command, CommandBehavior behavior) =>
        ExecuteReaderWithRetry(command, behavior, RetryManager.Instance.GetDefaultSqlCommandRetryPolicy());

    /// <summary>
    /// Sends the specified command to the connection and builds a SqlDataReader object by using the specified
    /// command behavior. Uses the specified retry policies when executing the command
    /// and establishing a connection.
    /// </summary>
    /// <param name="command">The command object that is required for the extension method declaration.</param>
    /// <param name="behavior">One of the enumeration values that specifies the command behavior.</param>
    /// <param name="cmdRetryPolicy">The command retry policy that determines whether to retry a command if it fails while executing.</param>
    /// <param name="conRetryPolicy">The connection retry policy that determines whether to re-establish a connection if it drops while executing the command.</param>
    /// <returns>A System.Data.SqlClient.SqlDataReader object.</returns>
    [Obsolete("Use ExecuteReaderWithRetry for Microsoft.Data.SqlClient.SqlCommand in Microsoft.Data.SqlClient.")]
    public static SqlDataReader ExecuteReaderWithRetry(this SqlCommand command, CommandBehavior behavior, RetryPolicy? cmdRetryPolicy, RetryPolicy? conRetryPolicy = null)
    {
        command.ThrowIfNull().ConnectionNotNull();

        // Check if retry policy was specified, if not, use the default retry policy.
        return (cmdRetryPolicy ?? RetryPolicy.NoRetry).ExecuteAction(() =>
        {
            bool hasOpenConnection = EnsureValidConnection(command, conRetryPolicy);

            try
            {
                return command.ExecuteReader(behavior);
            }
            catch (Exception)
            {
                if (hasOpenConnection && command.Connection?.State == ConnectionState.Open)
                {
                    command.Connection.Close();
                }

                throw;
            }
        });
    }

    #endregion

    #region ExecuteScalarWithRetry method implementations

    /// <summary>
    /// Executes the query, and returns the first column of the first row in the result set returned by the query. Additional columns or rows are ignored.
    /// Uses the default retry policy when executing the command.
    /// </summary>
    /// <param name="command">The command object that is required for the extension method declaration.</param>
    /// <returns> The first column of the first row in the result set, or a null reference if the result set is empty. Returns a maximum of 2033 characters.</returns>
    [Obsolete("Use ExecuteScalarWithRetry for Microsoft.Data.SqlClient.SqlCommand in Microsoft.Data.SqlClient.")]
    public static object ExecuteScalarWithRetry(this SqlCommand command) =>
        ExecuteScalarWithRetry(command, RetryManager.Instance.GetDefaultSqlCommandRetryPolicy());

    /// <summary>
    /// Executes the query, and returns the first column of the first row in the result set returned by the query. Additional columns or rows are ignored.
    /// Uses the specified retry policies when executing the command and establishing a connection.
    /// </summary>
    /// <param name="command">The command object that is required for the extension method declaration.</param>
    /// <param name="cmdRetryPolicy">The command retry policy that determines whether to retry a command if it fails while executing.</param>
    /// <param name="conRetryPolicy">The connection retry policy that determines whether to re-establish a connection if it drops while executing the command.</param>
    /// <returns> The first column of the first row in the result set, or a null reference if the result set is empty. Returns a maximum of 2033 characters.</returns>
    [Obsolete("Use ExecuteScalarWithRetry for Microsoft.Data.SqlClient.SqlCommand in Microsoft.Data.SqlClient.")]
    public static object ExecuteScalarWithRetry(this SqlCommand command, RetryPolicy? cmdRetryPolicy, RetryPolicy? conRetryPolicy = null)
    {
        command.ThrowIfNull().ConnectionNotNull();

        // Check if retry policy was specified, if not, use the default retry policy.
        return (cmdRetryPolicy ?? RetryPolicy.NoRetry).ExecuteAction(() =>
        {
            bool hasOpenConnection = EnsureValidConnection(command, conRetryPolicy);

            try
            {
                return command.ExecuteScalar();
            }
            finally
            {
                if (hasOpenConnection && command.Connection is not null && command.Connection.State == ConnectionState.Open)
                {
                    command.Connection.Close();
                }
            }
        });
    }

    #endregion

    #region ExecuteXmlReaderWithRetry method implementations

    /// <summary>
    /// Sends the specified command to the connection and builds an XmlReader object that contains the results.
    /// Uses the default retry policy when executing the command.
    /// </summary>
    /// <param name="command">The command object that is required for the extension method declaration.</param>
    /// <returns>An System.Xml.XmlReader object.</returns>
    [Obsolete("Use ExecuteXmlReaderWithRetry for Microsoft.Data.SqlClient.SqlCommand in Microsoft.Data.SqlClient.")]
    public static XmlReader ExecuteXmlReaderWithRetry(this SqlCommand command) =>
        ExecuteXmlReaderWithRetry(command, RetryManager.Instance.GetDefaultSqlCommandRetryPolicy());
    /// <summary>
    /// Sends the specified command to the connection and builds an XmlReader object that contains the results.
    /// Uses the specified retry policies when executing the command and establishing a connection.
    /// </summary>
    /// <param name="command">The command object that is required for the extension method declaration.</param>
    /// <param name="cmdRetryPolicy">The command retry policy that determines whether to retry a command if it fails while executing.</param>
    /// <param name="conRetryPolicy">The connection retry policy that determines whether to re-establish a connection if it drops while executing the command.</param>
    /// <returns>An System.Xml.XmlReader object.</returns>
    [Obsolete("Use ExecuteXmlReaderWithRetry for Microsoft.Data.SqlClient.SqlCommand in Microsoft.Data.SqlClient.")]
    public static XmlReader ExecuteXmlReaderWithRetry(this SqlCommand command, RetryPolicy? cmdRetryPolicy, RetryPolicy? conRetryPolicy = null)
    {
        command.ThrowIfNull().ConnectionNotNull();

        // Check if retry policy was specified, if not, use the default retry policy.
        return (cmdRetryPolicy ?? RetryPolicy.NoRetry).ExecuteAction(() =>
        {
            bool hasOpenConnection = EnsureValidConnection(command, conRetryPolicy);

            try
            {
                return command.ExecuteXmlReader();
            }
            catch (Exception)
            {
                if (hasOpenConnection && command.Connection?.State == ConnectionState.Open)
                {
                    command.Connection.Close();
                }

                throw;
            }
        });
    }

    #endregion

    private static SqlCommand ConnectionNotNull(this SqlCommand command)
    {
        if (command.Connection is null)
        {
            throw new InvalidOperationException(Resources.ConnectionHasNotBeenInitialized);
        }

        return command;
    }

    private static bool EnsureValidConnection(SqlCommand? command, RetryPolicy? retryPolicy)
    {
        if (command is not null)
        {
            // Verify whether or not the connection is valid and is open. This code may be retried therefore
            // it is important to ensure that a connection is re-established should it have previously failed.
            if (command.ConnectionNotNull().Connection.State != ConnectionState.Open)
            {
                // Attempt to open the connection using the retry policy that matches the policy for SQL commands.
#pragma warning disable CS0618 // Type or member is obsolete
                command.Connection.OpenWithRetry(retryPolicy);
#pragma warning restore CS0618 // Type or member is obsolete

                return true;
            }
        }

        return false;
    }
}