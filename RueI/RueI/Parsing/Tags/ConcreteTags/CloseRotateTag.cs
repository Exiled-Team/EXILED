namespace RueI.Parsing.Tags.ConcreteTags;

/// <summary>
/// Provides a way to handle closing rotate tags.
/// </summary>
[RichTextTag]
public class CloseRotateTag : ClosingTag<CloseRotateTag>
{
    /// <inheritdoc/>
    public override string Name { get; } = "/rotate";
}
