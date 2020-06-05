// -----------------------------------------------------------------------
// <copyright file="TriggeringTesla.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Patches
{
    #pragma warning disable SA1313
    using System.Collections.Generic;
    using Exiled.Events.Handlers;
    using Exiled.Events.Handlers.EventArgs;
    using HarmonyLib;
    using UnityEngine;

    /// <summary>
    /// Patches <see cref="TeslaGate.PlayersInRange(bool)"/>.
    /// Adds the <see cref="Player.TriggeringTesla"/> event.
    /// </summary>
    [HarmonyPatch(typeof(TeslaGate), nameof(TeslaGate.PlayersInRange))]
    public class TriggeringTesla
    {
        /// <summary>
        /// Postfix of <see cref="TeslaGate.PlayersInRange(bool)"/>.
        /// </summary>
        /// <param name="__instance">The <see cref="TeslaGate"/> instance.</param>
        /// <param name="hurtRange"><inheritdoc cref="TriggeringTeslaEventArgs.IsInHurtingRange"/></param>
        /// <param name="__result">The list of players in the tesla hurting range.</param>
        public static void Postfix(TeslaGate __instance, bool hurtRange, ref List<PlayerStats> __result)
        {
            __result = new List<PlayerStats>();

            foreach (GameObject player in PlayerManager.players)
            {
                if (Vector3.Distance(__instance.transform.position, player.transform.position) < __instance.sizeOfTrigger &&
                    player.GetComponent<CharacterClassManager>().CurClass != RoleType.Spectator)
                {
                    var ev = new TriggeringTeslaEventArgs(API.Features.Player.Get(player), hurtRange);

                    Player.OnTriggeringTesla(ev);

                    if (ev.IsTriggerable)
                        __result.Add(player.GetComponent<PlayerStats>());
                }
            }
        }
    }
}