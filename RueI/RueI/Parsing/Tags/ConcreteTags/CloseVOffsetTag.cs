namespace RueI.Parsing.Tags.ConcreteTags;

/// <summary>
/// Provides a way to handle closing voffset tags.
/// </summary>
[RichTextTag]
public class CloseVOffsetTag : ClosingTag<CloseVOffsetTag>
{
    /// <inheritdoc/>
    public override string Name { get; } = "/voffset";
}
