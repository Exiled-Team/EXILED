// -----------------------------------------------------------------------
// <copyright file="AnnouncingScpTermination.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

using Sexiled.Events.EventArgs;
using Sexiled.Events.Handlers;

namespace Sexiled.Events.Patches.Events.Map
{
    using Sexiled.Events.EventArgs;
    using Sexiled.Events.Handlers;

    using HarmonyLib;

    /// <summary>
    /// Patches <see cref="NineTailedFoxAnnouncer.AnnounceScpTermination(Role, PlayerStats.HitInfo, string)"/>.
    /// Adds the <see cref="Map.AnnouncingScpTermination"/> event.
    /// </summary>
    [HarmonyPatch(typeof(NineTailedFoxAnnouncer), nameof(NineTailedFoxAnnouncer.AnnounceScpTermination))]
    internal static class AnnouncingScpTermination
    {
        private static bool Prefix(Role scp, ref PlayerStats.HitInfo hit, ref string groupId)
        {
            var ev = new AnnouncingScpTerminationEventArgs(string.IsNullOrEmpty(hit.Attacker) ? null : API.Features.Player.Get(hit.Attacker), scp, hit, groupId);

            Handlers.Map.OnAnnouncingScpTermination(ev);

            hit = ev.HitInfo;
            groupId = ev.TerminationCause;

            return ev.IsAllowed;
        }
    }
}
