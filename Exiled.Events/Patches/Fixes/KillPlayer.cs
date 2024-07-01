// -----------------------------------------------------------------------
// <copyright file="KillPlayer.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Fixes
{
#pragma warning disable SA1313 // Parameter names should begin with lower-case letter

    using API.Features;
    using API.Features.DamageHandlers;

    using EventArgs.Player;

    using HarmonyLib;

    using Mirror;

    using PlayerStatsSystem;

    using DamageHandlerBase = PlayerStatsSystem.DamageHandlerBase;

    /// <summary>
    /// Prefix of KillPlayer action.
    /// </summary>
    [HarmonyPatch(typeof(PlayerStats), nameof(PlayerStats.KillPlayer))]
    internal class KillPlayer
    {
        private static void Prefix(PlayerStats __instance, ref DamageHandlerBase handler)
        {
            if (!DamageHandlers.IdsByTypeHash.ContainsKey(handler.GetType().FullName.GetStableHashCode()))
            {
                if (handler is GenericDamageHandler exiledHandler)
                {
                    handler = exiledHandler.Base;
                }
                else
                {
                    KillingPlayerEventArgs ev = new(Player.Get(__instance._hub), ref handler);
                    Handlers.Player.OnKillPlayer(ev);

                    handler = ev.Handler;
                }
            }
        }
    }
}