namespace RueI.Displays.Scheduling;

using System.Diagnostics;

/// <summary>
/// Provides a way to implement a cooldown easily.
/// </summary>
public class Cooldown
{
    private readonly Stopwatch stopwatch = new();

    /// <summary>
    /// Gets or sets the current length of the cooldown.
    /// </summary>
    public TimeSpan Length { get; set; } = TimeSpan.Zero;

    /// <summary>
    /// Gets the amount of time left for the cooldown.
    /// </summary>
    public TimeSpan TimeLeft => Length - stopwatch.Elapsed;

    /// <summary>
    /// Gets a value indicating whether or not the cooldown is active.
    /// </summary>
    public bool Active => stopwatch.Elapsed < Length;

    /// <summary>
    /// Starts the cooldown.
    /// </summary>
    /// <param name="length">How long the cooldown should last.</param>
    public void Start(TimeSpan length)
    {
        Length = length;
        stopwatch.Restart();
    }

    /// <summary>
    /// Starts the cooldown.
    /// </summary>
    /// <param name="length">In seconds, how long the cooldown should last.</param>
    public void Start(float length) => this.Start(TimeSpan.FromSeconds(length));

    /// <summary>
    /// Pauses the cooldown.
    /// </summary>
    public void Pause()
    {
        stopwatch.Stop();
    }

    /// <summary>
    /// Resume the cooldown if it is paused.
    /// </summary>
    public void Resume()
    {
        stopwatch.Start();
    }
}