// -----------------------------------------------------------------------
// <copyright file="Died.cs" company="Exiled Team">
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
    using UnityEngine;

    /// <summary>
    /// Patches <see cref="PlayerStats.HurtPlayer(PlayerStats.HitInfo, GameObject)"/>.
    /// Adds the <see cref="Player.Died"/> event.
    /// </summary>
    [HarmonyPatch(typeof(PlayerStats), nameof(PlayerStats.HurtPlayer))]
    public class Died
    {
        /// <summary>
        /// Postfix of <see cref="PlayerStats.HurtPlayer(PlayerStats.HitInfo, GameObject)"/>.
        /// </summary>
        /// <param name="__instance">The <see cref="PlayerStats"/> instance.</param>
        /// <param name="info"><inheritdoc cref="DiedEventArgs.HitInformations"/></param>
        /// <param name="go">The player's game object.</param>
        public static void Postfix(PlayerStats __instance, ref PlayerStats.HitInfo info, GameObject go)
        {
            API.Features.Player attacker = API.Features.Player.Get(__instance.gameObject);
            API.Features.Player target = API.Features.Player.Get(go);

            if ((target != null && (target.Role != RoleType.Spectator || target.IsGodModeEnabled || target.IsHost)) || attacker == null)
                return;

            var ev = new DiedEventArgs(API.Features.Player.Get(__instance.gameObject), target, info);

            Player.OnDied(ev);

            info = ev.HitInformations;
        }
    }
}
