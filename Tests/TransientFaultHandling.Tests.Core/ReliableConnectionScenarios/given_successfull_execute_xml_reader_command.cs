﻿namespace Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling.Tests.ReliableConnectionScenarios.given_successfull_execute_xml_reader_command
{
    using System.Data;
    using System.Xml;
    using Microsoft.Data.SqlClient;
    using Microsoft.Practices.EnterpriseLibrary.Common.TestSupport.ContextBase;
    using Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling.TestSupport;
    using Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling;
    using VisualStudio.TestTools.UnitTesting;

    public abstract class Context : ArrangeActAssert
    {
        protected ReliableSqlConnection reliableConnection;
        protected TestRetryStrategy connectionStrategy;
        protected TestRetryStrategy commandStrategy;
        protected SqlCommand command;

        protected override void Arrange()
        {
            this.command = new SqlCommand(TestSqlSupport.ValidForXmlSqlQuery);

            this.connectionStrategy = new TestRetryStrategy();

            this.commandStrategy = new TestRetryStrategy();

            this.reliableConnection = new ReliableSqlConnection(
                TestSqlSupport.SqlDatabaseConnectionString,
                new RetryPolicy(ErrorDetectionStrategy.AlwaysTransient, this.connectionStrategy),
                new RetryPolicy(ErrorDetectionStrategy.AlwaysTransient, this.commandStrategy));
        }
    }

    [TestClass]
    public class when_executing_command_with_no_connection : Context
    {
        private XmlReader reader;

        protected override void Act()
        {
            this.reader = this.reliableConnection.ExecuteCommand<XmlReader>(this.command);
        }

        [TestMethod]
        public void then_connection_is_opened()
        {
            Assert.IsTrue(this.command.Connection.State == ConnectionState.Open);
        }

        [TestMethod]
        public void then_can_read_results()
        {
            Assert.IsTrue(this.reader.Read());
        }

        [TestMethod]
        public void then_retried()
        {
            Assert.AreEqual(0, this.connectionStrategy.ShouldRetryCount);
            Assert.AreEqual(0, this.commandStrategy.ShouldRetryCount);
        }
    }

    [TestClass]
    public class when_executing_command_with_closed_connection : Context
    {
        private XmlReader reader;

        protected override void Act()
        {
            this.command.Connection = new SqlConnection(TestSqlSupport.SqlDatabaseConnectionString);

            this.reader = this.reliableConnection.ExecuteCommand<XmlReader>(this.command);
        }

        [TestMethod]
        public void then_connection_is_opened()
        {
            Assert.IsTrue(this.command.Connection.State == ConnectionState.Open);
        }

        [TestMethod]
        public void then_can_read_results()
        {
            Assert.IsTrue(this.reader.Read());
        }

        [TestMethod]
        public void then_retried()
        {
            Assert.AreEqual(0, this.connectionStrategy.ShouldRetryCount);
            Assert.AreEqual(0, this.commandStrategy.ShouldRetryCount);
        }
    }

    [TestClass]
    public class when_executing_command_with_opened_connection : Context
    {
        private XmlReader reader;

        protected override void Act()
        {
            this.command.Connection = new SqlConnection(TestSqlSupport.SqlDatabaseConnectionString);
            this.command.Connection.Open();

            this.reader = this.reliableConnection.ExecuteCommand<XmlReader>(this.command);
        }

        [TestMethod]
        public void then_connection_is_opened()
        {
            Assert.IsTrue(this.command.Connection.State == ConnectionState.Open);
        }

        [TestMethod]
        public void then_can_read_results()
        {
            Assert.IsTrue(this.reader.Read());
        }

        [TestMethod]
        public void then_retried()
        {
            Assert.AreEqual(0, this.connectionStrategy.ShouldRetryCount);
            Assert.AreEqual(0, this.commandStrategy.ShouldRetryCount);
        }
    }
}