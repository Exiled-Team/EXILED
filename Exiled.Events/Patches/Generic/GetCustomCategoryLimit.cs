// -----------------------------------------------------------------------
// <copyright file="GetCustomCategoryLimit.cs" company="Exiled Team">
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
    /// Patches the <see cref="InventoryLimits.GetCategoryLimit(ItemCategory, ReferenceHub)"/> delegate.
    /// Sync <see cref="API.Features.Player.SetAmmoLimit(API.Enums.AmmoType, ushort)"/>.
    /// Changes <see cref="ushort.MaxValue"/> to <see cref="ushort.MinValue"/>.
    /// </summary>
    [HarmonyPatch(typeof(InventoryLimits), nameof(InventoryLimits.GetCategoryLimit), new Type[] { typeof(ItemCategory), typeof(ReferenceHub), })]
    internal class GetCustomCategoryLimit
    {
        private int Postfix(API.Features.Player player, int value, ItemType ammotype)
        {
            if (player?.categoryLimits is null)
                return value;

            return player.categoryLimits[(int)ammotype] + value - InventoryLimits.GetAmmoLimit(null, ammotype);
        }
    }
}
