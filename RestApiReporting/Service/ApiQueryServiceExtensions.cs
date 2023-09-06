
namespace RestApiReporting.Service;

/// <summary>Extension for <see cref="IApiQueryService"/></summary>
public static class ApiQueryServiceExtensions
{

    /// <summary>Execute an api query to a data set</summary>
    /// <param name="queryService">The query service</param>
    /// <param name="dataSetName">The data set name</param>
    /// <param name="reportQuery">The report query</param>
    /// <param name="itemsLoaded">Action after items</param>
    /// <returns>Data set including the report data</returns>
    public static async Task<ReportDataSet?> QuerySetAsync(this IApiQueryService queryService,
        string dataSetName, ReportQuery reportQuery,
        Action<ReportQuery, IList<object>>? itemsLoaded = null)
    {
        // query table
        var table = await queryService.QueryAsync(
            reportQuery: new ReportQuery(
                controllerContext: reportQuery.ControllerContext,
                methodName: reportQuery.MethodName,
                parameters: reportQuery.Parameters),
            itemsLoaded: itemsLoaded);
        if (table == null)
        {
            return null;
        }

        // single table set
        var resultDataSet = new ReportDataSet(dataSetName);
        resultDataSet.Tables.Add(table.ToReportDataTable());
        return resultDataSet;
    }
}