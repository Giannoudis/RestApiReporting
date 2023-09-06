using Microsoft.AspNetCore.Mvc;

namespace RestApiReporting.Service;

/// <summary>Base class for reporting query controller</summary>
[Route("reporting")]
[ApiController]
public abstract class QueryControllerBase : ControllerBase
{
    private IApiQueryService ApiQueryService { get; }

    protected QueryControllerBase(IApiQueryService queryService)
    {
        ApiQueryService = queryService;
    }

    /// <summary>Get all api query methods</summary>
    [HttpGet("queries", Name = "GetQueries")]
    public virtual async Task<ActionResult<List<ApiMethod>>> GetQueries() =>
        await Task.Run(() => ApiQueryService.Methods.Values.ToList());

    /// <summary>Get an api query method</summary>
    /// <param name="name">The method name</param>
    [HttpGet("queries/{name}", Name = "GetQuery")]
    public virtual ActionResult<ApiMethod> GetQuery(string name)
    {
        var queryKey = ApiQueryService.Methods.Keys
            .FirstOrDefault(x => string.Equals(x, name, StringComparison.OrdinalIgnoreCase));
        if (queryKey == null)
        {
            return NotFound($"Unknown method {name}");
        }
        return Ok(ApiQueryService.Methods[queryKey]);
    }

    /// <summary>Execute an api query method</summary>
    /// <param name="name">The method name</param>
    /// <param name="parameters">The method parameters</param>
    /// <returns>Excluded as reporting query method</returns>
    /// <returns>Report response including the data sets</returns>
    [ReportingIgnore]
    [HttpGet("queries/{name}/execute", Name = "ExecuteQuery")]
    public virtual async Task<ActionResult<ReportResponse>> ExecuteQueryAsync(
        string name, [FromQuery] Dictionary<string, string>? parameters)
    {
        try
        {
            // method
            var method = ApiQueryService.GetMethod(name);
            if (method == null)
            {
                return NotFound($"Unknown query method {name}");
            }

            // method query
            var dataSet = await ApiQueryService.QuerySetAsync(
                dataSetName: method.MethodName,
                new ReportQuery(ControllerContext,
                    methodName: method.MethodName,
                    parameters: parameters));
            return new ReportResponse(
                dataSet: dataSet,
                parameters: parameters);
        }
        catch (ReportException exception)
        {
            return BadRequest(exception.Message);
        }
    }
}