namespace RueI.Displays.Interfaces;

using RueI.Elements;

/// <summary>
/// Defines a container for multiple elements.
/// </summary>
public interface IElementContainer
{
    /// <summary>
    /// Gets the elements of this <see cref="IElementContainer"/>.
    /// </summary>
    public List<Element> Elements { get; }
}
