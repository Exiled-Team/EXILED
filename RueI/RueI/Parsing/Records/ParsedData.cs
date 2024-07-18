namespace RueI.Parsing.Records;

/// <summary>
/// Defines a class that contains parsed information about a single element, used for displaying multiple at a time.
/// </summary>
public class ParsedData
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ParsedData"/> class.
    /// </summary>
    /// <param name="content">The element's content.</param>
    /// <param name="offset">The offset that should be applied. Equivalent to the total linebreaks within the element.</param>
    internal ParsedData(string content, float offset)
    {
        Content = content;
        Offset = offset;
    }

    /// <summary>
    /// Gets the content of the element.
    /// </summary>
    public string Content { get; }

    /// <summary>
    /// Gets the offset that should be applied to the element.
    /// </summary>
    public float Offset { get; }

    /// <summary>
    /// Deconstructs this <see cref="ParsedData"/>.
    /// </summary>
    /// <param name="content">The returned new content of the element.</param>
    /// <param name="offset">The returned offset of the element.</param>
    public void Deconstruct(out string content, out float offset)
    {
        content = Content;
        offset = Offset;
    }
}
