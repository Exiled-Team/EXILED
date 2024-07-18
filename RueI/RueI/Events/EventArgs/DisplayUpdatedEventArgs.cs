namespace RueI.Events;

using RueI.Displays;
using RueI.Displays.Scheduling;

/// <summary>
/// Contains all information after a player's <see cref="Displays.DisplayCore"/> is updated.
/// </summary>
public class DisplayUpdatedEventArgs : EventArgs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DisplayUpdatedEventArgs"/> class.
    /// </summary>
    /// <param name="core">The <see cref="Displays.DisplayCore"/> that has been updated.</param>
    public DisplayUpdatedEventArgs(DisplayCore core)
    {
        DisplayCore = core;
    }

    /// <summary>
    /// Gets the updated <see cref="Displays.DisplayCore"/>.
    /// </summary>
    public DisplayCore DisplayCore { get; }

    /// <summary>
    /// Gets the <see cref="global::ReferenceHub"/> of the updated core.
    /// </summary>
    public ReferenceHub ReferenceHub => DisplayCore.Hub;

    /// <summary>
    /// Gets the <see cref="Displays.Scheduling.Scheduler"/> of the updated core.
    /// </summary>
    public Scheduler Scheduler => DisplayCore.Scheduler;
}
