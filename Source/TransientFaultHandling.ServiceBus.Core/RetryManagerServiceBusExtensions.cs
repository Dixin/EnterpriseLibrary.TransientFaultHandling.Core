namespace Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling
{
    /// <summary>
    /// Provides extensions to the <see cref="RetryManager"/> class for using the Windows Azure Service Bus retry strategy.
    /// </summary>
    public static class RetryManagerServiceBusExtensions
    {
        /// <summary>
        /// The technology name that can be used to get default Service Bus retry strategy name.
        /// </summary>
        public const string DefaultStrategyTechnologyName = "ServiceBus";

        /// <summary>
        /// Returns the default retry strategy for the Windows Azure Service Bus.
        /// </summary>
        /// <returns>The default Windows Azure Service Bus retry strategy (or the default strategy if no default for Windows Azure Service Bus could be found).</returns>
        public static RetryStrategy GetDefaultAzureServiceBusRetryStrategy(this RetryManager retryManager)
        {
            Argument.NotNull(retryManager, nameof(retryManager));

            return retryManager.GetDefaultRetryStrategy(DefaultStrategyTechnologyName);
        }

        /// <summary>
        /// Returns the default retry policy dedicated to handling transient conditions with Windows Azure Service Bus.
        /// </summary>
        /// <returns>The retry policy for Windows Azure Service Bus with the corresponding default strategy (or the default strategy if no retry strategy definition for Windows Azure Service Bus was found).</returns>
        public static RetryPolicy GetDefaultAzureServiceBusRetryPolicy(this RetryManager retryManager)
        {
            Argument.NotNull(retryManager, nameof(retryManager));

            return new RetryPolicy(new ServiceBusTransientErrorDetectionStrategy(), retryManager.GetDefaultAzureServiceBusRetryStrategy());
        }
    }
}
