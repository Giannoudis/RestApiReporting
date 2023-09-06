using System.Globalization;

namespace RestApiReporting.WebApi.Model;

public class CultureDescription
{
    public string Name { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string NativeName { get; set; } = string.Empty;

    public CultureDescription()
    {
    }

    public CultureDescription(CultureInfo cultureInfo)
    {
        Name = cultureInfo.Name;
        DisplayName = cultureInfo.DisplayName;
        NativeName = cultureInfo.NativeName;
    }

    public override string ToString() =>
        Name;
}