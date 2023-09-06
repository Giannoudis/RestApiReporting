using System.Data;
using System.Drawing;
using FastReport;
using FastReport.Export.PdfSimple;
using FastReport.Table;
using FastReport.Utils;

namespace RestApiReporting.WebApp.Shared;

public static class FastReportExtensions
{
    /// <summary>Download report data set as pdf file</summary>
    /// <param name="dataSet">The data set to download</param>
    /// <returns>The download file name</returns>
    public static byte[] ConvertToToPdf(this ReportDataSet dataSet)
    {
        var systemDataSet = dataSet.ToDataSet();

        // word and pdf requires a word template
        using var report = CreateFastReport(systemDataSet);

        // register the data set
        report.RegisterData(systemDataSet);

        // prepare the report
        report.Prepare();

        // export to pdf
        var pdfExport = new PDFSimpleExport();
        var resultStream = new MemoryStream();
        report.Export(pdfExport, resultStream);

        resultStream.Position = 0;
        return resultStream.ToArray();
    }

    /// <summary>Create a simple report from all tables</summary>
    /// <param name="dataSet"></param>
    /// <returns></returns>
    private static Report CreateFastReport(DataSet dataSet)
    {
        var report = new Report();

        // page
        var page = new ReportPage();
        page.Name = dataSet.DataSetName;
        page.Landscape = true;
        report.Pages.Add(page);
        // 10 for the borders
        var paperWidth = page.PaperWidth - 20;

        // report title
        page.ReportTitle = new ReportTitleBand();
        page.ReportTitle.CreateUniqueName();
        page.ReportTitle.Height = 20 * Units.Millimeters;
        var titleText = GetTitleObject(dataSet.DataSetName, paperWidth);
        page.ReportTitle.Objects.Add(titleText);

        // data band
        var dataBand = new DataBand();
        dataBand.Name = "DataBand";
        dataBand.CanGrow = true;
        page.Bands.Add(dataBand);
        // tables
        foreach (DataTable dataTable in dataSet.Tables)
        {
            AddDataTable(dataTable, dataBand, paperWidth);
        }

        return report;
    }

    private static TextObject GetTitleObject(string text, float paperWidth)
    {
        var titleText = new TextObject();
        titleText.CreateUniqueName();
        titleText.Width = paperWidth * Units.Millimeters;
        titleText.Height = 10 * Units.Millimeters;
        titleText.HorzAlign = HorzAlign.Center;
        titleText.VertAlign = VertAlign.Center;
        titleText.Border.Lines = BorderLines.All;
        titleText.FillColor = Color.WhiteSmoke;
        titleText.Border.Width = 0.75f;
        titleText.Text = text;
        return titleText;
    }

    private static void AddDataTable(DataTable dataTable, DataBand dataBand, float paperWidth)
    {
        // data table
        var table = new TableObject();
        table.Name = dataTable.TableName;
        var headerRowCount = 2;
        // + header row
        table.RowCount = dataTable.Rows.Count + headerRowCount;
        table.ColumnCount = dataTable.Columns.Count;

        // columns
        var colWidth = paperWidth / table.ColumnCount;
        foreach (TableColumn column in table.Columns)
        {
            column.Width = Units.Millimeters * colWidth;
            column.AutoSize = false;
        }

        // row 0: title header
        var title = table[0, 0];
        title.Text = dataTable.TableName;
        title.Border.Lines = BorderLines.All;
        title.FillColor = Color.WhiteSmoke;
        title.Border.Width = 0.75f;
        title.HorzAlign = HorzAlign.Center;
        title.ColSpan = table.Columns.Count;

        // row 1: column header
        for (var col = 0; col < dataTable.Columns.Count; col++)
        {
            // header row
            var tableObject = table[col, 1];
            tableObject.Text = dataTable.Columns[col].ColumnName;
            tableObject.Border.Lines = BorderLines.All;
            tableObject.FillColor = Color.WhiteSmoke;
            tableObject.Border.Width = 0.75f;
        }

        // row 2-n: cells
        for (var row = 0; row < dataTable.Rows.Count; row++)
        {
            for (var col = 0; col < dataTable.Columns.Count; col++)
            {
                // header row
                var tableObject = table[col, row + headerRowCount];
                var value = dataTable.Rows[row][col];
                if (value is string stringValue)
                {
                    value = stringValue
                        .Replace("[", string.Empty)
                        .Replace("]", string.Empty);
                }

                tableObject.Text = $"{value}";
                tableObject.Border.Lines = BorderLines.All;
                tableObject.Border.Width = 0.75f;
            }
        }

        dataBand.Objects.Add(table);

        // footer table
        var footerTable = new TableObject();
        footerTable.Name = $"{dataTable.TableName}Footer";
        footerTable.RowCount = 2;
        footerTable.ColumnCount = 1;
        footerTable[0, 0].Height = Units.Millimeters * 10;
        dataBand.Objects.Add(footerTable);

        table.CreateUniqueNames();
    }
}