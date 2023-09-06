using RestApiReporting.Service;

namespace RestApiReporting.WebApi.Reports;

public class DotNetTypesReport : IReport
{
    private const string methodName = "GetDotNetTypes";

    public string Name => "DotNetTypes";
    public string Description => "Report the .NET basic types";
    public IList<string>? SupportedCultures => null;
    public IList<ApiMethodParameter>? Parameters => null;

    /// <inheritdoc />
    public async Task<ReportResponse> BuildAsync(
        IApiQueryService queryService, ReportRequest request)
    {
        var resultDataSet = await queryService.QuerySetAsync(methodName,
            reportQuery: new ReportQuery(
                controllerContext: request.ControllerContext,
                methodName: methodName));
        return new ReportResponse(resultDataSet);
    }

}