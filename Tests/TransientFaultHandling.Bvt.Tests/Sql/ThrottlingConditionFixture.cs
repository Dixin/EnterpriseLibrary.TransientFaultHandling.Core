namespace Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling.Bvt.Tests.Sql;

[TestClass]
public class ThrottlingConditionFixture
{
    [TestMethod]
    public void GetsRejectUpdateInsertThrottlingConditionFromSqlError()
    {
        SqlError sqlError = SqlExceptionCreator.GenerateFakeSqlError(ThrottlingCondition.ThrottlingErrorNumber, "Code: 12345 SQL Error");
        ThrottlingCondition condition = ThrottlingCondition.FromError(sqlError);

        Assert.AreEqual(ThrottlingMode.RejectUpdateInsert, condition.ThrottlingMode);
    }
}