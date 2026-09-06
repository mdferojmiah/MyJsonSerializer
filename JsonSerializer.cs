using System.Collections;
using System.Globalization;
using System.Text;

namespace MyJsonSerializer;

public static class JsonSerializer
{
    public static string Serialize(object? obj)
    {
        if(obj == null)
        {
            return "null";
        }

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
}