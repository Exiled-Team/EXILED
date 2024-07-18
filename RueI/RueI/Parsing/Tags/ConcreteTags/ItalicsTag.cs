namespace RueI.Parsing.Tags.ConcreteTags;

/// <summary>
/// Provides a way to handle italics tags.
/// </summary>
[RichTextTag]
public class ItalicsTag : NoParamsTag
{
    /// <inheritdoc/>
    public override string[] Names { get; } = { "i" };

    /// <inheritdoc/>
    public override bool HandleTag(ParserContext context)
    {
        context.AddEndingTag<CloseItalicsTag>();
        context.ResultBuilder.Append("<i>");

        return true;
    }
}
