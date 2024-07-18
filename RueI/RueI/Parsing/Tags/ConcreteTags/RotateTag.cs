namespace RueI.Parsing.Tags.ConcreteTags;

using RueI.Parsing.Enums;
using RueI.Parsing.Records;

/// <summary>
/// Provides a way to handle rotate tags.
/// </summary>
[RichTextTag]
public class RotateTag : MeasurementTag
{
    /// <inheritdoc/>
    public override string[] Names { get; } = { "rotate" };

    /// <inheritdoc/>
    public override bool HandleTag(ParserContext context, MeasurementInfo info)
    {
        context.ResultBuilder.Append($"<rotate={info}>");
        context.AddEndingTag<CloseRotateTag>();

        return true;
    }
}
