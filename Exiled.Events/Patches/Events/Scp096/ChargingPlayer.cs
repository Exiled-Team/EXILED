// -----------------------------------------------------------------------
// <copyright file="ChargingPlayer.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Scp096
{
#pragma warning disable SA1129
#pragma warning disable SA1313
    using Exiled.Events.EventArgs;

    using HarmonyLib;

    using Mirror;

    using PlayableScps;
    using PlayableScps.Messages;

    using UnityEngine;

    /// <summary>
    /// Patches <see cref="Scp096.ChargePlayer"/>.
    /// Adds the <see cref="Handlers.Scp096.ChargingPlayer"/> event.
    /// </summary>
    [HarmonyPatch(typeof(Scp096), nameof(Scp096.ChargePlayer))]
    internal static class ChargingPlayer
    {
        private static readonly int ChargingMask = LayerMask.GetMask("Default", "Door", "Glass");

        private static bool Prefix(Scp096 __instance, ReferenceHub player)
        {
            if (player.characterClassManager.IsAnyScp())
                return false;

            if (Physics.Linecast(__instance.Hub.transform.position, player.transform.position, ChargingMask))
                return false;

            bool isTarget = __instance._targets.Contains(player);

            var ev = new ChargingPlayerEventArgs(__instance, API.Features.Player.Get(__instance.Hub), API.Features.Player.Get(player), isTarget, isTarget ? 9696f : 35f, isTarget);

            Handlers.Scp096.OnChargingPlayer(ev);

            if (ev.IsAllowed)
            {
                if (__instance.Hub.playerStats.HurtPlayer(new PlayerStats.HitInfo(ev.Damage, player.LoggedNameFromRefHub(), DamageTypes.Scp096, __instance.Hub.queryProcessor.PlayerId), player.gameObject))
                {
                    __instance._targets.Remove(player);
                    NetworkServer.SendToClientOfPlayer(__instance.Hub.characterClassManager.netIdentity, new Scp096HitmarkerMessage(1.35f));
                    NetworkServer.SendToAll(new Scp096OnKillMessage());
                }

                if (ev.EndCharge)
                {
                    __instance.EndChargeNextFrame();
                }
            }

            return false;
        }
    }
}
