namespace RueI.Parsing.Tags.ConcreteTags;

using RueI.Parsing.Enums;
using RueI.Parsing.Records;

/// <summary>
/// Provides a way to handle margin tags.
/// </summary>
[RichTextTag]
public class MarginTag : MeasurementTag
{
    /// <inheritdoc/>
    public override string[] Names { get; } = { "margin" };

    /// <inheritdoc/>
    public override bool HandleTag(ParserContext context, MeasurementInfo info)
    {
        var (value, style) = info;

        float convertedValue = style switch
        {
            MeasurementUnit.Ems => value * Constants.EMSTOPIXELS,
            MeasurementUnit.Percentage => info.value / 100 * Constants.DISPLAYAREAWIDTH,
            _ => value
        };

        context.LeftMargin = convertedValue;
        context.RightMargin = convertedValue;

        if (context.LineHasAnyChars && (context.WidthSinceSpace + convertedValue) > context.FunctionalWidth)
        {
            Parser.CreateLineBreak(context, true);
        }

        context.SkipOverflow = true;
        context.WidthSinceSpace = 0;
        context.ResultBuilder.Append($"<margin={convertedValue}>");

        context.AddEndingTag<CloseMarginTag>();

        return true;
    }
}

/// <summary>
/// Provides a way to handle margin tags.
/// </summary>
[RichTextTag]
public class MarginLeft : MeasurementTag
{
    /// <inheritdoc/>
    public override string[] Names { get; } = { "margin-left" };

    /// <inheritdoc/>
    public override bool HandleTag(ParserContext context, MeasurementInfo info)
    {
        var (value, style) = info;

        float convertedValue = style switch
        {
            MeasurementUnit.Ems => value * Constants.EMSTOPIXELS,
            MeasurementUnit.Percentage => info.value / 100 * Constants.DISPLAYAREAWIDTH,
            _ => value
        };

        context.LeftMargin = convertedValue;

        if (context.WidthSinceSpace > 0.0001 && (context.WidthSinceSpace + convertedValue) > context.FunctionalWidth)
        {
            Parser.CreateLineBreak(context, true);
        }

        context.SkipOverflow = true;
        context.WidthSinceSpace = 0;
        context.ResultBuilder.Append($"<margin-left={convertedValue}>");

        context.AddEndingTag<CloseMarginTag>();

        return true;
    }
}

/// <summary>
/// Provides a way to handle margin tags.
/// </summary>
[RichTextTag]
public class MarginRight : MeasurementTag
{
    /// <inheritdoc/>
    public override string[] Names { get; } = { "margin-right" };

    /// <inheritdoc/>
    public override bool HandleTag(ParserContext context, MeasurementInfo info)
    {
        var (value, style) = info;

        float convertedValue = style switch
        {
            MeasurementUnit.Ems => value * Constants.EMSTOPIXELS,
            MeasurementUnit.Percentage => info.value / 100 * Constants.DISPLAYAREAWIDTH,
            _ => value
        };

        context.RightMargin = convertedValue;

        if (context.WidthSinceSpace > 0.0001 && (context.WidthSinceSpace + convertedValue) > context.FunctionalWidth)
        {
            Parser.CreateLineBreak(context, true);
        }

        context.SkipOverflow = true;
        context.WidthSinceSpace = 0;
        context.ResultBuilder.Append($"<margin-right={convertedValue}>");

        context.AddEndingTag<CloseMarginTag>();

        return true;
    }
}

/// <summary>
/// Provides a way to handle closing indent tags.
/// </summary>
[RichTextTag]
public class CloseMarginTag : ClosingTag<CloseMarginTag>
{
    /// <inheritdoc/>
    public override string Name { get; } = "/margin";

    /// <inheritdoc/>
    protected override void ApplyTo(ParserContext context)
    {
        context.LeftMargin = 0;
        context.RightMargin = 0;
    }
}
