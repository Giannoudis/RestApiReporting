using RestApiReporting.WebApp.Shared;

namespace RestApiReporting.WebApp;

/// <summary>The program console configuration</summary>
internal sealed class ProgramConfiguration
{
    /// <summary>The API URL</summary>
    internal string ApiUrl { get; set; } = string.Empty;

    /// <summary>The application title</summary>
    internal string AppTitle { get; set; } = string.Empty;

    /// <summary>Dense mode</summary>
    internal bool DenseMode { get; set; } = false;

    /// <summary>Item page count</summary>
    internal int DataPageCount { get; set; } = 10;

    /// <summary>The browser layout mode (default: large)</summary>
    internal BrowserLayoutMode LayoutMode { get; set; } = BrowserLayoutMode.Large;

    /// <summary>Data filter mode (default: simple)</summary>
    internal DataFilterMode FilterMode { get; set; } = DataFilterMode.Simple;

    /// <summary>Name format type (default: pascal sentence)</summary>
    internal NameFormatType NameFormat { get; set; } = NameFormatType.PascalSentence;
}