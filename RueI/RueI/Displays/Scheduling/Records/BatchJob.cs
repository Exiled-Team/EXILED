namespace RueI.Displays.Scheduling.Records;

/// <summary>
/// Defines a number of <see cref="ScheduledJob"/>s that will performed at a certain time.
/// </summary>
///
internal record BatchJob
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BatchJob"/> class.
    /// </summary>
    /// <param name="jobs">The jobs to perform.</param>
    /// <param name="performAt">When the jobs should be performed.</param>
    public BatchJob(List<ScheduledJob> jobs, DateTimeOffset performAt)
    {
        Jobs = jobs;
        PerformAt = performAt;
    }

    /// <summary>
    /// Gets the list of <see cref="ScheduledJob"/> for this <see cref="BatchJob"/>.
    /// </summary>
    public List<ScheduledJob> Jobs { get; }

    /// <summary>
    /// Gets when this <see cref="BatchJob"/> will be performed.
    /// </summary>
    public DateTimeOffset PerformAt { get; }
}
