﻿namespace Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling.TestSupport
{
    using Microsoft.Extensions.Configuration;

    public class ConfigurationHelper
    {
        public static string GetSetting(string settingName, string fileName = RetryConfiguration.DefaultConfigurationFile)
        {
            IConfiguration configuration = RetryConfiguration.GetConfiguration(fileName);
            return configuration[settingName];
        }
    }
}