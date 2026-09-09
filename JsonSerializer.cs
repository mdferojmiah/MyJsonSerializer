using System.Collections;
using System.Globalization;
using System.Text;

namespace MyJsonSerializer;

public static class JsonSerializer
{
    [ThreadStatic]
    private static HashSet<object>? _serializedObjects;
    private static HashSet<object> SerializedObjects
    {
        get
        {
            _serializedObjects ??= new HashSet<object>();
            return _serializedObjects;
        }
    }

    public static string Serialize(object? obj)
    {
        if (obj == null) return "null";

        if (SerializedObjects.Contains(obj))
            throw new InvalidOperationException(
                $"Circular reference detected for type '{obj.GetType().Name}'");

        SerializedObjects.Add(obj);

        try
        {
            return SerializeInternal(obj);
        }
        finally
        {
            SerializedObjects.Remove(obj);
        }
    }
    private static string SerializeInternal(object obj)
    {
        var type = obj.GetType();

        if (type == typeof(string))
        {
            return EscapeString(Convert.ToString(obj));   
        }
        else if (type == typeof(bool))
        {
            return obj.ToString()!.ToLowerInvariant();
        }
        else if (type == typeof(int) || type == typeof(long) || type == typeof(float) || type == typeof(double) || type == typeof(decimal))
        {
            return ((IFormattable)obj).ToString(null, CultureInfo.InvariantCulture);
        }
        else if (typeof(IDictionary).IsAssignableFrom(type))
        {
            return SerializeDictionary(obj);
        }
        else if (type != typeof(string) && typeof(IEnumerable).IsAssignableFrom(type))
        {
            return SerializeCollection(obj);
        }
        else
        {
            return SerializeObject(obj);
        }
    }

    private static string EscapeString(string? input)
    {
        StringBuilder result = new StringBuilder();
        result.Append('"');

        foreach (var ch in input!)
        {
            if (ch == '"') result.Append("\\\"");
            else if (ch == '\\') result.Append("\\\\");
            else if (ch == '\n') result.Append("\\n");
            else if (ch == '\t') result.Append("\\t");
            else if (ch == '\r') result.Append("\\r");
            else
            {
                result.Append(ch);
            }
        }

        result.Append('"');
        return result.ToString();
    }

    private static string SerializeObject(object obj)
    {
        StringBuilder json = new StringBuilder();
        json.Append('{');

        var type = obj.GetType();
        var properties = type.GetProperties();

        for (int i = 0; i < properties.Length; i++)
        {
            var propertyKey = $"\"{properties[i].Name}\"";
            json.Append(propertyKey);
            json.Append(':');
            var propertyValue = Serialize(properties[i].GetValue(obj));
            json.Append(propertyValue);
            if (i < properties.Length - 1) json.Append(',');
        }

        json.Append('}');
        return json.ToString();
    }

    private static string SerializeCollection(object obj)
    {
        var collection = (IEnumerable)obj;
        StringBuilder result = new StringBuilder();
        result.Append('[');

        bool first = true;
        foreach (var i in collection)
        {
            if(!first) result.Append(',');
            var item = Serialize(i);
            result.Append(item);
            first = false;
        }

        result.Append(']');
        return result.ToString();
    }

    private static string SerializeDictionary(object obj)
    {
        var dictionary = (IDictionary)obj;
        StringBuilder result = new StringBuilder();
        result.Append('{');

        bool first = true;
        var keys = dictionary.Keys;
        foreach (var k in keys)
        {
            if (!first) result.Append(',');
            var key = $"\"{k}\"";
            result.Append(key);
            result.Append(':');
            var value = Serialize(dictionary[k]);
            result.Append(value);
            first = false;
        }

        result.Append('}');
        return result.ToString();
    }


