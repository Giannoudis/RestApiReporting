namespace RestApiReporting.Service;

/// <summary>API report service</summary>
public interface IApiReportService
{
    /// <summary>Get reports</summary>
    Task<IEnumerable<IReport>> GetReportsAsync(string? culture = null);

    /// <summary>Get report by type</summary>
    /// <param name="name">The report name</param>
    Task<IReport?> GetReportAsync(string name);

    /// <summary>Add report</summary>
    /// <param name="report">The report</param>
    Task AddReportAsync(IReport report);

    /// <summary>Remove report</summary>
    /// <param name="report">The report</param>
    Task<bool> RemoveReportAsync(IReport report);

    /// <summary>Build a report</summary>
    /// <param name="request">The report request</param>
    /// <returns>The report response including the data set</returns>
    Task<ReportResponse> BuildReportAsync(ReportRequest request);
}