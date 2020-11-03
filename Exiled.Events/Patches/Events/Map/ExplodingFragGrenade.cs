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

    using Exiled.API.Features;
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
    internal static class ExplodingFragGrenade
    {
        private static bool Prefix(FragGrenade __instance)
        {
            try
            {
                Player thrower = Player.Get(__instance.thrower.gameObject);

                Dictionary<Player, float> players = new Dictionary<Player, float>();

                Vector3 position = ((EffectGrenade)__instance).transform.position;

                float humanMultiplier = ConfigFile.ServerConfig.GetFloat("human_grenade_multiplier", 0.7f);
                float scpMultiplier = ConfigFile.ServerConfig.GetFloat("scp_grenade_multiplier", 1f);

                foreach (GameObject obj2 in PlayerManager.players)
                {
                    if (ServerConsole.FriendlyFire || obj2 == __instance.thrower.gameObject ||
                        obj2.GetComponent<WeaponManager>().GetShootPermission(__instance.throwerTeam, false))
                    {
                        PlayerStats component = obj2.GetComponent<PlayerStats>();
                        if ((component != null) && component.ccm.InWorld)
                        {
                            players.Add(Player.Get(obj2), (float)(__instance.damageOverDistance.Evaluate(Vector3.Distance(position, component.transform.position)) * (component.ccm.IsHuman() ? humanMultiplier : scpMultiplier)));
                        }
                    }
                }

                var ev = new ExplodingGrenadeEventArgs(thrower, players, true, __instance.gameObject);

                Handlers.Map.OnExplodingGrenade(ev);

                return ev.IsAllowed;
            }
            catch (Exception exception)
            {
                API.Features.Log.Error($"Exiled.Events.Patches.Events.Map.ExplodingFragGrenade: {exception}\n{exception.StackTrace}");

                return true;
            }
        }
    }
}
