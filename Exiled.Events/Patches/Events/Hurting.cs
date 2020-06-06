// -----------------------------------------------------------------------
// <copyright file="Hurting.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events
{
    #pragma warning disable SA1313
    using System;
    using Exiled.API.Features;
    using Exiled.Events.Handlers;
    using Exiled.Events.Handlers.EventArgs;
    using HarmonyLib;
    using UnityEngine;

    /// <summary>
    /// Patches <see cref="PlayerStats.HurtPlayer(PlayerStats.HitInfo, GameObject)"/>.
    /// Adds the <see cref="Player.Hurting"/> event.
    /// </summary>
    [HarmonyPatch(typeof(PlayerStats), nameof(PlayerStats.HurtPlayer))]
    public class Hurting
    {
        /// <summary>
        /// Prefix of <see cref="PlayerStats.HurtPlayer(PlayerStats.HitInfo, GameObject)"/>.
        /// </summary>
        /// <param name="__instance">The <see cref="PlayerStats"/> instance.</param>
        /// <param name="info"><inheritdoc cref="DiedEventArgs.HitInformations"/></param>
        /// <param name="go">The player's game object.</param>
        public static void Prefix(PlayerStats __instance, ref PlayerStats.HitInfo info, GameObject go)
        {
            try
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

                Handlers.Player.OnHurting(ev);

                info = ev.HitInformations;
            }
            catch (Exception e)
            {
                Log.Error($"Hurting: {e}");
            }
        }
    }
}
