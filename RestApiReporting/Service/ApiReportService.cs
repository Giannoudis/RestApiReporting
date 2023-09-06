namespace RestApiReporting.Service;

/// <summary>Reporting API report service implementation</summary>
public class ApiReportService : IApiReportService
{
    private IApiQueryService ApiQueryService { get; }

    /// <summary>The reports</summary>
    private Dictionary<string, IReport> Reports { get; }

    public ApiReportService(IApiQueryService queryService, ReportFilter? filter = null)
    {
        ApiQueryService = queryService ?? throw new ArgumentNullException(nameof(queryService));
        Reports = new ReportReflector(filter).GetReports();
    }

    /// <inheritdoc />
    public virtual Task<IReport?> GetReportAsync(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException(nameof(name));
        }
        var report = Reports.GetValueByName(name);
        return Task.FromResult(report);
    }

    /// <inheritdoc />
    public virtual Task<IEnumerable<IReport>> GetReportsAsync(string? culture) =>
        Task.FromResult(Reports.Values
            .Where(x => x.IsMatchingCulture(culture))
            .Select(x => x));

    /// <inheritdoc />
    public virtual Task AddReportAsync(IReport report)
    {
        if (report == null)
        {
            throw new ArgumentNullException(nameof(report));
        }
        if (Reports.ContainsKey(report.Name))
        {
            throw new ArgumentException($"Report already registered {report.Name}");
        }
        Reports.Add(report.Name, report);
        return Task.FromResult(report);
    }

    /// <inheritdoc />
    public virtual Task<bool> RemoveReportAsync(IReport report)
    {
        if (report == null)
        {
            throw new ArgumentNullException(nameof(report));
        }
        if (!Reports.ContainsKey(report.Name))
        {
            throw new ArgumentException($"Report is not registered {report.Name}");
        }
        return Task.FromResult(Reports.Remove(report.Name));
    }

    /// <inheritdoc />
    public virtual async Task<ReportResponse> BuildReportAsync(
        ReportRequest request)
    {
        var typeReport = Reports.FirstOrDefault(
            x => string.Equals(x.Value.Name, request.ReportName, StringComparison.OrdinalIgnoreCase));
        if (typeReport.Key == null)
        {
            throw new ReportException($"Unknown report {request.ReportName}");
        }

        // build the report
        var report = Reports[typeReport.Key];
        var response = await report.BuildAsync(ApiQueryService, request);
        return response;
    }
}