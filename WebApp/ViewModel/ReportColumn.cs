using RestApiReporting.WebApp.Shared;

namespace RestApiReporting.WebApp.ViewModel;

public class ReportColumn
{
    public ReportDataColumn DataColumn { get; }

    public string Name => DataColumn.ColumnName;

    public ReportColumn()
    {
        DataColumn = new ReportDataColumn();
    }

    public ReportColumn(ReportDataColumn dataColumn)
    {
        DataColumn = dataColumn ?? throw new ArgumentNullException(nameof(dataColumn));
        DataType = dataColumn.ValueType.ToReportDataType();
    }

    public ReportDataType DataType { get; }
}