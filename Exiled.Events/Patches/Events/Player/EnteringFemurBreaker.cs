// -----------------------------------------------------------------------
// <copyright file="EnteringFemurBreaker.cs" company="Exiled Team">
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

    using Mirror;

    using UnityEngine;

    /// <summary>
    /// Patches <see cref="CharacterClassManager.AllowContain"/>.
    /// Adds the <see cref="Player.EnteringFemurBreaker"/> event.
    /// </summary>
    [HarmonyPatch(typeof(CharacterClassManager), nameof(CharacterClassManager.AllowContain))]
    internal static class EnteringFemurBreaker
    {
        private static bool Prefix(CharacterClassManager __instance)
        {
            try
            {
                if (!NetworkServer.active || !NonFacilityCompatibility.currentSceneSettings.enableStandardGamplayItems)
                    return false;

                foreach (GameObject player in PlayerManager.players)
                {
                    if (Vector3.Distance(player.transform.position, __instance._lureSpj.transform.position) <
                        1.97000002861023)
                    {
                        CharacterClassManager component1 = player.GetComponent<CharacterClassManager>();
                        PlayerStats component2 = player.GetComponent<PlayerStats>();

                        if (component1.Classes.SafeGet(component1.CurClass).team != Team.SCP &&
                            component1.CurClass != RoleType.Spectator && !component1.GodMode)
                        {
                            EnteringFemurBreakerEventArgs ev = new EnteringFemurBreakerEventArgs(API.Features.Player.Get(component2.gameObject));

                            Player.OnEnteringFemurBreaker(ev);

                            if (ev.IsAllowed)
                            {
                                component2.HurtPlayer(new PlayerStats.HitInfo(10000f, "WORLD", DamageTypes.Lure, 0, true), player, true);
                                __instance._lureSpj.SetState(false, true);
                            }
                        }
                    }
                }

                return false;
            }
            catch (Exception e)
            {
                Exiled.API.Features.Log.Error($"Exiled.Events.Patches.Events.Player.EnteringFemurBreaker: {e}\n{e.StackTrace}");

                return true;
            }
        }
    }
}
