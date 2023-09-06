namespace RestApiReporting;

/// <summary>Report response</summary>
public class ReportResponse
{
    /// <summary>The resulting report data set</summary>
    public ReportDataSet? DataSet { get; set; }

    /// <summary>The report culture</summary>
    public string? Culture { get; set; }

    /// <summary>The report parameters</summary>
    public Dictionary<string, string>? Parameters { get; set; }

    public ReportResponse()
    {
    }

    public ReportResponse(ReportDataSet? dataSet,
        string? culture = null, Dictionary<string, string>? parameters = null)
    {
        DataSet = dataSet;
        Culture = culture;
        Parameters = parameters;
    }

    public ReportResponse(ReportDataSet dataSet, ReportRequest request) :
        this(request)
    {
        DataSet = dataSet;
    }

    public ReportResponse(ReportRequest request)
    {
        Culture = request.Culture;
        Parameters = request.Parameters;
    }

    public override string? ToString() =>
        DataSet?.DataSetName ?? base.ToString();
}
