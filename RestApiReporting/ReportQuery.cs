using Microsoft.AspNetCore.Mvc;

namespace RestApiReporting;

/// <summary>Report query</summary>
public class ReportQuery
{
    /// <summary>The REST API controller context</summary>
    public ControllerContext ControllerContext { get; set; }

    /// <summary>The REST method name</summary>
    public string MethodName { get; set; }

    /// <summary>The table primary key column name</summary>
    public string? PrimaryKey { get; set; }

    /// <summary>The report parameters</summary>
    public Dictionary<string, string>? Parameters { get; set; }

    /// <summary>Strict handling of unknown report parameters</summary>
    public bool StrictParameters { get; set; }

    public ReportQuery(ControllerContext controllerContext, string methodName,
        string? primaryKey = null, Dictionary<string, string>? parameters = null,
        bool strictParameters = false)
    {
        ControllerContext = controllerContext ?? throw new ArgumentNullException(nameof(controllerContext));
        if (string.IsNullOrWhiteSpace(methodName))
        {
            throw new ArgumentException(nameof(methodName));
        }

        MethodName = methodName;
        PrimaryKey = primaryKey;
        Parameters = parameters;
        StrictParameters = strictParameters;
    }

    public override string ToString() => MethodName;
}