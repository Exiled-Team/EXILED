// -----------------------------------------------------------------------
// <copyright file="KillPlayer.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Server
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    using HarmonyLib;

    using Mirror;

    using PlayerStatsSystem;
#pragma warning disable SA1313 // Parameter names should begin with lower-case letter

    /// <summary>
    /// Prefix of KillPlayer action.
    /// </summary>
    [HarmonyPatch(typeof(PlayerStats), nameof(PlayerStats.KillPlayer))]
    internal class KillPlayer
    {
        /// <summary>
        /// Handles processing of kill player.
        /// </summary>
        /// <param name="__instance"> PlayerStat instance. </param>
        /// <param name="handler"> DamageHandlerBase instance. </param>
        /// <returns> True if original function should run. </returns>
        [HarmonyPrefix]
        public static bool KillPlayerPrefix(PlayerStats __instance, ref DamageHandlerBase handler)
        {
            if(!DamageHandlers.IdsByTypeHash.ContainsKey(handler.GetType().FullName.GetStableHashCode()))
            {
                try
                {
                    handler = ((API.Features.GenericDamageHandler)handler).Base;
                }
                catch
                {
                }
            }

            return true;
        }
    }
#pragma warning restore SA1313 // Parameter names should begin with lower-case letter
}
