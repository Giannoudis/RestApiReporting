using System.Reflection;

namespace RestApiReporting.Service;

/// <summary>Report filter</summary>
public sealed class ReportFilter
{
    /// <summary>Load reports filter</summary>
    public bool? LoadReports { get; set; }

    /// <summary>The assembly filter function</summary>
    public Func<Assembly, bool>? AssemblyFilter { get; set; }

    /// <summary>The type filter function</summary>
    public Func<Type, bool>? TypeFilter { get; set; }

    /// <summary>Add report services</summary>
    /// <param name="loadReports">Load reports filter</param>
    /// <param name="assemblyFilter">The controller assembly filter</param>
    /// <param name="typeFilter">The controller  type filter</param>
    public ReportFilter(
        bool? loadReports = true,
        Func<Assembly, bool>? assemblyFilter = null,
        Func<Type, bool>? typeFilter = null)
    {
        LoadReports = loadReports;
        AssemblyFilter = assemblyFilter;
        TypeFilter = typeFilter;
    }
}