using System.Text.Json;

namespace RestApiReporting;

/// <summary>Json extensions</summary>
public static class JsonExtensions
{
    /// <summary>Get the json element value</summary>
    /// <param name="jsonElement">The json element</param>
    /// <returns>The json element value</returns>
    public static object? GetValue(this JsonElement jsonElement)
    {
        switch (jsonElement.ValueKind)
        {
            case JsonValueKind.String:
                return jsonElement.GetString();
            case JsonValueKind.Number:
                return jsonElement.GetDecimal();
            case JsonValueKind.True:
            case JsonValueKind.False:
                return jsonElement.GetBoolean();
            case JsonValueKind.Array:
                // recursive values
                return jsonElement.EnumerateArray().
                    Select(GetValue).ToList();
            case JsonValueKind.Object:
                // recursive values
                return jsonElement.EnumerateObject().
                    ToDictionary(item => item.Name, item => item.Value.GetValue());
            case JsonValueKind.Undefined:
            case JsonValueKind.Null:
                return null;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}