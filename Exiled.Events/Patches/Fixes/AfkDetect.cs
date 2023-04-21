// -----------------------------------------------------------------------
// <copyright file="AfkDetect.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Fixes
{
    using System.Diagnostics;

    using AFK;
    using Exiled.API.Features;
    using HarmonyLib;
    using Mirror;

    /// <summary>
    /// Patches <see cref="AFKManager.AddPlayer"/> to prevent NPCs from being added as AFK.
    /// </summary>
    [HarmonyPatch(typeof(AFKManager), nameof(AFKManager.AddPlayer))]
    internal static class AfkDetect
    {
        [HarmonyPrefix]
        private static void OnDetect(ReferenceHub hub)
        {
            var player = Player.Get(hub);

            if (!NetworkServer.active || player.IsNpc || hub == ReferenceHub.HostHub || AFKManager.AFKTimers.ContainsKey(hub) || PermissionsHandler.IsPermitted(hub.serverRoles.Permissions, PlayerPermissions.AFKImmunity))
                return;
            AFKManager.AFKTimers.Add(hub, Stopwatch.StartNew());
        }
    }
}