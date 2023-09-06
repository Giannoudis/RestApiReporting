using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;
using RestApiReporting.WebApp.Shared;
using RestApiReporting.WebApp.ViewModel;
using System.Diagnostics;
using System.Text;
using RestApiReporting.WebApp.Service;

namespace RestApiReporting.WebApp.Pages;

public partial class Queries
{
    [Inject] public IConfiguration? Configuration { get; set; }
    [Inject] public IQueryService? QueryService { get; set; }
    [Inject] private HttpClient? HttpClient { get; set; }
    [Inject] private IJSRuntime? JsRuntime { get; set; }

    [Parameter] public string? Query { get; set; }

    private MudForm? parameterForm;
    private NameFormatType NameFormatType { get; set; }
    private List<ApiMethod>? ReportQueries { get; set; }
    private string? InfoMessage { get; set; }
    private string? ErrorMessage { get; set; }
    private bool ShowParameterColumns { get; set; } = true;
    private bool Loading { get; set; }

    private ApiMethod? SelectedQuery { get; set; }
    private string? Route { get; set; }

    private ReportDataSet? SelectedDataSet { get; set; }
    private ReportDataTable? SelectedTable { get; set; }
    private List<ReportParameter> SelectedParameters { get; } = new();
    private List<ReportColumn> SelectedColumns { get; } = new();
    private List<ReportRow> SelectedRows { get; } = new();

    private bool Dense => Configuration?.DenseMode() ?? false;

    private void ChangeQuery(string queryName)
    {
        if (string.Equals(queryName, SelectedQuery?.MethodName))
        {
            return;
        }

        ResetMessages();

        var query = ReportQueries?.FirstOrDefault(
            x => string.Equals(x.MethodName, queryName, StringComparison.OrdinalIgnoreCase));
        if (query == null)
        {
            SetErrorMessage($"Unknown query {queryName}");
            return;
        }

        // query selection
        ResetSelection();
        SelectedQuery = query;
        Query = query.MethodName;

        // load parameters
        LoadParameters();
        UpdateRoute();
    }

    private void LoadParameters()
    {
        SelectedParameters.Clear();
        if (SelectedQuery?.Parameters != null)
        {
            foreach (var parameter in SelectedQuery.Parameters)
            {
                SelectedParameters.Add(new ReportParameter(parameter, UpdateRoute));
            }
        }
    }

    private bool FilteredColumn(int index)
    {
        if (index >= SelectedColumns.Count)
        {
            return true;
        }
        if (ShowParameterColumns)
        {
            return false;
        }
        
        var column = SelectedColumns[index];
        var parameter = SelectedParameters.FirstOrDefault(
            x => string.Equals(x.Name, column.Name, StringComparison.OrdinalIgnoreCase));
        return parameter != null;
    }

    private bool HasParameterColumns()
    {
        if (!SelectedParameters.Any())
        {
            return false;
        }

        foreach (var parameter in SelectedParameters)
        {
            if (SelectedColumns.Any(
                    x => string.Equals(x.Name, parameter.Name, StringComparison.OrdinalIgnoreCase)))
            {
                return true;
            }
        }
        return false;
    }

    private async Task ExecuteQuery()
    {
        // no change
        if (QueryService == null || SelectedQuery == null)
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

            // performance
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            // parameters
            Dictionary<string, string>? parameters = null;
            if (SelectedQuery.Parameters != null)
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

            // retrieve query response
            var response = await QueryService.ExecuteQueryAsync(
                SelectedQuery.MethodName, parameters);
            SelectedDataSet = response?.DataSet;

            // empty report
            if (SelectedDataSet == null)
            {
                ResetSelection();
                SetInfoMessage($"Empty report on query method {SelectedQuery.MethodName}");
                return;
            }

            // empty result
            if (!SelectedDataSet.HasData())
            {
                ResetSelection();
                SetInfoMessage("Empty query result");
                return;
            }

            // init with the first table
            SelectedTable = SelectedDataSet.Tables[0];
            // columns
            SelectedColumns.Clear();
            foreach (var column in SelectedTable.Columns)
            {
                SelectedColumns.Add(new(column));
            }

            // rows
            SelectedRows.Clear();
            foreach (var row in SelectedTable.Rows)
            {
                var reportRow = new ReportRow(SelectedColumns, row);
                SelectedRows.Add(reportRow);
            }

            stopwatch.Stop();
            SetInfoMessage($"{SelectedQuery?.MethodName} • {stopwatch.ElapsedMilliseconds:n0} ms");
        }
        catch (Exception exception)
        {
            ResetSelection();
            SetErrorMessage(exception);
        }
        finally
        {
            LoadParameters();
            UpdateRoute();
            await ChangeLoadingState(false);
        }
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

    private void UpdateRoute()
    {
        string? route = null;
        if (SelectedQuery != null && HttpClient != null)
        {
            var buffer = new StringBuilder();

            // base address
            var baseAddress = HttpClient.BaseAddress?.ToString();
            if (baseAddress != null)
            {
                buffer.Append(baseAddress);
            }

            // route
            var routeParameters = new List<ReportParameter>();
            if (!string.IsNullOrWhiteSpace(SelectedQuery.Route))
            {
                if (baseAddress != null && !baseAddress.EndsWith('/'))
                {
                    buffer.Append('/');
                }

                var methodRoute = SelectedQuery.Route;
                // route parameters
                foreach (var parameter in SelectedParameters)
                {
                    if (string.IsNullOrWhiteSpace(parameter.Value))
                    {
                        continue;
                    }
                    var template = $"{{{parameter.Name}}}";
                    if (methodRoute.Contains(template))
                    {
                        methodRoute = methodRoute.Replace(template, parameter.Value);
                        routeParameters.Add(parameter);
                    }
                }
                buffer.Append(methodRoute);
            }

            // parameters
            var first = true;
            foreach (var parameter in SelectedParameters)
            {
                if (string.IsNullOrWhiteSpace(parameter.Value) ||
                    routeParameters.Contains(parameter))
                {
                    continue;
                }
                buffer.Append(first ? '?' : '&');
                first = false;
                buffer.Append($"{parameter.Name}={parameter.Value}");
            }
            route = buffer.ToString();
        }

        Route = route;
    }

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
        SelectedTable = null;
        SelectedParameters.Clear();
        SelectedColumns.Clear();
        SelectedRows.Clear();
    }

    private async Task SetupQueries()
    {
        try
        {
            if (QueryService != null)
            {
                ReportQueries = await QueryService.GetQueriesAsync();
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
        await SetupQueries();
        if (!string.IsNullOrWhiteSpace(Query))
        {
            ChangeQuery(Query);
        }
        await base.OnInitializedAsync();
    }

    #endregion

}