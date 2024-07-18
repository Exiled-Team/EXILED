namespace RueI.Parsing.Tags.ConcreteTags;

using RueI.Extensions;
using RueI.Parsing.Enums;

/// <summary>
/// Provides a way to handle italic angles tags.
/// </summary>
[RichTextTag]
public class ItalicsAngleTag : RichTextTag
{
    /// <inheritdoc/>
    public override string[] Names { get; } = { "i" };

    /// <inheritdoc/>
    public override TagStyle TagStyle { get; } = TagStyle.Attributes;

    /// <inheritdoc/>
    public override bool HandleTag(ParserContext context, string content)
    {
        if (!Parser.GetTagAttributes(content, out Dictionary<string, string> attributes))
        {
            return false;
        }

        if (attributes.Only(x => x.Key == "angle" && (TagHelpers.ExtractFromQuotations(x.Value) != null)))
        {
            return false;
        }

        context.AddEndingTag<CloseItalicsTag>();
        context.ResultBuilder.Append($"<i {content}>");

        return true;
    }
}
