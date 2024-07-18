namespace RueI.Displays;

using RueI.Elements;
using RueI.Extensions;

/// <summary>
/// Represents a display attached to a <see cref="DisplayCore"/> with support for <see cref="Screen"/>s.
/// </summary>
/// <remarks>
/// A <see cref="ScreenDisplay"/> is a version of the <see cref="Display"/> that contains a list of <see cref="Screen"/>s.
/// A <see cref="Screen"/> acts as a container for <see cref="Element"/>s, and only one can be active in a <see cref="ScreenDisplay"/>
/// at a time.
/// </remarks>
public class ScreenDisplay : DisplayBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ScreenDisplay"/> class.
    /// </summary>
    /// <param name="hub">The <see cref="ReferenceHub"/> to assign the display to.</param>
    /// <param name="screen">The default <see cref="Screen"/> to use for this <see cref="ScreenDisplay"/>.</param>
    public ScreenDisplay(ReferenceHub hub, Screen screen)
        : base(hub)
    {
        CurrentScreen = screen;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ScreenDisplay"/> class.
    /// </summary>
    /// <param name="coordinator">The <see cref="DisplayCore"/> to assign the display to.</param>
    /// <param name="screen">The default <see cref="Screen"/> to use for this <see cref="ScreenDisplay"/>.</param>w
    public ScreenDisplay(DisplayCore coordinator, Screen screen)
        : base(coordinator)
    {
        CurrentScreen = screen;
    }

    /// <summary>
    /// Gets the current screen of this display.
    /// </summary>
    public Screen CurrentScreen { get; private set; }

    /// <summary>
    /// Gets all of the screens of this display.
    /// </summary>
    public List<Screen> Screens { get; } = new();

    /// <summary>
    /// Gets the elements of this display that will be displayed regardless of screen.
    /// </summary>
    public List<Element> GlobalElements { get; } = new();

    /// <summary>
    /// Sets the <see cref="CurrentScreen"/> of this display.
    /// </summary>
    /// <param name="screen">The <see cref="Screen"/> to set the <see cref="CurrentScreen"/> to.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="screen"/> is not a <see cref="Screen"/> within <see cref="Screens"/>.</exception>
    public void SetScreen(Screen screen)
    {
        if (Screens.Contains(screen))
        {
            CurrentScreen = screen;
        }
        else
        {
            throw new ArgumentOutOfRangeException("screen", "Must be a screen within the ScreenDisplay");
        }
    }

    /// <inheritdoc/>
    public override IEnumerable<Element> GetAllElements()
    {
        return CurrentScreen.Elements.FilterDisabled().Concat(GlobalElements.FilterDisabled());
    }
}