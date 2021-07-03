// -----------------------------------------------------------------------
// <copyright file="ChangingItem.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
#pragma warning disable SA1313
    using System;
    using System.Diagnostics;

    using Exiled.Events.EventArgs;
    using Exiled.Events.Handlers;

    using HarmonyLib;

    /// <summary>
    /// Patches <see cref="Inventory.CallCmdSetUnic(int)"/>.
    /// Adds the <see cref="Player.ChangingItem"/> event.
    /// </summary>
    [HarmonyPatch(typeof(Inventory), nameof(Inventory.CallCmdSetUnic))]
    internal static class ChangingItem
    {
        private static void Prefix(Inventory __instance, int i)
        {
            try
            {
                if (__instance.itemUniq == i)
                    return;

                int oldItemIndex = __instance.GetItemIndex();

                if (oldItemIndex == -1 && i == -1)
                    return;

                Inventory.SyncItemInfo oldItem = oldItemIndex == -1
                    ? new Inventory.SyncItemInfo() { id = ItemType.None }
                    : __instance.GetItemInHand();
                Inventory.SyncItemInfo newItem = new Inventory.SyncItemInfo() { id = ItemType.None };

                foreach (Inventory.SyncItemInfo item in __instance.items)
                {
                    if (item.uniq == i)
                        newItem = item;
                }

                var ev = new ChangingItemEventArgs(API.Features.Player.Get(__instance.gameObject), oldItem, newItem);

                Player.OnChangingItem(ev);

                oldItemIndex = __instance.GetItemIndex();

                if (oldItemIndex != -1)
                    __instance.items[oldItemIndex] = ev.OldItem;
            }
            catch (Exception e)
            {
                Exiled.API.Features.Log.Error($"Exiled.Events.Patches.Events.Player.ChangingItem:\n{e.ToStringDemystified()}");
            }
        }
    }
}
