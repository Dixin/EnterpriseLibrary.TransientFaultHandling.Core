﻿namespace Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling.Bvt.Tests.TestObjects;

public class MockErrorDetectionStrategy : ITransientErrorDetectionStrategy
{
    public MockErrorDetectionStrategy()
    {
        this.CallCount = 0;
        this.ThreadIdList = new List<int>();
    }

    public bool IsTransient(Exception ex)
    {
        this.ThreadIdList.Add(Thread.CurrentThread.ManagedThreadId);
        ++this.CallCount;

        return this.IsTransientNonAggregate(ex is AggregateException ? ex.InnerException! : ex);
    }

    private bool IsTransientNonAggregate(Exception ex) =>
        ex switch
        {
            InvalidCastException => true,
            InvalidOperationException => true,
            SecurityException => true,
            _ => false
        };

    public int CallCount { get; set; }

    public List<int> ThreadIdList { get; set; }
}