// -----------------------------------------------------------------------
// <copyright file="GetCustomAmmoLimit.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Fixes
{
    using System;

    using Exiled.API.Extensions;
    using Exiled.API.Features;
    using HarmonyLib;
    using InventorySystem.Configs;
    using UnityEngine;

    /// <summary>
    /// Patches the <see cref="InventoryLimits.GetAmmoLimit(ItemType, ReferenceHub)"/> delegate.
    /// Sync <see cref="Player.SetAmmoLimit(API.Enums.AmmoType, ushort)"/>.
    /// Changes <see cref="ushort.MaxValue"/> to <see cref="ushort.MinValue"/>.
    /// </summary>
    [HarmonyPatch(typeof(InventoryLimits), nameof(InventoryLimits.GetAmmoLimit), new Type[] { typeof(ItemType), typeof(ReferenceHub) })]
    internal static class GetCustomAmmoLimit
    {
#pragma warning disable SA1313
        private static void Postfix(ItemType ammoType, ReferenceHub player, ref ushort __result)
        {
            if (!Player.TryGet(player, out Player ply) || !ply.CustomAmmoLimits.TryGetValue(ammoType.GetAmmoType(), out ushort limit))
                return;

            __result = (ushort)Mathf.Clamp(limit + __result - InventoryLimits.GetAmmoLimit(null, ammoType), ushort.MinValue, ushort.MaxValue);
        }
    }
}
