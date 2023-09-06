namespace RestApiReporting.WebApp.Service;

public interface IReportingService : IQueryService
{
    Task<List<ReportInfo>> GetReportsAsync();

    Task<ReportResponse?> BuildReportAsync(string reportName,
        string? culture = null,
        Dictionary<string, string>? parameters = null);
}
