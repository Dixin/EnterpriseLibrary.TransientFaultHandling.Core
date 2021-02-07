namespace Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling.Tests.SqlCommandExtensionsScenarios.given_failing_execute_reader_command
{
    using System.Data;
    using Microsoft.Data.SqlClient;
    using Microsoft.Practices.EnterpriseLibrary.Common.TestSupport.ContextBase;
    using Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling.TestSupport;
    using Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling;
    using VisualStudio.TestTools.UnitTesting;

    public abstract class Context : ArrangeActAssert
    {
        protected TestRetryStrategy connectionStrategy;
        protected TestRetryStrategy commandStrategy;
        protected SqlCommand command;
        protected RetryPolicy connectionPolicy;
        protected RetryPolicy commandPolicy;

        protected override void Arrange()
        {
            this.command = new SqlCommand(TestSqlSupport.InvalidSqlText);

            this.connectionStrategy = new TestRetryStrategy();

            this.commandStrategy = new TestRetryStrategy();

            this.connectionPolicy = new RetryPolicy(ErrorDetectionStrategy.AlwaysTransient, this.connectionStrategy);

            this.commandPolicy = new RetryPolicy(ErrorDetectionStrategy.AlwaysTransient, this.commandStrategy);
        }
    }

    [TestClass]
    public class when_executing_command_with_closed_connection : Context
    {
        protected override void Act()
        {
            try
            {
                this.command.Connection = new SqlConnection(TestSqlSupport.SqlDatabaseConnectionString);
                this.command.ExecuteReaderWithRetry(this.commandPolicy, this.connectionPolicy);
                Assert.Fail();
            }
            catch (SqlException)
            {
            }
        }

        [TestMethod]
        public void then_connection_is_closed()
        {
            Assert.IsNotNull(this.command.Connection);
            Assert.IsTrue(this.command.Connection.State == ConnectionState.Closed);
        }

        [TestMethod]
        public void then_retried()
        {
            Assert.AreEqual(0, this.connectionStrategy.ShouldRetryCount);
            Assert.AreEqual(1, this.commandStrategy.ShouldRetryCount);
        }
    }

    [TestClass]
    public class when_executing_command_with_opened_connection : Context
    {
        protected override void Act()
        {
            try
            {
                this.command.Connection = new SqlConnection(TestSqlSupport.SqlDatabaseConnectionString);
                this.command.Connection.Open();
                this.command.ExecuteReaderWithRetry(this.commandPolicy, this.connectionPolicy);
                Assert.Fail();
            }
            catch (SqlException)
            {
            }
        }

        [TestMethod]
        public void then_connection_is_opened()
        {
            Assert.IsNotNull(this.command.Connection);
            Assert.IsTrue(this.command.Connection.State == ConnectionState.Open);
        }

        [TestMethod]
        public void then_retried()
        {
            Assert.AreEqual(0, this.connectionStrategy.ShouldRetryCount);
            Assert.AreEqual(1, this.commandStrategy.ShouldRetryCount);
        }
    }
}