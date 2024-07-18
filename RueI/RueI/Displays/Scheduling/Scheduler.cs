namespace RueI.Displays.Scheduling;

using Utils.NonAllocLINQ;

using RueI.Displays.Scheduling.Records;
using RueI.Extensions;

/// <summary>
/// Provides a means of doing batch operations.
/// </summary>
/// <remarks>
/// The <see cref="Scheduler"/> is a powerful class that enables "batch operations". This means that multiple updates to a display can happen at once, helping to avoid the hint ratelimit.
/// More detailed information is available at <see href="https://ruemena.github.io/RueI/markdown/scheduling.html">Using the Scheduler</see>.
/// </remarks>
public class Scheduler
{
    private static readonly Action EmptyAction = () => { };
    private static readonly TimeSpan MinimumBatch = TimeSpan.FromMilliseconds(300);

    private readonly DisplayCore core;

    private readonly Cooldown hintRateLimit = new();
    private readonly List<ScheduledJob> jobs = new();

    private RueI.UnityAlternative.IAsyncOperation? performTask;

    private BatchJob? nextBatch;

    /// <summary>
    /// Initializes a new instance of the <see cref="Scheduler"/> class.
    /// </summary>
    /// <param name="core">The <see cref="DisplayCore"/> to use.</param>
    internal Scheduler(DisplayCore core)
    {
        this.core = core;
    }

    /// <summary>
    /// Gets a value indicating whether or not the rate limit is currently active.
    /// </summary>
    internal bool RateLimitActive => hintRateLimit.Active;

    /// <summary>
    /// Gets the <see cref="DateTimeOffset"/> used by <see cref="Scheduler"/> classes for the current timeSChedued.
    /// </summary>
    private static DateTimeOffset Now => DateTimeOffset.UtcNow;

    /// <summary>
    /// Calculates the weighted time for a list of jobs to be performed.
    /// </summary>
    /// <param name="jobs">The <see cref="ScheduledJob"/> operations to schedule.</param>
    /// <returns>The weighted <see cref="DateTimeOffset"/> of all of the jobs.</returns>
    public static DateTimeOffset CalculateWeighted(IEnumerable<ScheduledJob> jobs)
    {
        if (!jobs.Any())
        {
            return default;
        }

        long currentSum = 0;
        int prioritySum = 0;

        foreach (ScheduledJob job in jobs)
        {
            currentSum += job.FinishAt.ToUnixTimeMilliseconds() * job.Priority;
            prioritySum += job.Priority;
        }

        return DateTimeOffset.FromUnixTimeMilliseconds(currentSum / prioritySum);
    }

    /// <summary>
    /// Schedules a <see cref="ScheduledJob"/>.
    /// </summary>
    /// <param name="job">The job to schedule.</param>
    public void Schedule(ScheduledJob job)
    {
        ScheduleNoUpdate(job);
        UpdateBatches();
    }

    /// <summary>
    /// Schedules multiple <see cref="ScheduledJob"/> operations.
    /// </summary>
    /// <param name="jobs">The jobs to schedule.</param>
    /// <remarks>
    /// When scheduling multiple jobs at a time, this method is preferred to calling <see cref="Schedule(ScheduledJob)"/> several
    /// times since it only recalculates the batches once.
    /// </remarks>
    public void Schedule(IEnumerable<ScheduledJob> jobs)
    {
        // since we must check every job for duplicates using the JobToken,
        // AddRange or similar can't be used
        foreach (ScheduledJob job in jobs)
        {
            ScheduleNoUpdate(job);
        }

        UpdateBatches();
    }

    /// <summary>
    /// Schedules multiple <see cref="ScheduledJob"/> operations.
    /// </summary>
    /// <param name="job">The first <see cref="ScheduledJob"/> to schedule.</param>
    /// <param name="jobs">The rest of the <see cref="ScheduledJob"/> operations to schedule.</param>
    /// <inheritdoc cref="Schedule(IEnumerable{ScheduledJob})" path="/remarks"/>
    public void Schedule(ScheduledJob job, params ScheduledJob[] jobs)
    {
        ScheduleNoUpdate(job);

        Schedule(jobs); // unnecessary to call UpdateBatches, since this already does it
    }

    /// <summary>
    /// Schedules an uncancellable update <see cref="ScheduledJob"/>.
    /// </summary>
    /// <param name="time">How long into the future to update at.</param>
    /// <param name="priority">The priority of the <see cref="ScheduledJob"/>, giving it additional weight when calculating.</param>
    public void ScheduleUpdate(TimeSpan time, int priority)
    {
        jobs.Add(new(Now + time, EmptyAction, priority));
        UpdateBatches();
    }

    /// <summary>
    /// Schedules an update <see cref="ScheduledJob"/> with the <see cref="JobToken"/>.
    /// </summary>
    /// <param name="time">How long into the future to update at.</param>
    /// <param name="priority">The priority of the <see cref="ScheduledJob"/>, giving it additional weight when calculating.</param>
    /// <param name="token">A token to assign to the <see cref="ScheduledJob"/>.</param>
    public void ScheduleUpdateToken(TimeSpan time, int priority, JobToken token)
    {
        Schedule(new ScheduledJob(Now + time, EmptyAction, priority, token));
    }

