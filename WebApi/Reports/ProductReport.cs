using RestApiReporting.Service;
using RestApiReporting.WebApi.Service;

namespace RestApiReporting.WebApi.Reports;

public class ProductReport : IReport
{
    public string Name => "Product";
    public string Description => "List products from query";
    public IList<string>? SupportedCultures => null;
    public IList<ApiMethodParameter>? Parameters => null;

    public Task<ReportResponse> BuildAsync(
        IApiQueryService queryService, ReportRequest request)
    {
        // map products to dto
        var products = new ProductService().GetProducts();

        var dataSet = products
            .ToReportDataTable()
            .ToReportDataSet(dataSetName: "Products");

        return Task.FromResult(new ReportResponse(dataSet, request.Culture, request.Parameters));
    }
}