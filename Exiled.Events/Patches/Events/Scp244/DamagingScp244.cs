// -----------------------------------------------------------------------
// <copyright file="DamagingScp244.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Scp244
{
#pragma warning disable SA1313
    using System;

    using Exiled.API.Features;
    using Exiled.Events.EventArgs;

    using HarmonyLib;

    using InventorySystem;
    using InventorySystem.Items.Usables.Scp244;
    using InventorySystem.Searching;

    using PlayerStatsSystem;

    using UnityEngine;

    /// <summary>
    /// Patches <see cref="Scp244DeployablePickup.Damage"/>.
    /// Adds the <see cref="Handlers.Scp244.PickingUpScp244"/> event.
    /// </summary>
    [HarmonyPatch(typeof(Scp244DeployablePickup), nameof(Scp244DeployablePickup.Damage))]
    internal static class DamagingScp244
    {
        private static bool Prefix(Scp244DeployablePickup __instance, bool __result,float damage, DamageHandlerBase handler, Vector3 exactHitPos)
        {
            try
            {
                if (__instance._health <= 0f || __instance.ModelDestroyed
                    || __instance.State == Scp244State.Destroyed)
                {
                    __result = false;
                    return false;
                }

                DamagingScp244EventArgs ev = new(__instance, damage, handler);
                Handlers.Scp244.OnDamagingScp244(ev);

                if (!ev.IsAllowed)
                {
                    __result = false;
                    return false;
                }

                __instance._health -= damage;
                if (__instance._health <= 0f)
                {
                    __instance.State = Scp244State.Destroyed;
                }

                __result = true;
                return false;
            }
            catch (Exception ex)
            {
                Log.Error($"{typeof(DamagingScp244).FullName}.{nameof(Prefix)}:\n{ex}");
                return true;
            }
        }
    }
}
