using System.Globalization;
using System.Text.Json;

namespace RestApiReporting.WebApp.ViewModel;

public class ReportRow
{
    public IList<ReportColumn> Columns { get; }
    public ReportDataRow DataRow { get; }

    public ReportRow(IList<ReportColumn> columns, ReportDataRow dataRow)
    {
        Columns = columns ?? throw new ArgumentNullException(nameof(columns));
        DataRow = dataRow ?? throw new ArgumentNullException(nameof(dataRow));
    }

    /// <summary>Get string value</summary>
    /// <param name="index">The column index</param>
    public string? GetStringValue(int index)
    {
        if (index >= Columns.Count)
        {
            return null;
        }
        var value = DataRow.Values[index];
        return string.IsNullOrWhiteSpace(value) ? null :
            value.StartsWith('\'') ? JsonSerializer.Deserialize<string>(value) : value;
    }

    /// <summary>Get integer value</summary>
    /// <param name="index">The column index</param>
    public int? GetIntegerValue(int index)
    {
        if (index >= Columns.Count)
        {
            return null;
        }
        var value = DataRow.Values[index];
        return string.IsNullOrWhiteSpace(value) ? null : JsonSerializer.Deserialize<int>(value);
    }

    /// <summary>Get boolean value</summary>
    /// <param name="index">The column index</param>
    public bool? GetBooleanValue(int index)
    {
        if (index >= Columns.Count)
        {
            return null;
        }
        var value = DataRow.Values[index];
        return string.IsNullOrWhiteSpace(value) ? null : JsonSerializer.Deserialize<bool>(value);
    }

    /// <summary>Get decimal value</summary>
    /// <param name="index">The column index</param>
    public decimal? GetDecimalValue(int index)
    {
        if (index >= Columns.Count)
        {
            return null;
        }
        var value = DataRow.Values[index];
        return string.IsNullOrWhiteSpace(value) ? null : JsonSerializer.Deserialize<decimal>(value);
    }

    /// <summary>Get date time value</summary>
    /// <param name="index">The column index</param>
    public DateTime? GetDateTimeValue(int index)
    {
        if (index >= Columns.Count)
        {
            return null;
        }
        var value = DataRow.Values[index];
        return string.IsNullOrWhiteSpace(value) ? null :
            value.StartsWith('"') ?
                JsonSerializer.Deserialize<DateTime>(value) :
                DateTime.Parse(value, null, DateTimeStyles.AdjustToUniversal);
    }
}