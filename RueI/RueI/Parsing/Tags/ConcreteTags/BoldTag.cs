namespace RueI.Parsing.Tags.ConcreteTags;

/// <summary>
/// Provides a way to handle bold tags.
/// </summary>
[RichTextTag]
public class BoldTag : NoParamsTag
{
    /// <inheritdoc/>
    public override string[] Names { get; } = { "b" };

    /// <inheritdoc/>
    public override bool HandleTag(ParserContext context)
    {
        context.IsBold = true;

        context.AddEndingTag<CloseBoldTag>();
        context.ResultBuilder.Append("<b>");

        return true;
    }
}
