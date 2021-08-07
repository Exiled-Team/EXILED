// -----------------------------------------------------------------------
// <copyright file="ItemManagementPatch.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Generic
{
    using System.Collections.Generic;

    using Exiled.API.Features;

    using HarmonyLib;

    using InventorySystem;
    using InventorySystem.Items;

    /// <summary>
    /// Handles syncing between <see cref="InventorySystem.Inventory.UserInventory"/> and <see cref="Player.Items"/>.
    /// </summary>
    [HarmonyPatch(typeof(Dictionary<ushort, ItemBase>), nameof(Inventory.UserInventory.Items.Add))]
    internal static class ItemManagementPatch
    {
        private static bool Prefix(ushort key, ItemBase value)
        {
            Log.Error($"idk if this will work but here goes? {key} {value.ItemTypeId}");
            return true;
        }
    }
}
