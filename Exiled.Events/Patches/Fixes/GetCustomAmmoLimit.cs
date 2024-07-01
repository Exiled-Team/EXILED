// -----------------------------------------------------------------------
// <copyright file="GetCustomAmmoLimit.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Fixes
{
    using System;

    using Exiled.API.Features;
    using HarmonyLib;
    using InventorySystem.Configs;

    /// <summary>
    /// Patches the <see cref="InventoryLimits.GetAmmoLimit(ItemType, ReferenceHub)"/> delegate.
    /// Sync <see cref="API.Features.Player.SetAmmoLimit(API.Enums.AmmoType, ushort)"/>.
    /// Changes <see cref="ushort.MaxValue"/> to <see cref="ushort.MinValue"/>.
    /// </summary>
    [HarmonyPatch(typeof(InventoryLimits), nameof(InventoryLimits.GetAmmoLimit), new Type[] { typeof(ItemType), typeof(ReferenceHub) })]
    internal static class GetCustomAmmoLimit
    {
#pragma warning disable SA1313
        private static void Postfix(ItemType ammoType, ReferenceHub player, ref ushort __result)
        {
            if (!Player.TryGet(player, out Player ply) || ply.AmmoLimits is null)
                return;

            int index = ply.AmmoLimits.FindIndex(x => x.AmmoType == ammoType);

            if (ply.AmmoLimits.TryGet(index, out ServerConfigSynchronizer.AmmoLimit limit))
                __result = limit.Limit;
        }
    }
}
