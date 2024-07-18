namespace RueI.Displays;

using RueI.Elements;

/// <summary>
/// Defines the base class for all displays.
/// </summary>
public abstract class DisplayBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DisplayBase"/> class.
    /// </summary>
    /// <param name="hub">The <see cref="global::ReferenceHub"/> to assign the display to.</param>
    public DisplayBase(ReferenceHub hub)
    {
        ReferenceHub = hub;
        Coordinator = DisplayCore.Get(hub);

        Coordinator.AddDisplay(this);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DisplayBase"/> class.
    /// </summary>
    /// <param name="coordinator">The <see cref="DisplayCore"/> to assign the display to.</param>
    public DisplayBase(DisplayCore coordinator)
    {
        Coordinator = coordinator;
        ReferenceHub = coordinator.Hub;

        coordinator.AddDisplay(this);
    }

    /// <summary>
    /// Gets a value indicating whether or not this display is active.
    /// </summary>
    public bool IsActive { get; private set; } = true;

    /// <summary>
    /// Gets the <see cref="global::ReferenceHub"/> that this display is assigned to.
    /// </summary>
    public ReferenceHub ReferenceHub { get; }

    /// <summary>
    /// Gets the <see cref="DisplayCore"/> that this display is attached to.
    /// </summary>
    public DisplayCore Coordinator { get; }

    /// <summary>
    /// Gets all of the elements of this display.
    /// </summary>
    /// <returns>The <see cref="IEnumerator{IElement}"/> of elements.</returns>
    public abstract IEnumerable<Element> GetAllElements();

    /// <summary>
    /// Updates the parent <see cref="DisplayCore"/> of this <see cref="DisplayBase"/>.
    /// </summary>
    public void Update() => DisplayCore.Get(ReferenceHub).Update();

    /// <summary>
    /// Deletes this display, removing it from the player's core.
    /// </summary>
    public void Delete()
    {
        Coordinator.RemoveDisplay(this);
        IsActive = false;
    }
}