    /// <summary>
    /// Schedules a new <see cref="ScheduledJob"/>.
    /// </summary>
    /// <param name="time">How long into the future to run the action at.</param>
    /// <param name="action">The <see cref="Action"/> to run.</param>
    /// <param name="priority">The priority of the job, giving it additional weight when calculating.</param>
    /// <param name="token">An optional token to assign to the <see cref="ScheduledJob"/>.</param>
    public void Schedule(TimeSpan time, Action action, int priority, JobToken? token = null)
    {
        Schedule(new ScheduledJob(Now + time, action, priority, token));
    }

    /// <summary>
    /// Schedules a <see cref="ScheduledJob"/> with a priority of 1.
    /// </summary>
    /// <param name="time">How long into the future to run the action at.</param>
    /// <param name="action">The <see cref="Action"/> to run.</param>
    /// <param name="token">An optional token to assign to the <see cref="ScheduledJob"/>.</param>
    public void Schedule(TimeSpan time, Action action, JobToken? token = null)
    {
        Schedule(time, action, 1, token);
    }

    /// <summary>
    /// Replaces the <see cref="ScheduledJob"/> with the <see cref="JobToken"/> with a new job.
    /// </summary>
    /// <param name="time">How long into the future to run the action at.</param>
    /// <param name="action">The <see cref="Action"/> to run.</param>
    /// <param name="token">An optional token to assign to the <see cref="ScheduledJob"/>.</param>
    /// <param name="priority">The priority of the job.</param>
    public void ReplaceJob(TimeSpan time, Action action, JobToken token, int priority = 1)
    {
        int index = jobs.FindIndex(x => x.Token == token);
        if (index != -1)
        {
            jobs.RemoveAt(index);
        }

        Schedule(time, action, priority, token);
    }

    /// <summary>
    /// Attempts to kill the <see cref="ScheduledJob"/> with the <see cref="JobToken"/>.
    /// </summary>
    /// <param name="token">The <see cref="JobToken"/> to use as a reference.</param>
    public void KillJob(JobToken token)
    {
        int index = jobs.FindIndex(x => x.Token == token);
        if (index != -1)
        {
            jobs.RemoveAt(index);
        }

        UpdateBatches();
    }

    /// <summary>
    /// Delays any updates from occuring for a certain period of time.
    /// </summary>
    /// <param name="time">The amount of time to delay for.</param>
    internal void Delay(TimeSpan time)
    {
        hintRateLimit.Start(time.Max(MinimumBatch));
    }

    /// <summary>
    /// Recalculates the next <see cref="BatchJob"/> for this <see cref="Scheduler"/>, and potentially starts it.
    /// </summary>
    private void UpdateBatches()
    {
        if (!jobs.Any())
        {
            performTask?.Cancel();
            nextBatch = null;
            return;
        }

        if (core.IgnoreUpdate)
        {
            return;
        }

        List<ScheduledJob> currentBatch = new(2);

        DateTimeOffset least = DateTimeOffset.MaxValue;
        foreach (ScheduledJob job in jobs)
        {
            DateTimeOffset finishAt = job.FinishAt;
            if (finishAt < least)
            {
                least = finishAt;
            }
        }

        DateTimeOffset currentBatchTime = least + MinimumBatch;

        foreach (ScheduledJob job in jobs)
        {
            if (job.FinishAt < currentBatchTime)
            {
                currentBatch.Add(job);
            }
        }

        if (currentBatch.Count == 0)
        {
            // handle cases where a scheduledjob being removed
            // results in nothing to do
            performTask?.Cancel();
            nextBatch = null;
        }
        else
        {
            DateTimeOffset dateTimePerform = CalculateWeighted(currentBatch);
            nextBatch = new(currentBatch, dateTimePerform);

            TimeSpan performIn = (dateTimePerform - Now).MaxIf(hintRateLimit.Active, hintRateLimit.TimeLeft);

            performTask?.Cancel();
            performTask = UnityAlternative.Provider.PerformAsync(performIn, PerformFirstBatch);
        }
    }

    /// <summary>
    /// Immediately performs the first <see cref="BatchJob"/>.
    /// </summary>
    private void PerformFirstBatch()
    {
        if (nextBatch == null || core.IgnoreUpdate)
        {
            return;
        }

        core.IgnoreUpdate = true;

        nextBatch.Jobs.Sort();

        foreach (ScheduledJob job in nextBatch.Jobs)
        {
            jobs.Remove(job);
            job.Action();
        }

        core.IgnoreUpdate = false;

        hintRateLimit.Start(Constants.HintRateLimit);

        core.InternalUpdate();
        UpdateBatches();
    }

    /// <summary>
    /// Schedules a job without recalculating the batches.
    /// </summary>
    /// <param name="job">The <see cref="ScheduledJob"/> to schedule.</param>
    private void ScheduleNoUpdate(ScheduledJob job)
    {
        if (job.Token == null || !jobs.Any(x => x.Token == job.Token))
        {
            jobs.Add(job);
        }
    }
}
