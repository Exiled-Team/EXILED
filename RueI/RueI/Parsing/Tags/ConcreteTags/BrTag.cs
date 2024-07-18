namespace RueI.Parsing.Tags.ConcreteTags;

/// <summary>
/// Provides a way to handle br tags.
/// </summary>
[RichTextTag]
public class BrTag : NoParamsTag
{
    /// <inheritdoc/>
    public override string[] Names { get; } = { "br" };

    /// <inheritdoc/>
    public override bool HandleTag(ParserContext context)
    {
        Parser.CreateLineBreak(context);
        context.ResultBuilder.Append("<br>");

        return true;
    }
}
