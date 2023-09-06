using Microsoft.AspNetCore.Mvc;

namespace RestApiReporting.Service;

/// <summary>Base class for reporting controller</summary>
public abstract class ReportingControllerBase : QueryControllerBase
{
    private IApiReportService ApiReportService { get; }

    protected ReportingControllerBase(IApiReportingService reportingService) :
        base(reportingService.Query)
    {
        ApiReportService = reportingService.Report;
    }

    /// <summary>Get report infos</summary>
    /// <param name="culture">The supported report culture (default: all)</param>
    [HttpGet("reports", Name = "GetReports")]
    public virtual async Task<ActionResult<IEnumerable<ReportInfo>>> GetReportsAsync(
        string? culture)
    {
        try
        {
            var reports = await ApiReportService.GetReportsAsync(culture);
            return Ok(reports);
        }
        catch (ReportException exception)
        {
            return BadRequest(exception.Message);
        }
    }

    /// <summary>Get report info</summary>
    /// <param name="name">The report name</param>
    [HttpGet("reports/{name}", Name = "GetReport")]
    public virtual async Task<ActionResult<ReportInfo>> GetReportAsync(string name)
    {
        try
        {
            var report = await ApiReportService.GetReportAsync(name);
            if (report == null)
            {
                return NotFound($"Unknown report {name}");
            }
            return Ok(report);
        }
        catch (ReportException exception)
        {
            return BadRequest(exception.Message);
        }
    }

    /// <summary>Build a report</summary>
    /// <param name="name">The report name</param>
    /// <param name="culture">The report culture</param>
    /// <param name="parameters">The report parameters</param>
    /// <returns>Excluded as reporting query method</returns>
    /// <returns>Report response including the data sets</returns>
    [ReportingIgnore]
    [HttpGet("reports/{name}/build", Name = "BuildReport")]
    public virtual async Task<ActionResult<ReportResponse>> BuildReportAsync(
        string name,
        [FromQuery] string? culture,
        [FromQuery] Dictionary<string, string>? parameters)
    {
        try
        {
            // report
            var report = await ApiReportService.GetReportAsync(name);
            if (report == null)
            {
                return NotFound($"Unknown report {name}");
            }

            // build report
            var response = await ApiReportService.BuildReportAsync(
                new ReportRequest(ControllerContext,
                    reportName: report.Name,
                    culture: culture,
                    parameters: parameters));
            return response;
        }
        catch (ReportException exception)
        {
            return BadRequest(exception.Message);
        }
    }
}