// -----------------------------------------------------------------------
// <copyright file="GetCustomAmmoLimit.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Fixes
{
    using System;

    using HarmonyLib;
    using InventorySystem.Configs;

    /// <summary>
    /// Patches the <see cref="InventoryLimits.GetAmmoLimit(ItemType, ReferenceHub)"/> delegate.
    /// Sync <see cref="API.Features.Player.SetAmmoLimit(API.Enums.AmmoType, ushort)"/>.
    /// Changes <see cref="ushort.MaxValue"/> to <see cref="ushort.MinValue"/>.
    /// </summary>
    [HarmonyPatch(typeof(InventoryLimits), nameof(InventoryLimits.GetAmmoLimit), new Type[] { typeof(ItemType), typeof(ReferenceHub), })]
    internal class GetCustomAmmoLimit
    {
        private int Postfix(API.Features.Player player, int value, ItemType ammotype)
        {
            if (player?.AmmoLimits is null)
                return value;

            return player.AmmoLimits.Find(x => x.AmmoType == ammotype).Limit + value - InventoryLimits.GetAmmoLimit(null, ammotype);
        }
    }
}
