namespace Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling.Bvt.Tests.TestObjects
{
    using System;
    using Microsoft.Data.SqlClient;

    public sealed class FakeSqlAzureTransientErrorDetectionStrategy : ITransientErrorDetectionStrategy
    {
        public bool IsTransient(Exception ex)
        {
            if (ex is SqlException sqlException)
            {
                // Enumerate through all errors found in the exception.
                foreach (SqlError err in sqlException.Errors)
                {
                    switch (err.Number)
                    {
                        case 18054:
                            return true;
                    }
                }
            }

            return false;
        }
    }
}
