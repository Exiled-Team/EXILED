namespace RueI.Parsing.Tags.ConcreteTags;

using RueI.Parsing.Enums;
using RueI.Parsing.Records;

/// <summary>
/// Provides a way to handle line indent tags.
/// </summary>
[RichTextTag]
public class LineIndentTag : MeasurementTag
{
    /// <inheritdoc/>
    public override string[] Names { get; } = { "line-indent" };

    /// <inheritdoc/>
    public override bool HandleTag(ParserContext context, MeasurementInfo info)
    {
        float value = info.style switch
        {
            MeasurementUnit.Percentage => info.value / 100 * Constants.DISPLAYAREAWIDTH,
            MeasurementUnit.Ems => info.value * Constants.EMSTOPIXELS,
            _ => info.value
        };

        context.Indent = value;
        context.ResultBuilder.Append($"<line-indent={value}>");
        context.AddEndingTag<CloseLineIndentTag>();

        return true;
    }
}
