namespace RueI.Parsing.Tags.ConcreteTags;

/// <summary>
/// Provides a way to handle closing noparse tags.
/// </summary>
/// <remarks>
/// The RueI <see cref="Parser"/> allows this tag to be matched even when <see cref="ParserContext.ShouldParse"/> is <c>false</c>.
/// This replicates the behavior of normal TextMesh Pro.
/// </remarks>
[RichTextTag]
public class CloseNoparseTag : ClosingTag<CloseNoparseTag>
{
    /// <inheritdoc/>
    public override string Name { get; } = "/noparse";

    /// <inheritdoc/>
    protected override void ApplyTo(ParserContext context)
    {
        context.ShouldParse = true;
    }
}
