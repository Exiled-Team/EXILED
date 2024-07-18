namespace RueI.Displays;

using RueI.Elements;

/// <summary>
/// Represents a reference to an element present within any number of player's displays.
/// </summary>
/// <typeparam name="T">The type of the element to act as a reference to.</typeparam>
public interface IElemReference<out T>
    where T : Element
{
}
