﻿#if !NET5_0_OR_GREATER
namespace System.Runtime.CompilerServices;

/// <summary>
/// Allows capturing of the expressions passed to a method.
/// </summary>
[AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = false)]
internal sealed class CallerArgumentExpressionAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="T:System.Runtime.CompilerServices.CallerArgumentExpressionAttribute" /> class.
    /// </summary>
    /// <param name="parameterName">The name of the targeted parameter.</param>
    public CallerArgumentExpressionAttribute(string parameterName) => this.ParameterName = parameterName;

    /// <summary>
    /// Gets the target parameter name of the <c>CallerArgumentExpression</c>.
    /// </summary>
    /// <returns>
    /// The name of the targeted parameter of the <c>CallerArgumentExpression</c>.
    /// </returns>
    public string ParameterName { get; }
}
#endif