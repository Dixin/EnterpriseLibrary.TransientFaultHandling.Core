namespace Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling.Bvt.Tests.Sql;

[TestClass]
public class SqlExceptionTests
{
    private const string CustomErrorMessage = nameof(CustomErrorMessage);

    [TestMethod]
    public async Task CanHandleSqlExceptionAsync()
    {
        RetryPolicy<SqlDatabaseTransientErrorDetectionStrategy> retryPolicy = new (new Incremental(RetryStrategy.DefaultClientRetryCount, RetryStrategy.DefaultRetryInterval, RetryStrategy.DefaultRetryIncrement));

        try
        {
            await retryPolicy.ExecuteAsync(async () =>
            {
                await using SqlConnection connection = new (TestDatabase.TransientFaultHandlingTestDatabase);
                await connection.OpenAsync();
                await using SqlCommand command = connection.CreateCommand();
                command.CommandText = $"RAISERROR('{CustomErrorMessage}', 16, 1)";
                await command.ExecuteNonQueryAsync();
                Assert.Fail();
            });
        }
        catch (Exception ex)
        {
            if (ex is SqlException { Message: CustomErrorMessage })
            {
                return;
            }

            Assert.Fail();
        }

        Assert.Fail();
    }

    [TestMethod]
    public void CanHandleSqlException()
    {
        RetryPolicy<SqlDatabaseTransientErrorDetectionStrategy> retryPolicy = new (new Incremental(RetryStrategy.DefaultClientRetryCount, RetryStrategy.DefaultRetryInterval, RetryStrategy.DefaultRetryIncrement));

        try
        {
            retryPolicy.ExecuteAction(() =>
            {
                using SqlConnection connection = new (TestDatabase.TransientFaultHandlingTestDatabase);
                connection.Open();
                using SqlCommand command = connection.CreateCommand();
                command.CommandText = $"RAISERROR('{CustomErrorMessage}', 16, 1)";
                command.ExecuteNonQuery();
                Assert.Fail();
            });
        }
        catch (Exception ex)
        {
            if (ex is SqlException { Message: CustomErrorMessage })
            {
                return;
            }

            Assert.Fail();
        }

        Assert.Fail();
    }
}