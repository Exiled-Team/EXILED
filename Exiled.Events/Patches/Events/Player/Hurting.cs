// -----------------------------------------------------------------------
// <copyright file="Hurting.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
#pragma warning disable SA1313
    using Exiled.Events.EventArgs;
    using Exiled.Events.Handlers;
    using HarmonyLib;
    using UnityEngine;

    /// <summary>
    /// Patches <see cref="PlayerStats.HurtPlayer(PlayerStats.HitInfo, GameObject)"/>.
    /// Adds the <see cref="Player.Hurting"/> event.
    /// </summary>
    [HarmonyPatch(typeof(PlayerStats), nameof(PlayerStats.HurtPlayer))]
    internal class Hurting
    {
        private static void Prefix(PlayerStats __instance, ref PlayerStats.HitInfo info, GameObject go)
        {
            if (go == null)
                return;

            API.Features.Player attacker = API.Features.Player.Get(__instance.gameObject);
            API.Features.Player target = API.Features.Player.Get(go);

            if (attacker == null || target == null || attacker.IsHost || target.IsHost)
                return;

            var ev = new HurtingEventArgs(API.Features.Player.Get(__instance.gameObject), API.Features.Player.Get(go), info);

            if (ev.Target.IsHost)
                return;

            Player.OnHurting(ev);

            info = ev.HitInformations;
        }
    }
}
