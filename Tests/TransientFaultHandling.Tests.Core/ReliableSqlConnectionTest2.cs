
namespace Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling.Tests
{
    using Microsoft.Data.SqlClient;
    using Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling.TestSupport;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ReliableSqlConnectionTest2
    {
        private string connectionString;

        [TestInitialize]
        public void Initialize()
        {
            this.connectionString = TestSqlSupport.SqlDatabaseConnectionString;
            RetryPolicyFactory.CreateDefault();
        }

        [TestCleanup]
        public void Cleanup()
        {
            RetryPolicyFactory.SetRetryManager(null, false);
        }

        [TestMethod]
        public void TestDoubleCommandsWithoutClosing()
        {
            ReliableSqlConnection connection = new(this.connectionString);

            SqlCommand command = new("SELECT 1");
            SqlCommand command2 = new("SELECT 2");

            connection.ExecuteCommand(command);
            connection.ExecuteCommand(command2);
        }

        [TestMethod]
        public void TestDoubleCommandsWithClosing()
        {
            ReliableSqlConnection connection = new(this.connectionString);

            SqlCommand command = new("SELECT 1");
            SqlCommand command2 = new("SELECT 2");

            connection.ExecuteCommand(command);
            connection.Close();
            connection.ExecuteCommand(command2);
        }

        [TestMethod]
        public void TestSingleCommandWithoutClosing()
        {
            ReliableSqlConnection connection = new(this.connectionString);

            SqlCommand command = new("SELECT 1");

            connection.ExecuteCommand(command);
            connection.ExecuteCommand(command);
        }

        [TestMethod]
        public void TestSingleCommandWithClosing()
        {
            ReliableSqlConnection connection = new(this.connectionString);

            SqlCommand command = new("SELECT 1");

            connection.ExecuteCommand(command);
            connection.Close();
            connection.ExecuteCommand(command);
        }
    }
}
