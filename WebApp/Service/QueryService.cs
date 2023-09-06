using System.Text.Json;

namespace RestApiReporting.WebApp.Service;

public class QueryService : IQueryService
{
    protected HttpClient HttpClient { get; }

    protected readonly JsonSerializerOptions serializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public QueryService(HttpClient httpClient)
    {
        HttpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    }

    public async Task<List<ApiMethod>> GetQueriesAsync()
    {
        var uri = "reporting/queries";
        using var response = await HttpClient.GetAsync(uri);
        response.EnsureSuccessStatusCode();
        var json = await response.Content.ReadAsStringAsync();
        if (string.IsNullOrWhiteSpace(json))
        {
            return new();
        }
        var reports = JsonSerializer.Deserialize<List<ApiMethod>>(json, serializerOptions);
        return reports ?? new();
    }

    public async Task<ReportResponse?> ExecuteQueryAsync(
        string methodName,
        Dictionary<string, string>? parameters = null)
    {
        if (string.IsNullOrWhiteSpace(methodName))
        {
            throw new ArgumentException(nameof(methodName));
        }

        // url parameters
        var uri = $"reporting/queries/{methodName}/execute";
        var first = true;
        if (parameters != null)
        {
            foreach (var parameter in parameters)
            {
                if (string.IsNullOrWhiteSpace(parameter.Value))
                {
                    continue;
                }
                uri += first ? '?' : '&';
                first = false;
                uri += $"{parameter.Key}={parameter.Value}";
            }
        }

        using var response = await HttpClient.GetAsync(uri);

        // error handling
        if (!response.IsSuccessStatusCode)
        {
            var error = response.Content.ReadAsStringAsync().Result;
            if (!string.IsNullOrWhiteSpace(error))
            {
                throw new ReportException(error);
            }
        }
        response.EnsureSuccessStatusCode();

        // result
        var json = await response.Content.ReadAsStringAsync();
        if (string.IsNullOrWhiteSpace(json))
        {
            return null;
        }
        var reports = JsonSerializer.Deserialize<ReportResponse>(json, serializerOptions);
        return reports;
    }
}