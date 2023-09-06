using System.Data;
using System.Reflection;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Task = System.Threading.Tasks.Task;

namespace RestApiReporting.Service;

/// <summary>Report API query service implementation</summary>
public class ApiQueryService : IApiQueryService
{

    #region Controller Context

    private sealed class ApiControllerContext
    {
        private ControllerContext Context { get; }

        internal ApiControllerContext(ControllerContext context)
        {
            Context = context ?? throw new ArgumentNullException(nameof(context));
        }

        internal object? Activate(Type targetType)
        {
            if (targetType == null)
            {
                throw new ArgumentNullException(nameof(targetType));
            }

            // controller
            // https://andrewlock.net/controller-activation-and-dependency-injection-in-asp-net-core-mvc/
            var controller = Context.HttpContext.RequestServices.GetService(targetType) as ControllerBase;
            if (controller != null)
            {
                // https://stackoverflow.com/a/50777301
                controller.ControllerContext = Context;
            }
            return controller;
        }
    }

    #endregion

    public ApiQueryService(QueryFilter? filter = null)
    {
        var queryMethods = new QueryReflector(filter).GetQueryMethods();
        QueryMethods = queryMethods.ToDictionary(
            key => key.Key,
            value => value.Value.Item1);
        Methods = queryMethods.ToDictionary(
            key => key.Key,
            value => value.Value.Item2);
        Controllers = new HashSet<Type>(QueryMethods.Values.Select(x => x.ControllerType)).ToList();
    }

    private Dictionary<string, ControllerMethod> QueryMethods { get; }

    /// <inheritdoc />
    public IList<Type> Controllers { get; }

    /// <inheritdoc />
    public IDictionary<string, ApiMethod> Methods { get; }

    #region Query

    /// <inheritdoc />
    public virtual ApiMethod? GetMethod(string name)
    {
        var key = Methods.Keys.FirstOrDefault(x => string.Equals(x, name, StringComparison.OrdinalIgnoreCase));
        return key != null ? Methods[key] : null;
    }

    /// <inheritdoc />
    public virtual async Task<DataTable?> QueryAsync(ReportQuery reportQuery,
        Action<ReportQuery, IList<object>>? itemsLoaded = null)
    {
        if (reportQuery == null)
        {
            throw new ArgumentNullException(nameof(reportQuery));
        }

        // query
        var queryMethod = QueryMethods.GetValueByName(reportQuery.MethodName);
        if (queryMethod == null)
        {
            throw new ReportException($"Unknown report {reportQuery.MethodName}");
        }

        // controller
        var controller = new ApiControllerContext(reportQuery.ControllerContext).
            Activate(queryMethod.ControllerType) as ControllerBase;
        if (controller == null)
        {
            throw new ReportException($"Missing web controller {queryMethod.ControllerType}");
        }

        // parameter values
        var methodParameters = queryMethod.GetParameters(reportQuery.Parameters, reportQuery.StrictParameters);
        var parameterValues = methodParameters.Select(x => x.Item2).ToArray();

        // method execution
        IList<object>? result;
        var methodResult = queryMethod.MethodInfo.Invoke(controller, parameterValues);
        if (methodResult == null)
        {
            return null;
        }

        if (methodResult is Task taskResult)
        {
            await taskResult;
            // query result
            result = GetQueryResult(taskResult);
        }
        else
        {
            result = GetActionResult(methodResult);
        }
        if (result == null)
        {
            return null;
        }

        // loaded handler
        if (itemsLoaded != null)
        {
            itemsLoaded(reportQuery, result);
        }

        // result table
        var resultTable = GetResultTable(queryMethod.MethodInfo.Name, result, methodParameters, reportQuery.PrimaryKey);
        return resultTable;
    }

