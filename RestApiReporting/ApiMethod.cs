namespace RestApiReporting;

/// <summary>REST API method description</summary>
public class ApiMethod
{
    /// <summary>The controller name</summary>
    public string ControllerName { get; set; } = string.Empty;

    /// <summary>The method route</summary>
    public string Route { get; set; } = string.Empty;

    /// <summary>The report name</summary>
    public string MethodName { get; set; } = string.Empty;

    /// <summary>Report parameters</summary>
    public IList<ApiMethodParameter>? Parameters { get; set; } = new List<ApiMethodParameter>();

    public override string ToString() =>
        $"{MethodName} ({Route})";
}
