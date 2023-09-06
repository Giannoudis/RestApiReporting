//#define LOG_STOPWATCH

namespace RestApiReporting.Service;

/// <summary>Report reflector</summary>
public class ReportReflector : ReflectorBase
{
    public ReportFilter? Filter { get; }

    public ReportReflector(ReportFilter? filter = null)
    {
        Filter = filter;
    }

    /// <summary>Loads queries on demand (GET endpoints)</summary>
    public Dictionary<string, IReport> GetReports()
    {
        var reports = new Dictionary<string, IReport>();
        if (Filter?.LoadReports != null && !Filter.LoadReports.Value)
        {
            return reports;
        }

#if LOG_STOPWATCH
        var stopwatch = new System.Diagnostics.Stopwatch();
        stopwatch.Start();
#endif

        // collect query methods from any assemblies, marked with 'query assembly attribute'
        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            // assembly filter
            if (AssemblyFilter(Filter?.AssemblyFilter, assembly))
            {
                continue;
            }

            var types = assembly.GetTypes();
            foreach (var type in types)
            {
                // ignore non-report types or abstract types
                if (type.GetInterface(nameof(IReport)) == null || type.IsAbstract)
                {
                    continue;
                }

                // type filter
                if (TypeFilter(Filter?.TypeFilter, type))
                {
                    continue;
                }

                // report instance
                if (Activator.CreateInstance(type) is IReport report)
                {
                    reports.TryAdd(report.Name, report);
                }
            }
        }

#if LOG_STOPWATCH
        stopwatch.Stop();
        System.Diagnostics.Debug.WriteLine($"Report reflector get reports: {stopwatch.ElapsedMilliseconds} ms");
#endif

        return reports;
    }
}