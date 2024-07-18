namespace RueI.Parsing.Tags.ConcreteTags;

/// <summary>
/// Provides a way to handle superscript tags.
/// </summary>
[RichTextTag]
public class SuperscriptTag : NoParamsTag
{
    /// <inheritdoc/>
    public override string[] Names { get; } = { "sup" };

    /// <inheritdoc/>
    public override bool HandleTag(ParserContext context)
    {
        context.IsSuperscript = true;

        context.AddEndingTag<CloseSuperscriptTag>();
        context.ResultBuilder.Append("<sup>");

        return true;
    }
}