    // Deserializer
    public static T? Deserialize<T>(string json)
    {
        if (string.IsNullOrWhiteSpace(json))
        {
            throw new ArgumentException("Json can be null or empty");
        }

        var parser = new JsonParser(json);
        var parsed = parser.Parse();

        if (parsed == null && typeof(T).IsValueType)
            throw new Exception($"Cannot deserialize null to value type '{typeof(T)}'");

        if (parsed == null)
            return default;
        
        return (T?)ConvertValue(parsed, typeof(T));
    }

    private static object? ConvertValue(object? parsedValue, Type targetType)
    {
        if (parsedValue == null)
            return null;

        if (targetType.IsInstanceOfType(parsedValue))
            return parsedValue;
        
        var underlyingType = Nullable.GetUnderlyingType(targetType);
        if (underlyingType != null)
        {
            return ConvertValue(parsedValue, underlyingType);
        }

        if(targetType == typeof(string))
        {
            return parsedValue.ToString();
        }
        if(targetType == typeof(int))
        {
            return Convert.ToInt32(parsedValue);
        }
        if(targetType == typeof(long))
        {
            return Convert.ToInt64(parsedValue);
        }
        if(targetType == typeof(float))
        {
            return Convert.ToSingle(parsedValue);
        }
        if(targetType == typeof(double))
        {
            return Convert.ToDouble(parsedValue);
        }
        if(targetType == typeof(decimal))
        {
            return Convert.ToDecimal(parsedValue);
        }
        if(targetType == typeof(bool))
        {
            return Convert.ToBoolean(parsedValue);
        }
        if(targetType == typeof(DateTime))
        {
            return DateTime.Parse(parsedValue.ToString()!);
        }
        if(targetType == typeof(Guid))
        {
            return Guid.Parse(parsedValue.ToString()!);
        }
        if(targetType.IsEnum)
        {
            return Enum.Parse(targetType, parsedValue.ToString()!);
        }

        if(targetType.IsArray && parsedValue is List<object?> list)
        {
            Type elementType = targetType.GetElementType()!;
            Array array = Array.CreateInstance(elementType, list.Count);
            for(int i = 0; i < list.Count; i++)
            {
                array.SetValue(ConvertValue(list[i], elementType), i);
            }
            return array;
        }

        if(typeof(IList).IsAssignableFrom(targetType) && parsedValue is List<object?> list2)
        {
            Type elementType = targetType.GetGenericArguments()[0];
            var listInstance = (IList)Activator.CreateInstance(targetType)!;
            foreach (var item in list2)
            {
                listInstance.Add(ConvertValue(item, elementType));
            }

            return listInstance;
        }

        if(typeof(IDictionary).IsAssignableFrom(targetType) && parsedValue is Dictionary<string, object?> dictionary)
        {
            var keyType = targetType.GetGenericArguments()[0];
            var valueType = targetType.GetGenericArguments()[1];
            var dictionaryInstance = (IDictionary)Activator.CreateInstance(targetType)!;
            foreach (var pair in dictionary)
            {
                object key = ConvertValue(pair.Key, keyType)!;
                object value = ConvertValue(pair.Value, valueType)!;
                dictionaryInstance.Add(key, value);
            }

            return dictionaryInstance;
        }

        if(parsedValue is Dictionary<string, object?> objectDictionary)
        {
            return PopulateObject(targetType, objectDictionary);
        }

        throw new Exception();
    }

    private static object PopulateObject(Type targetType, Dictionary<string, object?> dict)
    {
        var instance = Activator.CreateInstance(targetType);
        if(instance == null)
        {
            throw new Exception($"Can't create instance of target type: {targetType}");
        }

        var properties = targetType.GetProperties();
        foreach (var property in properties)
        {
            var key = dict.Keys.FirstOrDefault(x => 
                string.Equals(x, property.Name, StringComparison.OrdinalIgnoreCase));
            
            if(key != null && dict.TryGetValue(key, out object? value))
            {
                var converted= ConvertValue(value, property.PropertyType);
                property.SetValue(instance, converted);
            }
        }

        return instance;
    }
}