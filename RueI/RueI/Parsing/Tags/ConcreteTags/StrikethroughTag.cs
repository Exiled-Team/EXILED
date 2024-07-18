namespace RueI.Parsing.Tags.ConcreteTags;

/// <summary>
/// Provides a way to handle strikethrough tags.
/// </summary>
[RichTextTag]
public class StrikethroughTag : NoParamsTag
{
    /// <inheritdoc/>
    public override string[] Names { get; } = { "s" };

    /// <inheritdoc/>
    public override bool HandleTag(ParserContext context)
    {
        context.AddEndingTag<CloseStrikethroughTag>();
        context.ResultBuilder.Append("<s>");

        return true;
    }
}
