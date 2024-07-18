namespace RueI.Elements;

using RueI.Displays;
using RueI.Elements.Enums;
using RueI.Parsing;
using RueI.Parsing.Records;

/// <summary>
/// Represents the base class for all elements, which are individual 'hints' present within an arbitrary number of <see cref="Displays.Display"/>s.
/// </summary>
/// <remarks>
/// An <see cref="Element"/> is how text is displayed within RueI. Each <see cref="Element"/>
/// acts like an individual <see cref="Hints.Hint"/>, and cannot influence other <see cref="Element"/>s.
/// </remarks>
public abstract class Element
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Element"/> class.
    /// </summary>
    /// <param name="position">The position of the element, where 0 represents the bottom of the screen and 1000 represents the top.</param>
    public Element(float position)
    {
        Position = position;
    }

    /// <summary>
    /// Gets or sets a value indicating whether or not this element is enabled and will show.
    /// </summary>
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// Gets or sets the position of the element on a scale from 0-1000, where 0 represents the bottom of the screen and 1000 represents the top.
    /// </summary>
    public float Position { get; set; }

    /// <summary>
    /// Gets or sets the priority of the hint (determining if it shows above another hint).
    /// </summary>
    public int ZIndex { get; set; } = 1;

    /// <summary>
    /// Gets or sets the <see cref="Parser"/> currently in use by this <see cref="Element"/>.
    /// </summary>
    /// <remarks>
    /// Implementations should default this to <see cref="Parser.DefaultParser"/>.
    /// </remarks>
    public Parser Parser { get; set; } = Parser.DefaultParser;

    /// <summary>
    /// Gets or sets the options for this element.
    /// </summary>
    public virtual ElementOptions Options { get; protected set; } = ElementOptions.Default;

    /// <summary>
    /// Gets the data used for parsing.
    /// </summary>
    /// <param name="core">The <see cref="DisplayCore"/> of the player.</param>
    /// <returns>The <see cref="ParsedData"/> for the element.</returns>
    /// <remarks>
    /// This contains information used to ensure that multiple elements can be displayed at once. To obtain this, you must use <see cref="Parser.Parse"/>.
    /// </remarks>
    protected internal abstract ParsedData GetParsedData(DisplayCore core);
}