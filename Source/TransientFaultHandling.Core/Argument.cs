namespace Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling;

using System.Runtime.CompilerServices;

/// <summary>
/// Provides the argument validation methods.
/// </summary>
internal static class Argument
{
    /// <summary>
    /// Checks a string argument to ensure that it isn't null or empty.
    /// </summary>
    /// <param name="value">The argument value to check.</param>
    /// <param name="name">The name of the argument.</param>
    /// <returns>The return value should be ignored. It is intended to be used only when validating arguments during instance creation (for example, when calling the base constructor).</returns>
    public static string NotNullOrEmpty([NotNull] this string? value, [CallerArgumentExpression("value")] string name = "") =>
        string.IsNullOrEmpty(value)
            ? throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, Resources.StringCannotBeEmpty, name), name)
            : value;

    /// <summary>
    /// Checks an argument to ensure that it isn't null.
    /// </summary>
    /// <param name="value">The argument value to check.</param>
    /// <param name="name">The name of the argument.</param>
    /// <returns>The return value should be ignored. It is intended to be used only when validating arguments during instance creation (for example, when calling the base constructor).</returns>
    public static T NotNull<T>([NotNull] this T? value, [CallerArgumentExpression("value")] string name = "") where T : class =>
        value ?? throw new ArgumentNullException(name);

    /// <summary>
    /// Checks an argument to ensure that its 32-bit signed value isn't negative.
    /// </summary>
    /// <param name="value">The <see cref="T:System.Int32" /> value of the argument.</param>
    /// <param name="name">The name of the argument for diagnostic purposes.</param>
    public static int NotNegative(this int value, [CallerArgumentExpression("value")] string name = "") =>
        value < 0
            ? throw new ArgumentOutOfRangeException(name, value, string.Format(CultureInfo.CurrentCulture, Resources.ArgumentCannotBeNegative, name))
            : value;

    /// <summary>
    /// Checks an argument to ensure that its 64-bit signed value isn't negative.
    /// </summary>
    /// <param name="value">The <see cref="T:System.Int64" /> value of the argument.</param>
    /// <param name="name">The name of the argument for diagnostic purposes.</param>
    public static long NotNegative(this long value, [CallerArgumentExpression("value")] string name = "") =>
        value < 0L
            ? throw new ArgumentOutOfRangeException(name, value, string.Format(CultureInfo.CurrentCulture, Resources.ArgumentCannotBeNegative, name))
            : value;

    /// <summary>
    /// Checks an argument to ensure that its value doesn't exceed the specified ceiling baseline.
    /// </summary>
    /// <param name="value">The <see cref="T:System.Double" /> value of the argument.</param>
    /// <param name="ceiling">The <see cref="T:System.Double" /> ceiling value of the argument.</param>
    /// <param name="name">The name of the argument for diagnostic purposes.</param>
    public static double NotGreaterThan(this double value, double ceiling, [CallerArgumentExpression("value")] string name = "") =>
        value > ceiling
            ? throw new ArgumentOutOfRangeException(name, value, string.Format(CultureInfo.CurrentCulture, Resources.ArgumentCannotBeGreaterThanBaseline, name, ceiling))
            : value;

    /// <summary>
    /// Checks an argument to ensure that its 64-bit signed value isn't negative.
    /// </summary>
    /// <param name="value">The <see cref="T:System.TimeSpan" /> value of the argument.</param>
    /// <param name="min">The min value.</param>
    /// <param name="max">The max value.</param>
    /// <param name="name">The name of the argument for diagnostic purposes.</param>
    public static TimeSpan InRange(this TimeSpan value, TimeSpan min, TimeSpan max, [CallerArgumentExpression("value")] string name = "") =>
        value < min || value > max
            ? throw new ArgumentOutOfRangeException(name, value, string.Format(CultureInfo.CurrentCulture, Resources.ArgumentCannotBeOutOutRange, name, min, max))
            : value;

    /// <summary>
    /// Checks an argument to ensure that its 64-bit signed value isn't negative.
    /// </summary>
    /// <param name="value">The <see cref="T:System.TimeSpan" /> value of the argument.</param>
    /// <param name="name">The name of the argument for diagnostic purposes.</param>
    public static TimeSpan NotNegative(this TimeSpan value, [CallerArgumentExpression("value")] string name = "") =>
        value < TimeSpan.Zero
            ? throw new ArgumentOutOfRangeException(name, value, string.Format(CultureInfo.CurrentCulture, Resources.ArgumentCannotBeNegative, name))
            : value;
}