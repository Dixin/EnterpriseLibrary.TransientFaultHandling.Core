namespace Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling.Tests;

[TestClass]
public class SqlConnectionExtensionsTest
{
    [TestInitialize]
    public void Setup() => RetryPolicyFactory.CreateDefault();

    [TestCleanup]
    public void Cleanup() => RetryPolicyFactory.SetRetryManager(null, false);

    [Description("F5.1.1")]
    [Priority(1)]
    [TestMethod]
    public void TestSqlConnectionExtensions()
    {
        using SqlConnection connection = new(TestSqlSupport.SqlDatabaseConnectionString);
        using SqlCommand command = new("SELECT [ProductCategoryID], [Name] FROM [SalesLT].[ProductCategory]", connection);
        connection.OpenWithRetry();

        using IDataReader reader = command.ExecuteReader();
        while (reader.Read())
        {
            int id = reader.GetInt32(0);
            string name = reader.GetString(1);

            Trace.WriteLine($"{id}: {name}");
        }

        reader.Close();

        connection.Close();
    }
}