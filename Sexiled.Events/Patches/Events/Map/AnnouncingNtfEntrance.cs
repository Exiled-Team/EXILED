// -----------------------------------------------------------------------
// <copyright file="AnnouncingNtfEntrance.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

using Sexiled.Events.EventArgs;
using Sexiled.Events.Handlers;

namespace Sexiled.Events.Patches.Events.Map
{
    using System.Linq;

    using Sexiled.Events.EventArgs;
    using Sexiled.Events.Handlers;

    using HarmonyLib;

    using Respawning.NamingRules;

    /// <summary>
    /// Patch the <see cref="NineTailedFoxNamingRule.PlayEntranceAnnouncement(string)"/>.
    /// Adds the <see cref="Map.AnnouncingNtfEntrance"/> event.
    /// </summary>
    [HarmonyPatch(typeof(NineTailedFoxNamingRule), nameof(NineTailedFoxNamingRule.PlayEntranceAnnouncement))]
    internal static class AnnouncingNtfEntrance
    {
        private static bool Prefix(ref string regular)
        {
            int scpsLeft = API.Features.Player.List.Where(player => player.Team == Team.SCP && player.Role != RoleType.Scp0492).Count();
            string[] unitInformations = regular.Split('-');

            var ev = new AnnouncingNtfEntranceEventArgs(scpsLeft, unitInformations[0], int.Parse(unitInformations[1]));

            Handlers.Map.OnAnnouncingNtfEntrance(ev);

            regular = $"{ev.UnitName}-{ev.UnitNumber}";

            return ev.IsAllowed;
        }
    }
}
