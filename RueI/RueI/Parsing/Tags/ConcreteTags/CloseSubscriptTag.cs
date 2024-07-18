namespace RueI.Parsing.Tags.ConcreteTags;

/// <summary>
/// Provides a way to handle closing subscript tags.
/// </summary>
[RichTextTag]
public class CloseSubscriptTag : ClosingTag<CloseSubscriptTag>
{
    /// <inheritdoc/>
    public override string Name { get; } = "/sub";

    /// <inheritdoc/>
    protected override void ApplyTo(ParserContext context)
    {
        context.IsSubscript = false;
    }
}
