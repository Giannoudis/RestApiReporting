using RestApiReporting.Service;

namespace RestApiReporting.WebApi.Reports;

public class TenantEmployeeReport : IReport
{
    public string Name => "TenantEmployee";
    public string Description => "Employees by tenant";
    public IList<string>? SupportedCultures => null;
    public IList<ApiMethodParameter>? Parameters => null;

    public async Task<ReportResponse> BuildAsync(
        IApiQueryService queryService, ReportRequest request)
    {
        // tenants
        var tenants = await queryService.QueryAsync(
            new ReportQuery(
                controllerContext: request.ControllerContext,
                methodName: "GetTenants",
                primaryKey: "Id"));
        if (tenants == null)
        {
            return new ReportResponse(request);
        }

        // employees
        var employees = await queryService.QueryAsync(
            new ReportQuery(
                controllerContext: request.ControllerContext,
                methodName: "GetEmployees",
                primaryKey: "Id",
                parameters: request.Parameters));
        if (employees == null)
        {
            return new ReportResponse(request);
        }

        // data set
        var dataSet = new ReportDataSet(nameof(TenantEmployeeReport));
        dataSet.Tables.Add(tenants.ToReportDataTable());
        dataSet.Tables.Add(employees.ToReportDataTable());
        dataSet.Relations.Add(new()
        {
            Name = Name,
            ParentTable = tenants.TableName,
            ParentColumn = "Id",
            ChildTable = employees.TableName,
            ChildColumn = "TenantId"
        });

        return new ReportResponse(dataSet, request);
    }
}