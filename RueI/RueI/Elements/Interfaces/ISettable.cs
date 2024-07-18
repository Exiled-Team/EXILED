namespace RueI.Elements.Interfaces;

/// <summary>
/// Defines an <see cref="Element"/> that can be set.
/// </summary>
public interface ISettable
{
    /// <summary>
    /// Gets or sets the content of this element.
    /// </summary>
    public string Content { get; set; }
}
