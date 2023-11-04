using System.Diagnostics.CodeAnalysis;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace WestPacificUniversity.Utilities;

[DebuggerStepThrough]
public static class CheckArgument
{
    public static T ThrowIfNull<T>([NotNull] T? argument,
        [CallerArgumentExpression(nameof(argument))] string? paramName = null)
        where T : class
    {
        ArgumentNullException.ThrowIfNull(argument, paramName);
        return argument;
    }

    public static string ThrowIfNullOrEmpty([NotNull] string? argument,
        [CallerArgumentExpression(nameof(argument))] string? paramName = null)
    {
        // ArgumentException.ThrowIfNullOrEmpty(argument, paramName);
        if (string.IsNullOrEmpty(argument))
        {
            ArgumentNullException.ThrowIfNull(argument, paramName);
            throw new ArgumentException($"{paramName} cannot be an empty string.", paramName);
        }
        return argument;
    }

    public static string ThrowIfNullOrWhiteSpace([NotNull] string? argument,
        [CallerArgumentExpression(nameof(argument))] string? paramName = null)
    {
        if (string.IsNullOrWhiteSpace(argument))
        {
            ThrowIfNullOrEmpty(argument, paramName);
            throw new ArgumentException($"{paramName} cannot be whitespace only.", paramName);
        }
        return argument;
    }

    public static T ThrowIfOutOfRange<T>(Func<T, bool> outOfRangeDetect,
        T argument,
        [CallerArgumentExpression(nameof(argument))] string? paramName = null)
        // where T : IBinaryInteger<T>
    {
        if (outOfRangeDetect(argument))
        {
            throw new ArgumentOutOfRangeException(paramName, $"{paramName} is out of range.");
        }
        return argument;
    }
}
