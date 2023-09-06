using System.Reflection;

namespace RestApiReporting;

/// <summary>Query filter</summary>
public sealed class QueryFilter
{
    /// <summary>The assembly filter function</summary>
    public Func<Assembly, bool>? AssemblyFilter { get; set; }

    /// <summary>The type filter function</summary>
    public Func<Type, bool>? TypeFilter { get; set; }

    /// <summary>The method filter function</summary>
    public Func<MethodInfo, bool>? MethodFilter { get; set; }

    /// <summary>Add report services</summary>
    /// <param name="assemblyFilter">The controller assembly filter</param>
    /// <param name="typeFilter">The controller  type filter</param>
    /// <param name="methodFilter">The controller  method filter</param>
    public QueryFilter(
        Func<Assembly, bool>? assemblyFilter = null,
        Func<Type, bool>? typeFilter = null,
        Func<MethodInfo, bool>? methodFilter = null)
    {
        AssemblyFilter = assemblyFilter;
        TypeFilter = typeFilter;
        MethodFilter = methodFilter;
    }
}