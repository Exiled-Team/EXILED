namespace RueI.Elements;

using RueI.Displays;
using RueI.Elements.Enums;
using RueI.Elements.Interfaces;
using RueI.Parsing;
using RueI.Parsing.Records;

/// <summary>
/// Represents a simple cached element with settable content.
/// </summary>
public class SetElement : Element, ISettable
{
    private ParsedData cachedContent;
    private string content;

    /// <summary>
    /// Initializes a new instance of the <see cref="SetElement"/> class.
    /// </summary>
    /// <param name="position">The scaled position of the element, where 0 is the bottom of the screen and 1000 is the top.</param>
    /// <param name="content">The content to set the element to.</param>
    public SetElement(float position, string content = "")
        : base(position)
    {
        Position = position;
        this.content = content;
        cachedContent = Parser.Parse(content, Options);
    }

    /// <summary>
    /// Gets or sets the content of this element.
    /// </summary>
    public virtual string Content
    {
        get => content;
        set
        {
            content = value;
            cachedContent = Parser.Parse(value, Options);
        }
    }

    /// <summary>
    /// Gets or sets the options for this element.
    /// </summary>
    // we shadow here so that we can effectively override the getter/setter, which is normally not supported
    public new ElementOptions Options
    {
        get => base.Options;
        set
        {
            base.Options = value;
            cachedContent = Parser.Parse(content, value);
        }
    }

    /// <inheritdoc/>
#pragma warning disable SA1313 // Parameter names should begin with lower-case letter
    protected internal override ParsedData GetParsedData(DisplayCore _) => cachedContent;
#pragma warning restore SA1313 // Parameter names should begin with lower-case letter
}