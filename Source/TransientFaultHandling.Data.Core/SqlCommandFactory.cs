namespace Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling;

using System.Data;

/// <summary>
/// Provides factory methods for instantiating SQL commands.
/// </summary>
public static class SqlCommandFactory
{
    #region Public members
    /// <summary>
    /// Returns the default time-out that will be applied to all SQL commands constructed by this factory class.
    /// </summary>
    public const int DefaultCommandTimeoutSeconds = 60;
    #endregion

    #region Generic SQL commands
    /// <summary>
    /// Creates a generic command of type Stored Procedure and assigns the default command time-out.
    /// </summary>
    /// <param name="connection">The database connection object to be associated with the new command.</param>
    /// <returns>A new SQL command that is initialized with the Stored Procedure command type and initial settings.</returns>
    public static IDbCommand CreateCommand(IDbConnection connection)
    {
        Argument.NotNull(connection, nameof(connection));

        IDbCommand command = connection.CreateCommand();
        command.CommandType = CommandType.StoredProcedure;
        command.CommandTimeout = DefaultCommandTimeoutSeconds;
        return command;
    }

    /// <summary>
    /// Creates a generic command of type Stored Procedure and assigns the specified command text and default command time-out to it.
    /// </summary>
    /// <param name="connection">The database connection object to be associated with the new command.</param>
    /// <param name="commandText">The text of the command to run against the data source.</param>
    /// <returns>A new SQL command that is initialized with the Stored Procedure command type, specified text, and initial settings.</returns>
    [SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities", Justification = "As designed. User must review")]
    public static IDbCommand CreateCommand(IDbConnection connection, string commandText)
    {
        Argument.NotNull(connection, nameof(connection));
        Argument.NotNullOrEmpty(commandText, nameof(commandText));

        IDbCommand command = CreateCommand(connection);
        try
        {
            command.CommandText = commandText;
            return command;
        }
        catch
        {
            command.Dispose();
            throw;
        }
    }
    #endregion

    #region Other system commands
    /// <summary>
    /// Creates a SQL command that is intended to return the connection's context ID, which is useful for tracing purposes.
    /// </summary>
    /// <param name="connection">The database connection object to be associated with the new command.</param>
    /// <returns>A new SQL command that is initialized with the specified connection.</returns>
    [SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities", Justification = "As designed. User must review")]
    public static IDbCommand CreateGetContextInfoCommand(IDbConnection connection)
    {
        Argument.NotNull(connection, nameof(connection));

        IDbCommand command = CreateCommand(connection);
        try
        {
            command.CommandType = CommandType.Text;
            command.CommandText = "SELECT CONVERT(UNIQUEIDENTIFIER, CONVERT(NVARCHAR(36), CONTEXT_INFO()))";
            return command;
        }
        catch
        {
            command.Dispose();
            throw;
        }
    }
    #endregion
}