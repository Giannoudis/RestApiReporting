using System.Reflection;

namespace RestApiReporting;

/// <summary>Property with value</summary>
internal sealed class PropertyValue
{
    /// <summary>The property</summary>
    internal PropertyInfo? Property { get; init; }

    /// <summary>The property value</summary>
    internal object? Value { get; init; }

    /// <summary>The dictionary key</summary>
    internal string? DictionaryKey { get; init; }

    public override string ToString() =>
        $"{Property?.Name}={Value}";
}