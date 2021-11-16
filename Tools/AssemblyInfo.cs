using System.Security;

[assembly: CLSCompliant(true)]
[assembly: SecurityTransparent]

#if NETSTANDARD1_6 || NETSTANDARD2_0
// The NETSTANDARD1_6 and NETSTANDARD2_0 builds can be used by .NET Framework projects.
// Without this, when TransientFaultHandling.Data.Core is used by .NET Framework projects, it throws System.TypeAccessException while handling SqlException:
// Attempt by security transparent method 'Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling.SqlDatabaseTransientErrorDetectionStrategy.IsTransient(System.Exception)' to access security critical type 'Microsoft.Data.SqlClient.SqlException' failed.
// NETSTANDARD1_0 to NETSTANDARD1_5 does not support [SecurityRules]. And NETSTANDARD2_1_OR_GREATER is not supported by .NET Framework.
// https://docs.microsoft.com/en-us/dotnet/standard/net-standard#specification
[assembly: SecurityRules(SecurityRuleSet.Level1)]
#endif