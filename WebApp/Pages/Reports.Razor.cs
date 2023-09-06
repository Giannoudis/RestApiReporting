using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;
using RestApiReporting.WebApp.Shared;
using RestApiReporting.WebApp.ViewModel;
using System.Diagnostics;
using RestApiReporting.WebApp.Service;

namespace RestApiReporting.WebApp.Pages;

public partial class Reports
{
    [Inject] public IConfiguration? Configuration { get; set; }
    [Inject] public IReportingService? ReportService { get; set; }
    [Inject] private IJSRuntime? JsRuntime { get; set; }

    [Parameter] public string? Report { get; set; }

    private MudForm? parameterForm;
    private NameFormatType NameFormatType { get; set; }
    private List<ReportInfo>? AvailableReports { get; set; }
    private string? InfoMessage { get; set; }
    private string? ErrorMessage { get; set; }
    private bool ShowParameterColumns { get; set; } = true;
    private bool Loading { get; set; }

    private ReportInfo? SelectedReport { get; set; }
    private ReportDataSet? SelectedDataSet { get; set; }
    private List<ReportParameter> SelectedParameters { get; } = new();
    private Dictionary<string, List<ReportColumn>> SelectedColumns { get; } = new();
    private Dictionary<string, List<ReportRow>> SelectedRows { get; } = new();

    private bool Dense => Configuration?.DenseMode() ?? false;

    private void ChangeReport(string reportName)
    {
        if (string.Equals(reportName, SelectedReport?.Name))
        {
            return;
        }

        ResetMessages();

        var report = AvailableReports?.FirstOrDefault(
            x => string.Equals(x.Name, reportName, StringComparison.OrdinalIgnoreCase));
        if (report == null)
        {
            SetErrorMessage($"Unknown report {reportName}");
            return;
        }

        // report selection
        ResetSelection();
        SelectedReport = report;
        Report = report.Name;

        // load parameters
        LoadParameters();
    }

    private async Task ExecuteReport()
    {
        // no change
        if (ReportService == null || SelectedReport == null)
        {
            return;
        }

        // form validation
        if (parameterForm != null)
        {
            parameterForm.ResetValidation();
            await parameterForm.Validate();
            if (!parameterForm.IsValid)
            {
                return;
            }
        }

        try
        {
            ResetMessages();
            await ChangeLoadingState(true);

            var stopwatch = new Stopwatch();
            stopwatch.Start();

            // parameters
            Dictionary<string, string>? parameters = null;
            if (SelectedReport.Parameters != null)
            {
                parameters = new();
                foreach (var parameter in SelectedParameters)
                {
                    if (!string.IsNullOrWhiteSpace(parameter.Value))
                    {
                        parameters.Add(parameter.Name, parameter.Value);
                    }
                }
            }

            // retrieve report response
            var response = await ReportService.BuildReportAsync(
                SelectedReport.Name,
                parameters: parameters);
            SelectedDataSet = response?.DataSet;

            // empty report
            if (SelectedDataSet == null)
            {
                ResetSelection();
                SetErrorMessage($"Empty report on report {SelectedReport.Name}");
                return;
            }

            // empty result
            if (!SelectedDataSet.HasData())
            {
                ResetSelection();
                SetErrorMessage("Empty report result");
                return;
            }

            SetupTables();
            // LoadTable(SelectedDataSet.Tables.FirstOrDefault());

            stopwatch.Stop();
            SetInfoMessage($"{SelectedReport?.Name} • {stopwatch.ElapsedMilliseconds:n0} ms");
        }
        catch (Exception exception)
        {
            ResetSelection();
            SetErrorMessage(exception);
        }
        finally
        {
            LoadParameters();
            await ChangeLoadingState(false);
        }
    }

    private void SetupTables()
    {
        SelectedColumns.Clear();
        SelectedRows.Clear();
        if (SelectedDataSet == null)
        {
            return;
        }

        foreach (var table in SelectedDataSet.Tables)
        {
            // columns
            var columns = new List<ReportColumn>();
            foreach (var column in table.Columns)
            {
                columns.Add(new(column));
            }
            SelectedColumns.Add(table.TableName, columns);

            var rows = new List<ReportRow>();
            foreach (var row in table.Rows)
            {
                var reportRow = new ReportRow(SelectedColumns[table.TableName], row);
                rows.Add(reportRow);
            }
            SelectedRows.Add(table.TableName, rows);
        }
    }

    private void LoadParameters()
    {
        SelectedParameters.Clear();
        if (SelectedReport?.Parameters != null)
        {
            foreach (var parameter in SelectedReport.Parameters)
            {
                SelectedParameters.Add(new ReportParameter(parameter));
            }
        }
    }

