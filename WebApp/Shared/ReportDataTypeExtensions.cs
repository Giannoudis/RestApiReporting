
namespace RestApiReporting.WebApp.Shared;

public static class ReportDataTypeExtensions
{
    /// <summary>Download report data set as json file</summary>
    /// <param name="typeName">The type name</param>
    /// <returns>The report data type</returns>
    public static ReportDataType ToReportDataType(this string? typeName)
    {
        var type = typeName != null ? Type.GetType(typeName) : null;
        if (type == null || type == typeof(string) || type == typeof(char))
        {
            return ReportDataType.String;
        }
        if (type == typeof(DateTime))
        {
            return ReportDataType.DateTime;
        }
        if (type == typeof(DateOnly))
        {
            return ReportDataType.DateTime;
        }
        if (type == typeof(byte) || type == typeof(sbyte) ||
            type == typeof(short) || type == typeof(ushort) ||
            type == typeof(int) || type == typeof(uint) ||
            type == typeof(long) || type == typeof(ulong) ||
            type.IsEnum)
        {
            return ReportDataType.Integer;
        }
        if (type == typeof(bool))
        {
            return ReportDataType.Boolean;
        }
        if (type == typeof(decimal) || type == typeof(float) || type == typeof(double))
        {
            return ReportDataType.Decimal;
        }

        // default
        return ReportDataType.String;
    }
}