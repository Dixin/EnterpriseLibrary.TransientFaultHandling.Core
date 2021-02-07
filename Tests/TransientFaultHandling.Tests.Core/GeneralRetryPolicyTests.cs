namespace Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling.Tests
{
    #region Using statements
    using System;
    using Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    #endregion

    /// <summary>
    /// Implements general test cases for retry policies.
    /// </summary>
    [TestClass]
    public class GeneralRetryPolicyTests
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

        [TestMethod]
        public void TestNegativeRetryCount()
        {
            try
            {
                // First, instantiate a policy directly bypassing the configuration data validation.
                RetryPolicy<SqlDatabaseTransientErrorDetectionStrategy> retryPolicy = new(-1);
                Assert.Fail("When the RetryCount is negative, the retry policy should throw an exception.");
            }
            catch (ArgumentOutOfRangeException ex)
            {
                Assert.AreEqual("retryCount", ex.ParamName, $"A wrong argument has caused the {ex.GetType().Name} exception");
            }

            // Removed - different approach when dealing with the factory.
            ////try
            ////{
            ////    // Second, attempt to instantiate a retry policy from configuration with invalid settings.
            ////    RetryPolicy retryPolicy = RetryPolicyFactory.GetRetryPolicy<SqlDatabaseTransientErrorDetectionStrategy>("NegativeRetryCount");
            ////    Assert.Fail("When the RetryCount is negative, the retry policy should throw an exception.");
            ////}
            ////catch (ConfigurationErrorsException ex)
            ////{
            ////    Assert.IsTrue(ex.Message.Contains("maxRetryCount"), String.Format("A wrong argument has caused the {0} exception", ex.GetType().Name));
            ////}

            try
            {
                // And last, instantiate a policy description with invalid settings.
                new FixedIntervalOptions() { RetryCount = -1 };
                Assert.Fail("When the RetryCount is negative, the retry policy should throw an exception.");
            }
            catch (ArgumentOutOfRangeException ex)
            {
                Assert.IsTrue(ex.Message.Contains(nameof(FixedIntervalOptions.RetryCount)), $"A wrong argument has caused the {ex.GetType().Name} exception");
            }

            try
            {
                // And last, instantiate a policy description with invalid settings.
                new IncrementalOptions { RetryCount = -1 };
                Assert.Fail("When the RetryCount is negative, the retry policy should throw an exception.");
            }
            catch (ArgumentOutOfRangeException ex)
            {
                Assert.IsTrue(ex.Message.Contains(nameof(IncrementalOptions.RetryCount)), $"A wrong argument has caused the {ex.GetType().Name} exception");
            }

            try
            {
                // And last, instantiate a policy description with invalid settings.
                new ExponentialBackoffOptions { RetryCount = -1 };
                Assert.Fail("When the RetryCount is negative, the retry policy should throw an exception.");
            }
            catch (ArgumentOutOfRangeException ex)
            {
                Assert.IsTrue(ex.Message.Contains(nameof(ExponentialBackoffOptions.RetryCount)), $"A wrong argument has caused the {ex.GetType().Name} exception");
            }
        }

        [TestMethod]
        public void TestNegativeRetryInterval()
        {
            try
            {
                // First, instantiate a policy directly bypassing the configuration data validation.
                RetryPolicy<SqlDatabaseTransientErrorDetectionStrategy> retryPolicy = new(3, TimeSpan.FromMilliseconds(-2));
                Assert.Fail("When the RetryInterval is negative, the retry policy should throw an exception.");
            }
            catch (ArgumentOutOfRangeException ex)
            {
                Assert.AreEqual("retryInterval", ex.ParamName, $"A wrong argument has caused the {ex.GetType().Name} exception");
            }

            ////try
            ////{
            ////    // Second, attempt to instantiate a retry policy from configuration with invalid settings.
            ////    RetryPolicy retryPolicy = RetryPolicyFactory.GetRetryPolicy<SqlDatabaseTransientErrorDetectionStrategy>("NegativeRetryInterval");
            ////    Assert.Fail("When the RetryInterval is negative, the retry policy should throw an exception.");
            ////}
            ////catch (ConfigurationErrorsException ex)
            ////{
            ////    Assert.IsTrue(ex.Message.Contains("retryInterval"), String.Format("A wrong argument has caused the {0} exception", ex.GetType().Name));
            ////}

            try
            {
                // And last, instantiate a policy description with invalid settings.
                new FixedIntervalOptions { RetryInterval = TimeSpan.FromMilliseconds(-2) };
                Assert.Fail("When the RetryInterval is negative, the retry policy should throw an exception.");
            }
            catch (ArgumentOutOfRangeException ex)
            {
                Assert.IsTrue(ex.Message.Contains(nameof(FixedIntervalOptions.RetryInterval), StringComparison.InvariantCultureIgnoreCase), $"A wrong argument has caused the {ex.GetType().Name} exception");
            }
        }

        [TestMethod]
        public void TestZeroRetryInterval()
        {
            RetryPolicy retryPolicy = RetryPolicyFactory.GetRetryPolicy<SqlDatabaseTransientErrorDetectionStrategy>(retryStrategyName: "ZeroRetryInterval");

            int retryCount = 0;
            TimeSpan totalDelay;

            TestRetryPolicy(retryPolicy, out retryCount, out totalDelay);

            Assert.AreEqual(3, retryCount, "The action was not retried using the expected amount of times");
        }

        [TestMethod]
        public void TestNegativeRetryIncrement()
        {
            ////RetryPolicy retryPolicy = RetryPolicyFactory.GetRetryPolicy<SqlDatabaseTransientErrorDetectionStrategy>("NegativeRetryIncrement");

            ////int retryCount = 0;
            ////TimeSpan totalDelay;

            ////GeneralRetryPolicyTests.TestRetryPolicy(retryPolicy, out retryCount, out totalDelay);

            ////Assert.AreEqual<int>(15, retryCount, "The action was not retried using the expected amount of times");
            ////Assert.AreEqual<double>(550, totalDelay.TotalMilliseconds, "The total delay between retries does not match the expected result");

            ////retryPolicy = new RetryPolicy<SqlDatabaseTransientErrorDetectionStrategy>(new Incremental(3, TimeSpan.FromMilliseconds(100), TimeSpan.FromMilliseconds(-10)));

            ////GeneralRetryPolicyTests.TestRetryPolicy(retryPolicy, out retryCount, out totalDelay);

            ////Assert.AreEqual<int>(3, retryCount, "The action was not retried using the expected amount of times");
            ////Assert.AreEqual<double>(270, totalDelay.TotalMilliseconds, "The total delay between retries does not match the expected result");

            try
            {
                // And last, instantiate a policy description with invalid settings.
                new IncrementalOptions { Increment = TimeSpan.FromSeconds(-1) };
                Assert.Fail("When the RetryCount is negative, the retry policy should throw an exception.");
            }
            catch (ArgumentOutOfRangeException ex)
            {
                Assert.IsTrue(ex.Message.Contains(nameof(IncrementalOptions.Increment), StringComparison.InvariantCultureIgnoreCase), $"A wrong argument has caused the {ex.GetType().Name} exception");
            }
        }

        [TestMethod]
        public void TestNegativeMinBackoff()
        {
            try
            {
                // First, instantiate a policy directly bypassing the configuration data validation.
                new RetryPolicy<SqlDatabaseTransientErrorDetectionStrategy>(3, TimeSpan.FromMilliseconds(-1), TimeSpan.FromMilliseconds(1000), TimeSpan.FromMilliseconds(100));
                Assert.Fail("When the MinBackoff is negative, the retry policy should throw an exception.");
            }
            catch (ArgumentOutOfRangeException ex)
            {
                Assert.AreEqual("minBackoff", ex.ParamName, $"A wrong argument has caused the {ex.GetType().Name} exception");
            }

            ////try
            ////{
            ////    // Second, attempt to instantiate a retry policy from configuration with invalid settings.
            ////    RetryPolicy retryPolicy = RetryPolicyFactory.GetRetryPolicy<SqlDatabaseTransientErrorDetectionStrategy>("NegativeMinBackoff");
            ////    Assert.Fail("When the MinBackoff is negative, the retry policy should throw an exception.");
            ////}
            ////catch (ConfigurationErrorsException ex)
            ////{
            ////    Assert.IsTrue(ex.Message.Contains("minBackoff"), String.Format("A wrong argument has caused the {0} exception", ex.GetType().Name));
            ////}

            try
            {
                // And last, instantiate a policy description with invalid settings.
                new ExponentialBackoffOptions { MinBackOff = TimeSpan.FromMilliseconds(-1) };
                Assert.Fail("When the MinBackoff is negative, the retry policy should throw an exception.");
            }
            catch (ArgumentOutOfRangeException ex)
            {
                Assert.IsTrue(ex.Message.Contains("minBackoff", StringComparison.InvariantCultureIgnoreCase), $"A wrong argument has caused the {ex.GetType().Name} exception");
            }
        }

        [TestMethod]
        public void TestNegativeMaxBackoff()
        {
            try
            {
                // First, instantiate a policy directly bypassing the configuration data validation.
                new RetryPolicy<SqlDatabaseTransientErrorDetectionStrategy>(3, TimeSpan.FromMilliseconds(100), TimeSpan.FromMilliseconds(-2), TimeSpan.FromMilliseconds(100));
                Assert.Fail("When the MaxBackoff is negative, the retry policy should throw an exception.");
            }
            catch (ArgumentOutOfRangeException ex)
            {
                Assert.AreEqual("minBackoff", ex.ParamName, $"A wrong argument has caused the {ex.GetType().Name} exception");
            }

            ////try
            ////{
            ////    // Second, attempt to instantiate a retry policy from configuration with invalid settings.
            ////    RetryPolicy retryPolicy = RetryPolicyFactory.GetRetryPolicy<SqlDatabaseTransientErrorDetectionStrategy>("NegativeMaxBackoff");
            ////    Assert.Fail("When the MaxBackoff is negative, the retry policy should throw an exception.");
            ////}
            ////catch (ConfigurationErrorsException ex)
            ////{
            ////    Assert.IsTrue(ex.Message.Contains("maxBackoff"), String.Format("A wrong argument has caused the {0} exception", ex.GetType().Name));
            ////}

            try
            {
                // And last, instantiate a policy description with invalid settings.
                new ExponentialBackoffOptions { MaxBackOff = TimeSpan.FromMilliseconds(-2) };
                Assert.Fail("When the MaxBackoff is negative, the retry policy should throw an exception.");
            }
            catch (ArgumentOutOfRangeException ex)
            {
                Assert.IsTrue(ex.Message.Contains("maxBackoff", StringComparison.InvariantCultureIgnoreCase), $"A wrong argument has caused the {ex.GetType().Name} exception");
            }
        }

        [TestMethod]
        public void TestNegativeDeltaBackoff()
        {
            try
            {
                // First, instantiate a policy directly bypassing the configuration data validation.
                new RetryPolicy<SqlDatabaseTransientErrorDetectionStrategy>(3, TimeSpan.FromMilliseconds(100), TimeSpan.FromMilliseconds(1000), TimeSpan.FromMilliseconds(-1));
                Assert.Fail("When the DeltaBackoff is negative, the retry policy should throw an exception.");
            }
            catch (ArgumentOutOfRangeException ex)
            {
                Assert.AreEqual("deltaBackoff", ex.ParamName, $"A wrong argument has caused the {ex.GetType().Name} exception");
            }

            try
            {
                // And last, instantiate a policy description with invalid settings.
                new ExponentialBackoffOptions { DeltaBackOff = TimeSpan.FromMilliseconds(-1) };
                Assert.Fail("When the DeltaBackoff is negative, the retry policy should throw an exception.");
            }
            catch (ArgumentOutOfRangeException ex)
            {
                Assert.IsTrue(ex.Message.Contains("deltaBackoff", StringComparison.InvariantCultureIgnoreCase), $"A wrong argument has caused the {ex.GetType().Name} exception");
            }
        }

        [TestMethod]
        public void TestMinBackoffGreaterThanMax()
        {
            try
            {
                // First, instantiate a policy directly bypassing the configuration data validation.
                new RetryPolicy<SqlDatabaseTransientErrorDetectionStrategy>(3, TimeSpan.FromMilliseconds(1000), TimeSpan.FromMilliseconds(100), TimeSpan.FromMilliseconds(100));
                Assert.Fail("When the MinBackoff greater than MaxBackoff, the retry policy should throw an exception.");
            }
            catch (ArgumentOutOfRangeException ex)
            {
                Assert.AreEqual("minBackoff", ex.ParamName, $"A wrong argument has caused the {ex.GetType().Name} exception");
            }

            ////try
            ////{
            ////    // Second, attempt to instantiate a retry policy from configuration with invalid settings.
            ////    RetryPolicy retryPolicy = RetryPolicyFactory.GetRetryPolicy<SqlDatabaseTransientErrorDetectionStrategy>("MinBackoffGreaterThanMax");
            ////    Assert.Fail("When the MinBackoff greater than MaxBackoff, the retry policy should throw an exception.");
            ////}
            ////catch (ArgumentOutOfRangeException ex)
            ////{
            ////    Assert.AreEqual("minBackoff", ex.ParamName, String.Format("A wrong argument has caused the {0} exception", ex.GetType().Name));
            ////}

            // this cannot be validated through config validators
            ////try
            ////{
            ////    // And last, instantiate a policy description with invalid settings.
            ////    new ExponentialBackoffData { MinBackoff = TimeSpan.FromMilliseconds(1000), MaxBackoff = TimeSpan.FromMilliseconds(100) };

            ////    Assert.Fail("When the MinBackoff greater than MaxBackoff, the retry policy should throw an exception.");
            ////}
            ////catch (ArgumentOutOfRangeException ex)
            ////{
            ////    Assert.AreEqual("minBackoff", ex.ParamName, String.Format("A wrong argument has caused the {0} exception", ex.GetType().Name));
            ////}
        }

        [TestMethod]
        public void TestLargeDeltaBackoff()
        {
            int retryCount = 0;
            TimeSpan totalDelay;

            try
            {
                // Second, attempt to instantiate a retry policy from configuration with invalid settings.
                RetryPolicy retryPolicy = RetryPolicyFactory.GetRetryPolicy<SqlDatabaseTransientErrorDetectionStrategy>(retryStrategyName: "LargeDeltaBackoff");

                TestRetryPolicy(retryPolicy, out retryCount, out totalDelay);
                Assert.AreEqual(3, retryCount, "The action was not retried using the expected amount of times");
            }
            catch
            {
                Assert.Fail("When the DeltaBackoff is very large, the retry policy should work normally.");
            }

            try
            {
                // First, instantiate a policy directly bypassing the configuration data validation.
                RetryPolicy<SqlDatabaseTransientErrorDetectionStrategy> retryPolicy = new(3, TimeSpan.FromMilliseconds(100), TimeSpan.FromMilliseconds(1000), TimeSpan.FromMilliseconds(100000000000000));

                TestRetryPolicy(retryPolicy, out retryCount, out totalDelay);
                Assert.AreEqual(3, retryCount, "The action was not retried using the expected amount of times");
            }
            catch
            {
                Assert.Fail("When the DeltaBackoff is very large, the retry policy should work normally.");
            }
        }

        #region Private methods
        internal static void TestRetryPolicy(RetryPolicy retryPolicy, out int retryCount, out TimeSpan totalDelay)
        {
            int callbackCount = 0;
            double totalDelayInMs = 0;

            retryPolicy.Retrying += (sender, args) =>
            {
                callbackCount++;
                totalDelayInMs += args.Delay.TotalMilliseconds;
            };

            try
            {
                retryPolicy.ExecuteAction(() => throw new TimeoutException("Forced Exception"));
            }
            catch (TimeoutException ex)
            {
                Assert.AreEqual("Forced Exception", ex.Message);
            }

            retryCount = callbackCount;
            totalDelay = TimeSpan.FromMilliseconds(totalDelayInMs);
        }
        #endregion
    }
}
