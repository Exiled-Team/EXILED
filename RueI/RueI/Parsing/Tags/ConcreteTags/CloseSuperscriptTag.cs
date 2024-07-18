namespace RueI.Parsing.Tags.ConcreteTags;

/// <summary>
/// Provides a way to handle closing superscript tags.
/// </summary>
[RichTextTag]
public class CloseSuperscriptTag : ClosingTag<CloseSuperscriptTag>
{
    /// <inheritdoc/>
    public override string Name { get; } = "/sup";

    /// <inheritdoc/>
    protected override void ApplyTo(ParserContext context)
    {
        context.IsSuperscript = false;
    }
}
