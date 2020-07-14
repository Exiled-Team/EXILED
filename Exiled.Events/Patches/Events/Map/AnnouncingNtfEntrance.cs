// -----------------------------------------------------------------------
// <copyright file="AnnouncingNtfEntrance.cs" company="Exiled Team">
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
    using Respawning.NamingRules;

    /// <summary>
    /// Patch the <see cref="UnitNamingRule.PlayEntranceAnnouncement(string)"/>.
    /// Adds the <see cref="Map.AnnouncingNtfEntrance"/> event.
    /// </summary>
    [HarmonyPatch(typeof(UnitNamingRule), nameof(UnitNamingRule.PlayEntranceAnnouncement))]
    internal class AnnouncingNtfEntrance
    {
        private static bool Prefix(UnitNamingRule __instance, ref string regular)
        {
            var ev = new AnnouncingNtfEntranceEventArgs(0, 0, 'a');

            Map.OnAnnouncingNtfEntrance(ev);

            return ev.IsAllowed;
        }
    }
}
