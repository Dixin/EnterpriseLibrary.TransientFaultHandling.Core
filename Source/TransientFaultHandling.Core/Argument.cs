namespace Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling;

using System.Runtime.CompilerServices;

/// <summary>
/// Provides the argument validation methods.
/// </summary>
internal static class Argument
{
    /// <summary>
    /// Throws an exception if <paramref name="argument"/> is null or empty.
    /// </summary>
    /// <param name="argument">The string argument to validate as non-null and non-empty.</param>
    /// <param name="paramName">The name of the parameter with which <paramref name="argument"/> corresponds.</param>
    /// <exception cref="ArgumentNullException"><paramref name="argument"/> is null.</exception>
    /// <exception cref="ArgumentException"><paramref name="argument"/> is empty.</exception>
    /// <returns>The string argument to validate as non-null and non-empty.</returns>
    public static string ThrowIfNullOrEmpty([NotNull] this string? argument, [CallerArgumentExpression(nameof(argument))] string paramName = "") =>
        string.IsNullOrEmpty(argument.ThrowIfNull())
            ? throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, Resources.StringCannotBeEmpty, paramName), paramName)
            : argument;

    /// <summary>
    /// Throws an <see cref="ArgumentNullException"/> if <paramref name="argument"/> is null.
    /// </summary>
    /// <param name="argument">The reference type argument to validate as non-null.</param>
    /// <param name="paramName">The name of the parameter with which <paramref name="argument"/> corresponds.</param>
    /// <returns>The reference type argument to validate as non-null.</returns>
    public static T ThrowIfNull<T>([NotNull] this T? argument, [CallerArgumentExpression(nameof(argument))] string paramName = "") where T : class =>
        argument ?? throw new ArgumentNullException(paramName);

    /// <summary>
    /// Checks an argument to ensure that its 32-bit signed value isn't negative.
    /// </summary>
    /// <param name="value">The <see cref="T:System.Int32" /> value of the argument.</param>
    /// <param name="paramName">The name of the argument for diagnostic purposes.</param>
    public static int ThrowIfNegative(this int value, [CallerArgumentExpression(nameof(value))] string paramName = "") =>
        value < 0
            ? throw new ArgumentOutOfRangeException(paramName, value, string.Format(CultureInfo.CurrentCulture, Resources.ArgumentCannotBeNegative, paramName))
            : value;

    /// <summary>
    /// Throws an <see cref="ArgumentOutOfRangeException"/> if <paramref name="value"/> is negative.
    /// </summary>
    /// <param name="value">The argument to validate as non-negative.</param>
    /// <param name="paramName">The name of the parameter with which <paramref name="value"/> corresponds.</param>
    /// <returns>The argument to validate as non-negative.</returns>
    public static long ThrowIfNegative(this long value, [CallerArgumentExpression(nameof(value))] string paramName = "") =>
        value < 0L
            ? throw new ArgumentOutOfRangeException(paramName, value, string.Format(CultureInfo.CurrentCulture, Resources.ArgumentCannotBeNegative, paramName))
            : value;

    /// <summary>
    /// Throws an <see cref="ArgumentOutOfRangeException"/> if <paramref name="value"/> is greater than <paramref name="other"/>.
    /// </summary>
    /// <param name="value">The argument to validate as less or equal than <paramref name="other"/>.</param>
    /// <param name="other">The value to compare with <paramref name="value"/>.</param>
    /// <param name="paramName">The name of the parameter with which <paramref name="value"/> corresponds.</param>
    /// <returns>The argument to validate as less or equal than <paramref name="other"/>.</returns>
    public static T ThrowIfGreaterThan<T>(this T value, T other, [CallerArgumentExpression(nameof(value))] string paramName = "") where T : IComparable<T> =>
        value.CompareTo(other) > 0
            ? throw new ArgumentOutOfRangeException(paramName, value, string.Format(CultureInfo.CurrentCulture, Resources.ArgumentCannotBeGreaterThanBaseline, paramName, other))
            : value;

    /// <summary>
    /// Throws an <see cref="ArgumentOutOfRangeException"/> if <paramref name="value"/> is less than <paramref name="min"/> or greater than <paramref name="max"/>.
    /// </summary>
    /// <param name="value">The argument to validate as greater or equal than <paramref name="min"/> and less or equal than <paramref name="max"/>.</param>
    /// <param name="min">The min value to compare with <paramref name="value"/>.</param>
    /// <param name="max">The max value to compare with <paramref name="value"/>.</param>
    /// <param name="paramName">The name of the parameter with which <paramref name="value"/> corresponds.</param>
    /// <returns>The argument to validate as greater or equal than <paramref name="min"/> and less or equal than <paramref name="max"/>.</returns>
    public static T ThrowIfOutOfRange<T>(this T value, T min, T max, [CallerArgumentExpression(nameof(value))] string paramName = "") where T : IComparable<T> =>
        value.CompareTo(min) < 0 || value.CompareTo(max) > 0
            ? throw new ArgumentOutOfRangeException(paramName, value, string.Format(CultureInfo.CurrentCulture, Resources.ArgumentCannotBeOutOutRange, paramName, min, max))
            : value;

    /// <summary>
    /// Throws an <see cref="ArgumentOutOfRangeException"/> if <paramref name="value"/> is negative.
    /// </summary>
    /// <param name="value">The argument to validate as non-negative.</param>
    /// <param name="paramName">The name of the parameter with which <paramref name="value"/> corresponds.</param>
    /// <returns>The argument to validate as non-negative.</returns>
    public static TimeSpan ThrowIfNegative(this TimeSpan value, [CallerArgumentExpression(nameof(value))] string paramName = "") =>
        value < TimeSpan.Zero
            ? throw new ArgumentOutOfRangeException(paramName, value, string.Format(CultureInfo.CurrentCulture, Resources.ArgumentCannotBeNegative, paramName))
            : value;
}