namespace Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling.Tests.RetryManagerScenarios;

public class Context : ArrangeActAssert
{
    protected RetryStrategy defaultStrategy;
    protected RetryStrategy defaultSqlConnectionStrategy;
    protected RetryStrategy defaultSqlCommandStrategy;
    protected RetryStrategy defaultAzureServiceBusStrategy;
    protected RetryStrategy defaultAzureCachingStrategy;
    protected RetryStrategy defaultAzureStorageStrategy;
    protected RetryStrategy otherStrategy;

    protected RetryManager managerWithAllDefaults;
    protected RetryManager managerWithOnlyDefault;

    protected override void Arrange()
    {
        this.defaultStrategy = new FixedInterval("default", 5, TimeSpan.FromMilliseconds(10));
        this.otherStrategy = new Incremental("other", 5, TimeSpan.FromMilliseconds(10), TimeSpan.FromMilliseconds(10));
        this.defaultSqlConnectionStrategy = new FixedInterval("defaultSqlConnection", 5, TimeSpan.FromMilliseconds(10));
        this.defaultSqlCommandStrategy = new FixedInterval("defaultSqlCommand", 5, TimeSpan.FromMilliseconds(10));
        this.defaultAzureServiceBusStrategy = new FixedInterval("defaultAzureServiceBusStrategy", 5, TimeSpan.FromMilliseconds(10));
        this.defaultAzureCachingStrategy = new FixedInterval("defaultAzureCachingStrategy", 5, TimeSpan.FromMilliseconds(10));
        this.defaultAzureStorageStrategy = new FixedInterval("defaultAzureStorageStrategy", 5, TimeSpan.FromMilliseconds(10));

        this.managerWithAllDefaults = new RetryManager(
            new[]
            {
                this.defaultStrategy,
                this.defaultSqlConnectionStrategy,
                this.defaultSqlCommandStrategy,
                this.otherStrategy,
                this.defaultAzureServiceBusStrategy,
                this.defaultAzureCachingStrategy, this.defaultAzureStorageStrategy
            },
            "default",
            new Dictionary<string, string>
            {
                { "SQL", "defaultSqlCommand" },
                { "SQLConnection", "defaultSqlConnection" },
                { "ServiceBus", "defaultAzureServiceBusStrategy" },
                { "Caching", "defaultAzureCachingStrategy" },
                { "WindowsAzure.Storage", "defaultAzureStorageStrategy" },
            });

        this.managerWithOnlyDefault = new RetryManager(
            new[]
            {
                this.defaultStrategy,
                this.defaultSqlConnectionStrategy,
                this.defaultSqlCommandStrategy,
                this.otherStrategy,
                this.defaultAzureServiceBusStrategy,
                this.defaultAzureCachingStrategy, this.defaultAzureStorageStrategy
            },
            "default");
    }
}
