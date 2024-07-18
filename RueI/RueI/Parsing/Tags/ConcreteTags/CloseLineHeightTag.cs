namespace RueI.Parsing.Tags.ConcreteTags;

/// <summary>
/// Provides a way to handle closing line height tags.
/// </summary>
[RichTextTag]
public class CloseLineHeightTag : NoParamsTag
{
    private const string TAGFORMAT = "<line-height=40.665>";

    /// <inheritdoc/>
    public override string[] Names { get; } = { "/line-height" };

    /// <inheritdoc/>
    public override bool HandleTag(ParserContext context)
    {
        context.CurrentLineHeight = Constants.DEFAULTHEIGHT;
        context.ResultBuilder.Append(TAGFORMAT);

        return true;
    }
}
