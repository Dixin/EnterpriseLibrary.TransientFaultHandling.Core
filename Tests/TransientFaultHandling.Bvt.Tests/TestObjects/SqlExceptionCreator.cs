namespace Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling.Bvt.Tests.TestObjects
{
    using System;
    using System.Reflection;
    using Microsoft.Data.SqlClient;
    public class SqlExceptionCreator
    {
        public static SqlException CreateSqlException(string errorMessage, int errorNumber)
        {
            SqlErrorCollection collection = Construct<SqlErrorCollection>();
            SqlError error = GenerateFakeSqlError(errorNumber, errorMessage);

            typeof(SqlErrorCollection)
                .GetMethod("Add", BindingFlags.NonPublic | BindingFlags.Instance)
                ?.Invoke(collection, new object[] { error });

            MethodInfo? createException = typeof(SqlException)
                .GetMethod("CreateException", BindingFlags.NonPublic | BindingFlags.Static, null, new Type[] { typeof(SqlErrorCollection), typeof(string) }, null);

            SqlException e = createException?.Invoke(null, new object[] { collection, "7.0.0" }) as SqlException;

            return e;
        }

        public static SqlError GenerateFakeSqlError(int errorNumber, string errorMessage = "") =>
            (SqlError)(Activator.CreateInstance(
                typeof(SqlError),
                BindingFlags.NonPublic | BindingFlags.Instance,
                null,
                new object?[]
                {
                    errorNumber, // int infoNumber
                    default(byte), // byte errorState
                    default(byte), // byte errorClass
                    string.Empty, // string server
                    errorMessage, // string errorMessage
                    string.Empty, // string procedure
                    0, // int lineNumber
                    null// Exception exception
                },
                null) ?? throw new InvalidOperationException("Failed to create SqlError"));

        private static T Construct<T>(params object[] p)
        {
            return (T)typeof(T).GetConstructors(BindingFlags.NonPublic | BindingFlags.Instance)[0].Invoke(p);
        }
    }
}
