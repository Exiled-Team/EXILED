namespace RueI.Parsing.Tags;

/// <summary>
/// Provides a number of helper functions for <see cref="RichTextTag"/>s.
/// </summary>
public static class TagHelpers
{
    /// <summary>
    /// Validates and extracts the text from inside quotations for tag parameters, or returns the original string.
    /// </summary>
    /// <param name="str">The <see cref="string"/> to extract the quotations from.</param>
    /// <returns>The string with the quotes removed, or null if the string is invalid.</returns>
    /// <example>
    /// This code demonstrates the behavior of <see cref="ExtractFromQuotations(string)"/>.
    /// <code>
    /// ExtractFromQuotations("\"hello world\"") // -> hello world
    /// ExtractFromQuotations("hello world") // -> hello world
    /// ExtractFromQuotations("\"hello world") // -> null
    /// ExtractFromQuotations("hello world\"") // -> null
    /// </code>
    /// </example>
    public static string? ExtractFromQuotations(string str)
    {
        return (str.StartsWith("\""), str.EndsWith("\"")) switch
        {
            (true, true) => str.Substring(1, str.Length - 1),
            (false, true) => null,
            (true, false) => null,
            (false, false) => str,
        };
    }
}
