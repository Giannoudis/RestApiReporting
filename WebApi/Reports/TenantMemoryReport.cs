﻿using RestApiReporting.Service;
using RestApiReporting.WebApi.Model;

namespace RestApiReporting.WebApi.Reports;

public class TenantMemoryReport : IReport
{
    public string Name => "TenantMemory";
    public string Description => "List tenants from memory";
    public IList<string>? SupportedCultures => null;
    public IList<ApiMethodParameter> Parameters => new List<ApiMethodParameter>
    {
        new()
        {
            Name = "count",
            Type = typeof(int).FullName!,
            Value = "10"
        }
    };

    public Task<ReportResponse> BuildAsync(
        IApiQueryService queryService, ReportRequest request)
    {
        var tenants = new List<Tenant>();

        // parameter count
        var count = 10;
        if (request.Parameters != null)
        {
            var countParameter = request.Parameters.GetValueByName(nameof(count));
            if (countParameter != null)
            {
                int.TryParse(countParameter, out count);
            }
        }

        // test tenants
        for (var i = 1; i <= count; i++)
        {
            tenants.Add(new() { Id = $"T{i}", Name = $"Tenant {i}" });
        }

        var reportDataSet = tenants
            .ToReportDataTable(primaryKey: nameof(Tenant.Id))
            .ToReportDataSet(dataSetName: "Tenants");
        return Task.FromResult(new ReportResponse(reportDataSet, request.Culture, request.Parameters));
    }
}