namespace RestApiReporting.Service;

/// <summary>Report</summary>
public interface IReport
{
    /// <summary>The report name</summary>
    string Name { get; }

    /// <summary>The report description</summary>
    string? Description { get; }

    /// <summary>Supported report cultures, undefined is any culture</summary>
    IList<string>? SupportedCultures { get; }

    /// <summary>Report parameters</summary>
    IList<ApiMethodParameter>? Parameters { get; }

    /// <summary>Build a report</summary>
    /// <param name="queryService">The report query service</param>
    /// <param name="request">The report request</param>
    /// <returns>The report response including the data set</returns>
    Task<ReportResponse> BuildAsync(IApiQueryService queryService, ReportRequest request);
}