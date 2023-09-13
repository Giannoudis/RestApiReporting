﻿using RestApiReporting.Service;
using RestApiReporting.WebApi.Model;
using RestApiReporting.WebApi.Service;

namespace RestApiReporting.WebApi.Reports;

/// <summary>Report generated by DI</summary>
public interface ITenantMemoryReport : IReport
{
}

// ignore report during the reporting api registration
[ReportingIgnore]
public class TenantMemoryReport : ITenantMemoryReport
{
    public ITenantService TenantService { get; }

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

    public TenantMemoryReport(ITenantService tenantService)
    {
        TenantService = tenantService;
    }

    public Task<ReportResponse> BuildAsync(
        IApiQueryService queryService, ReportRequest request)
    {
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

        var tenants = TenantService.GetTenants().Take(count);

        var reportDataSet = tenants
            .ToReportDataTable(primaryKey: nameof(Tenant.Id))
            .ToReportDataSet(dataSetName: "Tenants");
        return Task.FromResult(new ReportResponse(reportDataSet, request.Culture, request.Parameters));
    }
}