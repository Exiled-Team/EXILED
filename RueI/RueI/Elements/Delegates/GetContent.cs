namespace RueI.Elements.Delegates;

using RueI.Displays;

/// <summary>
/// Defines a method used to get content for an element.
/// </summary>
/// <param name="hub">The <see cref="DisplayCore"/> of the player.</param>
/// <returns>A string with the new content.</returns>
public delegate string GetContent(DisplayCore hub);
