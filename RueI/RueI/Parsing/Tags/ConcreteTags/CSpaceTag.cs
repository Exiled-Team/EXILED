namespace RueI.Parsing.Tags.ConcreteTags;

using RueI.Parsing.Enums;
using RueI.Parsing.Records;

/// <summary>
/// Provides a way to handle cspace tags.
/// </summary>
[RichTextTag]
public class CSpaceTag : MeasurementTag
{
    /// <inheritdoc/>
    public override string[] Names { get; } = { "cspace" };

    /// <inheritdoc/>
    public override bool AllowPercentages { get; } = false;

    /// <inheritdoc/>
    public override bool HandleTag(ParserContext context, MeasurementInfo info)
    {
        var (value, style) = info;

        float convertedValue = style switch
        {
            MeasurementUnit.Ems => value * Constants.EMSTOPIXELS,
            _ => value
        };

        context.CurrentCSpace = convertedValue;
        context.ResultBuilder.Append($"<cspace={convertedValue}>");

        context.AddEndingTag<CloseCSpaceTag>();

        return true;
    }
}
