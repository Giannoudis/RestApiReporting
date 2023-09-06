using Microsoft.Extensions.DependencyInjection;

namespace RestApiReporting.Service;

/// <summary>Extensions for <see cref="IServiceCollection"/></summary>
public static class ServiceCollectionExtensions
{
    /// <summary>Add reporting query service only</summary>
    /// <param name="serviceCollection">The service collection</param>
    /// <param name="queryFilter">The query filter</param>
    /// <returns>The service collections</returns>
    public static IServiceCollection AddReportingQuery(this IServiceCollection serviceCollection,
        QueryFilter? queryFilter = null) =>
        AddReporting(serviceCollection, queryFilter, new ReportFilter { LoadReports = false });

    /// <summary>Add reporting service</summary>
    /// <param name="serviceCollection">The service collection</param>
    /// <param name="queryFilter">The query filter</param>
    /// <param name="reportFilter">The report filter</param>
    /// <returns>The service collections</returns>
    public static IServiceCollection AddReporting(this IServiceCollection serviceCollection,
        QueryFilter? queryFilter = null,
        ReportFilter? reportFilter = null)
    {
        // query service (static controller reflection)
        var apiQueryService = new ApiQueryService(queryFilter);
        serviceCollection.AddSingleton<IApiQueryService>(apiQueryService);

        // register query controllers
        foreach (var controller in apiQueryService.Controllers)
        {
            serviceCollection.AddTransient(controller);
        }

        // reporting service (transient report reflection)
        serviceCollection.AddTransient<IApiReportingService>(x => new ApiReportingService(
            x.GetRequiredService<IApiQueryService>(),
            new ApiReportService(x.GetRequiredService<IApiQueryService>(), reportFilter)));
        return serviceCollection;
    }
}