using System.Collections;
using System.Data;
using System.Text.Json;

namespace RestApiReporting;

/// <summary>System data table extension methods</summary>
public static class SystemDataTableExtensions
{
    /// <summary>Convert a system data table to a report data table</summary>
    /// <param name="dataTable">The system data table to convert</param>
    /// <returns>The report data table</returns>
    public static ReportDataTable ToReportDataTable(this DataTable dataTable)
    {
        // table
        var table = new ReportDataTable(dataTable.TableName);

        // columns
        foreach (DataColumn column in dataTable.Columns)
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
            table.Columns.Add(reportDataColumn);
        }

        // rows
        foreach (var row in dataTable.AsEnumerable())
        {
            if (row.ItemArray.Length != dataTable.Columns.Count)
            {
                throw new ReportException($"Row value count ({row.ItemArray.Length} is not matching the column count ({dataTable.Columns.Count})");
            }
            table.AddRow(row.ItemArray!);
        }

        return table;
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
    public static DataTable ToDataTable(this IEnumerable items, string? tableName = null,
        bool ignoreRows = false, string? primaryKey = null, IList<string>? properties = null)
    {
        if (items == null)
        {
            throw new ArgumentNullException(nameof(items));
        }

        // data set
        DataTable? dataTable = null;
        foreach (var item in items)
        {
            // table
            if (dataTable == null)
            {
                var name = string.IsNullOrWhiteSpace(tableName) ? item.GetType().Name : tableName;
                dataTable = new(name);

                var itemProperties = ObjectInfo.GetProperties(item.GetType());
                var convertPropertyNames = properties ?? itemProperties.Select(x => x.Name).ToList();
                foreach (var convertPropertyName in convertPropertyNames)
                {
                    ApplyProperty(primaryKey, item, convertPropertyName, dataTable);
                }
            }

            // rows
            if (!ignoreRows)
            {
                dataTable.AppendItem(item, properties);
            }
        }

        return dataTable ?? new DataTable(tableName);
    }

    private static void ApplyProperty(string? primaryKey, object item, string convertPropertyName, DataTable dataTable)
    {
        var resolvedProperty = item.ResolvePropertyValue(convertPropertyName);
        if (resolvedProperty == null)
        {
            return;
        }

        var property = resolvedProperty.Property;
        if (property == null)
        {
            return;
        }

        // add column
        var nullableType = Nullable.GetUnderlyingType(property.PropertyType);
        var columnType = nullableType ?? property.PropertyType;

        // json column
        if (columnType.IsSerializedType())
        {
            columnType = typeof(string);
        }

        var propertyName = resolvedProperty.DictionaryKey ?? property.Name;
        var dataColumn = new DataColumn(propertyName, columnType);
        if (nullableType != null)
        {
            dataColumn.AllowDBNull = true;
        }

        dataTable.Columns.Add(dataColumn);

        // primary key
        if (primaryKey != null && string.Equals(property.Name, primaryKey))
        {
            dataTable.PrimaryKey = new[] { dataColumn };
        }
    }

    /// <summary>Append items to a system data table</summary>
    /// <param name="dataTable">The target table</param>
    /// <param name="items">The items to convert</param>
    /// <param name="properties">The properties to convert int columns (default: all)</param>
    /// <remarks>Property expressions:
    /// simple property: {PropertyName}
    /// child property: {ChildName1}.{ChildNameN}.{PropertyName}
    /// dictionary property: {ChildName}.{PropertyName}.{DictionaryKey}</remarks>
    /// <returns>Data table with items data</returns>
    public static void AppendItems(this DataTable dataTable, IEnumerable items,
        IList<string>? properties = null)
    {
        if (dataTable == null)
        {
            throw new ArgumentNullException(nameof(dataTable));
        }
        if (items == null)
        {
            throw new ArgumentNullException(nameof(items));
        }

        foreach (var item in items)
        {
            dataTable.AppendItem(item, properties);
        }
    }

    /// <summary>Append items to a system data table</summary>
    /// <param name="dataTable">The target table</param>
    /// <param name="item">The items to append</param>
    /// <param name="properties">The properties to convert int columns (default: all)</param>
    /// <remarks>Property expressions:
    /// simple property: {PropertyName}
    /// child property: {ChildName1}.{ChildNameN}.{PropertyName}
    /// dictionary property: {ChildName}.{PropertyName}.{DictionaryKey}</remarks>
    /// <returns>Data table with items data</returns>
    private static void AppendItem(this DataTable dataTable, object item,
        IList<string>? properties = null)
    {
        if (dataTable == null)
        {
            throw new ArgumentNullException(nameof(dataTable));
        }
        if (item == null)
        {
            throw new ArgumentNullException(nameof(item));
        }

        // properties
        var itemProperties = ObjectInfo.GetProperties(item.GetType());
        var propertyNames = properties ?? itemProperties.Select(x => x.Name).ToList();
        var propertyValues = new List<PropertyValue>();
        foreach (var propertyName in propertyNames)
        {
            var propertyValue = item.ResolvePropertyValue(propertyName);
            if (propertyValue == null)
            {
                continue;
            }
            propertyValues.Add(propertyValue);
        }

        // row
        var dataRow = dataTable.NewRow();
        if (!propertyValues.Any())
        {
            return;
        }

        // collect row item array
        var rowItems = new object[dataTable.Columns.Count];
        foreach (var propertyValue in propertyValues)
        {
            if (propertyValue.Property == null)
            {
                continue;
            }
            var index = dataTable.Columns.IndexOf(propertyValue.Property.Name);
            // ignore unknown column properties
            if (index < 0)
            {
                continue;
            }

            // value
            var value = propertyValue.Value;
            if (value != null && propertyValue.Property.PropertyType.IsSerializedType())
            {
                value = JsonSerializer.Serialize(value);
            }
            if (value != null)
            {
                rowItems[index] = value;
            }
        }

        // values row
        dataRow.ItemArray = rowItems;
        dataTable.Rows.Add(dataRow);
    }
}