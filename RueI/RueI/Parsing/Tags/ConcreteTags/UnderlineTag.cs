namespace RueI.Parsing.Tags.ConcreteTags;

/// <summary>
/// Provides a way to handle underline tags.
/// </summary>
[RichTextTag]
public class UnderlineTag : NoParamsTag
{
    /// <inheritdoc/>
    public override string[] Names { get; } = { "u" };

    /// <inheritdoc/>
    public override bool HandleTag(ParserContext context)
    {
        context.AddEndingTag<CloseUnderlineTag>();
        context.ResultBuilder.Append("<u>");

        return true;
    }
}
