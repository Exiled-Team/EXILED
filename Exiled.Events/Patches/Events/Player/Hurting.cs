// -----------------------------------------------------------------------
// <copyright file="Hurting.cs" company="Exiled Team">
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

    using UnityEngine;

    /// <summary>
    /// Patches <see cref="PlayerStats.HurtPlayer(PlayerStats.HitInfo, UnityEngine.GameObject)"/>.
    /// Adds the <see cref="Player.Hurting"/> event.
    /// </summary>
    [HarmonyPatch(typeof(PlayerStats), nameof(PlayerStats.HurtPlayer))]
    internal class Hurting
    {
        private static bool Prefix(PlayerStats __instance, ref PlayerStats.HitInfo info, GameObject go)
        {
            try
            {
                if (go == null)
                    return true;

                API.Features.Player attacker = API.Features.Player.Get(__instance.gameObject);
                API.Features.Player target = API.Features.Player.Get(go);

                if (attacker == null || target == null || attacker.IsHost || target.IsHost)
                    return true;

                var ev = new HurtingEventArgs(API.Features.Player.Get(__instance.gameObject), API.Features.Player.Get(go), info);

                if (ev.Target.IsHost)
                    return true;

                Player.OnHurting(ev);

                info = ev.HitInformations;

                if (!ev.IsAllowed)
                    return false;

                if (ev.Amount >= ev.Target.Health + ev.Target.AdrenalineHealth)
                {
                    var dyingEv = new DyingEventArgs(ev.Attacker, ev.Target, ev.HitInformations);

                    if (!ev.IsAllowed)
                        return false;
                }

                return true;
            }
            catch (Exception e)
            {
                Exiled.API.Features.Log.Error($"Exiled.Events.Patches.Events.Player.Hurting: {e}\n{e.StackTrace}");
                return true;
            }
        }
    }
}
