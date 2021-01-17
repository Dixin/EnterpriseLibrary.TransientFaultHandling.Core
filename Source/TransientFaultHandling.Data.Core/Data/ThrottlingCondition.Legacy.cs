namespace Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling.Data
{
    using System;
    using System.Data.SqlClient;
    using System.Text.RegularExpressions;

    /// <summary>
    /// Implements an object that holds the decoded reason code returned from SQL Database when throttling conditions are encountered.
    /// </summary>
    public partial class ThrottlingCondition
    {
        /// <summary>
        /// Determines throttling conditions from the specified SQL exception.
        /// </summary>
        /// <param name="ex">The <see cref="SqlException"/> object that contains information relevant to an error returned by SQL Server when throttling conditions were encountered.</param>
        /// <returns>An instance of the object that holds the decoded reason codes returned from SQL Database when throttling conditions were encountered.</returns>
        [Obsolete("Use FromException for Microsoft.Data.SqlClient.SqlException in Microsoft.Data.SqlClient.", false)]
        public static ThrottlingCondition FromException(SqlException ex)
        {
            if (ex is not null)
            {
                foreach (SqlError error in ex.Errors)
                {
                    if (error.Number == ThrottlingErrorNumber)
                    {
                        return FromError(error);
                    }
                }
            }

            return Unknown;
        }

        /// <summary>
        /// Determines the throttling conditions from the specified SQL error.
        /// </summary>
        /// <param name="error">The <see cref="SqlError"/> object that contains information relevant to a warning or error returned by SQL Server.</param>
        /// <returns>An instance of the object that holds the decoded reason codes returned from SQL Database when throttling conditions were encountered.</returns>
        [Obsolete("Use FromError for Microsoft.Data.SqlClient.SqlError in Microsoft.Data.SqlClient.", false)]
        public static ThrottlingCondition FromError(SqlError error)
        {
            if (error is not null)
            {
                Match match = SqlErrorCodeRegEx.Match(error.Message);

                if (match.Success && int.TryParse(match.Groups[1].Value, out int reasonCode))
                {
                    return FromReasonCode(reasonCode);
                }
            }

            return Unknown;
        }
    }
}
