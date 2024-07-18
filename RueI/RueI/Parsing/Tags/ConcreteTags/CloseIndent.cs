namespace RueI.Parsing.Tags.ConcreteTags;

/// <summary>
/// Provides a way to handle closing indent tags.
/// </summary>
[RichTextTag]
public class CloseIndentTag : ClosingTag<CloseIndentTag>
{
    /// <inheritdoc/>
    public override string Name { get; } = "/indent";

    /// <inheritdoc/>
    protected override void ApplyTo(ParserContext context)
    {
        context.Indent = 0;
    }
}
