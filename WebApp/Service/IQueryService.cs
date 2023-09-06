namespace RestApiReporting.WebApp.Service;

public interface IQueryService
{
    Task<List<ApiMethod>> GetQueriesAsync();

    Task<ReportResponse?> ExecuteQueryAsync(string methodName,
        Dictionary<string, string>? parameters = null);
}