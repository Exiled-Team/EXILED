namespace RueI.Parsing.Tags.ConcreteTags;

using RueI.Parsing.Enums;

/// <summary>
/// Provides a way to handle color tags.
/// </summary>
[RichTextTag]
public class ColorTag : RichTextTag
{
    /// <inheritdoc/>
    public override string[] Names { get; } = { "color" };

    /// <inheritdoc/>
    public override TagStyle TagStyle { get; } = TagStyle.ValueParam;

    /// <inheritdoc/>
    public override bool HandleTag(ParserContext context, string content)
    {
        if (content.StartsWith("#"))
        {
            if (!Constants.ValidColorSizes.Contains(content.Length - 1))
            {
                return false;
            }
        }
        else
        {
            string? unquoted = TagHelpers.ExtractFromQuotations(content);
            if (unquoted == null || !Constants.Colors.Contains(unquoted))
            {
                return false;
            }
        }

        context.ResultBuilder.Append($"<color={content}>");
        context.AddEndingTag<CloseColorTag>(allowDuplicates: true);
        return true;
    }
}
