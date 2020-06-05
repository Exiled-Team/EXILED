// -----------------------------------------------------------------------
// <copyright file="RestartingRound.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events
{
    #pragma warning disable SA1313
    using Exiled.Events.Handlers;
    using Exiled.Events.Handlers.EventArgs;
    using HarmonyLib;

    /// <summary>
    /// Patches <see cref="PlayerStats.Roundrestart"/>.
    /// Adds the RestartingRound event.
    /// </summary>
    [HarmonyPatch(typeof(PlayerStats), nameof(PlayerStats.Roundrestart))]
    public class RestartingRound
    {
        /// <summary>
        /// Prefix of <see cref="PlayerStats.Roundrestart"/>.
        /// </summary>
        /// <param name="__instance">The <see cref="PlayerStats"/> instance.</param>
        public static void Prefix(PlayerStats __instance)
        {
            API.Features.Log.Debug("Round restarting");

            var ev = new RoundEndedEventArgs(RoundSummary.LeadingTeam.Draw, default, 0);

            Server.OnRestartingRound();
            Server.OnRoundEnded(ev);
        }
    }
}