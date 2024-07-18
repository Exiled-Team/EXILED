namespace RueI.Parsing.Tags.ConcreteTags;

using RueI.Parsing.Enums;

/// <summary>
/// Provides a way to handle scale tags.
/// </summary>
[RichTextTag]
public class ScaleTag : RichTextTag
{
    /// <inheritdoc/>
    public override string[] Names { get; } = { "scale" };

    /// <inheritdoc/>
    public override TagStyle TagStyle { get; } = TagStyle.ValueParam;

    /// <inheritdoc/>
    public override bool HandleTag(ParserContext context, string content)
    {
        if (!float.TryParse(content, out float result))
        {
            return false;
        }

        context.Scale = result;
        context.ResultBuilder.Append($"<scale={result}>");

        context.AddEndingTag<CloseLineHeightTag>();

        return true;
    }
}
