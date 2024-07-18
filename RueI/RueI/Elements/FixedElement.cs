namespace RueI.Elements;

using RueI.Displays;
using RueI.Elements.Delegates;
using RueI.Elements.Enums;
using RueI.Parsing;
using RueI.Parsing.Records;

/// <summary>
/// Represents a cached element with a fixed content.
/// </summary>
public class FixedElement : Element
{
    private readonly ParsedData parsedData;

    /// <summary>
    /// Initializes a new instance of the <see cref="FixedElement"/> class.
    /// </summary>
    /// <param name="position">The scaled position of the element, where 0 is the bottom of the screen and 1000 is the top.</param>
    /// <param name="content">The content to set the element to.</param>
    /// <param name="options">The options of the element.</param>
    /// <param name="parser">A <see cref="Parser"/> to use, or null to use the default parser.</param>
    public FixedElement(float position, string content, ElementOptions options = ElementOptions.Default, Parser? parser = null)
        : base(position)
    {
        parsedData = (parser ?? Parser.DefaultParser).Parse(content, options);
    }

    /// <inheritdoc/>
    protected internal override ParsedData GetParsedData(DisplayCore core) => parsedData;
}