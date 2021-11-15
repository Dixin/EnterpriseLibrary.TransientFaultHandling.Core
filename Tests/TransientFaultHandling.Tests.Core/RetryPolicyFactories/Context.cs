namespace Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling.Tests.RetryPolicyFactories;

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
