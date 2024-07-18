namespace RueI.Parsing.Tags.ConcreteTags;

using RueI.Parsing.Records;

/// <summary>
/// Provides a way to handle voffset tags.
/// </summary>
[RichTextTag]
public class VOffsetTag : MeasurementTag
{
    /// <inheritdoc/>
    public override bool AllowPercentages { get; } = false;

    /// <inheritdoc/>
    public override string[] Names { get; } = { "voffset" };

    /// <inheritdoc/>
    public override bool HandleTag(ParserContext context, MeasurementInfo info)
    {
        // this is far from how voffsets actually work but this works fine enough
        SharedTag<LineHeightTag>.Singleton.HandleTag(context, info with { value = -info.value });

        context.ResultBuilder.Append('\n');
        Parser.CreateLineBreak(context);

        SharedTag<CloseLineHeightTag>.Singleton.HandleTag(context);

        return true;
    }
}
