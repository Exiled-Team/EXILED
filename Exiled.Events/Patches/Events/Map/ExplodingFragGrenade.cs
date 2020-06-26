// -----------------------------------------------------------------------
// <copyright file="ExplodingFragGrenade.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Map
{
#pragma warning disable SA1313
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
    public class ExplodingFragGrenade
    {
        /// <summary>
        /// Prefix of <see cref="FragGrenade.ServersideExplosion()"/>.
        /// </summary>
        /// <param name="__instance">The <see cref="FragGrenade"/> instance.</param>
        /// <returns>Returns a value indicating whether the original method has to be executed or not.</returns>
        public static bool Prefix(FragGrenade __instance)
        {
            Vector3 position = __instance.transform.position;
            List<Exiled.API.Features.Player> players = new List<Exiled.API.Features.Player>();
            foreach (GameObject gameObject in PlayerManager.players)
            {
                if (ServerConsole.FriendlyFire || !(gameObject != __instance.thrower.gameObject) || gameObject.GetComponent<WeaponManager>().GetShootPermission(__instance.throwerTeam, false))
                {
                    PlayerStats component2 = gameObject.GetComponent<PlayerStats>();
                    if (component2 == null || !component2.ccm.InWorld)
                        continue;

                    float num2 = __instance.damageOverDistance.Evaluate(Vector3.Distance(position, component2.transform.position)) * (component2.ccm.IsHuman() ? ConfigFile.ServerConfig.GetFloat("human_grenade_multiplier", 0.7f) : ConfigFile.ServerConfig.GetFloat("scp_grenade_multiplier", 1f));
                    if (num2 > __instance.absoluteDamageFalloff)
                        players.Add(Exiled.API.Features.Player.Get(gameObject));
                }
            }

            ExplodingGrenadeEventArgs ev = new ExplodingGrenadeEventArgs(players, true, __instance.gameObject);
            Exiled.Events.Handlers.Map.OnExplodingGrenade(ev);
            return ev.IsAllowed;
        }
    }
}
