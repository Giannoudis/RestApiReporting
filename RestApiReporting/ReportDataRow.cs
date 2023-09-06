namespace RestApiReporting;

/// <summary>Represents a row of data in a report data table</summary>
public class ReportDataRow
{
    /// <summary>Gets or sets the row values (JSON, type matching to the column)</summary>
    /// <value>The items.</value>
    public List<string> Values { get; set; } = new();

    public override string ToString() =>
        string.Join(", ", Values);
}