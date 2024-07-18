namespace RueI.Parsing;

using System.Text;

using NorthwoodLib.Pools;

using RueI.Parsing.Tags;

/// <summary>
/// Describes the state of a parser at a time.
/// </summary>
/// <remarks>
/// The <see cref="ParserContext"/> class provides a way for the general state of the parser,
/// such as the current line width or the vertical height, to be modified by passing it along.
/// Tags should modify this in order to change the end result of parsing.
/// </remarks>
public class ParserContext : TextInfo, IDisposable
{
    /// <summary>
    /// Gets a list of tags that the parser should add at the end.
    /// </summary>
    private readonly List<NoParamsTag> endingTags = new(10);

    /// <summary>
    /// Gets the end result string builder.
    /// </summary>
    public StringBuilder ResultBuilder { get; } = StringBuilderPool.Shared.Rent();

    /// <summary>
    /// Gets or sets the final offset for the element as a whole.
    /// </summary>
    public float NewOffset { get; set; } = 0;

    /// <summary>
    /// Gets or sets the current width of the text.
    /// </summary>
    public float DisplayAreaWidth { get; set; } = Constants.DISPLAYAREAWIDTH;

    /// <summary>
    /// Gets the current functional width of the text.
    /// </summary>
    public float FunctionalWidth => DisplayAreaWidth - LeftMargin - RightMargin;

    /// <summary>
    /// Gets a stack containing all of the nested sizes.
    /// </summary>
    public Stack<float> SizeTags { get; } = new();

    /// <summary>
    /// Gets or sets the current line width of the parser.
    /// </summary>
    public float CurrentLineWidth { get; set; } = 0;

    /// <summary>
    /// Gets or sets the current space buffer of the parser.
    /// </summary>
    public float SpaceBuffer { get; set; } = 0;

    /// <summary>
    /// Gets or sets the newline buffer of the parser.
    /// </summary>
    public float NewlineBuffer { get; set; } = 0;

    /// <summary>
    /// Gets or sets the current indent of the parser.
    /// </summary>
    public float Indent { get; set; } = 0;

    /// <summary>
    /// Gets or sets the current voffset of the parser.
    /// </summary>
    public float VOffset { get; set; } = 0;

    /// <summary>
    /// Gets or sets the current line indent of the parser.
    /// </summary>
    public float LineIndent { get; set; } = 0;

    /// <summary>
    /// Gets or sets a value indicating whether the parser should parse tags other than <see cref="Tags.ConcreteTags.CloseNoparseTag"/>.
    /// </summary>
    public bool ShouldParse { get; set; } = true;

    /// <summary>
    /// Gets or sets the total width since a space.
    /// </summary>
    public float WidthSinceSpace { get; set; } = 0;

    /// <summary>
    /// Gets or sets a value indicating whether or not words are currently in no break.
    /// </summary>
    public bool NoBreak { get; set; } = false;

    /// <summary>
    /// Gets or sets the number of color tags that are nested.
    /// </summary>
    public int ColorTags { get; set; } = 0;

    /// <summary>
    /// Gets or sets the biggest char size of the line.
    /// </summary>
    public float BiggestCharSize { get; set; } = 0;

    /// <summary>
    /// Gets or sets a value indicating whether or not overflow checks will be skipped.
    /// </summary>
    public bool SkipOverflow { get; set; } = false;

    /// <summary>
    /// Gets or sets the right margin of the line.
    /// </summary>
    public float RightMargin { get; set; } = 0;

    /// <summary>
    /// Gets or sets the left margin of the line.
    /// </summary>
    public float LeftMargin { get; set; } = 0;

    /// <summary>
    /// Gets or sets a value indicating whether or not the current line has any characters.
    /// </summary>
    public bool LineHasAnyChars { get; set; } = false;

    /// <summary>
    /// Adds a <see cref="RichTextTag"/> to a list of tags that will be added to the end of the parser's result.
    /// </summary>
    /// <typeparam name="T">The type of the <see cref="RichTextTag"/> to be added as an ending tag (as a <see cref="SharedTag{Tags}"/>).</typeparam>
    /// <param name="allowDuplicates">Whether or not duplicates are allowed, accommodating for nested tags.</param>
    public void AddEndingTag<T>(bool allowDuplicates = false)
        where T : NoParamsTag, new()
    {
        NoParamsTag singleton = SharedTag<T>.Singleton;

        if (!allowDuplicates && endingTags.Contains(singleton))
        {
            return;
        }

        endingTags.Add(singleton);
    }

    /// <summary>
    /// Removes a <see cref="RichTextTag"/> from the list list of tags that will be added to the end of the parser's result.
    /// </summary>
    /// <typeparam name="T">The type of the <see cref="RichTextTag"/> to be removed from the ending tags (as a <see cref="SharedTag{Tags}"/>).</typeparam>
    public void RemoveEndingTag<T>()
        where T : NoParamsTag, new()
    {
        endingTags.Remove(SharedTag<T>.Singleton);
    }

    /// <summary>
    /// Applies the <see cref="endingTags"/> and closing <see cref="SizeTags"/> tags to this <see cref="ParserContext"/>.
    /// </summary>
    public void ApplyClosingTags()
    {
        foreach (NoParamsTag tag in endingTags.ToList())
        {
            tag.HandleTag(this);
        }

        foreach (float t in SizeTags)
        {
            ResultBuilder.Append("</size>");
        }

        SizeTags.Clear();
        endingTags.Clear();
    }

    /// <summary>
    /// Disposes this ParserContext, returning the string builder to the pool.
    /// </summary>
    public void Dispose()
    {
        StringBuilderPool.Shared.Return(ResultBuilder);
    }
}