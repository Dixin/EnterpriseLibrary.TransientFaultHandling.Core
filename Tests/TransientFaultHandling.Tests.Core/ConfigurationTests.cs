namespace Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling.Tests;

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
        RetryPolicy retryPolicy = RetryPolicyFactory.GetRetryPolicy<SqlDatabaseTransientErrorDetectionStrategy>(retryStrategyName: nameof(this.NegativeRetryCount));
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
        RetryPolicy retryPolicy = RetryPolicyFactory.GetRetryPolicy<SqlDatabaseTransientErrorDetectionStrategy>(retryStrategyName: nameof(this.ZeroRetryCount));
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
        RetryPolicy negativeRetryCountRetryPolicy = RetryPolicyFactory.GetRetryPolicy<SqlDatabaseTransientErrorDetectionStrategy>(retryStrategyName: nameof(this.NegativeRetryInterval));
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
        RetryPolicy retryPolicy = RetryPolicyFactory.GetRetryPolicy<SqlDatabaseTransientErrorDetectionStrategy>(retryStrategyName: nameof(this.ZeroRetryInterval));
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
        RetryPolicy retryPolicy = RetryPolicyFactory.GetRetryPolicy<SqlDatabaseTransientErrorDetectionStrategy>(retryStrategyName: nameof(this.NegativeRetryIncrement));
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
        RetryPolicy retryPolicy = RetryPolicyFactory.GetRetryPolicy<SqlDatabaseTransientErrorDetectionStrategy>(retryStrategyName: nameof(this.ZeroRetryIncrement));
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
        const string Key = nameof(this.NegativeMinBackoff);
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
            Assert.IsTrue(exception.InnerException is ArgumentOutOfRangeException { ParamName: "value" });
        }
    }

    [Description("F3.1.8")]
    [Priority(1)]
    [TestMethod]
    public void ZeroMinBackoff()
    {
        RetryPolicy retryPolicy = RetryPolicyFactory.GetRetryPolicy<SqlDatabaseTransientErrorDetectionStrategy>(retryStrategyName: nameof(this.ZeroMinBackoff));
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
        RetryPolicy retryPolicy = RetryPolicyFactory.GetRetryPolicy<SqlDatabaseTransientErrorDetectionStrategy>(retryStrategyName: nameof(this.NegativeMaxBackoff));
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
        RetryPolicy retryPolicy = RetryPolicyFactory.GetRetryPolicy<SqlDatabaseTransientErrorDetectionStrategy>(retryStrategyName: nameof(this.ZeroMaxBackoff));
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
        RetryPolicy retryPolicy = RetryPolicyFactory.GetRetryPolicy<SqlDatabaseTransientErrorDetectionStrategy>(retryStrategyName: nameof(this.NegativeDeltaBackoff));
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
        RetryPolicy retryPolicy = RetryPolicyFactory.GetRetryPolicy<SqlDatabaseTransientErrorDetectionStrategy>(retryStrategyName: nameof(this.ZeroDeltaBackoff));
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
        RetryPolicy retryPolicy = RetryPolicyFactory.GetRetryPolicy<SqlDatabaseTransientErrorDetectionStrategy>(retryStrategyName: nameof(this.MinBackoffEqualsMax));
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
        const string Key = nameof(this.MinBackoffGreaterThanMax);
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
        RetryPolicy retryPolicy = RetryPolicyFactory.GetRetryPolicy<SqlDatabaseTransientErrorDetectionStrategy>(retryStrategyName: nameof(this.LargeDeltaBackoff));
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
        const string Key = nameof(this.FixedInterval_MissingRetryInterval);
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
        const string Key = nameof(this.IncrementalInterval_MissingRetryInterval);
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
        const string Key = nameof(this.ExponentialInterval_MissingMinBackoff);
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
        const string Key = nameof(this.ExponentialInterval_MissingMaxBackoff);
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
        const string Key = nameof(this.ExponentialInterval_MissingDeltaBackoff);
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
        const string Key = nameof(this.NonExist);
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