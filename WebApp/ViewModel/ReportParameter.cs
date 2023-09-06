using System.Text.Json;
using System.Text.Json.Serialization;
using RestApiReporting.WebApp.Shared;

namespace RestApiReporting.WebApp.ViewModel;

public class ReportParameter
{
    public ApiMethodParameter ParameterInfo { get; }
    public ReportDataType DataType { get; }

    public string Name => ParameterInfo.Name;
    public string Type => ParameterInfo.Type;
    public bool Required => ParameterInfo.Required;
    public bool Nullable => ParameterInfo.Nullable;

    public Action? ValueChanged { get; set; }

    public ReportParameter(ApiMethodParameter parameter, Action? valueChanged = null)
    {
        ParameterInfo = parameter ?? throw new ArgumentNullException(nameof(parameter));
        DataType = parameter.Type.ToReportDataType();
        ValueChanged = valueChanged;
    }

    public string? Value
    {
        get => ParameterInfo.Value;
        set
        {
            if (string.Equals(value, Value))
            {
                return;
            }
            ParameterInfo.Value = value;
            OnValueChanged();
        }
    }

    [JsonIgnore]
    public int? ValueAsInteger
    {
        get => string.IsNullOrWhiteSpace(Value) ? null : JsonSerializer.Deserialize<int>(Value);
        set
        {
            if (value == ValueAsInteger)
            {
                return;
            }
            Value = value == null ? null : JsonSerializer.Serialize(value);
            OnValueChanged();
        }
    }

    [JsonIgnore]
    public bool? ValueAsBoolean
    {
        get => string.IsNullOrWhiteSpace(Value) ? null : JsonSerializer.Deserialize<bool>(Value);
        set
        {
            if (value == ValueAsBoolean)
            {
                return;
            }
            Value = value == null ? null : JsonSerializer.Serialize(value);
            OnValueChanged();
        }
    }

    [JsonIgnore]
    public decimal? ValueAsDecimal
    {
        get => string.IsNullOrWhiteSpace(Value) ? null : JsonSerializer.Deserialize<decimal>(Value);
        set
        {
            if (value == ValueAsDecimal)
            {
                return;
            }
            Value = value == null ? null : JsonSerializer.Serialize(value);
            OnValueChanged();
        }
    }

    [JsonIgnore]
    public DateTime? ValueAsDateTime
    {
        get => string.IsNullOrWhiteSpace(Value) ? null : JsonSerializer.Deserialize<DateTime>(Value);
        set
        {
            if (value == ValueAsDateTime)
            {
                return;
            }
            Value = value == null ? null : JsonSerializer.Serialize(value);
            OnValueChanged();
        }
    }

    private void OnValueChanged() =>
        ValueChanged?.Invoke();
}