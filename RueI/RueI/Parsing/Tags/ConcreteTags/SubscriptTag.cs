namespace RueI.Parsing.Tags.ConcreteTags;

/// <summary>
/// Provides a way to handle subscript tags.
/// </summary>
[RichTextTag]
public class SubscriptTag : NoParamsTag
{
    /// <inheritdoc/>
    public override string[] Names { get; } = { "sub" };

    /// <inheritdoc/>
    public override bool HandleTag(ParserContext context)
    {
        context.IsSubscript = true;

        context.AddEndingTag<CloseSubscriptTag>();
        context.ResultBuilder.Append("<sub>");

        return true;
    }
}
