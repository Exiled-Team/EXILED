namespace RueI.Parsing.Tags.ConcreteTags;

/// <summary>
/// Provides a way to handle closing cspace tags.
/// </summary>
[RichTextTag]
public class CloseCSpaceTag : ClosingTag<CloseCSpaceTag>
{
    /// <inheritdoc/>
    public override string Name { get; } = "/cspace";

    /// <inheritdoc/>
    protected override void ApplyTo(ParserContext context)
    {
        context.CurrentCSpace = 0;
    }
}
