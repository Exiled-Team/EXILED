namespace RueI.Displays;

using RueI.Displays.Interfaces;
using RueI.Elements;

/// <summary>
/// Represents a <see cref="IElementContainer"/> inside a <see cref="ScreenDisplay"/>.
/// </summary>
public class Screen : IElementContainer
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Screen"/> class.
    /// </summary>
    public Screen()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Screen"/> class.
    /// </summary>
    /// <param name="scrDisplay">The <see cref="ScreenDisplay"/> to add this to.</param>
    public Screen(ScreenDisplay scrDisplay)
    {
        scrDisplay.Screens.Add(this);
    }

    /// <summary>
    /// Gets the elements of this screen.
    /// </summary>
    public List<Element> Elements { get; } = new();
}