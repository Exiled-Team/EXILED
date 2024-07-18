namespace RueI.Parsing;

using RueI.Parsing.Enums;

/// <summary>
/// Defines a <see cref="RichTextTag"/> that does not take in parameters.
/// </summary>
public abstract class NoParamsTag : RichTextTag
{
    /// <inheritdoc/>
    public sealed override TagStyle TagStyle { get; } = TagStyle.NoParams;

    /// <inheritdoc/>
    public sealed override bool HandleTag(ParserContext context, string parameters) => HandleTag(context);

    /// <summary>
    /// Applies this tag (without parameters) to a <see cref="ParserContext"/>.
    /// </summary>
    /// <param name="context">The context of the parser.</param>
    /// <returns>true if the tag is valid, otherwise false.</returns>
    public abstract bool HandleTag(ParserContext context);
}
