using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;

namespace RestApiReporting;

/// <summary>Extensions for <see cref="Type"/></summary>
public static class TypeExtensions
{
    private const string Controller = $"{nameof(Controller)}";

    /// <summary>Test for serialized type</summary>
    /// <param name="type">The underlying type</param>
    public static bool IsSerializedType(this Type type)
    {
        var nullableType = Nullable.GetUnderlyingType(type);
        var baseType = nullableType ?? type;
        return baseType != typeof(string) && !baseType.IsEnum && (baseType.IsArray || baseType.IsClass || baseType.IsGenericType);
    }

    /// <summary>Test for nullable parameter</summary>
    /// <param name="method">The parameter info</param>
    /// <returns></returns>
    public static bool IsNullable(this ParameterInfo method)
    {
        var nullable = Nullable.GetUnderlyingType(method.ParameterType) != null ||
                       method.CustomAttributes.Any(x => x.AttributeType.Name == "NullableAttribute") ||
                       method.CustomAttributes.Any(x => x.AttributeType.Name == "NullableContextAttribute");
        return nullable;
    }

    #region Reporting Attributes

    /// <summary>Test for method parameter type</summary>
    public static bool IsParameterType(this Type type) =>
        type.IsArray || type == typeof(string) || !type.IsClass;

    /// <summary>Test for report assembly using the <see cref="ReportingAttribute"/> </summary>
    public static bool IsReporting(this Assembly assembly) =>
        Attribute.IsDefined(assembly, typeof(ReportingAttribute));

    /// <summary>Test for report type using the <see cref="ReportingAttribute"/> </summary>
    public static bool IsReporting(this Type type) =>
        Attribute.IsDefined(type, typeof(ReportingAttribute));

    /// <summary>Test for report method using the <see cref="ReportingAttribute"/> </summary>
    public static bool IsReporting(this MethodInfo method) =>
        Attribute.IsDefined(method, typeof(ReportingAttribute));

    /// <summary>Test for excluded report assembly using the <see cref="ReportingIgnoreAttribute"/> </summary>
    public static bool IsReportingIgnore(this Assembly assembly) =>
        Attribute.IsDefined(assembly, typeof(ReportingIgnoreAttribute));

    /// <summary>Test for report type using the <see cref="ReportingIgnoreAttribute"/> </summary>
    public static bool IsReportingIgnore(this Type type) =>
        Attribute.IsDefined(type, typeof(ReportingIgnoreAttribute));

    /// <summary>Test for report method using the <see cref="ReportingIgnoreAttribute"/> </summary>
    public static bool IsReportingIgnore(this MethodInfo method) =>
        Attribute.IsDefined(method, typeof(ReportingIgnoreAttribute));

    #endregion

    #region Route

    /// <summary>Get the type route</summary>
    /// <param name="type">The type</param>
    public static string? GetRoute(this Type type)
    {
        var attribute = type.GetCustomAttribute<RouteAttribute>();
        if (attribute == null || string.IsNullOrWhiteSpace(attribute.Template))
        {
            return null;
        }

        var controllerName = type.Name.Replace(nameof(Controller), string.Empty);
        var route = attribute.Template.Replace($"[{nameof(Controller)}]", controllerName, StringComparison.OrdinalIgnoreCase);
        return route;
    }

    /// <summary>Get the method route</summary>
    /// <param name="method">The method info</param>
    public static string? GetRoute(this MethodInfo method)
    {
        var attribute = method.GetCustomAttribute<HttpMethodAttribute>();
        return attribute?.Template;
    }

    #endregion

}