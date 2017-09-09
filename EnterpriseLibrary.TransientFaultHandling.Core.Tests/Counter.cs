namespace Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling.Tests
{
    using System;
    using System.Collections.Generic;

    internal class Counter<TException> where TException : Exception, new()
    {
        private readonly int count;

        internal Counter(int count) => this.count = count;

        internal List<DateTime> Time { get; } = new List<DateTime>();

        internal void Increase()
        {
            this.Time.Add(DateTime.Now);
            if (this.Time.Count < this.count)
            {
                throw new TException();
            }
        }
    }
}
