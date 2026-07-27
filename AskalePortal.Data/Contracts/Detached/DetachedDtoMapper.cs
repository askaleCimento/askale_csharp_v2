#nullable enable
using System.Collections;
using System.Reflection;
using System.Text.Json;

namespace AskalePortal.Data.Contracts.Detached;

public static class DetachedDtoMapper
{
    private const string EntityNamespace = "AskalePortal.Data.Models";
    private const string DtoNamespace = "AskalePortal.Data.Contracts.Detached";

    public static TEntity ToEntity<TEntity>(this object source)
        where TEntity : class, new()
    {
        ArgumentNullException.ThrowIfNull(source);
        var target = new TEntity();
        CopyMatchingScalarProperties(source, target);
        return target;
    }

    public static void ApplyToEntity<TEntity>(this object source, TEntity target)
        where TEntity : class
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(target);
        CopyMatchingScalarProperties(source, target);
    }

    public static object? ToDetached(object? value)
    {
        return ToDetached(value, new HashSet<object>(ReferenceEqualityComparer.Instance), 0);
    }

    private static object? ToDetached(object? value, HashSet<object> visited, int depth)
    {
        if (value is null) return null;
        var type = value.GetType();
        if (IsScalar(type)) return value;
        if (depth > 12) return null;

        if (!type.IsValueType && !visited.Add(value)) return null;

        if (value is IDictionary dictionary)
        {
            var result = new Dictionary<string, object?>();
            foreach (DictionaryEntry item in dictionary)
                result[Convert.ToString(item.Key) ?? string.Empty] = ToDetached(item.Value, visited, depth + 1);
            return result;
        }

        if (value is IEnumerable enumerable && value is not string)
        {
            var result = new List<object?>();
            foreach (var item in enumerable)
                result.Add(ToDetached(item, visited, depth + 1));
            return result;
        }

        if (type.Namespace == EntityNamespace)
        {
            var dtoType = type.Assembly
                .GetReferencedAssemblies()
                .Select(Assembly.Load)
                .Append(typeof(DetachedDtoMapper).Assembly)
                .Select(a => a.GetType($"{DtoNamespace}.{type.Name}Dto", throwOnError: false))
                .FirstOrDefault(t => t is not null);

            if (dtoType is null) return ScalarDictionary(value, visited, depth);
            var dto = Activator.CreateInstance(dtoType)!;
            CopyMatchingScalarProperties(value, dto);
            return dto;
        }

        // Existing request/response DTOs can still contain entity navigation
        // properties. Rebuild them as a detached dictionary recursively.
        return ScalarDictionary(value, visited, depth);
    }

    private static Dictionary<string, object?> ScalarDictionary(
        object source,
        HashSet<object> visited,
        int depth)
    {
        var result = new Dictionary<string, object?>();
        foreach (var property in source.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public))
        {
            if (!property.CanRead || property.GetIndexParameters().Length != 0) continue;
            object? propertyValue;
            try { propertyValue = property.GetValue(source); }
            catch { continue; }
            result[property.Name] = ToDetached(propertyValue, visited, depth + 1);
        }
        return result;
    }

    private static void CopyMatchingScalarProperties(object source, object target)
    {
        var sourceProperties = source.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public)
            .Where(p => p.CanRead && p.GetIndexParameters().Length == 0)
            .ToDictionary(p => p.Name, StringComparer.OrdinalIgnoreCase);

        foreach (var targetProperty in target.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public))
        {
            if (!targetProperty.CanWrite || !IsScalar(targetProperty.PropertyType)) continue;
            if (!sourceProperties.TryGetValue(targetProperty.Name, out var sourceProperty)) continue;
            if (!IsScalar(sourceProperty.PropertyType)) continue;
            var value = sourceProperty.GetValue(source);
            if (value is JsonElement jsonElement)
                value = ConvertJsonElement(jsonElement, targetProperty.PropertyType);

            if (value is null || targetProperty.PropertyType.IsInstanceOfType(value))
                targetProperty.SetValue(target, value);
        }
    }

    private static object? ConvertJsonElement(JsonElement element, Type targetType)
    {
        if (element.ValueKind is JsonValueKind.Null or JsonValueKind.Undefined) return null;
        var actualType = Nullable.GetUnderlyingType(targetType) ?? targetType;
        if (actualType == typeof(string)) return element.GetString();
        if (actualType == typeof(int)) return element.GetInt32();
        if (actualType == typeof(long)) return element.GetInt64();
        if (actualType == typeof(short)) return element.GetInt16();
        if (actualType == typeof(bool)) return element.GetBoolean();
        if (actualType == typeof(decimal)) return element.GetDecimal();
        if (actualType == typeof(double)) return element.GetDouble();
        if (actualType == typeof(float)) return element.GetSingle();
        if (actualType == typeof(DateTime)) return element.GetDateTime();
        if (actualType == typeof(DateTimeOffset)) return element.GetDateTimeOffset();
        if (actualType == typeof(Guid)) return element.GetGuid();
        if (actualType.IsEnum)
        {
            return element.ValueKind == JsonValueKind.String
                ? Enum.Parse(actualType, element.GetString()!, ignoreCase: true)
                : Enum.ToObject(actualType, element.GetInt32());
        }
        return JsonSerializer.Deserialize(element.GetRawText(), targetType);
    }

    private static bool IsScalar(Type type)
    {
        type = Nullable.GetUnderlyingType(type) ?? type;
        return type.IsPrimitive || type.IsEnum || type == typeof(string) ||
               type == typeof(decimal) || type == typeof(DateTime) ||
               type == typeof(DateTimeOffset) || type == typeof(TimeSpan) ||
               type == typeof(Guid) || type == typeof(byte[]);
    }
}
