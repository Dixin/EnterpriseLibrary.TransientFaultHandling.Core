namespace Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling.Tests
{
    using System;
    using System.Reflection;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ConfigurationTests
    {
        [TestInitialize]
        public void Setup()
        {
            RetryPolicyFactory.CreateDefault();
        }

        [TestCleanup]
        public void Cleanup()
        {
            RetryPolicyFactory.SetRetryManager(null, false);
        }

        [Description("F3.1.1; F3.2.3")]
        [Priority(1)]
        [TestMethod]
        [Ignore]    // REVIEW = Negative retry counts are not allowed by configuration.
        public void NegativeRetryCount()
        {
            RetryPolicy retryPolicy = RetryPolicyFactory.GetRetryPolicy<SqlDatabaseTransientErrorDetectionStrategy>(retryStrategyName: "NegativeRetryCount");
            int execCount = 0;
            double totalDuration = 0;

            retryPolicy.Retrying += (sender, args) => totalDuration += args.Delay.TotalMilliseconds;

            try
            {
                retryPolicy.ExecuteAction(() =>
                {
                    execCount++;
                    throw new TimeoutException("Forced Exception");
                });
            }
            catch (TimeoutException ex)
            {
                Assert.AreEqual("Forced Exception", ex.Message);
            }

            Assert.AreEqual(1, execCount, "The action was not executed the expected amount of times");
            Assert.AreEqual(0, totalDuration, "Unexpected duration of retry block");
        }

        [Description("F3.1.2")]
        [Priority(1)]
        [TestMethod]
        [Ignore]    // REVIEW
        public void ZeroRetryCount()
        {
            RetryPolicy retryPolicy = RetryPolicyFactory.GetRetryPolicy<SqlDatabaseTransientErrorDetectionStrategy>(retryStrategyName: "ZeroRetryCount");
            int execCount = 0;
            double totalDuration = 0;

            retryPolicy.Retrying += (sender, args) => totalDuration += args.Delay.TotalMilliseconds;

            try
            {
                retryPolicy.ExecuteAction(() =>
                {
                    execCount++;
                    throw new TimeoutException("Forced Exception");
                });
            }
            catch (TimeoutException ex)
            {
                Assert.AreEqual("Forced Exception", ex.Message);
            }

            Assert.AreEqual(1, execCount, "The action was not executed the expected amount of times");
            Assert.AreEqual(0, totalDuration, "Unexpected duration of retry block");
        }

        [Description("F3.1.3")]
        [Priority(1)]
        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void NegativeRetryInterval()
        {
            RetryPolicy negativeRetryCountRetryPolicy = RetryPolicyFactory.GetRetryPolicy<SqlDatabaseTransientErrorDetectionStrategy>(retryStrategyName: "NegativeRetryInterval");
            int execCount = 0;

            try
            {
                negativeRetryCountRetryPolicy.ExecuteAction(() =>
                {
                    execCount++;
                    throw new TimeoutException("Forced Exception");
                });
            }
            catch (Exception)
            {
                throw;
            }
        }

        [Description("F3.1.4")]
        [Priority(1)]
        [TestMethod]
        public void ZeroRetryInterval()
        {
            RetryPolicy retryPolicy = RetryPolicyFactory.GetRetryPolicy<SqlDatabaseTransientErrorDetectionStrategy>(retryStrategyName: "ZeroRetryInterval");
            int execCount = 0;
            double totalDuration = 0;

            retryPolicy.Retrying += (sender, args) => totalDuration += args.Delay.TotalMilliseconds;

            try
            {
                retryPolicy.ExecuteAction(() =>
                {
                    execCount++;
                    throw new TimeoutException("Forced Exception");
                });
            }
            catch (TimeoutException ex)
            {
                Assert.AreEqual("Forced Exception", ex.Message);
            }

            Assert.AreEqual(4, execCount, "The action was not executed the expected amount of times");
            Assert.AreEqual(0, totalDuration, "Unexpected duration of retry block");
        }

        [Description("F3.1.5")]
        [Priority(1)]
        [TestMethod]
        [Ignore]    // REVIEW - Negative values are not allowed by configuration
        public void NegativeRetryIncrement()
        {
            RetryPolicy retryPolicy = RetryPolicyFactory.GetRetryPolicy<SqlDatabaseTransientErrorDetectionStrategy>(retryStrategyName: "NegativeRetryIncrement");
            int execCount = 0;
            double totalDuration = 0;

            retryPolicy.Retrying += (sender, args) => totalDuration += args.Delay.TotalMilliseconds;

            try
            {
                retryPolicy.ExecuteAction(() =>
                {
                    execCount++;
                    throw new TimeoutException("Forced Exception");
                });
            }
            catch (TimeoutException ex)
            {
                Assert.AreEqual("Forced Exception", ex.Message);
            }

            Assert.AreEqual(4, execCount, "The action was not executed the expected amount of times");
            Assert.AreEqual(270, totalDuration, "Unexpected duration of retry block");
        }

        [Description("F3.1.6")]
        [Priority(1)]
        [TestMethod]
        public void ZeroRetryIncrement()
        {
            RetryPolicy retryPolicy = RetryPolicyFactory.GetRetryPolicy<SqlDatabaseTransientErrorDetectionStrategy>(retryStrategyName: "ZeroRetryIncrement");
            int execCount = 0;
            double totalDuration = 0;

            retryPolicy.Retrying += (sender, args) => totalDuration += args.Delay.TotalMilliseconds;

            try
            {
                retryPolicy.ExecuteAction(() =>
                {
                    execCount++;
                    throw new TimeoutException("Forced Exception");
                });
            }
            catch (TimeoutException ex)
            {
                Assert.AreEqual("Forced Exception", ex.Message);
            }

            Assert.AreEqual(4, execCount, "The action was not executed the expected amount of times");
            Assert.AreEqual(300, totalDuration, "Unexpected duration of retry block");
        }

        [Description("F3.1.7")]
        [Priority(1)]
        [TestMethod]
        public void NegativeMinBackoff()
        {
            const string Key = "NegativeMinBackoff";
            IConfigurationSection section = RetryConfiguration.GetConfiguration().GetSection("causeError").GetSection(Key);
            Assert.IsNotNull(section);
            Assert.IsTrue(section.Exists());
            try
            {
                section.GetExponentialBackoff();
                Assert.Fail();
            }
            catch (TargetInvocationException exception)
            {
                Assert.IsTrue(exception.InnerException is ArgumentOutOfRangeException innerException && innerException.Message.Contains(nameof(ExponentialBackoffOptions.MinBackOff), StringComparison.InvariantCultureIgnoreCase));
            }
        }

        [Description("F3.1.8")]
        [Priority(1)]
        [TestMethod]
        public void ZeroMinBackoff()
        {
            RetryPolicy retryPolicy = RetryPolicyFactory.GetRetryPolicy<SqlDatabaseTransientErrorDetectionStrategy>(retryStrategyName: "ZeroMinBackoff");
            int execCount = 0;

            try
            {
                retryPolicy.ExecuteAction(() =>
                {
                    execCount++;
                    throw new TimeoutException("Forced Exception");
                });
            }
            catch (TimeoutException ex)
            {
                Assert.AreEqual("Forced Exception", ex.Message);
            }

            Assert.AreEqual(4, execCount, "The action was not executed the expected amount of times");
        }

        [Description("F3.1.9")]
        [Priority(1)]
        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void NegativeMaxBackoff()
        {
            RetryPolicy retryPolicy = RetryPolicyFactory.GetRetryPolicy<SqlDatabaseTransientErrorDetectionStrategy>(retryStrategyName: "NegativeMaxBackoff");
            int execCount = 0;

            try
            {
                retryPolicy.ExecuteAction(() =>
                {
                    execCount++;
                    throw new TimeoutException("Forced Exception");
                });
            }
            catch (Exception)
            {
                throw;
            }
        }

        [Description("F3.1.10")]
        [Priority(1)]
        [TestMethod]
        public void ZeroMaxBackoff()
        {
            RetryPolicy retryPolicy = RetryPolicyFactory.GetRetryPolicy<SqlDatabaseTransientErrorDetectionStrategy>(retryStrategyName: "ZeroMaxBackoff");
            int execCount = 0;
            double totalDuration = 0;

            retryPolicy.Retrying += (sender, args) => totalDuration += args.Delay.TotalMilliseconds;

            try
            {
                retryPolicy.ExecuteAction(() =>
                {
                    execCount++;
                    throw new TimeoutException("Forced Exception");
                });
            }
            catch (TimeoutException ex)
            {
                Assert.AreEqual("Forced Exception", ex.Message);
            }

            Assert.AreEqual(4, execCount, "The action was not executed the expected amount of times");
            Assert.AreEqual(0, totalDuration, "Unexpected duration of retry block");
        }

        [Description("F3.1.11")]
        [Priority(1)]
        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void NegativeDeltaBackoff()
        {
            RetryPolicy retryPolicy = RetryPolicyFactory.GetRetryPolicy<SqlDatabaseTransientErrorDetectionStrategy>(retryStrategyName: "NegativeDeltaBackoff");
            int execCount = 0;

            try
            {
                retryPolicy.ExecuteAction(() =>
                {
                    execCount++;
                    throw new TimeoutException("Forced Exception");
                });
            }
            catch (Exception)
            {
                throw;
            }
        }

        [Description("F3.1.12")]
        [Priority(1)]
        [TestMethod]
        public void ZeroDeltaBackoff()
        {
            RetryPolicy retryPolicy = RetryPolicyFactory.GetRetryPolicy<SqlDatabaseTransientErrorDetectionStrategy>(retryStrategyName: "ZeroDeltaBackoff");
            int execCount = 0;
            double totalDuration = 0;

            retryPolicy.Retrying += (sender, args) => totalDuration += args.Delay.TotalMilliseconds;

            try
            {
                retryPolicy.ExecuteAction(() =>
                {
                    execCount++;
                    throw new TimeoutException("Forced Exception");
                });
            }
            catch (TimeoutException ex)
            {
                Assert.AreEqual("Forced Exception", ex.Message);
            }

            Assert.AreEqual(4, execCount, "The action was not executed the expected amount of times");
            Assert.AreEqual(300, totalDuration, "Unexpected duration of retry block");
        }

        [Description("F3.1.13")]
        [Priority(1)]
        [TestMethod]
        public void MinBackoffEqualsMax()
        {
            RetryPolicy retryPolicy = RetryPolicyFactory.GetRetryPolicy<SqlDatabaseTransientErrorDetectionStrategy>(retryStrategyName: "MinBackoffEqualsMax");
            int execCount = 0;
            double totalDuration = 0;

            retryPolicy.Retrying += (sender, args) => totalDuration += args.Delay.TotalMilliseconds;

            try
            {
                retryPolicy.ExecuteAction(() =>
                {
                    execCount++;
                    throw new TimeoutException("Forced Exception");
                });
            }
            catch (TimeoutException ex)
            {
                Assert.AreEqual("Forced Exception", ex.Message);
            }

            Assert.AreEqual(4, execCount, "The action was not executed the expected amount of times");
            Assert.AreEqual(3000, totalDuration, "Unexpected duration of retry block");
        }

        [Description("F3.1.14")]
        [Priority(1)]
        [TestMethod]
        public void MinBackoffGreaterThanMax()
        {
            const string Key = "MinBackoffGreaterThanMax";
            IConfigurationSection section = RetryConfiguration.GetConfiguration().GetSection("causeError").GetSection(Key);
            Assert.IsNotNull(section);
            Assert.IsTrue(section.Exists());
            try
            {
                section.GetExponentialBackoff();
                Assert.Fail();
            }
            catch (ArgumentOutOfRangeException exception)
            {
                Assert.IsTrue(exception.Message.Contains(nameof(ExponentialBackoffOptions.MinBackOff), StringComparison.InvariantCultureIgnoreCase));
            }
        }

        [Description("F3.1.15")]
        [Priority(1)]
        [Ignore]
        [TestMethod]
        public void LargeDeltaBackoff()
        {
            RetryPolicy retryPolicy = RetryPolicyFactory.GetRetryPolicy<SqlDatabaseTransientErrorDetectionStrategy>(retryStrategyName: "LargeDeltaBackoff");
            int execCount = 0;
            double totalDuration = 0;

            retryPolicy.Retrying += (sender, args) => totalDuration += args.Delay.TotalMilliseconds;

            try
            {
                retryPolicy.ExecuteAction(() =>
                {
                    execCount++;
                    throw new TimeoutException("Forced Exception");
                });
            }
            catch (TimeoutException ex)
            {
                Assert.AreEqual("Forced Exception", ex.Message);
            }

            Assert.AreEqual(4, execCount, "The action was not executed the expected amount of times");
            Assert.AreEqual(3000, totalDuration, "Unexpected duration of retry block");
        }

        [Description("F3.1.16")]
        [Priority(1)]
        [TestMethod]
        public void FixedInterval_MissingRetryInterval()
        {
            const string Key = "FixedInterval_MissingRetryInterval";
            IConfigurationSection section = RetryConfiguration.GetConfiguration().GetSection("causeError").GetSection(Key);
            Assert.IsNotNull(section);
            Assert.IsTrue(section.Exists());
            try
            {
                section.GetFixedInterval();
                Assert.Fail();
            }
            catch (ArgumentException exception)
            {
                StringAssert.Contains(exception.Message, Key);
            }
        }

        [Description("F3.1.17")]
        [Priority(1)]
        [TestMethod]
        public void IncrementalInterval_MissingRetryInterval()
        {
            const string Key = "IncrementalInterval_MissingRetryInterval";
            IConfigurationSection section = RetryConfiguration.GetConfiguration().GetSection("causeError").GetSection(Key);
            Assert.IsNotNull(section);
            Assert.IsTrue(section.Exists());
            try
            {
                section.GetIncremental();
                Assert.Fail();
            }
            catch (ArgumentException exception)
            {
                StringAssert.Contains(exception.Message, Key);
            }
        }

        [Description("F3.1.18")]
        [Priority(1)]
        [TestMethod]
        public void ExponentialInterval_MissingMinBackoff()
        {
            const string Key = "ExponentialInterval_MissingMinBackoff";
            IConfigurationSection section = RetryConfiguration.GetConfiguration().GetSection("causeError").GetSection(Key);
            Assert.IsNotNull(section);
            Assert.IsTrue(section.Exists());
            try
            {
                section.GetExponentialBackoff();
                Assert.Fail();
            }
            catch (ArgumentException exception)
            {
                StringAssert.Contains(exception.Message, Key);
            }
        }

        [Description("F3.1.19")]
        [Priority(1)]
        [TestMethod]
        public void ExponentialInterval_MissingMaxBackoff()
        {
            const string Key = "ExponentialInterval_MissingMaxBackoff";
            IConfigurationSection section = RetryConfiguration.GetConfiguration().GetSection("causeError").GetSection(Key);
            Assert.IsNotNull(section);
            Assert.IsTrue(section.Exists());
            try
            {
                section.GetExponentialBackoff();
                Assert.Fail();
            }
            catch (ArgumentException exception)
            {
                StringAssert.Contains(exception.Message, Key);
            }
        }

        [Description("F3.1.20")]
        [Priority(1)]
        [TestMethod]
        public void ExponentialInterval_MissingDeltaBackoff()
        {
            const string Key = "ExponentialInterval_MissingDeltaBackoff";
            IConfigurationSection section = RetryConfiguration.GetConfiguration().GetSection("causeError").GetSection(Key);
            Assert.IsNotNull(section);
            Assert.IsTrue(section.Exists());
            try
            {
                section.GetExponentialBackoff();
                Assert.Fail();
            }
            catch (ArgumentException exception)
            {
                StringAssert.Contains(exception.Message, Key);
            }
        }

        [Description("F3.1.21")]
        [Priority(1)]
        [TestMethod]
        public void NonExist()
        {
            const string Key = "NonExist";
            IConfigurationSection section = RetryConfiguration.GetConfiguration().GetSection("causeError").GetSection(Key);
            Assert.IsNotNull(section);
            Assert.IsFalse(section.Exists());
            try
            {
                section.GetFixedInterval();
                Assert.Fail();
            }
            catch (ArgumentException exception)
            {
                StringAssert.Contains(exception.Message, Key);
            }
            try
            {
                section.GetIncremental();
                Assert.Fail();
            }
            catch (ArgumentException exception)
            {
                StringAssert.Contains(exception.Message, Key);
            }
            try
            {
                section.GetExponentialBackoff();
                Assert.Fail();
            }
            catch (ArgumentException exception)
            {
                StringAssert.Contains(exception.Message, Key);
            }
        }
    }
}
