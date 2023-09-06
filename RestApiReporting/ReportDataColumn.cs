namespace RestApiReporting;

/// <summary>Represents a column in a report data table</summary>
public class ReportDataColumn
{
    /// <summary>Gets or sets the column name</summary>
    public string ColumnName { get; set; } = string.Empty;

    /// <summary>Column expression used to filter rows</summary>
    public string? Expression { get; set; }

    /// <summary>Gets or sets the type of data stored in the column</summary>
    public string? ValueType { get; set; }

    /// <summary>Gets or sets the base type of data stored in the column</summary>
    public string? ValueBaseType { get; set; }

    /// <summary>Initializes a new instance of the <see cref="ReportDataColumn"/> class</summary>
    public ReportDataColumn()
    {
    }

    /// <summary>Initializes a new instance of the <see cref="ReportDataColumn"/> class with data</summary>
    /// <param name="columnName">The data column name</param>
    /// <param name="type">The system type</param>
    public ReportDataColumn(string columnName, Type type)
    {
        ColumnName = columnName;
        SetValueType(type);
    }

    /// <summary>Initializes a new instance of the <see cref="ReportDataColumn"/> class with data</summary>
    /// <param name="columnName">The data column name</param>
    /// <param name="type">The system type</param>
    /// <param name="baseType">The system base type</param>
    public ReportDataColumn(string columnName, Type type, Type baseType)
    {
        ColumnName = columnName;
        SetValueType(type);
        SetValueBaseType(baseType);
    }

    /// <summary>Initializes a new instance of the <see cref="ReportDataColumn"/> class with data</summary>
    /// <param name="columnName">The data column name</param>
    /// <param name="valueType">The value type</param>
    /// <param name="valueBaseType">The base value type</param>
    public ReportDataColumn(string columnName, string valueType, string valueBaseType)
    {
        ColumnName = columnName;
        ValueType = valueType;
        ValueBaseType = valueBaseType;
    }

    /// <summary>Gets the system value type</summary>
    public Type? GetValueType() =>
        string.IsNullOrWhiteSpace(ValueType) ? null : Type.GetType(ValueType);

    /// <summary>Gets the system value type</summary>
    private void SetValueType(Type type) =>
        ValueType = type.FullName;

    /// <summary>Gets the system value base type</summary>
    public Type? GetValueBaseType() =>
        string.IsNullOrWhiteSpace(ValueBaseType) ? null : Type.GetType(ValueBaseType);

    /// <summary>Gets the system value base type</summary>
    private void SetValueBaseType(Type type) =>
        ValueBaseType = type.FullName;

    public override string ToString() => 
        $"{ColumnName} ({ValueType})";
}