namespace RueI.Parsing.Tags.ConcreteTags;

using RueI.Parsing.Enums;
using RueI.Parsing.Records;

/// <summary>
/// Provides a way to handle space tags.
/// </summary>
[RichTextTag]
public class SpaceTag : MeasurementTag
{
    /// <inheritdoc/>
    public override string[] Names { get; } = { "space" };

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

        if (context.WidthSinceSpace > 0.0001 && (context.WidthSinceSpace + convertedValue) > context.FunctionalWidth)
        {
            Parser.CreateLineBreak(context, true);
        }

        context.WidthSinceSpace += convertedValue;

        context.SkipOverflow = true;
        context.ResultBuilder.Append($"<space={convertedValue}>");

        return true;
    }
}
