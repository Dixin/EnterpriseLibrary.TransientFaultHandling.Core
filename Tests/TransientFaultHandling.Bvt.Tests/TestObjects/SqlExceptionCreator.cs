namespace Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling.Bvt.Tests.TestObjects;

public class SqlExceptionCreator
{
    public static SqlException CreateSqlException(string errorMessage, int errorNumber)
    {
        SqlErrorCollection collection = Construct<SqlErrorCollection>();
        SqlError error = GenerateFakeSqlError(errorNumber, errorMessage);

        typeof(SqlErrorCollection)
            .GetMethod("Add", BindingFlags.NonPublic | BindingFlags.Instance)
            ?.Invoke(collection, [error]);

        MethodInfo createException = typeof(SqlException)
            .GetMethod(
                "CreateException",
                BindingFlags.NonPublic | BindingFlags.Static,
                null,
                [typeof(SqlErrorCollection), typeof(string)],
                null)!;

        SqlException e = (SqlException)(createException.Invoke(
            null,
            [collection, "7.0.0"]) ?? throw new InvalidOperationException("Failed to create SqlException."));

        return e;
    }

    public static SqlError GenerateFakeSqlError(int errorNumber, string errorMessage = "") =>
        (SqlError)(Activator.CreateInstance(
            typeof(SqlError),
            BindingFlags.NonPublic | BindingFlags.Instance,
            null,
            [
                errorNumber, // int infoNumber
                default(byte), // byte errorState
                default(byte), // byte errorClass
                string.Empty, // string server
                errorMessage, // string errorMessage
                string.Empty, // string procedure
                0, // int lineNumber
                null// Exception exception
            ],
            null) ?? throw new InvalidOperationException("Failed to create SqlError"));

    private static T Construct<T>(params object[] p)
    {
        return (T)typeof(T).GetConstructors(BindingFlags.NonPublic | BindingFlags.Instance)[0].Invoke(p);
    }
}