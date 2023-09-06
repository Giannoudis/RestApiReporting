
namespace RestApiReporting;

/// <summary>Extensions for dictionaries</summary>
public static class DictionaryExtensions
{
    /// <summary>Get dictionary item with case-insensitive key</summary>
    /// <param name="dictionary">The dictionary</param>
    /// <param name="keyAsName">The dictionary key name</param>
    /// <returns>The dictionary item, matching the key name</returns>
    public static T? GetValueByName<T>(this Dictionary<string, T> dictionary, string? keyAsName)
    {
        if (string.IsNullOrWhiteSpace(keyAsName))
        {
            return default;
        }
        var dictKey = dictionary.Keys.FirstOrDefault(x =>
            string.Equals(x, keyAsName, StringComparison.OrdinalIgnoreCase));

        return dictKey != null ? dictionary[dictKey] : default;
    }
}