namespace Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling.Bvt.Tests.Sql;

using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;

[TestClass]
public class TestDatabase
{
    internal static readonly string TransientFaultHandlingTestDatabase = Environment.GetEnvironmentVariable(nameof(TransientFaultHandlingTestDatabase)) 
        ?? RetryConfiguration.GetConfiguration().GetConnectionString(nameof(TransientFaultHandlingTestDatabase));

    private static readonly string TransientFaultHandlingTestServer = Environment.GetEnvironmentVariable(nameof(TransientFaultHandlingTestServer))
        ?? RetryConfiguration.GetConfiguration().GetConnectionString(nameof(TransientFaultHandlingTestServer));

    [AssemblyInitialize]
    public static void InitializeTestDatabase(TestContext context)
    {
        Server server = new(new ServerConnection(new SqlConnection(TransientFaultHandlingTestServer)));
        if (server.Databases.Contains(nameof(TransientFaultHandlingTestDatabase)))
        {
            server.Databases[nameof(TransientFaultHandlingTestDatabase)].Drop();
        }

        Database database = new(server, nameof(TransientFaultHandlingTestDatabase));
        database.Create();
        database.ExecuteNonQuery(Resources.CreateTestDatabaseObjects);
    }
}