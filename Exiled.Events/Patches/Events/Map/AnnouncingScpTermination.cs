// -----------------------------------------------------------------------
// <copyright file="AnnouncingScpTermination.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Map
{
#pragma warning disable SA1313
    using Exiled.Events.EventArgs;
    using Exiled.Events.Handlers;
    using HarmonyLib;

    /// <summary>
    /// Patches <see cref="NineTailedFoxAnnouncer.AnnounceScpTermination(Role, PlayerStats.HitInfo, string)"/>.
    /// Adds the <see cref="Map.AnnouncingScpTermination"/> event.
    /// </summary>
    [HarmonyPatch(typeof(NineTailedFoxAnnouncer), nameof(NineTailedFoxAnnouncer.AnnounceScpTermination))]
    public class AnnouncingScpTermination
    {
        /// <summary>
        /// Prefix of <see cref="NineTailedFoxAnnouncer.AnnounceScpTermination(Role, PlayerStats.HitInfo, string)"/>.
        /// </summary>
        /// <param name="scp"><inheritdoc cref="AnnouncingScpTerminationEventArgs.Role"/></param>
        /// <param name="hit"><inheritdoc cref="AnnouncingScpTerminationEventArgs.HitInfo"/></param>
        /// <param name="groupId"><inheritdoc cref="AnnouncingScpTerminationEventArgs.TerminationCause"/></param>
        /// <returns>Returns a value indicating whether the original method has to be executed or not.</returns>
        public static bool Prefix(Role scp, ref PlayerStats.HitInfo hit, ref string groupId)
        {
            var ev = new AnnouncingScpTerminationEventArgs(string.IsNullOrEmpty(hit.Attacker) ? null : API.Features.Player.Get(hit.Attacker), scp, hit, groupId);

            Map.OnAnnouncingScpTermination(ev);

            return ev.IsAllowed;
        }
    }
}
