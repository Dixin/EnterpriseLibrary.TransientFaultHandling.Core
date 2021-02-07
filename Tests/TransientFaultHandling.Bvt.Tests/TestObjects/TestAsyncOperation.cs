namespace Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling.Bvt.Tests.TestObjects
{
    using System;
    using System.Threading;

    public class TestAsyncOperation
    {
        public bool ThrowFatalExceptionAtBegin = false;
        public int BeginMethodCount { get; private set; }
        public int EndMethodCount { get; private set; }
        public Exception ExceptionToThrowAtEnd { get; set; }
        public Exception ExceptionToThrowAtBegin { get; set; }
        public Exception FatalException { get; set; }
        public int CountToThrowAtBegin { get; set; }
        public int CountToThrowAtEnd { get; set; }

        public bool ThrowException { get; set; }

        public TestAsyncOperation(bool throwException = false) => this.ThrowException = throwException;

        public IAsyncResult BeginMethod(AsyncCallback callback, object state)
        {
            this.BeginMethodCount++;
            if (this.BeginMethodCount < this.CountToThrowAtBegin)
            {
                if (this.ExceptionToThrowAtBegin != null)
                {
                    throw this.ExceptionToThrowAtBegin;
                }
            }

            if (this.ThrowFatalExceptionAtBegin)
            {
                if (this.FatalException != null)
                {
                    Console.WriteLine("Throwing exception of type {0}", this.FatalException.GetType().ToString());
                    throw this.FatalException;
                }
            }

            TestAsyncResult asyncResult = new();
            ThreadPool.QueueUserWorkItem(_ => callback(asyncResult), null);
            return asyncResult;
        }

        public void EndMethod(IAsyncResult asyncResult)
        {
            this.EndMethodCount++;
            if (this.EndMethodCount < this.CountToThrowAtEnd)
            {
                if (this.ExceptionToThrowAtEnd != null)
                {
                    throw this.ExceptionToThrowAtEnd;
                }
            }

            if (this.FatalException != null)
            {
                throw this.FatalException;
            }
        }
    }

    public class TestAsyncResult : IAsyncResult
    {
        public bool IsCompleted { get; set; }

        public WaitHandle? AsyncWaitHandle { get; set; }

        public object AsyncState { get; set; }

        public bool CompletedSynchronously { get; set; }
    }
}
