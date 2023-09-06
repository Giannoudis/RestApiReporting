namespace RestApiReporting.Service;

/// <summary>API reporting service</summary>
public interface IApiReportingService
{
    /// <summary>API query service</summary>
    IApiQueryService Query { get; }

    /// <summary>API report service</summary>
    IApiReportService Report { get; }
}