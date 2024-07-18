namespace RueI.Parsing.Tags.ConcreteTags;

/// <summary>
/// Provides a way to handle closing align tags.
/// </summary>
[RichTextTag]
public class CloseAlignTag : ClosingTag<CloseAlignTag>
{
    /// <inheritdoc/>
    public override string Name { get; } = "/align";
}
