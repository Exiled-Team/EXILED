// -----------------------------------------------------------------------
// <copyright file="EventHandlers.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Metrics;

using System.Collections.Generic;

using Exiled.API.Features;
using Exiled.Events.EventArgs;
using Exiled.Metrics.API.Enums;

using MEC;

/// <summary>
/// General event handlers.
/// </summary>
public class EventHandlers
{
    private readonly Plugin plugin;
    private double tps;
    private CoroutineHandle coroutine;

    /// <summary>
    /// Initializes a new instance of the <see cref="EventHandlers"/> class.
    /// </summary>
    /// <param name="plugin">The <see cref="Plugin{TConfig}"/> class reference.</param>
    public EventHandlers(Plugin plugin) => this.plugin = plugin;

    public void OnRoundStarted()
    {
        coroutine = Timing.RunCoroutine(CalculateMetrics());
    }

    private IEnumerator<float> CalculateMetrics()
    {
        for (; ;)
        {
            plugin.Methods.SendMetrics(EventType.None);

            yield return Timing.WaitForSeconds(2f);
        }
    }

    public void OnRoundEnded(RoundEndedEventArgs ev)
    {
        Timing.KillCoroutines(coroutine);
        plugin.Methods.SendMetrics(EventType.RoundEnd, ev.LeadingTeam);
    }
}
