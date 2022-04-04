// -----------------------------------------------------------------------
// <copyright file="DamagingWindow.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
    using System;

    using Exiled.API.Features;
    using Exiled.Events.EventArgs;

    using HarmonyLib;

    using PlayerStatsSystem;

    using UnityEngine;

    using Player = Exiled.Events.Handlers.Player;

    #pragma warning disable SA1313
    /// <summary>
    /// Patch the <see cref="BreakableWindow.Damage(float, DamageHandlerBase, Vector3)"/>.
    /// Adds the <see cref="Player.PlayerDamageWindow"/> event.
    /// </summary>
    [HarmonyPatch(typeof(BreakableWindow), nameof(BreakableWindow.Damage))]
    internal static class DamagingWindow
    {
        private static bool Prefix(BreakableWindow __instance, ref bool __result, float damage, DamageHandlerBase handler, Vector3 pos)
        {
            try
            {
                if (handler is AttackerDamageHandler attackerDamageHandler)
                {
                    PlayerDamageWindowEventArgs ev = new(__instance, attackerDamageHandler.Attacker, handler, damage, __instance.CheckDamagePerms(attackerDamageHandler.Attacker.Role));

                    Player.OnPlayerDamageWindow(ev);

                    if (!ev.IsAllowed)
                    {
                        __result = false;
                        return false;
                    }

                    __instance.LastAttacker = ev.Player.Footprint;
                }

                __instance.ServerDamageWindow(damage);
                __result = true;
                return false;
            }
            catch (Exception ex)
            {
                Log.Error($"{typeof(DamagingWindow).FullName}.{nameof(Prefix)}:\n{ex}");
                return true;
            }
        }
    }
}
