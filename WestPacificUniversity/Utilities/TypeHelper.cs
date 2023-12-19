using Microsoft.CSharp;
using Newtonsoft.Json.Linq;
using NuGet.Protocol;
using System.CodeDom;
using System.Globalization;
using System.Reflection;

namespace WestPacificUniversity.Utilities;

internal static class TypeHelper
{
    public static Type InferJTokenValueType(JToken jToken)
    {
        CheckArgument.ThrowIfNull(jToken, nameof(jToken));

        return
            jToken.Type switch
            {
                JTokenType.Array => typeof(JArray),
                JTokenType.Object => typeof(JObject),
                // jToken is a JValue in the following cases
                JTokenType.Null => typeof(object),
                JTokenType.Boolean => typeof(bool),
                JTokenType.Integer => typeof(int),
                JTokenType.Float => typeof(double),
                JTokenType.String => typeof(string),
                JTokenType.Raw => typeof(string),
                JTokenType.Date => typeof(DateTime),
                JTokenType.TimeSpan => typeof(TimeSpan),
                _ => throw new NotSupportedException()
            };
    }

    public static Type InferJArrayElementType(JArray jArray)
    {
        CheckArgument.ThrowIfNull(jArray, nameof(jArray));

        if (jArray.Count == 0 || jArray[0].Type == JTokenType.Null)
        {
            throw new InvalidDataException();
        }

        return InferJTokenValueType(jArray[0]);
    }

    public static object? ConvertTo(JToken jToken, Type outputType)
    {
        CheckArgument.ThrowIfNull(jToken, nameof (jToken));
        CheckArgument.ThrowIfNull(outputType, nameof (outputType));

        if (jToken.Type == JTokenType.Null)
        {
            return null;
        }

        if (jToken.Type is JTokenType.Object or JTokenType.Array)
        {
            return jToken.ToObject(outputType);
        }

        if (jToken is JValue value)
        {
            return Convert.ChangeType(value.Value, outputType);
        }

        throw new InvalidCastException();
    }

    /// <summary>
    /// Converts a value from its string representation to its type representation.
    /// </summary>
    public static TOutputType? ConvertTo<TOutputType>(string value)
    {
        CheckArgument.ThrowIfNullOrWhiteSpace(value, nameof(value));

        // Only support 1-dimension array represented as: [elem1, elem2, ...]
        if (typeof(TOutputType).IsSZArray)
        {
            string[] elements = value.Trim('[', ']').Split(',', StringSplitOptions.TrimEntries);

            Type elementType = typeof(TOutputType).GetElementType()!;
            var array = Array.CreateInstance(elementType, elements.Length);
            for (int i = 0; i < elements.Length; i++)
            {
                array.SetValue(Convert.ChangeType(elements[i].Trim('"', '\''), elementType), i);
            }
            return (TOutputType)Convert.ChangeType(array, typeof(TOutputType));
        }
        else
        {
            if (typeof(DateTime) == typeof(TOutputType))
            {
                var datetime = DateTime.Parse(value, CultureInfo.InvariantCulture);
                return (TOutputType)Convert.ChangeType(datetime, typeof(TOutputType));
            }

            if (typeof(TimeSpan) == typeof(TOutputType))
            {
                var timespan = TimeSpan.Parse(value, CultureInfo.InvariantCulture);
                return (TOutputType)Convert.ChangeType(timespan, typeof(TOutputType));
            }
        }

        return (TOutputType?)Convert.ChangeType(value, typeof(TOutputType));
    }

    public static string GetFriendlyTypeName(Type clrType)
    {
        CheckArgument.ThrowIfNull(clrType, nameof(clrType));

        using var compiler = new CSharpCodeProvider();
        var typeReference = new CodeTypeReference(clrType);
        return compiler.GetTypeOutput(typeReference);
    }

    public static bool IsPrimitiveExtendedIncludingNullable(Type type, bool includeEnums = false)
    {
        CheckArgument.ThrowIfNull(type, nameof(type));

        if (IsPrimitiveExtended(type, includeEnums))
        {
            return true;
        }

        if (type.GetTypeInfo().IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
        {
            return IsPrimitiveExtended(type.GenericTypeArguments[0], includeEnums);
        }

        return false;
    }

    private static bool IsPrimitiveExtended(Type type, bool includeEnums)
    {
        if (type.GetTypeInfo().IsPrimitive)
        {
            return true;
        }

        if (includeEnums && type.GetTypeInfo().IsEnum)
        {
            return true;
        }

        return type == typeof(string) ||
               type == typeof(decimal) ||
               type == typeof(DateTime) ||
               type == typeof(DateTimeOffset) ||
               type == typeof(TimeSpan) ||
               type == typeof(Guid);
    }
}
