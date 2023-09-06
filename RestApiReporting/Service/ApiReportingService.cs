namespace RestApiReporting.Service;

/// <summary>Reporting API report service</summary>
public sealed class ApiReportingService : IApiReportingService
{
    public ApiReportingService(IApiQueryService query, IApiReportService report)
    {
        Query = query ?? throw new ArgumentNullException(nameof(query));
        Report = report ?? throw new ArgumentNullException(nameof(report));
    }

    /// <summary>API query service</summary>
    public IApiQueryService Query { get; }

    /// <summary>API report service</summary>
    public IApiReportService Report { get; }
}