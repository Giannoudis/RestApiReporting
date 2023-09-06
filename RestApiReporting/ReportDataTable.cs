using System.Text.Json;
using System.Text.RegularExpressions;

namespace RestApiReporting;

/// <summary>Represents one table of in-memory data</summary>
public class ReportDataTable
{
    /// <summary>Gets or sets the table name</summary>
    public string TableName { get; set; } = string.Empty;

    /// <summary>Gets the collection of columns that belong to this table</summary>
    public List<ReportDataColumn> Columns { get; set; } = new();

    /// <summary>Gets the collection of rows that belong to this table</summary>
    public List<ReportDataRow> Rows { get; set; } = new();

    /// <summary>Initializes a new instance of the <see cref="ReportDataTable"/> class</summary>
    public ReportDataTable()
    {
    }

    /// <summary>Initializes a new instance of the <see cref="ReportDataTable"/> class with items</summary>
    public ReportDataTable(string tableName)
    {
        TableName = tableName;
    }

    /// <summary>Row column index operator</summary>
    /// <param name="row">The row index</param>
    /// <param name="col">The column index</param>
    /// <returns></returns>
    public object this[int row, int col] =>
        Rows[row].Values[col];

    /// <summary>Gets the data row values</summary>
    /// <param name="row">The data row</param>
    /// <returns>The row raw data</returns>
    public IEnumerable<object> GetRawValues(ReportDataRow row)
    {
        if (row == null)
        {
            throw new ArgumentNullException(nameof(row));
        }
        if (Rows == null || !Rows.Contains(row))
        {
            throw new ArgumentException("Invalid table row");
        }
        if (Columns == null || row.Values == null)
        {
            throw new ReportException("Invalid data row size");
        }
        if (Columns.Count != row.Values.Count)
        {
            throw new ReportException("Invalid data row value count");
        }

        // convert raw values to json values
        var rawValues = new List<object>();
        for (var i = 0; i < Columns.Count; i++)
        {
            var column = Columns[i];
            if (!string.IsNullOrWhiteSpace(column.Expression))
            {
                continue;
            }

            var rawValue = row.Values[i];
            var type = Columns[i].GetValueType();

            object? value = null;
            // string/datetime escaping
            if (type != null && (type == typeof(string) || type == typeof(DateTime)))
            {
                value = Regex.Unescape(rawValue.Trim('"'));
            }
            else if (!string.IsNullOrWhiteSpace(rawValue))
            {
                value = type == null ?
                    // serialize unknown types to json string
                    JsonSerializer.Serialize(rawValue) :
                    JsonSerializer.Deserialize(rawValue, type);
            }

            // enum (string to int)
            var baseType = column.GetValueBaseType();
            if (baseType != null && baseType.IsEnum)
            {
                value = (int)Enum.Parse(baseType, rawValue.Trim('"'));
            }

            if (value != null)
            {
                rawValues.Add(value);
            }
        }
        return rawValues;
    }

    /// <summary>Add a row with raw data</summary>
    /// <param name="rawValues">The values to set</param>
    public void AddRow(IEnumerable<object> rawValues)
    {
        if (rawValues == null)
        {
            throw new ArgumentNullException(nameof(rawValues));
        }
        var row = new ReportDataRow();
        Rows.Add(row);
        SetRawValues(row, rawValues);
    }

    /// <summary>Set row data</summary>
    /// <param name="row">The data row</param>
    /// <param name="rawValues">The raw values to set</param>
    private void SetRawValues(ReportDataRow row, IEnumerable<object> rawValues)
    {
        if (row == null)
        {
            throw new ArgumentNullException(nameof(row));
        }
        if (rawValues == null)
        {
            throw new ArgumentNullException(nameof(rawValues));
        }
        if (Rows == null || !Rows.Contains(row))
        {
            throw new ArgumentException("Invalid table row");
        }
        if (Columns == null || rawValues == null)
        {
            throw new ReportException("Invalid data row size");
        }
        var rawValueList = rawValues.ToList();
        if (Columns.Count != rawValueList.Count)
        {
            throw new ReportException("Invalid data row value count");
        }

        // convert json values to raw values
        for (var i = 0; i < Columns.Count; i++)
        {
            var column = Columns[i];
            var rawValue = rawValueList[i];

            // null
            if (rawValue is DBNull)
            {
                row.Values.Add(string.Empty);
                continue;
            }

            // enum (int to string)
            var baseType = column.GetValueBaseType();
            if (baseType != null && baseType.IsEnum)
            {
                rawValue = Enum.GetName(baseType, rawValue);
            }

            var type = column.GetValueType();
            var jsonValue = type == null ?
                // unknown types as json string
                JsonSerializer.Serialize(rawValue) :
                JsonSerializer.Serialize(rawValue, type);

            // string/datetime escaping
            if (!string.IsNullOrWhiteSpace(jsonValue) && type != null &&
                (type == typeof(string) || type == typeof(DateTime)))
            {
                jsonValue = Regex.Unescape(jsonValue.Trim('"'));
            }

            row.Values.Add(jsonValue);
        }
    }

    public override string ToString() => TableName;
}