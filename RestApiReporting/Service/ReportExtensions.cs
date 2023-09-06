namespace RestApiReporting.Service;

/// <summary>Extensions for <see cref="IReport"/></summary>
public static class ReportExtensions
{
    /// <summary>Test for serialized type</summary>
    /// <param name="report">The report</param>
    /// <param name="culture">The test culture</param>
    public static bool IsMatchingCulture(this IReport report, string? culture)
    {
        if (string.IsNullOrWhiteSpace(culture) || report.SupportedCultures == null)
        {
            return true;
        }
        return report.SupportedCultures.Any(x => x.IsMatchingCulture(culture));
    }
}