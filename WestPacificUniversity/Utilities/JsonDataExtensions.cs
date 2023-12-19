using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace WestPacificUniversity.Utilities;

public static class JsonDataExtensions
{
    /// <summary>
    /// Gets the value of the given json property from the given json object.
    /// </summary>
    public static object? GetData(this JObject jsonData, string propertyName)
    {
        CheckArgument.ThrowIfNull(jsonData, nameof(jsonData));
        CheckArgument.ThrowIfNullOrWhiteSpace(propertyName, nameof(propertyName));

        if (jsonData.TryGetValue(propertyName, out JToken? value))
        {

            Type valueType = TypeHelper.InferJTokenValueType(value);
            if (TypeHelper.IsPrimitiveExtendedIncludingNullable(valueType))
            {
                return TypeHelper.ConvertTo(value, valueType);
            }

            return value;
        }

        return null;
    }

    /// <summary>
    /// Serializes the given property's value in a json object to an object of the specified type.
    /// </summary>
    public static T? GetData<T>(this JObject jsonData, string propertyName, JsonSerializer? jsonSerializer = null)
    {
        CheckArgument.ThrowIfNull(jsonData, nameof(jsonData));
        CheckArgument.ThrowIfNullOrWhiteSpace(propertyName, nameof(propertyName));

        if (jsonData.TryGetValue(propertyName, out JToken? value))
        {
            return
                TypeHelper.IsPrimitiveExtendedIncludingNullable(typeof(T))
                    ? value.Value<T>()
                    : (T?)value.ToObject(typeof(T), jsonSerializer ?? JsonSerializer.CreateDefault());
        }

        return default;
    }

    /// <summary>
    /// Serializes the element at given index in a json arry to an object of specified type.
    /// </summary>
    public static T? GetData<T>(this JArray jsonArray, int index, JsonSerializer? jsonSerializer = null)
    {
        CheckArgument.ThrowIfNull(jsonArray, nameof(jsonArray));
        CheckArgument.ThrowIfOutOfRange(idx => idx < 0 || idx >= jsonArray.Count, index, nameof(index));

        JToken item = jsonArray[index];

        return
            TypeHelper.IsPrimitiveExtendedIncludingNullable(typeof(T))
                ? item.Value<T>()
                : (T?)item.ToObject(typeof(T), jsonSerializer ?? JsonSerializer.CreateDefault());
    }

    /// <summary>
    /// Sets the property of the json object to the given value of the specified type.
    /// </summary>
    public static void SetData<T>(this JObject jsonData, string propertyName, T? value, JsonSerializer? jsonSerializer = null)
    {
        CheckArgument.ThrowIfNull(jsonData, nameof(jsonData));
        CheckArgument.ThrowIfNullOrWhiteSpace(propertyName, nameof(propertyName));

        if (value == null)
        {
            if (jsonData[propertyName] != null)
            {
                jsonData.Remove(propertyName);
            }
        }
        else
        {
            jsonData[propertyName] = TypeHelper.IsPrimitiveExtendedIncludingNullable(value.GetType())
                ? new JValue(value)
                : JToken.FromObject(value, jsonSerializer ?? JsonSerializer.CreateDefault());
        }
    }

    /// <summary>
    /// Sets the element at given index of a json array to the given value of the specified type.
    /// </summary>
    public static void SetData<T>(this JArray jsonArray, int index, T? value, JsonSerializer? jsonSerializer = null)
    {
        CheckArgument.ThrowIfNull(jsonArray, nameof(jsonArray));
        CheckArgument.ThrowIfOutOfRange(idx => idx < 0 || idx >= jsonArray.Count, index, nameof(index));

        if (value == null)
        {
            jsonArray.RemoveAt(index);
        }
        else
        {
            jsonArray[index] = TypeHelper.IsPrimitiveExtendedIncludingNullable(value.GetType())
                ? new JValue(value)
                : JToken.FromObject(value, jsonSerializer ?? JsonSerializer.CreateDefault());
        }
    }

    /// <summary>
    /// Adds the given value of the specified type to the json array.
    /// </summary>
    public static void AppendData<T>(this JArray jsonArray, T value, JsonSerializer? jsonSerializer = null)
    {
        CheckArgument.ThrowIfNull(jsonArray, nameof(jsonArray));

        if (value != null)
        {
            jsonArray.Add(
                TypeHelper.IsPrimitiveExtendedIncludingNullable(value.GetType())
                ? new JValue(value)
                : JToken.FromObject(value, jsonSerializer ?? JsonSerializer.CreateDefault())
            );
        }
    }

    /// <summary>
    /// Removes the specified property from a json object.
    /// </summary>
    public static bool RemoveData(this JObject jsonData, string propertyName)
    {
        CheckArgument.ThrowIfNull(jsonData, nameof(jsonData));
        CheckArgument.ThrowIfNullOrWhiteSpace(propertyName, nameof(propertyName));

        JToken? token = jsonData[propertyName];
        if (token != null)
        {
            jsonData.Remove(propertyName);
            return true;
        }

        return false;
    }

    /// <summary>
    /// Removes an element at the specified index from a json array.
    /// </summary>
    public static bool RemoveData(this JArray jsonArray, int index)
    {
        CheckArgument.ThrowIfNull(jsonArray, nameof(jsonArray));
        CheckArgument.ThrowIfOutOfRange(idx => idx < 0 || idx >= jsonArray.Count, index, nameof(index));

        jsonArray.RemoveAt(index);
        return true;
    }
}

