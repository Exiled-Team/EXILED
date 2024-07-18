namespace RueI.Parsing.Tags.ConcreteTags;

/// <summary>
/// Provides a way to handle closing italics tags.
/// </summary>
[RichTextTag]
public class CloseItalicsTag : ClosingTag<CloseItalicsTag>
{
    /// <inheritdoc/>
    public override string Name { get; } = "/i";
}
