//#define LOG_STOPWATCH
using System.Reflection;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;

namespace RestApiReporting.Service;

/// <summary>API query reflector</summary>
public class QueryReflector : ReflectorBase
{
    public QueryFilter? Filter { get; }

    public QueryReflector(QueryFilter? filter = null)
    {
        Filter = filter;
    }

    /// <summary>Loads queries on demand (GET endpoints)</summary>
    public Dictionary<string, Tuple<ControllerMethod, ApiMethod>> GetQueryMethods()
    {
        var queries = new Dictionary<string, Tuple<ControllerMethod, ApiMethod>>();

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

            // extract all query methods, excluding methods which are marked with the 'query ignore attribute'
            var types = assembly.GetTypes();
            foreach (var type in types)
            {
                // ignore non-controller types
                if (type.IsAbstract || !type.IsSubclassOf(typeof(ControllerBase)))
                {
                    continue;
                }

                // type filter
                if (TypeFilter(Filter?.TypeFilter, type))
                {
                    continue;
                }

                // process all GET endpoints
                var methods = type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy)
                    .Where(m => m.GetCustomAttributes(typeof(HttpGetAttribute), true).Any()).ToList();
                foreach (var method in methods)
                {
                    // ignore non-GET methods
                    var httpGetAttribute = method.GetCustomAttributes(typeof(HttpGetAttribute), false)
                        .FirstOrDefault() as HttpGetAttribute;
                    if (string.IsNullOrWhiteSpace(httpGetAttribute?.Name))
                    {
                        continue;
                    }

                    // method filter
                    if (MethodFilter(Filter?.MethodFilter, method))
                    {
                        continue;
                    }

                    // api method
                    var apiMethod = GetApiMethod(type, httpGetAttribute.Name, method);
                    queries.Add(httpGetAttribute.Name, new(new(type, method), apiMethod));
                }
            }
        }

#if LOG_STOPWATCH
        stopwatch.Stop();
        System.Diagnostics.Debug.WriteLine($"Report reflector get query methods: {stopwatch.ElapsedMilliseconds} ms");
#endif

        return queries;
    }

    protected virtual ApiMethod GetApiMethod(Type controllerType, string queryName, MethodInfo method)
    {
        // route
        var controllerRoute = controllerType.GetRoute();
        var methodRoute = method.GetRoute();
        var route = string.IsNullOrWhiteSpace(methodRoute) ? $"{controllerRoute}" : $"{controllerRoute}/{methodRoute}";

        var queryInfo = new ApiMethod
        {
            ControllerName = controllerType.Name,
            MethodName = queryName,
            Route = route,
            Parameters = new List<ApiMethodParameter>()
        };

        var methodParameters = method.GetParameters().ToList();
        foreach (var methodParameter in methodParameters)
        {
            if (!methodParameter.ParameterType.IsParameterType())
            {
                continue;
            }

            var parameterInfo = new ApiMethodParameter
            {
                Name = methodParameter.Name!,
                Required = !methodParameter.IsOptional,
                Type = GetParameterType(methodParameter.ParameterType),
                Nullable = methodParameter.IsNullable()
            };
            if (methodParameter.HasDefaultValue && methodParameter.DefaultValue != null)
            {
                parameterInfo.Value = JsonSerializer.Serialize(methodParameter.DefaultValue);
            }
            queryInfo.Parameters.Add(parameterInfo);
        }

        return queryInfo;
    }
    
    private static string GetParameterType(Type type)
    {
        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
        {
            return type.GenericTypeArguments[0].FullName!;
        }
        return type.FullName!;
    }
}