    private static DataTable GetResultTable(string methodName, IList<object> result,
        List<Tuple<ParameterInfo, object?>> methodParameters, string? primaryKey)
    {
        var tableName = GetOperationBaseName(methodName);
        var dataTable = result.ToDataTable(tableName,
            primaryKey: primaryKey,
            ignoreRows: true);

        // parameter columns
        var parameterColumns = GetParameterColumns(methodParameters);
        foreach (var parameterColumn in parameterColumns)
        {
            // table column
            var columnName = parameterColumn.Key.FirstCharacterToUpper();
            if (!dataTable.Columns.Contains(columnName))
            {
                dataTable.Columns.Add(columnName, parameterColumn.Value.Item1);
            }
        }

        // result items
        dataTable.AppendItems(result);

        // result parameters
        SetResultParameters(parameterColumns, dataTable);

        return dataTable;
    }

    /// <summary>Get operation base name</summary>
    /// <param name="operation">The operation name</param>
    /// <returns>Operation base name</returns>
    private static string GetOperationBaseName(string operation)
    {
        if (string.IsNullOrWhiteSpace(operation))
        {
            throw new ArgumentException(nameof(operation));
        }
        if (operation.StartsWith("Query"))
        {
            return operation.RemoveFromStart("Query");
        }
        return operation.StartsWith("Get") ?
            operation.RemoveFromStart("Get") :
            operation;
    }

    /// <summary>Get the parameter columns</summary>
    /// <param name="methodParameters">The method parameters</param>
    /// <returns>Parameter columns dictionary: key=column name, value=tuple of parameter type and parameter value</returns>
    private static Dictionary<string, Tuple<Type, object?>> GetParameterColumns(
        List<Tuple<ParameterInfo, object?>> methodParameters)
    {
        var parameterColumns = new Dictionary<string, Tuple<Type, object?>>();
        foreach (var methodParameter in methodParameters)
        {
            var parameterName = methodParameter.Item1.Name;
            if (parameterName == null)
            {
                // no parameter value
                continue;
            }

            // column type
            var nullableType = Nullable.GetUnderlyingType(methodParameter.Item1.ParameterType);
            var columnType = nullableType ?? methodParameter.Item1.ParameterType;
            // json column
            if (columnType.IsSerializedType())
            {
                columnType = typeof(string);
            }

            // column
            var columnName = parameterName.FirstCharacterToUpper();
            parameterColumns.Add(columnName, new(columnType, methodParameter.Item2));
        }
        return parameterColumns;
    }

    private static void SetResultParameters(Dictionary<string, Tuple<Type, object?>> parameterColumns, DataTable dataTable)
    {
        if (!parameterColumns.Any())
        {
            return;
        }

        foreach (var parameterColumn in parameterColumns)
        {
            foreach (var dataRow in dataTable.AsEnumerable())
            {
                var value = parameterColumn.Value.Item2;
                // json value
                if (parameterColumn.Value.Item1.IsSerializedType())
                {
                    value = JsonSerializer.Serialize(value);
                }
                if (value != null)
                {
                    dataRow[parameterColumn.Key] = value;
                }
            }
        }
    }

    private static IList<object>? GetQueryResult(Task methodResult)
    {
        var resultProperty = methodResult.GetType().GetProperty("Result");
        if (resultProperty == null)
        {
            return null;
        }
        var actionResult = resultProperty.GetValue(methodResult);
        return actionResult == null ? null : GetActionResult(actionResult);
    }

    private static IList<object>? GetActionResult(object? actionResult)
    {
        if (actionResult == null)
        {
            return null;
        }

        // action result
        if (actionResult is IConvertToActionResult convertToActionResult)
        {
            // recursive call
            var result = GetActionResult(convertToActionResult.Convert());
            return result;
        }

        if (actionResult is IEnumerable<object> resultList)
        {
            return new List<object>(resultList);
        }

        // object result
        if (actionResult is ObjectResult objectResult)
        {
            if (objectResult.StatusCode == 500)
            {
                throw new ReportException($"Internal server error {objectResult.Value}");
            }
            if (objectResult.StatusCode is null or < 200 or >= 300)
            {
                throw new ReportException($"{objectResult.Value} [{objectResult.StatusCode}]");
            }

            if (objectResult.Value is IEnumerable<object> array)
            {
                return new List<object>(array);
            }
            return new List<object> { objectResult.Value };
        }

        // single item
        return new List<object> { actionResult };
    }

    #endregion

}