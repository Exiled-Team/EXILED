// -----------------------------------------------------------------------
// <copyright file="AnnouncingNtfEntrance.cs" company="SEXiled Team">
// Copyright (c) SEXiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace SEXiled.Events.Patches.Events.Map
{
    using System.Linq;

    using SEXiled.Events.EventArgs;
    using SEXiled.Events.Handlers;

    using HarmonyLib;

    using Respawning.NamingRules;

    using Player = SEXiled.API.Features.Player;

    /// <summary>
    /// Patch the <see cref="NineTailedFoxNamingRule.PlayEntranceAnnouncement(string)"/>.
    /// Adds the <see cref="Map.AnnouncingNtfEntrance"/> event.
    /// </summary>
    [HarmonyPatch(typeof(NineTailedFoxNamingRule), nameof(NineTailedFoxNamingRule.PlayEntranceAnnouncement))]
    internal static class AnnouncingNtfEntrance
    {
        private static bool Prefix(ref string regular)
        {
            int scpsLeft = Player.List.Count(player => player.Role.Team == Team.SCP && player.Role != RoleType.Scp0492);
            string[] unitInformation = regular.Split('-');

            AnnouncingNtfEntranceEventArgs ev = new AnnouncingNtfEntranceEventArgs(scpsLeft, unitInformation[0], int.Parse(unitInformation[1]));

            Map.OnAnnouncingNtfEntrance(ev);

            regular = $"{ev.UnitName}-{ev.UnitNumber}";

            return ev.IsAllowed;
        }
    }
}
