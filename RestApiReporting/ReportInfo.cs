namespace RestApiReporting;

/// <summary>Basic report info</summary>
public class ReportInfo
{
    /// <summary>The report name</summary>
    public string Name { get; set; }

    /// <summary>The report description</summary>
    public string? Description { get; set; }

    /// <summary>Supported report cultures, undefined is any culture</summary>
    public IList<string> SupportedCultures { get; set; } = new List<string>();

    /// <summary>Report parameters</summary>
    public IList<ApiMethodParameter>? Parameters { get; set; } = new List<ApiMethodParameter>();

    public ReportInfo()
    {
        Name = GetType().Name;
    }

    public ReportInfo(string reportName,
        string? description = null,
        IEnumerable<string>? supportedCultures = null)
    {
        if (string.IsNullOrWhiteSpace(reportName))
        {
            throw new ArgumentException(nameof(reportName));
        }
        Name = reportName;
        Description = description;
        SupportedCultures = supportedCultures != null ?
            new List<string>(supportedCultures) :
            new();
    }

    public override string ToString() => Name;
}
