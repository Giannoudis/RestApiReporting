using MudBlazor;

namespace RestApiReporting.WebApp.Shared;

public static class ConfigurationExtensions
{

    public static string? ApiUrl(this IConfiguration configuration) =>
        configuration.GetValue<string?>(
            $"{nameof(ProgramConfiguration)}:{nameof(ProgramConfiguration.ApiUrl)}") ?? null;

    public static string? AppTitle(this IConfiguration configuration) =>
        configuration.GetValue<string?>(
            $"{nameof(ProgramConfiguration)}:{nameof(ProgramConfiguration.AppTitle)}") ?? null;

    public static bool DenseMode(this IConfiguration configuration) =>
        configuration.GetValue<bool?>(
            $"{nameof(ProgramConfiguration)}:{nameof(ProgramConfiguration.DenseMode)}") ?? false;

    public static int DataPageCount(this IConfiguration configuration) =>
        configuration.GetValue<int?>(
            $"{nameof(ProgramConfiguration)}:{nameof(ProgramConfiguration.DataPageCount)}") ?? 10;

    /// <summary>Get browser layout mode</summary>
    /// <param name="configuration">The configuration</param>
    /// <returns>The browser layout mode</returns>
    public static MaxWidth LayoutMode(this IConfiguration configuration)
    {
        var layoutModeText = configuration.GetValue<string?>(
            $"{nameof(ProgramConfiguration)}:{nameof(ProgramConfiguration.LayoutMode)}");
        if (string.IsNullOrWhiteSpace(layoutModeText) ||
            !Enum.TryParse<MaxWidth>(layoutModeText, true, out var layoutMode))
        {
            return MaxWidth.Large;
        }

        return layoutMode switch
        {
            MaxWidth.ExtraSmall => MaxWidth.ExtraSmall,
            MaxWidth.Small => MaxWidth.Small,
            MaxWidth.Medium => MaxWidth.Medium,
            MaxWidth.Large => MaxWidth.Large,
            MaxWidth.ExtraLarge => MaxWidth.ExtraLarge,
            MaxWidth.ExtraExtraLarge => MaxWidth.ExtraExtraLarge,
            _ => MaxWidth.Large
        };
    }

    /// <summary>Get the filter mode settings</summary>
    /// <param name="configuration">The configuration</param>
    /// <returns>The report filter mode</returns>
    public static DataGridFilterMode FilterMode(this IConfiguration configuration)
    {
        var filterModeText = configuration.GetValue<string?>(
            $"{nameof(ProgramConfiguration)}:{nameof(ProgramConfiguration.FilterMode)}");
        if (string.IsNullOrWhiteSpace(filterModeText) ||
            !Enum.TryParse<DataFilterMode>(filterModeText, true, out var filterMode))
        {
            return DataGridFilterMode.Simple;
        }

        return filterMode switch
        {
            DataFilterMode.Menu => DataGridFilterMode.ColumnFilterMenu,
            DataFilterMode.Row => DataGridFilterMode.ColumnFilterRow,
            DataFilterMode.Simple => DataGridFilterMode.Simple,
            _ => DataGridFilterMode.Simple
        };
    }

    /// <summary>Get the name format type settings</summary>
    /// <param name="configuration">The configuration</param>
    /// <returns>The report filter mode</returns>
    public static NameFormatType NameFormat(this IConfiguration configuration)
    {
        var nameFormatText = configuration.GetValue<string?>(
            $"{nameof(ProgramConfiguration)}:{nameof(ProgramConfiguration.NameFormat)}");
        if (string.IsNullOrWhiteSpace(nameFormatText) ||
            !Enum.TryParse<NameFormatType>(nameFormatText, true, out var nameFormat))
        {
            return NameFormatType.PascalSentence;
        }
        return nameFormat;
    }
}