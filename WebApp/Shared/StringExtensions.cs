using System.Text;

namespace RestApiReporting.WebApp.Shared;

internal static class StringExtensions
{

    /// <summary>Change to pascal case sentence</summary>
    /// <param name="source">The source value</param>
    /// <param name="formatType">The name format type</param>
    /// <returns>Camel case text sentence</returns>
    internal static string NameFormat(this string source,
        NameFormatType formatType)
    {
        switch (formatType)
        {
            case NameFormatType.None:
                return source;
            case NameFormatType.CamelSentence:
                return ToCamelSentence(source);
            default:
            case NameFormatType.PascalSentence:
                return ToPascalSentence(source);
        }
    }

    /// <summary>Change to camel case sentence</summary>
    /// <param name="source">The source value</param>
    /// <param name="wordCase">The word start character casing</param>
    /// <param name="separator">The word separator</param>
    /// <returns>Camel case text sentence</returns>
    internal static string ToCamelSentence(this string source,
        CharacterCase wordCase = CharacterCase.ToLower, string separator = " ") =>
        ToSentence(source, CharacterCase.ToLower, wordCase, separator);

    /// <summary>Change to pascal case sentence</summary>
    /// <param name="source">The source value</param>
    /// <param name="wordCase">The word start character casing</param>
    /// <param name="separator">The word separator</param>
    /// <returns>Camel case text sentence</returns>
    internal static string ToPascalSentence(this string source,
        CharacterCase wordCase = CharacterCase.ToLower, string separator = " ") =>
        ToSentence(source, CharacterCase.ToUpper, wordCase, separator);

    /// <summary>Change text sentence</summary>
    /// <remarks>source: https://stackoverflow.com/a/51310790/15659039 </remarks>
    /// <param name="source">The source value</param>
    /// <param name="startCase">The first character casing</param>
    /// <param name="wordCase">The word start character casing</param>
    /// <param name="separator">The word separator</param>
    /// <returns></returns>
    internal static string ToSentence(this string source,
        CharacterCase startCase = CharacterCase.Keep,
        CharacterCase wordCase = CharacterCase.Keep,
        string separator = " ")
    {
        if (string.IsNullOrWhiteSpace(source))
        {
            return source;
        }

        var buffer = new StringBuilder();
        // start with the first character -- consistent camelcase and pascal case
        buffer.Append(ChangeCase(source[0], startCase));

        // march through the rest of it
        for (var i = 1; i < source.Length; i++)
        {
            // any time we hit an uppercase OR number, it's a new word
            if (char.IsUpper(source[i]) || char.IsDigit(source[i]))
            {
                if (separator != null)
                {
                    buffer.Append(separator);
                }
                buffer.Append(ChangeCase(source[i], wordCase));
            }
            else
            {
                // add regularly
                buffer.Append(source[i]);
            }
        }

        return buffer.ToString();
    }

    private static char ChangeCase(char input, CharacterCase startCase)
    {
        switch (startCase)
        {
            case CharacterCase.ToUpper:
                return char.ToUpper(input);
            case CharacterCase.ToLower:
                return char.ToLower(input);
            default:
                return input;
        }
    }

}