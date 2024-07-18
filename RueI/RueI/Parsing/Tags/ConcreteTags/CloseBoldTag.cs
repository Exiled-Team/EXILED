namespace RueI.Parsing.Tags.ConcreteTags;

/// <summary>
/// Provides a way to handle closing bold tags.
/// </summary>
[RichTextTag]
public class CloseBoldTag : ClosingTag<CloseBoldTag>
{
    /// <inheritdoc/>
    public override string Name { get; } = "/b";

    /// <inheritdoc/>
    protected override void ApplyTo(ParserContext context)
    {
        context.IsBold = false;
    }
}
