// -----------------------------------------------------------------------
// <copyright file="EnteringFemurBreaker.cs" company="Exiled Team">
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
    using Mirror;
    using UnityEngine;

    /// <summary>
    /// Patches <see cref="CharacterClassManager.AllowContain"/>.
    /// Adds the <see cref="Player.EnteringFemurBreaker"/> event.
    /// </summary>
    [HarmonyPatch(typeof(CharacterClassManager), nameof(CharacterClassManager.AllowContain))]
    public class EnteringFemurBreaker
    {
        /// <summary>
        /// Prefix of <see cref="CharacterClassManager.AllowContain"/>.
        /// </summary>
        /// <param name="__instance">The <see cref="CharacterClassManager"/> instance.</param>
        /// <returns>Returns a value indicating whether the original method has to be executed or not.</returns>
        public static bool Prefix(CharacterClassManager __instance)
        {
            if (!NetworkServer.active || !NonFacilityCompatibility.currentSceneSettings.enableStandardGamplayItems)
                return false;

            foreach (GameObject player in PlayerManager.players)
            {
                if (Vector3.Distance(player.transform.position, __instance._lureSpj.transform.position) < 1.97000002861023)
                {
                    CharacterClassManager component1 = player.GetComponent<CharacterClassManager>();
                    PlayerStats component2 = player.GetComponent<PlayerStats>();

                    if (component1.Classes.SafeGet(component1.CurClass).team != Team.SCP && component1.CurClass != RoleType.Spectator && !component1.GodMode)
                    {
                        var ev = new EnteringFemurBreakerEventArgs(API.Features.Player.Get(component2.gameObject));

                        Player.OnEnteringFemurBreaker(ev);

                        if (ev.IsAllowed)
                        {
                            component2.HurtPlayer(new PlayerStats.HitInfo(10000f, "WORLD", DamageTypes.Lure, 0), player);
                            __instance._lureSpj.SetState(true);
                        }
                    }
                }
            }

            return false;
        }
    }
}
