using System.Text.Json;

namespace RestApiReporting.WebApp.Service;

public class ReportingService : QueryService, IReportingService
{
    public ReportingService(HttpClient httpClient) :
        base(httpClient)
    {
    }

    public async Task<List<ReportInfo>> GetReportsAsync()
    {
        var uri = "reporting/reports";
        using var response = await HttpClient.GetAsync(uri);
        response.EnsureSuccessStatusCode();
        var json = await response.Content.ReadAsStringAsync();
        if (string.IsNullOrWhiteSpace(json))
        {
            return new();
        }
        var reports = JsonSerializer.Deserialize<List<ReportInfo>>(json, serializerOptions);
        return reports ?? new();
    }

    public async Task<ReportResponse?> BuildReportAsync(
        string reportName,
        string? culture = null,
        Dictionary<string, string>? parameters = null)
    {
        if (string.IsNullOrWhiteSpace(reportName))
        {
            throw new ArgumentException(nameof(reportName));
        }

        // url parameters
        var uri = $"reporting/reports/{reportName}/build";
        var first = true;
        if (!string.IsNullOrWhiteSpace(culture))
        {
            uri += $"?{nameof(culture)}={culture}";
            first = false;
        }
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

        // build report request
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
