namespace Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling.Tests.TestSupport;

public class TestSqlSupport
{
    public const string ValidSqlText = "SELECT [ProductCategoryID], [Name] FROM [SalesLT].[ProductCategory]";

    public const string InvalidSqlText = "invalid sql";

    public const string ValidSqlScalarQuery = "SELECT count(*) FROM [SalesLT].[ProductCategory]";

    public const string ValidForXmlSqlQuery = "SELECT count(*) FROM [SalesLT].[ProductCategory] FOR XML AUTO, ELEMENTS";

    public const string InvalidConnectionString = "Data Source=tcp:invalidserver.database.windows.net;Initial Catalog=invalidcatalog;User ID=invaliduserid;Password=invalidpassword;";

    private static readonly string SqlDatabaseConnectionStringValue = ConfigurationHelper.GetSetting("SqlDatabaseAdventureWorksLT") ?? RetryConfiguration.GetConfiguration().GetConnectionString("SqlDatabaseAdventureWorksLT");

    public static string SqlDatabaseConnectionString
    {
        get
        {
            if (string.IsNullOrEmpty(SqlDatabaseConnectionStringValue)
                || SqlDatabaseConnectionStringValue.Contains("[INSERT SERVER NAME HERE]")
                || SqlDatabaseConnectionStringValue.Contains("[INSERT USER ID HERE]")
                || SqlDatabaseConnectionStringValue.Contains("[INSERT PASSWORD HERE]"))
            {
                Assert.Inconclusive("Cannot run tests because the Windows Azure SQL Database credentials are not configured in app.config. Please configure it to point to a SQL Database where the AdventureWorks Light (LT) sample database is installed.");
            }

            return SqlDatabaseConnectionStringValue;
        }
    }
}