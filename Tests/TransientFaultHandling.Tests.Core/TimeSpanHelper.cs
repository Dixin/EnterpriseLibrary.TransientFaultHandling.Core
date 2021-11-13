namespace Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling.Tests;

internal static class TimeSpanHelper
{
    internal static readonly long Delta = TimeSpan.FromSeconds(1).Ticks / 10L;

    internal static bool AlmostEquals(long a, long b) => Math.Abs(a - b) < Delta;
}