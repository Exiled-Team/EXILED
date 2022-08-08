// -----------------------------------------------------------------------
// <copyright file="CountTeam.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.NPCs
{
    using System.Linq;
    using Exiled.API.Extensions;
    using Exiled.API.Features;
    using HarmonyLib;

#pragma warning disable SA1313
    /// <summary>
    /// Patches <see cref="RoundSummary.CountTeam"/> to ignore NPCs.
    /// </summary>
    [HarmonyPatch(typeof(RoundSummary), nameof(RoundSummary.CountTeam))]
    internal static class CountTeam
    {
        private static void Postfix(Team team, ref int __result)
        {
            __result -= Player.List.Count(player => player.IsNpc() && player.Role.Team == team);
        }
    }
}
