using Microsoft.AspNetCore.Mvc;

namespace RestApiReporting;

/// <summary>Report request</summary>
public class ReportRequest
{
    /// <summary>The REST API controller context</summary>
    public ControllerContext ControllerContext { get; set; }

    /// <summary>The report name</summary>
    public string ReportName { get; set; }

    /// <summary>The report culture</summary>
    public string? Culture { get; set; }

    /// <summary>The report parameters</summary>
    public Dictionary<string, string>? Parameters { get; set; }

    /// <summary>Constructor</summary>
    /// <param name="controllerContext">The REST API controller context</param>
    /// <param name="reportName">The report name</param>
    /// <param name="culture">The report culture</param>
    /// <param name="parameters">The report parameters</param>
    public ReportRequest(ControllerContext controllerContext, string reportName,
        string? culture = null, Dictionary<string, string>? parameters = null)
    {
        ControllerContext = controllerContext ?? throw new ArgumentNullException(nameof(controllerContext));
        if (string.IsNullOrWhiteSpace(reportName))
        {
            throw new ArgumentException(nameof(reportName));
        }

        ReportName = reportName;
        Culture = culture;
        Parameters = parameters;
    }

    public override string ToString() => ReportName;
}