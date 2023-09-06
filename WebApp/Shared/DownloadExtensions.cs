using System.Globalization;
using Microsoft.JSInterop;

namespace RestApiReporting.WebApp.Shared;

public static class DownloadExtensions
{
    /// <summary>Download report data set as json file</summary>
    /// <param name="dataSet">The data set to download</param>
    /// <param name="jsRuntime">The JS runtime</param>
    /// <returns>The download file name</returns>
    public static async Task<string> DownloadJson(this ReportDataSet dataSet, IJSRuntime jsRuntime)
    {
        var download = GetDownloadName(dataSet, ".json");
        var json = await dataSet.ConvertToJsonAsync();
        await jsRuntime.SaveAs(download, json);
        return download;
    }

    /// <summary>Download report data set as xml file</summary>
    /// <param name="dataSet">The data set to download</param>
    /// <param name="jsRuntime">The JS runtime</param>
    /// <returns>The download file name</returns>
    public static async Task<string> DownloadXml(this ReportDataSet dataSet, IJSRuntime jsRuntime)
    {
        var download = GetDownloadName(dataSet, ".xml");
        var xml = await dataSet.ConvertToXmlAsync();
        await jsRuntime.SaveAs(download, xml);
        return download;
    }

    public static async Task<string> DownloadExcel(this ReportDataSet dataSet, IJSRuntime jsRuntime)
    {
        var download = GetDownloadName(dataSet, ".xlsx");
        var excel = dataSet.ConvertToExcel();
        await jsRuntime.SaveAs(download, excel);
        return download;
    }

    /// <summary>Download report data set as pdf file</summary>
    /// <param name="dataSet">The data set to download</param>
    /// <param name="jsRuntime">The JS runtime</param>
    /// <returns>The download file name</returns>
    public static async Task<string> DownloadPdf(this ReportDataSet dataSet, IJSRuntime jsRuntime)
    {
        var download = GetDownloadName(dataSet, ".pdf");
        var pdf = dataSet.ConvertToToPdf();
        await jsRuntime.SaveAs(download, pdf);
        return download;
    }

    private static string GetDownloadName(ReportDataSet dataSet, string extension)
    {
        var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmm", CultureInfo.InvariantCulture);
        var download = $"{dataSet.DataSetName}_{timestamp}{extension}";
        return download;
    }
}