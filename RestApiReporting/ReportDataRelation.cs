namespace RestApiReporting;

/// <summary>Represents a data relation in a report data table</summary>
public class ReportDataRelation
{
    /// <summary>Gets or sets the relation name</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>Gets or sets the relation parent table name</summary>
    public string? ParentTable { get; set; }

    /// <summary>Gets or sets the relation parent column names</summary>
    public string ParentColumn { get; set; } = "Id";

    /// <summary>Gets or sets the relation child table name</summary>
    public string? ChildTable { get; set; }

    /// <summary>Gets or sets the relation child column names</summary>
    public string? ChildColumn { get; set; }

    public override string ToString() =>
        $"{Name}: {ChildTable}.{ChildColumn} > {ParentTable}.{ParentColumn}";
}