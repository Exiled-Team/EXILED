namespace RueI.Parsing.Tags.ConcreteTags;

using RueI.Parsing.Enums;

/// <summary>
/// Provides a way to handle alpha tags.
/// </summary>
[RichTextTag]
public class AlphaTag : RichTextTag
{
    /// <inheritdoc/>
    public override string[] Names { get; } = { "alpha" };

    /// <inheritdoc/>
    public override TagStyle TagStyle { get; } = TagStyle.ValueParam;

    /// <inheritdoc/>
    public override bool HandleTag(ParserContext context, string content)
    {
        if (content.Length != 3 || content.EndsWith(" "))
        {
            return false;
        }

        context.ResultBuilder.Append($"<alpha={content}>");
        context.AddEndingTag<CloseAlphaTag>();
        return true;
    }
}
