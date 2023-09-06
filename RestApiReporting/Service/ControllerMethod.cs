using System.Globalization;
using System.Reflection;
using System.Text.Json;

namespace RestApiReporting.Service;

/// <summary>Controller method</summary>
public sealed class ControllerMethod
{
    public Type ControllerType { get; }
    public MethodInfo MethodInfo { get; }

    private string Name => MethodInfo.Name;

    public ControllerMethod(Type controllerType, MethodInfo methodInfo)
    {
        ControllerType = controllerType ?? throw new ArgumentNullException(nameof(controllerType));
        MethodInfo = methodInfo ?? throw new ArgumentNullException(nameof(methodInfo));
    }

    /// <summary>Get the method parameters</summary>
    /// <param name="parameters">The report parameters</param>
    /// <param name="strict">Test for invalid parameters</param>
    /// <returns>Collection of method parameter infos</returns>
    public List<Tuple<ParameterInfo, object?>> GetParameters(Dictionary<string, string>? parameters,
        bool strict = false)
    {
        parameters ??= new();

        var methodParameters = MethodInfo.GetParameters().ToList();
        if (strict)
        {
            foreach (var inputParameter in parameters)
            {
                if (!methodParameters.Any(x => string.Equals(x.Name, inputParameter.Key, StringComparison.OrdinalIgnoreCase)))
                {
                    throw new ReportException($"Unknown query parameter: {inputParameter.Key}={inputParameter.Value}");
                }
            }
        }

        // parameter values
        List<Tuple<ParameterInfo, object?>> parameterValues = new();
        foreach (var methodParameter in methodParameters)
        {
            // input parameter name
            var parameter = parameters.GetValueByName(methodParameter.Name);

            // not optional and nullable
            if (parameter == null)
            {
                if (!methodParameter.IsOptional && !methodParameter.IsNullable())
                {
                    throw new ReportException($"Missing mandatory query parameter: {methodParameter.Name}");
                }
            }

            // no value present
            if (parameter == null)
            {
                parameterValues.Add(new Tuple<ParameterInfo, object?>(methodParameter, null));
                continue;
            }

            // value
            if (string.IsNullOrWhiteSpace(parameter))
            {
                continue;
            }
            var value = ConvertParameterValue(parameter, methodParameter.ParameterType);
            parameterValues.Add(new Tuple<ParameterInfo, object?>(methodParameter, value));
        }

        return parameterValues;
    }

    private static object? ConvertParameterValue(string jsonValue, Type? type)
    {
        // resolve nullable types
        if (type != null && type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
        {
            type = Nullable.GetUnderlyingType(type);
        }
        if (type == null)
        {
            return null;
        }

        // cast string
        if (type == typeof(string))
        {
            return jsonValue;
        }

        // cast char
        if (type == typeof(char))
        {
            return jsonValue.Length > 0 ? jsonValue[0] : null;
        }

        // cast date time
        if (type == typeof(DateTime))
        {
            if (!DateTime.TryParse(jsonValue, CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal, out var dateValue))
            {
                return null;
            }
            return dateValue;
        }

        // cast date only
        if (type == typeof(DateOnly))
        {
            if (!DateOnly.TryParse(jsonValue, CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal, out var dateOnly))
            {
                return null;
            }
            return dateOnly;
        }

        // cast byte
        if (type == typeof(byte))
        {
            if (!byte.TryParse(jsonValue, out var byteValue))
            {
                return null;
            }
            return byteValue;
        }

        // cast sbyte
        if (type == typeof(sbyte))
        {
            if (!sbyte.TryParse(jsonValue, out var sbyteValue))
            {
                return null;
            }
            return sbyteValue;
        }

        // cast short
        if (type == typeof(short))
        {
            if (!short.TryParse(jsonValue, out var shortValue))
            {
                return null;
            }
            return shortValue;
        }

        // cast ushort
        if (type == typeof(ushort))
        {
            if (!ushort.TryParse(jsonValue, out var ushortValue))
            {
                return null;
            }
            return ushortValue;
        }

        // cast int
        if (type == typeof(int))
        {
            if (!int.TryParse(jsonValue, out var intValue))
            {
                return null;
            }
            return intValue;
        }

        // cast uint
        if (type == typeof(uint))
        {
            if (!uint.TryParse(jsonValue, out var uintValue))
            {
                return null;
            }
            return uintValue;
        }

        // cast long
        if (type == typeof(long))
        {
            if (!long.TryParse(jsonValue, out var longValue))
            {
                return null;
            }
            return longValue;
        }

        // cast ulong
        if (type == typeof(ulong))
        {
            if (!ulong.TryParse(jsonValue, out var ulongValue))
            {
                return null;
            }
            return ulongValue;
        }

        // cast bool
        if (type == typeof(bool))
        {
            if (!bool.TryParse(jsonValue, out var boolValue))
            {
                return null;
            }
            return boolValue;
        }

        // cast enum
        if (type.IsEnum)
        {
            if (!Enum.TryParse(type, jsonValue, out var enumValue))
            {
                return null;
            }
            return enumValue;
        }

        // cast decimal
        if (type == typeof(decimal))
        {
            if (!decimal.TryParse(jsonValue, CultureInfo.InvariantCulture, out var decimalValue))
            {
                return null;
            }
            return decimalValue;
        }

        // cast float
        if (type == typeof(float))
        {
            if (!float.TryParse(jsonValue, CultureInfo.InvariantCulture, out var floatValue))
            {
                return null;
            }
            return floatValue;
        }

        // cast double
        if (type == typeof(double))
        {
            if (!double.TryParse(jsonValue, CultureInfo.InvariantCulture, out var doubleValue))
            {
                return null;
            }
            return doubleValue;
        }

        // cast object
        try
        {
            return JsonSerializer.Deserialize(jsonValue, type);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public override string ToString() => Name;
}
