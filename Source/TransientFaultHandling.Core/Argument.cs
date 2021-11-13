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
    /// <param name="argumentValue">The argument value to check.</param>
    /// <param name="argumentName">The name of the argument.</param>
    /// <returns>The return value should be ignored. It is intended to be used only when validating arguments during instance creation (for example, when calling the base constructor).</returns>
    public static string NotNullOrEmpty([NotNull] this string? argumentValue, [CallerArgumentExpression("argumentValue")] string argumentName = "")
    {
        if (string.IsNullOrEmpty(argumentValue))
        {
            throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, Resources.StringCannotBeEmpty, argumentName));
        }

        return argumentValue;
    }

    /// <summary>
    /// Checks an argument to ensure that it isn't null.
    /// </summary>
    /// <param name="argumentValue">The argument value to check.</param>
    /// <param name="argumentName">The name of the argument.</param>
    /// <returns>The return value should be ignored. It is intended to be used only when validating arguments during instance creation (for example, when calling the base constructor).</returns>
    public static T NotNull<T>([NotNull] this T? argumentValue, [CallerArgumentExpression("argumentValue")] string argumentName = "") where T : class
    {
        if (argumentValue is null)
        {
            throw new ArgumentNullException(argumentName);
        }

        return argumentValue;
    }

    /// <summary>
    /// Checks an argument to ensure that its 32-bit signed value isn't negative.
    /// </summary>
    /// <param name="argumentValue">The <see cref="T:System.Int32" /> value of the argument.</param>
    /// <param name="argumentName">The name of the argument for diagnostic purposes.</param>
    public static int NotNegative(this int argumentValue, [CallerArgumentExpression("argumentValue")] string argumentName = "")
    {
        if (argumentValue < 0)
        {
            throw new ArgumentOutOfRangeException(
                argumentName,
                argumentValue,
                string.Format(CultureInfo.CurrentCulture, Resources.ArgumentCannotBeNegative, argumentName));
        }

        return argumentValue;
    }

    /// <summary>
    /// Checks an argument to ensure that its 64-bit signed value isn't negative.
    /// </summary>
    /// <param name="argumentValue">The <see cref="T:System.Int64" /> value of the argument.</param>
    /// <param name="argumentName">The name of the argument for diagnostic purposes.</param>
    public static long NotNegative(this long argumentValue, [CallerArgumentExpression("argumentValue")] string argumentName = "")
    {
        if (argumentValue < 0L)
        {
            throw new ArgumentOutOfRangeException(
                argumentName,
                argumentValue,
                string.Format(CultureInfo.CurrentCulture, Resources.ArgumentCannotBeNegative, argumentName));
        }

        return argumentValue;
    }

    /// <summary>
    /// Checks an argument to ensure that its value doesn't exceed the specified ceiling baseline.
    /// </summary>
    /// <param name="argumentValue">The <see cref="T:System.Double" /> value of the argument.</param>
    /// <param name="ceilingValue">The <see cref="T:System.Double" /> ceiling value of the argument.</param>
    /// <param name="argumentName">The name of the argument for diagnostic purposes.</param>
    public static double NotGreaterThan(this double argumentValue, double ceilingValue, [CallerArgumentExpression("argumentValue")] string argumentName = "")
    {
        if (argumentValue > ceilingValue)
        {
            throw new ArgumentOutOfRangeException(
                argumentName,
                argumentValue,
                string.Format(CultureInfo.CurrentCulture, Resources.ArgumentCannotBeGreaterThanBaseline, argumentName, ceilingValue));
        }

        return argumentValue;
    }

    /// <summary>
    /// Checks an argument to ensure that its 64-bit signed value isn't negative.
    /// </summary>
    /// <param name="argumentValue">The <see cref="T:System.TimeSpan" /> value of the argument.</param>
    /// <param name="minValue">The min value.</param>
    /// <param name="maxValue">The max value.</param>
    /// <param name="argumentName">The name of the argument for diagnostic purposes.</param>
    public static TimeSpan InRange(this TimeSpan argumentValue, TimeSpan minValue, TimeSpan maxValue, [CallerArgumentExpression("argumentValue")] string argumentName = "")
    {
        if (argumentValue < minValue || argumentValue > maxValue)
        {
            throw new ArgumentOutOfRangeException(
                argumentName,
                argumentValue,
                string.Format(CultureInfo.CurrentCulture, Resources.ArgumentCannotBeOutOutRange, argumentName, minValue, maxValue));
        }

        return argumentValue;
    }

    /// <summary>
    /// Checks an argument to ensure that its 64-bit signed value isn't negative.
    /// </summary>
    /// <param name="argumentValue">The <see cref="T:System.TimeSpan" /> value of the argument.</param>
    /// <param name="argumentName">The name of the argument for diagnostic purposes.</param>
    public static TimeSpan NotNegative(this TimeSpan argumentValue, [CallerArgumentExpression("argumentValue")] string argumentName = "")
    {
        if (argumentValue < TimeSpan.Zero)
        {
            throw new ArgumentOutOfRangeException(
                argumentName,
                argumentValue,
                string.Format(CultureInfo.CurrentCulture, Resources.ArgumentCannotBeNegative, argumentName));
        }

        return argumentValue;
    }
}