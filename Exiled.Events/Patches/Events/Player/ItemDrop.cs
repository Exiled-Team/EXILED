// -----------------------------------------------------------------------
// <copyright file="ItemDrop.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
#pragma warning disable SA1313
    using Exiled.Events.EventArgs;
    using Exiled.Events.Handlers;
    using HarmonyLib;

    /// <summary>
    /// Patches <see cref="Inventory.CallCmdDropItem(int)"/>.
    /// Adds the <see cref="Player.ItemDropped"/> and <see cref="Player.DroppingItem"/> events.
    /// </summary>
    [HarmonyPatch(typeof(Inventory), nameof(Inventory.CallCmdDropItem))]
    internal class ItemDrop
    {
        private static bool Prefix(Inventory __instance, int itemInventoryIndex)
        {
            if (!__instance._iawRateLimit.CanExecute(true) || itemInventoryIndex < 0 || itemInventoryIndex >= __instance.items.Count)
                return false;

            Inventory.SyncItemInfo syncItemInfo = __instance.items[itemInventoryIndex];

            if (__instance.items[itemInventoryIndex].id != syncItemInfo.id)
                return false;

            var droppingItemEventArgs = new DroppingItemEventArgs(API.Features.Player.Get(__instance.gameObject), syncItemInfo);

            Player.OnDroppingItem(droppingItemEventArgs);

            if (!droppingItemEventArgs.IsAllowed)
                return false;

            Pickup droppedPickup = __instance.SetPickup(
                syncItemInfo.id,
                syncItemInfo.durability,
                __instance.transform.position,
                __instance.camera.transform.rotation,
                syncItemInfo.modSight,
                syncItemInfo.modBarrel,
                syncItemInfo.modOther);

            __instance.items.RemoveAt(itemInventoryIndex);

            var itemDroppedEventArgs = new ItemDroppedEventArgs(API.Features.Player.Get(__instance.gameObject), droppedPickup);

            Player.OnItemDropped(itemDroppedEventArgs);

            return false;
        }
    }
}
