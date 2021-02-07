namespace Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling.Tests.ConfigurationScenarios.given_retry_policy_factory
{
    using System.Collections.Generic;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Practices.EnterpriseLibrary.Common.TestSupport.ContextBase;
    using Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling;

    using VisualStudio.TestTools.UnitTesting;

    public abstract class Context : ArrangeActAssert
    {
        protected const string Other = nameof(Other);

        protected const string Default = nameof(Default);

        protected const string DefaultSql = nameof(DefaultSql);

        protected override void Arrange()
        {
            RetryPolicyFactory.SetRetryManager(this.GetSettings().ToRetryManager(), false);
        }

        protected override void Teardown()
        {
            RetryPolicyFactory.SetRetryManager(null, false);
        }

        protected abstract RetryManagerOptions GetSettings();
    }

    [TestClass]
    public class when_getting_existing_default_sql_connection_policy : Context
    {
        private RetryPolicy retryPolicy;

        protected override void Act()
        {
            this.retryPolicy = RetryPolicyFactory.GetDefaultSqlConnectionRetryPolicy();
        }

        protected override RetryManagerOptions GetSettings()
        {
            Dictionary<string, string> dictionary = new ()
            {
                [$"{nameof(RetryStrategy)}:{Other}:{nameof(FixedIntervalOptions.FastFirstRetry)}"] = "true",
                [$"{nameof(RetryStrategy)}:{Other}:{nameof(FixedIntervalOptions.RetryCount)}"] = "5",
                [$"{nameof(RetryStrategy)}:{Other}:{nameof(FixedIntervalOptions.RetryInterval)}"] = "00:00:00.01",

                [$"{nameof(RetryStrategy)}:{Default}:{nameof(FixedIntervalOptions.FastFirstRetry)}"] = "true",
                [$"{nameof(RetryStrategy)}:{Default}:{nameof(FixedIntervalOptions.RetryCount)}"] = "5",
                [$"{nameof(RetryStrategy)}:{Default}:{nameof(FixedIntervalOptions.RetryInterval)}"] = "00:00:00.01",

                [$"{nameof(RetryStrategy)}:{DefaultSql}:{nameof(FixedIntervalOptions.FastFirstRetry)}"] = "true",
                [$"{nameof(RetryStrategy)}:{DefaultSql}:{nameof(FixedIntervalOptions.RetryCount)}"] = "5",
                [$"{nameof(RetryStrategy)}:{DefaultSql}:{nameof(FixedIntervalOptions.RetryInterval)}"] = "00:00:00.01",
            };
            IConfigurationRoot configurationRoot = new ConfigurationBuilder().AddInMemoryCollection(dictionary).Build();
            return new RetryManagerOptions()
            {
                DefaultRetryStrategy = Default,
                DefaultSqlConnectionRetryStrategy = DefaultSql,
                RetryStrategy = configurationRoot.GetSection(nameof(RetryStrategy))
            };
        }

        [TestMethod]
        public void then_get_value()
        {
            Assert.IsNotNull(this.retryPolicy);
            Assert.IsInstanceOfType(this.retryPolicy.ErrorDetectionStrategy, typeof(SqlDatabaseTransientErrorDetectionStrategy));
            Assert.AreEqual(DefaultSql, this.retryPolicy.RetryStrategy.Name);
        }
    }

    [TestClass]
    public class when_getting_non_existing_default_sql_connection_policy : Context
    {
        private RetryPolicy retryPolicy;

        protected override void Act()
        {
            this.retryPolicy = RetryPolicyFactory.GetDefaultSqlConnectionRetryPolicy();
        }

        protected override RetryManagerOptions GetSettings()
        {
            Dictionary<string, string> dictionary = new()
            {
                [$"{nameof(RetryStrategy)}:{Other}:{nameof(FixedIntervalOptions.FastFirstRetry)}"] = "true",
                [$"{nameof(RetryStrategy)}:{Other}:{nameof(FixedIntervalOptions.RetryCount)}"] = "5",
                [$"{nameof(RetryStrategy)}:{Other}:{nameof(FixedIntervalOptions.RetryInterval)}"] = "00:00:00.01",

                [$"{nameof(RetryStrategy)}:{Default}:{nameof(FixedIntervalOptions.FastFirstRetry)}"] = "true",
                [$"{nameof(RetryStrategy)}:{Default}:{nameof(FixedIntervalOptions.RetryCount)}"] = "5",
                [$"{nameof(RetryStrategy)}:{Default}:{nameof(FixedIntervalOptions.RetryInterval)}"] = "00:00:00.01",

                [$"{nameof(RetryStrategy)}:{DefaultSql}:{nameof(FixedIntervalOptions.FastFirstRetry)}"] = "true",
                [$"{nameof(RetryStrategy)}:{DefaultSql}:{nameof(FixedIntervalOptions.RetryCount)}"] = "5",
                [$"{nameof(RetryStrategy)}:{DefaultSql}:{nameof(FixedIntervalOptions.RetryInterval)}"] = "00:00:00.01",
            };
            IConfigurationRoot configurationRoot = new ConfigurationBuilder().AddInMemoryCollection(dictionary).Build();
            return new RetryManagerOptions()
            {
                DefaultRetryStrategy = Default,
                RetryStrategy = configurationRoot.GetSection(nameof(RetryStrategy))
            };
        }

        [TestMethod]
        public void then_get_value()
        {
            Assert.IsNotNull(this.retryPolicy);
            Assert.IsInstanceOfType(this.retryPolicy.ErrorDetectionStrategy, typeof(SqlDatabaseTransientErrorDetectionStrategy));
            Assert.AreEqual(Default, this.retryPolicy.RetryStrategy.Name);
        }
    }

    [TestClass]
    public class when_getting_existing_default_sql_command_policy : Context
    {
        private RetryPolicy retryPolicy;

        protected override void Act()
        {
            this.retryPolicy = RetryPolicyFactory.GetDefaultSqlCommandRetryPolicy();
        }

        protected override RetryManagerOptions GetSettings()
        {
            Dictionary<string, string> dictionary = new()
            {
                [$"{nameof(RetryStrategy)}:{Other}:{nameof(FixedIntervalOptions.FastFirstRetry)}"] = "true",
                [$"{nameof(RetryStrategy)}:{Other}:{nameof(FixedIntervalOptions.RetryCount)}"] = "5",
                [$"{nameof(RetryStrategy)}:{Other}:{nameof(FixedIntervalOptions.RetryInterval)}"] = "00:00:00.01",

                [$"{nameof(RetryStrategy)}:{Default}:{nameof(FixedIntervalOptions.FastFirstRetry)}"] = "true",
                [$"{nameof(RetryStrategy)}:{Default}:{nameof(FixedIntervalOptions.RetryCount)}"] = "5",
                [$"{nameof(RetryStrategy)}:{Default}:{nameof(FixedIntervalOptions.RetryInterval)}"] = "00:00:00.01",

                [$"{nameof(RetryStrategy)}:{DefaultSql}:{nameof(FixedIntervalOptions.FastFirstRetry)}"] = "true",
                [$"{nameof(RetryStrategy)}:{DefaultSql}:{nameof(FixedIntervalOptions.RetryCount)}"] = "5",
                [$"{nameof(RetryStrategy)}:{DefaultSql}:{nameof(FixedIntervalOptions.RetryInterval)}"] = "00:00:00.01",
            };
            IConfigurationRoot configurationRoot = new ConfigurationBuilder().AddInMemoryCollection(dictionary).Build();
            return new RetryManagerOptions()
            {
                DefaultRetryStrategy = Default,
                DefaultSqlCommandRetryStrategy = DefaultSql,
                RetryStrategy = configurationRoot.GetSection(nameof(RetryStrategy))
            };
        }

        [TestMethod]
        public void then_get_value()
        {
            Assert.IsNotNull(this.retryPolicy);
            Assert.IsInstanceOfType(this.retryPolicy.ErrorDetectionStrategy, typeof(SqlDatabaseTransientErrorDetectionStrategy));
            Assert.AreEqual(DefaultSql, this.retryPolicy.RetryStrategy.Name);
        }
    }

    [TestClass]
    public class when_getting_non_existing_default_sql_command_policy : Context
    {
        private RetryPolicy retryPolicy;

        protected override void Act()
        {
            this.retryPolicy = RetryPolicyFactory.GetDefaultSqlCommandRetryPolicy();
        }

        protected override RetryManagerOptions GetSettings()
        {
            Dictionary<string, string> dictionary = new()
            {
                [$"{nameof(RetryStrategy)}:{Other}:{nameof(FixedIntervalOptions.FastFirstRetry)}"] = "true",
                [$"{nameof(RetryStrategy)}:{Other}:{nameof(FixedIntervalOptions.RetryCount)}"] = "5",
                [$"{nameof(RetryStrategy)}:{Other}:{nameof(FixedIntervalOptions.RetryInterval)}"] = "00:00:00.01",

                [$"{nameof(RetryStrategy)}:{Default}:{nameof(FixedIntervalOptions.FastFirstRetry)}"] = "true",
                [$"{nameof(RetryStrategy)}:{Default}:{nameof(FixedIntervalOptions.RetryCount)}"] = "5",
                [$"{nameof(RetryStrategy)}:{Default}:{nameof(FixedIntervalOptions.RetryInterval)}"] = "00:00:00.01",

                [$"{nameof(RetryStrategy)}:{DefaultSql}:{nameof(FixedIntervalOptions.FastFirstRetry)}"] = "true",
                [$"{nameof(RetryStrategy)}:{DefaultSql}:{nameof(FixedIntervalOptions.RetryCount)}"] = "5",
                [$"{nameof(RetryStrategy)}:{DefaultSql}:{nameof(FixedIntervalOptions.RetryInterval)}"] = "00:00:00.01",
            };
            IConfigurationRoot configurationRoot = new ConfigurationBuilder().AddInMemoryCollection(dictionary).Build();
            return new RetryManagerOptions()
            {
                DefaultRetryStrategy = Default,
                RetryStrategy = configurationRoot.GetSection(nameof(RetryStrategy))
            };
        }

        [TestMethod]
        public void then_get_value()
        {
            Assert.IsNotNull(this.retryPolicy);
            Assert.IsInstanceOfType(this.retryPolicy.ErrorDetectionStrategy, typeof(SqlDatabaseTransientErrorDetectionStrategy));
            Assert.AreEqual(Default, this.retryPolicy.RetryStrategy.Name);
        }
    }

    // TODO.
    //[TestClass]
    //public class when_getting_existing_default_azure_service_bus_policy : Context
    //{
    //    private RetryPolicy retryPolicy;

    //    protected override void Act()
    //    {
    //        this.retryPolicy = RetryPolicyFactory.GetDefaultAzureServiceBusRetryPolicy();
    //    }

    //    protected override RetryManagerOptions GetSettings()
    //    {
    //        return new RetryPolicyConfigurationSettings()
    //        {
    //            DefaultRetryStrategy = "default",
    //            DefaultAzureServiceBusRetryStrategy = "defaultAzureServiceBus",
    //            RetryStrategies =
    //            {
    //                new FixedIntervalData("other") { MaxRetryCount = 5, RetryInterval = TimeSpan.FromMilliseconds(10) },
    //                new FixedIntervalData("default") { MaxRetryCount = 5, RetryInterval = TimeSpan.FromMilliseconds(10) },
    //                new FixedIntervalData("defaultAzureServiceBus") { MaxRetryCount = 5, RetryInterval = TimeSpan.FromMilliseconds(10) },
    //            }
    //        };
    //    }

    //    [TestMethod]
    //    public void then_get_value()
    //    {
    //        Assert.IsNotNull(this.retryPolicy);
    //        Assert.IsInstanceOfType(this.retryPolicy.ErrorDetectionStrategy, typeof(ServiceBusTransientErrorDetectionStrategy));
    //        Assert.AreEqual("defaultAzureServiceBus", this.retryPolicy.RetryStrategy.Name);
    //    }
    //}

    //[TestClass]
    //public class when_getting_inexisting_default_azure_service_bus_policy : Context
    //{
    //    private RetryPolicy retryPolicy;

    //    protected override void Act()
    //    {
    //        this.retryPolicy = RetryPolicyFactory.GetDefaultAzureServiceBusRetryPolicy();
    //    }

    //    protected override RetryManagerOptions GetSettings()
    //    {
    //        return new RetryPolicyConfigurationSettings()
    //        {
    //            DefaultRetryStrategy = "default",
    //            RetryStrategies =
    //            {
    //                new FixedIntervalData("other") { MaxRetryCount = 5, RetryInterval = TimeSpan.FromMilliseconds(10) },
    //                new FixedIntervalData("default") { MaxRetryCount = 5, RetryInterval = TimeSpan.FromMilliseconds(10) },
    //                new FixedIntervalData("defaultAzureServiceBus") { MaxRetryCount = 5, RetryInterval = TimeSpan.FromMilliseconds(10) },
    //            }
    //        };
    //    }

    //    [TestMethod]
    //    public void then_get_value()
    //    {
    //        Assert.IsNotNull(this.retryPolicy);
    //        Assert.IsInstanceOfType(this.retryPolicy.ErrorDetectionStrategy, typeof(ServiceBusTransientErrorDetectionStrategy));
    //        Assert.AreEqual("default", this.retryPolicy.RetryStrategy.Name);
    //    }
    //}

    //[TestClass]
    //public class when_getting_existing_default_azure_caching_policy : Context
    //{
    //    private RetryPolicy retryPolicy;

    //    protected override void Act()
    //    {
    //        this.retryPolicy = RetryPolicyFactory.GetDefaultAzureCachingRetryPolicy();
    //    }

    //    protected override RetryManagerOptions GetSettings()
    //    {
    //        return new RetryPolicyConfigurationSettings()
    //        {
    //            DefaultRetryStrategy = "default",
    //            DefaultAzureCachingRetryStrategy = "defaultAzureCaching",
    //            RetryStrategies =
    //            {
    //                new FixedIntervalData("other") { MaxRetryCount = 5, RetryInterval = TimeSpan.FromMilliseconds(10) },
    //                new FixedIntervalData("default") { MaxRetryCount = 5, RetryInterval = TimeSpan.FromMilliseconds(10) },
    //                new FixedIntervalData("defaultAzureCaching") { MaxRetryCount = 5, RetryInterval = TimeSpan.FromMilliseconds(10) },
    //            }
    //        };
    //    }

    //    [TestMethod]
    //    public void then_get_value()
    //    {
    //        Assert.IsNotNull(this.retryPolicy);
    //        Assert.IsInstanceOfType(this.retryPolicy.ErrorDetectionStrategy, typeof(CacheTransientErrorDetectionStrategy));
    //        Assert.AreEqual("defaultAzureCaching", this.retryPolicy.RetryStrategy.Name);
    //    }
    //}

    //[TestClass]
    //public class when_getting_inexisting_default_azure_caching_policy : Context
    //{
    //    private RetryPolicy retryPolicy;

    //    protected override void Act()
    //    {
    //        this.retryPolicy = RetryPolicyFactory.GetDefaultAzureCachingRetryPolicy();
    //    }

    //    protected override RetryManagerOptions GetSettings()
    //    {
    //        return new RetryPolicyConfigurationSettings()
    //        {
    //            DefaultRetryStrategy = "default",
    //            RetryStrategies =
    //            {
    //                new FixedIntervalData("other") { MaxRetryCount = 5, RetryInterval = TimeSpan.FromMilliseconds(10) },
    //                new FixedIntervalData("default") { MaxRetryCount = 5, RetryInterval = TimeSpan.FromMilliseconds(10) },
    //                new FixedIntervalData("defaultAzureCaching") { MaxRetryCount = 5, RetryInterval = TimeSpan.FromMilliseconds(10) },
    //            }
    //        };
    //    }

    //    [TestMethod]
    //    public void then_get_value()
    //    {
    //        Assert.IsNotNull(this.retryPolicy);
    //        Assert.IsInstanceOfType(this.retryPolicy.ErrorDetectionStrategy, typeof(CacheTransientErrorDetectionStrategy));
    //        Assert.AreEqual("default", this.retryPolicy.RetryStrategy.Name);
    //    }
    //}

    //[TestClass]
    //public class when_getting_existing_default_azure_storage_policy : Context
    //{
    //    private RetryPolicy retryPolicy;

    //    protected override void Act()
    //    {
    //        this.retryPolicy = RetryPolicyFactory.GetDefaultAzureStorageRetryPolicy();
    //    }

    //    protected override RetryManagerOptions GetSettings()
    //    {
    //        return new RetryPolicyConfigurationSettings()
    //        {
    //            DefaultRetryStrategy = "default",
    //            DefaultAzureStorageRetryStrategy = "defaultAzureStorage",
    //            RetryStrategies =
    //            {
    //                new FixedIntervalData("other") { MaxRetryCount = 5, RetryInterval = TimeSpan.FromMilliseconds(10) },
    //                new FixedIntervalData("default") { MaxRetryCount = 5, RetryInterval = TimeSpan.FromMilliseconds(10) },
    //                new FixedIntervalData("defaultAzureStorage") { MaxRetryCount = 5, RetryInterval = TimeSpan.FromMilliseconds(10) },
    //            }
    //        };
    //    }

    //    [TestMethod]
    //    public void then_get_value()
    //    {
    //        Assert.IsNotNull(this.retryPolicy);
    //        Assert.IsInstanceOfType(this.retryPolicy.ErrorDetectionStrategy, typeof(StorageTransientErrorDetectionStrategy));
    //        Assert.AreEqual("defaultAzureStorage", this.retryPolicy.RetryStrategy.Name);
    //    }
    //}

    //[TestClass]
    //public class when_getting_inexisting_default_azure_storage_policy : Context
    //{
    //    private RetryPolicy retryPolicy;

    //    protected override void Act()
    //    {
    //        this.retryPolicy = RetryPolicyFactory.GetDefaultAzureStorageRetryPolicy();
    //    }

    //    protected override RetryManagerOptions GetSettings()
    //    {
    //        return new RetryPolicyConfigurationSettings()
    //        {
    //            DefaultRetryStrategy = "default",
    //            RetryStrategies =
    //            {
    //                new FixedIntervalData("other") { MaxRetryCount = 5, RetryInterval = TimeSpan.FromMilliseconds(10) },
    //                new FixedIntervalData("default") { MaxRetryCount = 5, RetryInterval = TimeSpan.FromMilliseconds(10) },
    //                new FixedIntervalData("defaultAzureStorage") { MaxRetryCount = 5, RetryInterval = TimeSpan.FromMilliseconds(10) },
    //            }
    //        };
    //    }

    //    [TestMethod]
    //    public void then_get_value()
    //    {
    //        Assert.IsNotNull(this.retryPolicy);
    //        Assert.IsInstanceOfType(this.retryPolicy.ErrorDetectionStrategy, typeof(StorageTransientErrorDetectionStrategy));
    //        Assert.AreEqual("default", this.retryPolicy.RetryStrategy.Name);
    //    }
    //}

    [TestClass]
    public class when_getting_retry_policy_with_default_strategy : Context
    {
        private RetryPolicy retryPolicy;

        protected override void Act()
        {
            this.retryPolicy = RetryPolicyFactory.GetRetryPolicy(ErrorDetectionStrategy.AlwaysTransient);
        }

        protected override RetryManagerOptions GetSettings()
        {
            Dictionary<string, string> dictionary = new()
            {
                [$"{nameof(RetryStrategy)}:{Default}:{nameof(FixedIntervalOptions.FastFirstRetry)}"] = "true",
                [$"{nameof(RetryStrategy)}:{Default}:{nameof(FixedIntervalOptions.RetryCount)}"] = "5",
                [$"{nameof(RetryStrategy)}:{Default}:{nameof(FixedIntervalOptions.RetryInterval)}"] = "00:00:00.01"
            };
            IConfigurationRoot configurationRoot = new ConfigurationBuilder().AddInMemoryCollection(dictionary).Build();
            return new RetryManagerOptions()
            {
                DefaultRetryStrategy = Default,
                RetryStrategy = configurationRoot.GetSection(nameof(RetryStrategy))
            };
        }

        [TestMethod]
        public void then_get_value()
        {
            Assert.IsNotNull(this.retryPolicy);
            Assert.AreEqual(this.retryPolicy.ErrorDetectionStrategy, ErrorDetectionStrategy.AlwaysTransient);
            Assert.AreEqual(Default, this.retryPolicy.RetryStrategy.Name);
        }
    }

    [TestClass]
    public class when_getting_retry_policy_with_named_strategy : Context
    {
        private RetryPolicy retryPolicy;

        protected override void Act()
        {
            this.retryPolicy = RetryPolicyFactory.GetRetryPolicy(Other, ErrorDetectionStrategy.AlwaysTransient);
        }

        protected override RetryManagerOptions GetSettings()
        {
            Dictionary<string, string> dictionary = new()
            {
                [$"{nameof(RetryStrategy)}:{Other}:{nameof(FixedIntervalOptions.FastFirstRetry)}"] = "true",
                [$"{nameof(RetryStrategy)}:{Other}:{nameof(FixedIntervalOptions.RetryCount)}"] = "5",
                [$"{nameof(RetryStrategy)}:{Other}:{nameof(FixedIntervalOptions.RetryInterval)}"] = "00:00:00.01"
            };
            IConfigurationRoot configurationRoot = new ConfigurationBuilder().AddInMemoryCollection(dictionary).Build();
            return new RetryManagerOptions()
            {
                DefaultRetryStrategy = Other,
                RetryStrategy = configurationRoot.GetSection(nameof(RetryStrategy))
            };
        }

        [TestMethod]
        public void then_get_value()
        {
            Assert.IsNotNull(this.retryPolicy);
            Assert.AreEqual(this.retryPolicy.ErrorDetectionStrategy, ErrorDetectionStrategy.AlwaysTransient);
            Assert.AreEqual(Other, this.retryPolicy.RetryStrategy.Name);
        }
    }
}