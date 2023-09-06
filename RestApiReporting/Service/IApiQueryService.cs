using System.Data;

namespace RestApiReporting.Service;

/// <summary>Report API query service</summary>
public interface IApiQueryService
{
    /// <summary>The report API controllers</summary>
    IList<Type> Controllers { get; }

    /// <summary>The report API query method infos</summary>
    IDictionary<string, ApiMethod> Methods { get; }

    /// <summary>Get the method info</summary>
    /// <param name="name">The method name</param>
    ApiMethod? GetMethod(string name);

    /// <summary>Execute an api query to a data table</summary>
    /// <param name="reportQuery">The report query</param>
    /// <param name="itemsLoaded">Action after items</param>
    /// <returns>Data table including the report data</returns>
    Task<DataTable?> QueryAsync(ReportQuery reportQuery,
        Action<ReportQuery, IList<object>>? itemsLoaded = null);
}