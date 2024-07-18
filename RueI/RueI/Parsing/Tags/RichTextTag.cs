namespace RueI.Parsing;

using RueI.Parsing.Enums;

/// <summary>
/// Defines the base class for all rich text tags.
/// </summary>
public abstract class RichTextTag
{
    /// <summary>
    /// Gets the names of this tag.
    /// </summary>
    public abstract string[] Names { get; }

    /// <summary>
    /// Gets the parameter style of this tag.
    /// </summary>
    /// <remarks>
    /// This property is used to determine what delimiters between the tag name and parameters (e.g. space, equal sign, none) will be allowed. For multiple tag style, create different classes.
    /// </remarks>
    public abstract TagStyle TagStyle { get; }

    /// <summary>
    /// Applies this tag to a <see cref="ParserContext"/>.
    /// </summary>
    /// <param name="context">The context of the parser.</param>
    /// <param name="parameters">The parameters of the tag, excluding the delimiter.</param>
    /// <returns>true if the tag is valid, otherwise false.</returns>
    /// <remarks>If the tag does not have parameters, <paramref name="parameters"/> will always be <see cref="string.Empty"/>.</remarks>
    public abstract bool HandleTag(ParserContext context, string parameters);
}
