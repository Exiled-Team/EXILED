namespace RueI.Parsing.Tags.ConcreteTags;

/// <summary>
/// Provides a way to handle noparse tags.
/// </summary>
[RichTextTag]
public class NoparseTag : NoParamsTag
{
    /// <inheritdoc/>
    public override string[] Names { get; } = { "noparse" };

    /// <inheritdoc/>
    public override bool HandleTag(ParserContext context)
    {
        context.ShouldParse = true;

        context.AddEndingTag<CloseNoparseTag>();
        context.ResultBuilder.Append("<noparse>");

        return true;
    }
}
