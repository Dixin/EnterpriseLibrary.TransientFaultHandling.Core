namespace Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling.Tests;

internal static class FakeSqlExceptionGenerator
{
    public static SqlException[] GenerateFakeSqlExceptions(params int[] errorCodes)
    {
        SqlException[] exceptions = new SqlException[errorCodes.Length];

        for (int i = 0; i < errorCodes.Length; i++)
        {
            exceptions[i] = GenerateFakeSqlException(errorCodes[i]);
        }

        return exceptions;
    }

    public static SqlException GenerateFakeSqlException(int errorCode, string errorMessage = "")
    {
        SqlError sqlError = GenerateFakeSqlError(errorCode, errorMessage);
        SqlErrorCollection collection = GenerateFakeSqlErrorCollection(sqlError);

        return (SqlException)(Activator.CreateInstance(
            typeof(SqlException), 
            BindingFlags.NonPublic | BindingFlags.Instance, 
            null, 
            new object?[] { errorMessage, collection, null, Guid.Empty }, 
            null) ?? throw new InvalidOperationException("Failed to create SqlException."));
    }

    private static SqlErrorCollection GenerateFakeSqlErrorCollection(params SqlError[] errors)
    {
        Type type = typeof(SqlErrorCollection);

        SqlErrorCollection collection = (SqlErrorCollection)(Activator.CreateInstance(type, true) ?? throw new InvalidOperationException("Failed to create SqlErrorCollection."));

        MethodInfo method = type.GetMethod("Add", BindingFlags.NonPublic | BindingFlags.Instance)!;
        foreach (SqlError error in errors)
        {
            method?.Invoke(collection, new object[] { error });
        }

        return collection;
    }

    public static SqlError GenerateFakeSqlError(int errorCode, string errorMessage = "") =>
        (SqlError)(Activator.CreateInstance(
            typeof(SqlError),
            BindingFlags.NonPublic | BindingFlags.Instance,
            null,
            new object?[]
            {
                errorCode, // int infoNumber
                default(byte), // byte errorState
                default(byte), // byte errorClass
                string.Empty, // string server
                errorMessage, // string errorMessage
                string.Empty, // string procedure
                0, // int lineNumber
                null// Exception exception
            },
            null) ?? throw new InvalidOperationException("Failed to create SqlError"));
}