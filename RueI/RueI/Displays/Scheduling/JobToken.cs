namespace RueI.Displays.Scheduling;

/// <summary>
/// Represents a reference to any number of <see cref="ScheduledJob"/>.
/// </summary>
/// <remarks>
/// A <see cref="JobToken"/> provides a unique identifier for a <see cref="ScheduledJob"/> within any number of
/// <see cref="Scheduler"/>s. In other words, a <see cref="JobToken"/> can reference multiple (or no) jobs,
/// but only a single job with the given <see cref="JobToken"/> can exist in a <see cref="Scheduler"/>.
/// </remarks>
/// <seealso cref="ScheduledJob"/>
public class JobToken
{
    /// <summary>
    /// Initializes a new instance of the <see cref="JobToken"/> class.
    /// </summary>
    public JobToken()
    {
    }
}
