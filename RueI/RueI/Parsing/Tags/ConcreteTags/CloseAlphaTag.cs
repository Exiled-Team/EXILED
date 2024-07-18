namespace RueI.Parsing.Tags.ConcreteTags;

/// <summary>
/// Provides a way to handle closing alpha tags.
/// </summary>
[RichTextTag]
public class CloseAlphaTag : NoParamsTag
{
    /// <inheritdoc/>
    public override string[] Names { get; } = { }; // no names, since this tag doesn't really "exist"

    /// <inheritdoc/>
    public override bool HandleTag(ParserContext context)
    {
        context.ResultBuilder.Append("<alpha=#FF>");

        return true;
    }
}
