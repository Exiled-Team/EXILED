// -----------------------------------------------------------------------
// <copyright file="Died.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
#pragma warning disable SA1313
    using System;

    using Exiled.Events.EventArgs;
    using Exiled.Events.Handlers;

    using HarmonyLib;

    using UnityEngine;

    /// <summary>
    /// Patches <see cref="PlayerStats.HurtPlayer(PlayerStats.HitInfo, GameObject, bool)"/>.
    /// Adds the <see cref="Player.Died"/> event.
    /// </summary>
    [HarmonyPatch(typeof(PlayerStats), nameof(PlayerStats.HurtPlayer))]
    internal static class Died
    {
        private static void Postfix(PlayerStats __instance, ref PlayerStats.HitInfo info, GameObject go)
        {
            try
            {
                API.Features.Player attacker = API.Features.Player.Get(__instance.gameObject);
                API.Features.Player target = API.Features.Player.Get(go);

                if ((target != null &&
                     (target.Role != RoleType.Spectator || target.IsGodModeEnabled || target.IsHost)) ||
                    attacker == null)
                    return;

                var ev = new DiedEventArgs(API.Features.Player.Get(__instance.gameObject), target, info);

                Player.OnDied(ev);

                info = ev.HitInformations;
            }
            catch (Exception e)
            {
                Exiled.API.Features.Log.Error($"Exiled.Events.Patches.Events.Player.Died: {e}\n{e.StackTrace}");
            }
        }
    }
}