    private bool FilteredColumn(string tableName, int index)
    {
        var selectedColumns = SelectedColumns[tableName];
        if (index >= selectedColumns.Count)
        {
            return true;
        }
        if (ShowParameterColumns)
        {
            return false;
        }

        var column = selectedColumns[index];
        var parameter = SelectedParameters.FirstOrDefault(
            x => string.Equals(x.Name, column.Name, StringComparison.OrdinalIgnoreCase));
        return parameter != null;
    }

    private bool HasParameterColumns(string tableName)
    {
        var selectedColumns = SelectedColumns[tableName];
        if (!selectedColumns.Any())
        {
            return false;
        }

        foreach (var parameter in SelectedParameters)
        {
            if (selectedColumns.Any(
                x => string.Equals(x.Name, parameter.Name, StringComparison.OrdinalIgnoreCase)))
            {
                return true;
            }
        }
        return false;
    }

    #region Download

    private async Task DownloadJson()
    {
        if (JsRuntime == null || SelectedDataSet == null)
        {
            return;
        }

        try
        {
            ResetMessages();
            SetInfoMessage("Preparing JSON download...", false);
            await ChangeLoadingState(true);

            // performance
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            var download = await SelectedDataSet.DownloadJson(JsRuntime);
            SetInfoMessage($"JSON {download} • {stopwatch.ElapsedMilliseconds:n0} ms");
        }
        catch (Exception exception)
        {
            SetErrorMessage(exception);
        }
        finally
        {
            await ChangeLoadingState(false);
        }
    }

    private async Task DownloadXml()
    {
        if (JsRuntime == null || SelectedDataSet == null)
        {
            return;
        }

        try
        {
            ResetMessages();
            SetInfoMessage("Preparing XML download...", false);
            await ChangeLoadingState(true);

            // performance
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            var download = await SelectedDataSet.DownloadXml(JsRuntime);
            SetInfoMessage($"XML {download} • {stopwatch.ElapsedMilliseconds:n0} ms");
        }
        catch (Exception exception)
        {
            SetErrorMessage(exception);
        }
        finally
        {
            await ChangeLoadingState(false);
        }
    }

    private async Task DownloadExcel()
    {
        if (JsRuntime == null || SelectedDataSet == null)
        {
            return;
        }

        try
        {
            ResetMessages();
            SetInfoMessage("Preparing Excel download...", false);
            await ChangeLoadingState(true);

            // performance
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            var download = await SelectedDataSet.DownloadExcel(JsRuntime);
            SetInfoMessage($"Excel {download} • {stopwatch.ElapsedMilliseconds:n0} ms");
        }
        catch (Exception exception)
        {
            SetErrorMessage(exception);
        }
        finally
        {
            await ChangeLoadingState(false);
        }
    }

    private async Task DownloadPdf()
    {
        if (JsRuntime == null || SelectedDataSet == null)
        {
            return;
        }

        try
        {
            ResetMessages();
            SetInfoMessage("Preparing Pdf download...", false);
            await ChangeLoadingState(true);

            // performance
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            var download = await SelectedDataSet.DownloadPdf(JsRuntime);
            SetInfoMessage($"Pdf {download} • {stopwatch.ElapsedMilliseconds:n0} ms");
        }
        catch (Exception exception)
        {
            SetErrorMessage(exception);
        }
        finally
        {
            await ChangeLoadingState(false);
        }
    }

    #endregion

    #region Lifecycle

    private async Task ChangeLoadingState(bool loading)
    {
        Loading = loading;
        StateHasChanged();
        await Task.Delay(10);
    }

    private void SetInfoMessage(string message, bool timeStamp = true)
    {
        InfoMessage = message;
        if (timeStamp)
        {
            InfoMessage += $" • {DateTime.Now}";
        }
    }

    private void SetErrorMessage(Exception exception) =>
        SetErrorMessage(exception.GetBaseException().Message);

    private void SetErrorMessage(string message)
    {
        ErrorMessage = $"{message} • {DateTime.Now}";
    }

    private void ResetMessages()
    {
        InfoMessage = null;
        ErrorMessage = null;
    }

    private void ResetSelection()
    {
        SelectedDataSet = null;
        SelectedParameters.Clear();
        SelectedColumns.Clear();
        SelectedRows.Clear();
    }

    private async Task SetupReports()
    {
        try
        {
            if (ReportService != null)
            {
                AvailableReports = await ReportService.GetReportsAsync();
            }
        }
        catch (Exception exception)
        {
            ErrorMessage = exception.GetBaseException().Message;
        }
    }

    protected override async Task OnInitializedAsync()
    {
        // configuration
        if (Configuration != null)
        {
            NameFormatType = Configuration.NameFormat();
        }

        // query
        await SetupReports();
        if (!string.IsNullOrWhiteSpace(Report))
        {
            ChangeReport(Report);
        }
        await base.OnInitializedAsync();
    }

    #endregion

}