using System.Data;
using System.Text;
using System.Text.Json;
using System.Xml.Serialization;

namespace RestApiReporting;

/// <summary>Data set extension methods</summary>
public static class DataSetExtensions
{

    /// <summary>Test if data set hay any row</summary>
    /// <param name="dataSet">The data set to test</param>
    public static bool HasData(this ReportDataSet dataSet)
    {
        if (dataSet.Tables.Count == 0)
        {
            return false;
        }
        foreach (var table in dataSet.Tables)
        {
            if (table.Rows.Any())
            {
                return true;
            }
        }
        return false;
    }

    #region JSON & XML Convert

    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true
    };

    /// <summary>Download report data set as json file</summary>
    /// <param name="dataSet">The data set to download</param>
    /// <returns>The download file name</returns>
    public static Task<byte[]> ConvertToJsonAsync(this ReportDataSet dataSet)
    {
        // json
        var json = JsonSerializer.Serialize(dataSet, SerializerOptions);

        // result
        using var resultStream = new MemoryStream(Encoding.Default.GetBytes(json));
        return Task.FromResult(resultStream.ToArray());
    }

    /// <summary>Download report data set as xml file</summary>
    /// <param name="dataSet">The data set to download</param>
    /// <returns>The download file name</returns>
    public static async Task<byte[]> ConvertToXmlAsync(this ReportDataSet dataSet)
    {
        using var memoryStream = new MemoryStream();
        await using TextWriter streamWriter = new StreamWriter(memoryStream);

        // system data set
        var systemDataSet = dataSet.ToDataSet();
        var xmlSerializer = new XmlSerializer(typeof(DataSet));
        xmlSerializer.Serialize(streamWriter, systemDataSet);
        return memoryStream.ToArray();
    }

    #endregion

    #region Data Set Convert

    /// <summary>Convert a report data set to a system data set</summary>
    /// <param name="reportDataSet">The report data set to convert</param>
    /// <returns>The system data set</returns>
    public static DataSet ToDataSet(this ReportDataSet reportDataSet)
    {
        if (reportDataSet == null)
        {
            throw new ArgumentNullException(nameof(reportDataSet));
        }

        // data set
        var dataSet = new DataSet(reportDataSet.DataSetName);

        // tables
        if (!reportDataSet.Tables.Any())
        {
            return dataSet;
        }
        foreach (var table in reportDataSet.Tables)
        {
            // table
            var systemTable = table.ToDataTable();
            dataSet.Tables.Add(systemTable);
        }

        // relations
        if (reportDataSet.Relations.Any())
        {
            foreach (var relation in reportDataSet.Relations)
            {
                var parentTable = dataSet.Tables[relation.ParentTable];
                if (parentTable == null)
                {
                    throw new ReportException($"Missing relation parent table {relation.ParentTable}");
                }
                var parentColumn = parentTable.Columns[relation.ParentColumn];
                if (parentColumn == null)
                {
                    throw new ReportException($"Missing relation parent column {relation.ParentTable}.{relation.ParentColumn}");
                }
                var childTable = dataSet.Tables[relation.ChildTable];
                if (childTable == null)
                {
                    throw new ReportException($"Missing relation child table {relation.ChildTable}");
                }

                if (relation.ChildColumn != null)
                {
                    var childColumn = childTable.Columns[relation.ChildColumn];
                    if (childColumn == null)
                    {
                        throw new ReportException($"Missing relation parent column {relation.ChildTable}.{relation.ChildColumn}");
                    }
                    dataSet.Relations.Add(relation.Name, parentColumn, childColumn);
                }
            }
        }

        return dataSet;
    }

    /// <summary>Convert a system data set to a report data set</summary>
    /// <param name="dataSet">The system data set to convert</param>
    /// <returns>The report data set</returns>
    public static ReportDataSet ToReportDataSet(this DataSet dataSet)
    {
        if (dataSet == null)
        {
            throw new ArgumentNullException(nameof(dataSet));
        }

        // data set
        var reportDataSet = new ReportDataSet(dataSet.DataSetName);

        // tables
        foreach (DataTable table in dataSet.Tables)
        {
            // table
            var reportDataTable = new ReportDataTable(table.TableName);
            reportDataSet.Tables.Add(reportDataTable);

            // columns
            foreach (DataColumn column in table.Columns)
            {
                var columnType = column.DataType;

                // enum
                if (columnType.IsEnum)
                {
                    columnType = typeof(string);
                }

                var reportDataColumn = new ReportDataColumn
                {
                    ColumnName = column.ColumnName,
                    Expression = string.IsNullOrWhiteSpace(column.Expression) ? null : column.Expression,
                    ValueType = columnType.FullName,
                    ValueBaseType = column.DataType.FullName
                };
                reportDataTable.Columns.Add(reportDataColumn);
            }

            // rows
            foreach (var row in table.AsEnumerable())
            {
                if (row.ItemArray.Length != table.Columns.Count)
                {
                    throw new ReportException($"Row value count ({row.ItemArray.Length} is not matching the column count ({table.Columns.Count})");
                }
                reportDataTable.AddRow(row.ItemArray!);
            }
        }

        // relations
        AddRelations(dataSet, reportDataSet);

        return reportDataSet;
    }

    private static void AddRelations(DataSet dataSet, ReportDataSet reportDataSet)
    {
        if (dataSet.Relations.Count > 0)
        {
            foreach (DataRelation relation in dataSet.Relations)
            {
                var reportDataRelation = new ReportDataRelation
                {
                    Name = relation.RelationName,
                    ParentTable = relation.ParentTable.TableName,
                    ParentColumn = relation.ParentColumns.First().ColumnName,
                    ChildTable = relation.ChildTable.TableName,
                    ChildColumn = relation.ChildColumns.First().ColumnName
                };
                reportDataSet.Relations.Add(reportDataRelation);
            }
        }
    }

    #endregion

}