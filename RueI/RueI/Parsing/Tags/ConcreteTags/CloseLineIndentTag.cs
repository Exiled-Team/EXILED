namespace RueI.Parsing.Tags.ConcreteTags;

/// <summary>
/// Provides a way to handle closing line indent tags.
/// </summary>
[RichTextTag]
public class CloseLineIndentTag : ClosingTag<CloseLineIndentTag>
{
    /// <inheritdoc/>
    public override string Name { get; } = "/line-indent";

    /// <inheritdoc/>
    protected override void ApplyTo(ParserContext context)
    {
        context.LineIndent = 0;
    }
}
