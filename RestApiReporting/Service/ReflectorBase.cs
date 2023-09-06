using System.Reflection;

namespace RestApiReporting.Service;

/// <summary>API base reflector</summary>
public abstract class ReflectorBase
{
    private const string SystemNamespace = $"{nameof(System)}.";

    /// <summary>Test the assembly filter</summary>
    /// <param name="filter">Assembly filter</param>
    /// <param name="assembly">The assembly</param>
    /// <returns>True if the assembly is filtered</returns>
    protected virtual bool AssemblyFilter(Func<Assembly, bool>? filter, Assembly assembly)
    {
        // ignore assembly attribute
        if (assembly.IsReportingIgnore())
        {
            return true;
        }

        if (filter != null)
        {
            if (filter(assembly) == false)
            {
                return true;
            }
        }
        else if (assembly.GetName().FullName.StartsWith(SystemNamespace))
        {
            // ignore system assemblies
            return true;
        }

        return false;
    }

    /// <summary>Test the type filter</summary>
    /// <param name="filter">Type filter</param>
    /// <param name="type">The type</param>
    /// <returns>True if the type is filtered</returns>
    protected virtual bool TypeFilter(Func<Type, bool>? filter, Type type)
    {
        // ignore type attribute
        if (type.IsReportingIgnore())
        {
            return true;
        }

        // type filter
        if (filter != null && filter(type) == false)
        {
            return true;
        }

        return false;
    }

    /// <summary>Test the method filter</summary>
    /// <param name="filter">Method filter</param>
    /// <param name="method">The method</param>
    /// <returns>True if the method is filtered</returns>
    protected virtual bool MethodFilter(Func<MethodInfo, bool>? filter, MethodInfo method)
    {
        // ignore method attribute
        if (method.IsReportingIgnore())
        {
            return true;
        }

        // method filter
        if (filter != null && filter(method) == false)
        {
            return true;
        }

        return false;
    }
}