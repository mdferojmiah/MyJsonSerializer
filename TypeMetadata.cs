using System.Collections;
using System.Collections.Concurrent;
using System.Reflection;

namespace MyJsonSerializer;



public static class MetadataCache
{
    private static readonly ConcurrentDictionary<Type, TypeMetadata> Cache = new();

    public static TypeMetadata Get(Type type) => Cache.GetOrAdd(type, Build);

    private static TypeMetadata Build(Type type)
    {
        var list = new List<PropertyMetadata>();
        var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
        foreach (var property in properties)
        {
            if(!property.CanRead || property.GetIndexParameters().Length > 0)
            {
                continue;
            }

            list.Add(new PropertyMetadata
            {
                Name = property.Name,
                NameToken = $"\"{property.Name}\"",
                PropertyType = property.PropertyType,
                Property = property
            });
        }

        var propertiesByName = new Dictionary<string, PropertyMetadata>(StringComparer.OrdinalIgnoreCase);
        foreach (var p in list)
        {
            propertiesByName.TryAdd(p.Name, p);
        }

        return new TypeMetadata()
        {
            Kind = ResoleKind(type),
            Properties = list.ToArray(),
            PropertiesByName = propertiesByName,
            NullableUnderlyingType = Nullable.GetUnderlyingType(type),
            ElementType = type.IsArray ? type.GetElementType() : null,
            GenericArguments = type.IsGenericType ? type.GetGenericArguments() : Type.EmptyTypes
        };
    }
    
    private static TypeKind ResoleKind(Type type)
    {
        if (type == typeof(string)) return TypeKind.String;
        if (type == typeof(bool)) return TypeKind.Boolean;
         if (type == typeof(DateTime)) return TypeKind.DateTime;
        if (type == typeof(Guid)) return TypeKind.Guid;
        if (type.IsEnum) return TypeKind.Enum;

        if (type == typeof(int) || type == typeof(long) || type == typeof(float)
            || type == typeof(double) || type == typeof(decimal)) return TypeKind.Number;

        if (typeof(IDictionary).IsAssignableFrom(type)) return TypeKind.Dictionary;
        if (typeof(IEnumerable).IsAssignableFrom(type)) return TypeKind.Collection;

        return TypeKind.Object;
    }
}



public class TypeMetadata
{
    public TypeKind Kind { get; set; }
    public PropertyMetadata[] Properties { get; set; } = [];
    public Dictionary<string, PropertyMetadata> PropertiesByName { get; set; } = new();
    public Type? NullableUnderlyingType { get; set; }
    public Type? ElementType { get; set; }
    public Type[] GenericArguments { get; set; } = Type.EmptyTypes;
}

public class PropertyMetadata
{
    public string Name { get; set; } = string.Empty;
    public string NameToken { get; set; } = string.Empty;
    public Type PropertyType { get; set; } = typeof(object);
    public PropertyInfo Property { get; set; } = null!;
}

public enum TypeKind
{
    String, Boolean, Number, DateTime, Guid, Enum, Dictionary, Collection, Object
}