using System.Reflection;
using System.Text.Json;

namespace RestApiReporting;

/// <summary>Information about a c# object</summary>
internal static class ObjectInfo
{
    private static readonly Dictionary<Type, List<PropertyInfo>> Properties = new();
    private static readonly object Locker = new();

    /// <summary>Get object properties</summary>
    /// <remarks>Use the local type cache</remarks>
    /// <param name="type">The object type</param>
    /// <returns>The object property infos</returns>
    internal static IList<PropertyInfo> GetProperties(Type type)
    {
        lock (Locker)
        {
            if (!Properties.ContainsKey(type))
            {
                Properties[type] = new(
                    type.GetProperties(BindingFlags.Public | BindingFlags.Instance));
            }
            return Properties[type];
        }
    }

    /// <summary>Resolve property expression</summary>
    /// <param name="item">The value item</param>
    /// <param name="expression">The property expression</param>
    /// <returns>Value with property</returns>
    internal static PropertyValue? ResolvePropertyValue(this object? item, string? expression)
    {
        while (true)
        {
            if (item == null || string.IsNullOrWhiteSpace(expression))
            {
                return null;
            }

            // item properties
            var itemProperties = GetProperties(item.GetType());
            if (!itemProperties.Any())
            {
                return null;
            }

            // child property
            var index = expression.IndexOf('.');
            var propertyName = expression;
            string? childExpression = null;
            if (index > 0)
            {
                propertyName = expression.Substring(0, index);
                childExpression = expression.Substring(index + 1);
            }

            // property
            var property = itemProperties.FirstOrDefault(x => string.Equals(x.Name, propertyName));
            if (property == null)
            {
                return null;
            }

            // child expression
            if (!string.IsNullOrWhiteSpace(childExpression) && IsDictionary(property))
            {
                return ResolveDictionaryPropertyValue(item, childExpression, property);
            }

            // final property
            object? value = property.GetValue(item, null);
            if (value == null)
            {
                return null;
            }

            // final object property
            if (string.IsNullOrWhiteSpace(childExpression))
            {
                return new PropertyValue
                {
                    Property = property,
                    Value = value
                };
            }

            // child property
            item = value;
            expression = childExpression;
        }
    }

    private static PropertyValue? ResolveDictionaryPropertyValue(object item, string childExpression, PropertyInfo property)
    {
        // test child expression and string/object dictionary
        if (string.IsNullOrWhiteSpace(childExpression) ||
            property.GetValue(item, null) is not IDictionary<string, object> dictionary)
        {
            return null;
        }

        // missing dictionary value
        if (!dictionary.ContainsKey(childExpression))
        {
            // missing dictionary value
            return new PropertyValue
            {
                Property = property,
                Value = null,
                DictionaryKey = childExpression
            };
        }

        // dictionary value
        var value = dictionary[childExpression];
        if (value is JsonElement jsonElement)
        {
            value = jsonElement.GetValue();
        }

        return new PropertyValue
        {
            Property = property,
            Value = value,
            DictionaryKey = childExpression
        };
    }

    private static bool IsDictionary(PropertyInfo property)
    {
        if (property.PropertyType.IsGenericType &&
            property.PropertyType.GetGenericTypeDefinition() == typeof(Dictionary<,>))
        {
            var keyType = property.PropertyType.GetGenericArguments()[0];
            var valueType = property.PropertyType.GetGenericArguments()[1];
            return keyType == typeof(string) && valueType == typeof(object);
        }
        return false;
    }
}