namespace Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling.Tests;

internal class Counter<TException> where TException : Exception, new()
{
    private readonly int count;

    internal Counter(int count) => this.count = count;

    internal List<DateTime> Time { get; } = [];

    internal void Increase()
    {
        this.Time.Add(DateTime.Now);
        if (this.Time.Count < this.count)
        {
            throw new TException();
        }
    }
}

internal class Counter<TException1, TException2> where TException1 : Exception, new() where TException2 : Exception, new()
{
    private readonly int count;

    internal Counter(int count) => this.count = count;

    internal List<DateTime> Time { get; } = [];

    internal void Increase()
    {
        this.Time.Add(DateTime.Now);
        if (this.Time.Count < this.count)
        {
            if (this.Time.Count % 2 == 0)
            {
                throw new TException1();
            }

            throw new TException2();
        }
    }
}