namespace RueI.Parsing.Tags.ConcreteTags;

/// <summary>
/// Provides a way to handle nobr tags.
/// </summary>
[RichTextTag]
public class NobrTag : NoParamsTag
{
    /// <inheritdoc/>
    public override string[] Names { get; } = { "nobr" };

    /// <inheritdoc/>
    public override bool HandleTag(ParserContext context)
    {
        return false; // TODO: fix how nobr works

        context.NoBreak = true;

        context.AddEndingTag<CloseNobrTag>();
        context.ResultBuilder.Append("<nobr>");

        return true;
    }
}
