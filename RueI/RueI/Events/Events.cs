namespace RueI.Events;

/// <summary>
/// Provides events for use by plugins using RueI.
/// </summary>
public static class Events
{
    /// <summary>
    /// Represents a custom RueI event.
    /// </summary>
    /// <typeparam name="T">The <see cref="EventArgs"/> class to use.</typeparam>
    /// <param name="ev">The event args to use. </param>
    public delegate void RueIEvent<T>(T ev)
        where T : EventArgs;

    /// <summary>
    /// Called after a player's <see cref="Displays.DisplayCore"/> is updated.
    /// </summary>
    public static event RueIEvent<DisplayUpdatedEventArgs>? DisplayUpdated;

    /// <summary>
    /// Calls <see cref="DisplayUpdated"/> after a display is updated.
    /// </summary>
    /// <param name="ev">The event args to use.</param>
    internal static void OnDisplayUpdated(DisplayUpdatedEventArgs ev)
    {
        DisplayUpdated?.Invoke(ev);
    }
}
