namespace RueI.Parsing.Tags.ConcreteTags;

/// <summary>
/// Provides a way to handle closing size tags.
/// </summary>
[RichTextTag]
public class CloseSizeTag : NoParamsTag
{
    private const string TAGFORMAT = "</size>";

    /// <inheritdoc/>
    public override string[] Names { get; } = { "/size" };

    /// <inheritdoc/>
    public override bool HandleTag(ParserContext context)
    {
        if (context.SizeTags.Count > 0)
        {
            context.Size = context.SizeTags.Pop();
        }
        else
        {
            context.Size = Constants.DEFAULTSIZE;
        }

        context.ResultBuilder.Append(TAGFORMAT);

        return true;
    }
}
