namespace Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling.Tests.RetryManagerScenarios.given_the_default_retry_manager;

[TestClass]
public class when_getting_default_retry_strategy : Context
{
    private RetryStrategy? retryStrategy;

    protected override void Act()
    {
        this.retryStrategy = this.managerWithAllDefaults.GetRetryStrategy();
    }

    [TestMethod]
    public void then_value_is_matching()
    {
        Assert.AreSame(this.defaultStrategy, this.retryStrategy);
    }
}

[TestClass]
public class when_getting_retry_strategy_by_name : Context
{
    private RetryStrategy? retryStrategy;

    protected override void Act()
    {
        this.retryStrategy = this.managerWithAllDefaults.GetRetryStrategy("other");
    }

    [TestMethod]
    public void then_value_is_matching()
    {
        Assert.AreSame(this.otherStrategy, this.retryStrategy);
    }
}

[TestClass]
public class when_getting_retry_strategy_by_invalid_name : Context
{
    [TestMethod]
    [ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void then_value_is_matching()
    {
        this.managerWithAllDefaults.GetRetryStrategy("invalid");
    }
}

[TestClass]
public class when_getting_retry_policy_with_default_retry_strategy : Context
{
    private RetryPolicy? retryPolicy;

    protected override void Act()
    {
        this.retryPolicy = this.managerWithAllDefaults.GetRetryPolicy(ErrorDetectionStrategy.AlwaysTransient);
    }

    [TestMethod]
    public void then_values_are_matching()
    {
        Assert.AreEqual(this.retryPolicy?.ErrorDetectionStrategy, ErrorDetectionStrategy.AlwaysTransient);
        Assert.AreSame(this.defaultStrategy, this.retryPolicy?.RetryStrategy);
    }
}

[TestClass]
public class when_getting_retry_policy_with_retry_strategy_by_name : Context
{
    private RetryPolicy? retryPolicy;

    protected override void Act()
    {
        this.retryPolicy = this.managerWithAllDefaults.GetRetryPolicy("other", ErrorDetectionStrategy.AlwaysTransient);
    }

    [TestMethod]
    public void then_values_are_matching()
    {
        Assert.AreEqual(this.retryPolicy?.ErrorDetectionStrategy, ErrorDetectionStrategy.AlwaysTransient);
        Assert.AreSame(this.otherStrategy, this.retryPolicy?.RetryStrategy);
    }
}

[TestClass]
public class when_getting_retry_policy_with_unknown_retry_strategy : Context
{
    [TestMethod]
    [ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void then_default_strategy_is_returned()
    {
        this.managerWithAllDefaults.GetRetryPolicy("unknown", ErrorDetectionStrategy.AlwaysTransient);
    }
}

[TestClass]
public class when_getting_default_sql_connection_strategy_when_provided : Context
{
    private RetryPolicy? retryPolicy;

    protected override void Act()
    {
        this.retryPolicy = this.managerWithAllDefaults.GetDefaultSqlConnectionRetryPolicy();
    }

    [TestMethod]
    public void then_values_are_matching()
    {
        Assert.IsInstanceOfType(this.retryPolicy?.ErrorDetectionStrategy, typeof(SqlDatabaseTransientErrorDetectionStrategy));
        Assert.AreSame(this.defaultSqlConnectionStrategy, this.retryPolicy?.RetryStrategy);
    }
}

[TestClass]
public class when_getting_default_sql_connection_strategy_when_not_provided : Context
{
    private RetryPolicy? retryPolicy;

    protected override void Act()
    {
        this.retryPolicy = this.managerWithOnlyDefault.GetDefaultSqlConnectionRetryPolicy();
    }

    [TestMethod]
    public void then_fallback_to_default_retry_strategy()
    {
        Assert.IsInstanceOfType(this.retryPolicy?.ErrorDetectionStrategy, typeof(SqlDatabaseTransientErrorDetectionStrategy));
        Assert.AreSame(this.defaultStrategy, this.retryPolicy?.RetryStrategy);
    }
}

[TestClass]
public class when_getting_default_sql_command_strategy_when_provided : Context
{
    private RetryPolicy? retryPolicy;

    protected override void Act()
    {
        this.retryPolicy = this.managerWithAllDefaults.GetDefaultSqlCommandRetryPolicy();
    }

    [TestMethod]
    public void then_values_are_matching()
    {
        Assert.IsInstanceOfType(this.retryPolicy?.ErrorDetectionStrategy, typeof(SqlDatabaseTransientErrorDetectionStrategy));
        Assert.AreSame(this.defaultSqlCommandStrategy, this.retryPolicy?.RetryStrategy);
    }
}

[TestClass]
public class when_getting_default_sql_command_strategy_when_not_provided : Context
{
    private RetryPolicy retryPolicy;

