namespace RestApiReporting;

/// <summary>REST API method parameter description</summary>
public class ApiMethodParameter
{
    /// <summary>The parameter name</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>The parameter type</summary>
    public string Type { get; set; } = string.Empty;

    /// <summary>Required parameter</summary>
    public bool Required { get; set; }

    /// <summary>Nullable parameter</summary>
    public bool Nullable { get; set; }

    /// <summary>The parameter value as JSON</summary>
    public string? Value { get; set; }

    public override string ToString() =>
        Value != null ? $"{Name}={Value}" : Name;
}
