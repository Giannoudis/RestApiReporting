using System.Collections;
using System.Data;

namespace RestApiReporting;

/// <summary>Report data table conversion extension methods</summary>
public static class ReportDataTableExtensions
{
    /// <summary>Convert a report data table to a system data table</summary>
    /// <param name="reportDataTable">The report data table to convert</param>
    /// <returns>The system data table</returns>
    public static DataTable ToDataTable(this ReportDataTable reportDataTable)
    {
        if (reportDataTable == null)
        {
            throw new ArgumentNullException(nameof(reportDataTable));
        }

        // table
        DataTable systemTable = new(reportDataTable.TableName);

        // columns
        if (!reportDataTable.Columns.Any())
        {
            return systemTable;
        }
        foreach (var column in reportDataTable.Columns)
        {
            if (column.ValueType == null)
            {
                continue;
            }

            // treat unknown types as json string
            var columnType = Type.GetType(column.ValueType) ?? typeof(string);
            var systemColumn = new DataColumn(column.ColumnName, columnType)
            {
                Expression = column.Expression
            };
            systemTable.Columns.Add(systemColumn);
        }

        // rows
        foreach (var row in reportDataTable.Rows)
        {
            if (row.Values == null || row.Values.Count != reportDataTable.Columns.Count)
            {
                throw new ReportException($"Row value count ({row.Values?.Count} is not matching the column count ({reportDataTable.Columns.Count})");
            }
            var dataRow = systemTable.NewRow();
            systemTable.Rows.Add(dataRow);

            // values
            dataRow.ItemArray = reportDataTable.GetRawValues(row).ToArray();
        }

        return systemTable;
    }

    /// <summary>Convert a report data table to a system data table</summary>
    /// <param name="reportDataTable">The report data table to convert</param>
    /// <param name="dataSetName">The data set name (default: table name)</param>
    /// <returns>The system data table</returns>
    public static ReportDataSet ToReportDataSet(this ReportDataTable reportDataTable, string? dataSetName = null)
    {
        var dataSet = new ReportDataSet(dataSetName ?? reportDataTable.TableName);
        dataSet.Tables.Add(reportDataTable);
        return dataSet;
    }

    /// <summary>Convert items to a system data set</summary>
    /// <param name="items">The items to convert</param>
    /// <param name="tableName">The table name, default is the type name</param>
    /// <param name="ignoreRows">Ignore table rows (default: false)</param>
    /// <param name="primaryKey">The primary key column name</param>
    /// <param name="properties">The properties to convert int columns (default: all)</param>
    /// <remarks>Property expressions:
    /// simple property: {PropertyName}
    /// child property: {ChildName1}.{ChildNameN}.{PropertyName}
    /// dictionary property: {ChildName}.{PropertyName}.{DictionaryKey}</remarks>
    /// <returns>Data table with items data</returns>
    public static ReportDataTable ToReportDataTable(this IEnumerable items, string? tableName = null,
        bool ignoreRows = false, string? primaryKey = null, IList<string>? properties = null) =>
        items.ToDataTable(tableName, ignoreRows, primaryKey, properties).ToReportDataTable();
}