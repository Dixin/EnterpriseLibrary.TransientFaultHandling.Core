namespace Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling.Bvt.Tests.Sql
{
    using System;
    using System.Data.Entity.Core;
    using Microsoft.Data.SqlClient;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling.Bvt.Tests.TestObjects;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class SqlDatabaseTransientErrorDetectionStrategyFixture
    {
        private RetryManager retryManager;

        [TestInitialize]
        public void Initialize()
        {
            this.retryManager = RetryConfiguration.GetRetryManager(getCustomRetryStrategy: section =>
                section.Get<TestRetryStrategyOptions>().ToTestRetryStrategy(section.Key));
        }

        [TestCleanup]
        public void Cleanup()
        {
        }

        [TestMethod]
        public void RetriesWhenSqlExceptionIsThrownWithTransportLevelError()
        {
            int executeCount = 0;
            try
            {
                RetryPolicy<SqlDatabaseTransientErrorDetectionStrategy> retryPolicy = this.retryManager.GetRetryPolicy<SqlDatabaseTransientErrorDetectionStrategy>("Retry 5 times");
                retryPolicy.ExecuteAction(() =>
                {
                    executeCount++;

                    SqlException ex = SqlExceptionCreator.CreateSqlException("A transport-level error has occurred when sending the request to the server", 10053);
                    throw ex;
                });

                Assert.Fail("Should have thrown SqlException");
            }
            catch (SqlException)
            { }
            catch (Exception)
            {
                Assert.Fail("Should have thrown SqlException");
            }

            Assert.AreEqual(6, executeCount);
        }

        [TestMethod]
        public void RetriesWhenSqlExceptionIsThrownWithNetworkLevelError()
        {
            int executeCount = 0;
            try
            {
                RetryPolicy<SqlDatabaseTransientErrorDetectionStrategy> retryPolicy = this.retryManager.GetRetryPolicy<SqlDatabaseTransientErrorDetectionStrategy>("Retry 5 times");
                retryPolicy.ExecuteAction(() =>
                {
                    executeCount++;

                    SqlException ex = SqlExceptionCreator.CreateSqlException("A network-related or instance-specific error occurred while establishing a connection to SQL Server.", 10054);
                    throw ex;
                });

                Assert.Fail("Should have thrown SqlException");
            }
            catch (SqlException)
            { }
            catch (Exception)
            {
                Assert.Fail("Should have thrown SqlException");
            }

            Assert.AreEqual(6, executeCount);
        }

        [TestMethod]
        public void DoesNotRetryWhenSqlExceptionIsThrownWithSqlQueryError()
        {
            int executeCount = 0;
            try
            {
                RetryPolicy<SqlDatabaseTransientErrorDetectionStrategy> retryPolicy = this.retryManager.GetRetryPolicy<SqlDatabaseTransientErrorDetectionStrategy>("Retry 5 times");
                retryPolicy.ExecuteAction(() =>
                {
                    executeCount++;

                    SqlException ex = SqlExceptionCreator.CreateSqlException("ORDER BY items must appear in the select list if the statement contains a UNION, INTERSECT or EXCEPT operator.", 104);
                    throw ex;
                });

                Assert.Fail("Should have thrown SqlException");
            }
            catch (SqlException)
            { }
            catch (Exception)
            {
                Assert.Fail("Should have thrown SqlException");
            }

            Assert.AreEqual(1, executeCount);
        }

        [TestMethod]
        public void RetriesWhenTimeoutExceptionIsThrown()
        {
            int executeCount = 0;
            try
            {
                RetryPolicy<SqlDatabaseTransientErrorDetectionStrategy> retryPolicy = this.retryManager.GetRetryPolicy<SqlDatabaseTransientErrorDetectionStrategy>("Retry 5 times");
                retryPolicy.ExecuteAction(() =>
                {
                    executeCount++;

                    throw new TimeoutException();
                });

                Assert.Fail("Should have thrown TimeoutException");
            }
            catch (TimeoutException)
            { }
            catch (Exception)
            {
                Assert.Fail("Should have thrown TimeoutException");
            }

            Assert.AreEqual(6, executeCount);
        }

        [TestMethod]
        public void RetriesWhenEntityExceptionIsThrownWithTimeoutExceptionAsInnerException()
        {
            int executeCount = 0;
            try
            {
                RetryPolicy<SqlDatabaseTransientErrorDetectionStrategy> retryPolicy = this.retryManager.GetRetryPolicy<SqlDatabaseTransientErrorDetectionStrategy>("Retry 5 times");
                retryPolicy.ExecuteAction(() =>
                {
                    executeCount++;

                    throw new EntityException("Sample Error", new TimeoutException("Connection Timed out"));
                });

                Assert.Fail("Should have thrown EntityException");
            }
            catch (EntityException ex)
            {
                Assert.IsInstanceOfType(ex.InnerException, typeof(TimeoutException));
            }
            catch (Exception)
            {
                Assert.Fail("Should have thrown EntityException");
            }

            Assert.AreEqual(6, executeCount);
        }
    }
}
