// -----------------------------------------------------------------------
// <copyright file="ExplodingFragGrenade.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Map
{
#pragma warning disable SA1313
    using System;
    using System.Collections.Generic;
    using Exiled.Events.EventArgs;
    using GameCore;
    using Grenades;
    using HarmonyLib;
    using UnityEngine;

    /// <summary>
    /// Patches <see cref="FragGrenade.ServersideExplosion()"/>.
    /// Adds the <see cref="Handlers.Map.OnExplodingGrenade"/> event.
    /// </summary>
    [HarmonyPatch(typeof(FragGrenade), nameof(FragGrenade.ServersideExplosion))]
    internal class ExplodingFragGrenade
    {
        private static bool Prefix(FragGrenade __instance)
        {
            try
            {
                Vector3 position = __instance.transform.position;
                List<API.Features.Player> players = new List<API.Features.Player>();
                foreach (GameObject gameObject in PlayerManager.players)
                {
                    if (ServerConsole.FriendlyFire || !(gameObject != __instance.thrower.gameObject) || gameObject
                        .GetComponent<WeaponManager>().GetShootPermission(__instance.throwerTeam, false))
                    {
                        PlayerStats component2 = gameObject.GetComponent<PlayerStats>();
                        if (component2 == null || !component2.ccm.InWorld)
                            continue;

                        float num2 =
                            __instance.damageOverDistance.Evaluate(Vector3.Distance(
                                position,
                                component2.transform.position)) * (component2.ccm.IsHuman()
                                ? ConfigFile.ServerConfig.GetFloat("human_grenade_multiplier", 0.7f)
                                : ConfigFile.ServerConfig.GetFloat("scp_grenade_multiplier", 1f));
                        if (num2 > __instance.absoluteDamageFalloff)
                            players.Add(Exiled.API.Features.Player.Get(gameObject));
                    }
                }

                ExplodingGrenadeEventArgs ev = new ExplodingGrenadeEventArgs(players, true, __instance.gameObject);

                Handlers.Map.OnExplodingGrenade(ev);

                return ev.IsAllowed;
            }
            catch (Exception e)
            {
                Exiled.API.Features.Log.Error($"Exiled.Events.Patches.Events.Map.ExplodingFragGrenade: {e}\n{e.StackTrace}");

                return true;
            }
        }
    }
}
