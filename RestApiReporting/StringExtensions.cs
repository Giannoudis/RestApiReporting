namespace RestApiReporting;

/// <summary>Extensions for <see cref="string"/></summary>
public static class StringExtensions
{
    /// <summary>Test if the date is midnight</summary>
    /// <param name="moment">The moment to test</param>
    /// <returns>True in case the moment is date</returns>
    public static bool IsMidnight(this DateTime moment) =>
        moment.TimeOfDay.Ticks == 0;

    /// <summary>Format a compact date (removes empty time parts)</summary>
    /// <param name="moment">The period start date</param>
    /// <returns>The formatted period start date</returns>
    public static string ToCompactString(this DateTime moment) =>
        IsMidnight(moment) ? moment.ToShortDateString() : $"{moment.ToShortDateString()} {moment.ToShortTimeString()}";

    /// <summary>Ensures first string character is upper</summary>
    /// <param name="value">The string value</param>
    /// <returns>String starting uppercase</returns>
    public static string FirstCharacterToUpper(this string value) =>
        value[0].ToString().ToUpper() + value.Substring(1);

    /// <summary>Test for matching cultures</summary>
    /// <param name="source">The report</param>
    /// <param name="test">The test culture</param>
    public static bool IsMatchingCulture(this string source, string? test)
    {
        if (string.IsNullOrWhiteSpace(source) || string.IsNullOrWhiteSpace(test))
        {
            return true;
        }

        source = source.Trim();
        test = test.Trim();
        if (string.Equals(source, test, StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        if (test.Length > source.Length && test.StartsWith(source, StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }
        return false;
    }

    /// <summary>Remove prefix from string</summary>
    /// <param name="source">The source value</param>
    /// <param name="prefix">The prefix to remove</param>
    /// <returns>The string without suffix</returns>
    public static string RemoveFromStart(this string source, string prefix)
    {
        if (!string.IsNullOrWhiteSpace(source) && !string.IsNullOrWhiteSpace(prefix) &&
            source.StartsWith(prefix))
        {
            source = source.Substring(prefix.Length);
        }
        return source;
    }
}