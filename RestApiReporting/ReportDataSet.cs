
namespace RestApiReporting;

/// <summary>Represents an in-memory cache of data with multiple tables and relations</summary>
public class ReportDataSet
{
    /// <summary>Gets or sets the data set name</summary>
    public string DataSetName { get; set; } = string.Empty;

    /// <summary>Gets the collection of tables contained in the data set</summary>
    public List<ReportDataTable> Tables { get; set; } = new();

    /// <summary>Gets the collection of table relations contained in the data set</summary>
    public List<ReportDataRelation> Relations { get; set; } = new();

    /// <summary>Initializes a new instance of the <see cref="ReportDataSet"/> class</summary>
    public ReportDataSet()
    {
    }

    /// <summary>Initializes a new instance of the <see cref="ReportDataSet"/> class</summary>
    /// <param name="dataSetName">The data set name</param>
    public ReportDataSet(string dataSetName)
    {
        DataSetName = dataSetName;
    }

    /// <summary>Initializes a new instance of the <see cref="ReportDataSet"/> class</summary>
    public ReportDataSet(string dataSetName, params ReportDataTable[] tables) :
        this(dataSetName)
    {
        Tables.AddRange(tables);
    }

    public override string ToString() => DataSetName;
}