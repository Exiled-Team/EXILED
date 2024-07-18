namespace RueI.Parsing.Tags.ConcreteTags;

/// <summary>
/// Provides a way to handle page tags.
/// </summary>
[RichTextTag]
public class PageTag : NoParamsTag
{
    /// <inheritdoc/>
    public override string[] Names { get; } = { "page" };

    /// <inheritdoc/>
    public override bool HandleTag(ParserContext context)
    {
        return true;
    }
}
