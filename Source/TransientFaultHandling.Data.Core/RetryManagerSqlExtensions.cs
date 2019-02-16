﻿// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

namespace Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling
{
    using System;

    /// <summary>
    /// Extends the <see cref="RetryManager"/> class to use it with the SQL Database retry strategy.
    /// </summary>
    public static class RetryManagerSqlExtensions
    {
        /// <summary>
        /// The technology name that can be used to get the default SQL command retry strategy.
        /// </summary>
        public const string DefaultStrategyCommandTechnologyName = "SQL";

        /// <summary>
        /// The technology name that can be used to get the default SQL connection retry strategy.
        /// </summary>
        public const string DefaultStrategyConnectionTechnologyName = "SQLConnection";

        /// <summary>
        /// Returns the default retry strategy for SQL commands.
        /// </summary>
        /// <returns>The default retry strategy for SQL commands (or the default strategy, if no default could be found).</returns>
        public static RetryStrategy GetDefaultSqlCommandRetryStrategy(this RetryManager retryManager)
        {
            if (retryManager == null) throw new ArgumentNullException("retryManager");

            return retryManager.GetDefaultRetryStrategy(DefaultStrategyCommandTechnologyName);
        }

        /// <summary>
        /// Returns the default retry policy dedicated to handling transient conditions with SQL commands.
        /// </summary>
        /// <returns>The retry policy for SQL commands with the corresponding default strategy (or the default strategy, if no retry strategy assigned to SQL commands was found).</returns>
        public static RetryPolicy GetDefaultSqlCommandRetryPolicy(this RetryManager retryManager)
        {
            if (retryManager == null) throw new ArgumentNullException("retryManager");

            return new RetryPolicy(new SqlDatabaseTransientErrorDetectionStrategy(), retryManager.GetDefaultSqlCommandRetryStrategy());
        }

        /// <summary>
        /// Returns the default retry strategy for SQL connections.
        /// </summary>
        /// <returns>The default retry strategy for SQL connections (or the default strategy, if no default could be found).</returns>
        public static RetryStrategy GetDefaultSqlConnectionRetryStrategy(this RetryManager retryManager)
        {
            if (retryManager == null) throw new ArgumentNullException("retryManager");

            try
            {
                return retryManager.GetDefaultRetryStrategy(DefaultStrategyConnectionTechnologyName);
            }
            catch (ArgumentOutOfRangeException)
            {
                return retryManager.GetDefaultRetryStrategy(DefaultStrategyCommandTechnologyName);
            }
        }

        /// <summary>
        /// Returns the default retry policy dedicated to handling transient conditions with SQL connections.
        /// </summary>
        /// <returns>The retry policy for SQL connections with the corresponding default strategy (or the default strategy, if no retry strategy for SQL connections was found).</returns>
        public static RetryPolicy GetDefaultSqlConnectionRetryPolicy(this RetryManager retryManager)
        {
            if (retryManager == null) throw new ArgumentNullException("retryManager");

            return new RetryPolicy(new SqlDatabaseTransientErrorDetectionStrategy(), retryManager.GetDefaultSqlConnectionRetryStrategy());
        }
    }
}