    protected override void Act()
    {
        this.retryPolicy = this.managerWithOnlyDefault.GetDefaultSqlCommandRetryPolicy();
    }

    [TestMethod]
    public void then_fallback_to_default_retry_strategy()
    {
        Assert.IsInstanceOfType(this.retryPolicy.ErrorDetectionStrategy, typeof(SqlDatabaseTransientErrorDetectionStrategy));
        Assert.AreSame(this.defaultStrategy, this.retryPolicy.RetryStrategy);
    }
}

// TODO
//[TestClass]
//public class when_getting_default_azure_service_bus_strategy_when_provided : Context
//{
//    private RetryPolicy retryPolicy;

//    protected override void Act()
//    {
//        this.retryPolicy = this.managerWithAllDefaults.GetDefaultAzureServiceBusRetryPolicy();
//    }

//    [TestMethod]
//    public void then_values_are_matching()
//    {
//        Assert.IsInstanceOfType(this.retryPolicy.ErrorDetectionStrategy, typeof(ServiceBusTransientErrorDetectionStrategy));
//        Assert.AreSame(this.defaultAzureServiceBusStrategy, this.retryPolicy.RetryStrategy);
//    }
//}

//[TestClass]
//public class when_getting_default_azure_service_bus_strategy_when_not_provided : Context
//{
//    private RetryPolicy retryPolicy;

//    protected override void Act()
//    {
//        this.retryPolicy = this.managerWithOnlyDefault.GetDefaultAzureServiceBusRetryPolicy();
//    }

//    [TestMethod]
//    public void then_fallback_to_default_retry_strategy()
//    {
//        Assert.IsInstanceOfType(this.retryPolicy.ErrorDetectionStrategy, typeof(ServiceBusTransientErrorDetectionStrategy));
//        Assert.AreSame(this.defaultStrategy, this.retryPolicy.RetryStrategy);
//    }
//}

[TestClass]
public class when_getting_default_azure_caching_strategy_when_provided : Context
{
    private RetryPolicy? retryPolicy;

    protected override void Act()
    {
        this.retryPolicy = this.managerWithAllDefaults.GetDefaultCachingRetryPolicy();
    }

    [TestMethod]
    public void then_values_are_matching()
    {
        Assert.IsInstanceOfType(this.retryPolicy?.ErrorDetectionStrategy, typeof(CacheTransientErrorDetectionStrategy));
        Assert.AreSame(this.defaultAzureCachingStrategy, this.retryPolicy?.RetryStrategy);
    }
}

[TestClass]
public class when_getting_default_azure_caching_strategy_when_not_provided : Context
{
    private RetryPolicy? retryPolicy;

    protected override void Act()
    {
        this.retryPolicy = this.managerWithOnlyDefault.GetDefaultCachingRetryPolicy();
    }

    [TestMethod]
    public void then_fallback_to_default_retry_strategy()
    {
        Assert.IsInstanceOfType(this.retryPolicy?.ErrorDetectionStrategy, typeof(CacheTransientErrorDetectionStrategy));
        Assert.AreSame(this.defaultStrategy, this.retryPolicy?.RetryStrategy);
    }
}

// TODO.
//[TestClass]
//public class when_getting_default_azure_storage_strategy_when_provided : Context
//{
//    private RetryPolicy retryPolicy;

//    protected override void Act()
//    {
//        this.retryPolicy = this.managerWithAllDefaults.GetDefaultAzureStorageRetryPolicy();
//    }

//    [TestMethod]
//    public void then_values_are_matching()
//    {
//        Assert.IsInstanceOfType(this.retryPolicy.ErrorDetectionStrategy, typeof(StorageTransientErrorDetectionStrategy));
//        Assert.AreSame(this.defaultAzureStorageStrategy, this.retryPolicy.RetryStrategy);
//    }
//}

//[TestClass]
//public class when_getting_default_azure_storage_strategy_when_not_provided : Context
//{
//    private RetryPolicy retryPolicy;

//    protected override void Act()
//    {
//        this.retryPolicy = this.managerWithOnlyDefault.GetDefaultAzureStorageRetryPolicy();
//    }

//    [TestMethod]
//    public void then_fallback_to_default_retry_strategy()
//    {
//        Assert.IsInstanceOfType(this.retryPolicy.ErrorDetectionStrategy, typeof(StorageTransientErrorDetectionStrategy));
//        Assert.AreSame(this.defaultStrategy, this.retryPolicy.RetryStrategy);
//    }
//}