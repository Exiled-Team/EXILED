// -----------------------------------------------------------------------
// <copyright file="ExplodingFlashGrenade.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Map
{
#pragma warning disable SA1313
    using System.Collections.Generic;

    using Exiled.API.Features;
    using Exiled.Events.EventArgs;

    using HarmonyLib;

    using InventorySystem.Items.ThrowableProjectiles;

    using Mirror;

    using NorthwoodLib.Pools;

    using UnityEngine;

    /// <summary>
    /// Patches <see cref="FlashbangGrenade.PlayExplosionEffects()"/>.
    /// Adds the <see cref="Handlers.Map.OnExplodingGrenade"/> event.
    /// </summary>
    [HarmonyPatch(typeof(FlashbangGrenade), nameof(FlashbangGrenade.PlayExplosionEffects))]
    internal static class ExplodingFlashGrenade
    {
        private static bool Prefix(FlashbangGrenade __instance)
        {
            if (!NetworkServer.active || __instance.PreviousOwner.Hub == null)
                return false;
            double time = __instance._blindingOverDistance.keys[__instance._blindingOverDistance.length - 1].time;
            float num = (float)(time * time);
            List<Player> players = ListPool<Player>.Shared.Rent();
            foreach (KeyValuePair<GameObject, ReferenceHub> allHub in ReferenceHub.GetAllHubs())
            {
                if (!(allHub.Value == null) && ((__instance.transform.position - allHub.Value.transform.position).sqrMagnitude <= (double)num && !(allHub.Value == __instance.PreviousOwner.Hub) && HitboxIdentity.CheckFriendlyFire(__instance.PreviousOwner.Role, allHub.Value.characterClassManager.CurClass)))
                    players.Add(Player.Get(allHub.Value));
            }

            var ev = new ExplodingGrenadeEventArgs(Player.Get(__instance.PreviousOwner.Hub), __instance, players);
            Handlers.Map.OnExplodingGrenade(ev);
            if (!ev.IsAllowed)
                return false;

            foreach (Player player in ev.TargetsToAffect)
                __instance.ProcessPlayer(player.ReferenceHub);

            return false;
        }
    }
}
