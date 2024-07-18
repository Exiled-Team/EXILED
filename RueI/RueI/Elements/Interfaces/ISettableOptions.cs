namespace RueI.Elements.Interfaces;

using RueI.Elements.Enums;

/// <summary>
/// Defines an <see cref="Element"/> with options that can be set.
/// </summary>
public interface ISettableOptions
{
    /// <summary>
    /// Gets or sets the <see cref="ElementOptions"/> of this element.
    /// </summary>
    public ElementOptions Options { get; set; }
}
