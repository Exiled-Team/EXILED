// -----------------------------------------------------------------------
// <copyright file="ChangingItem.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

using Sexiled.API.Features;
using Sexiled.Events.EventArgs;

namespace Sexiled.Events.Patches.Events.Player
{
#pragma warning disable SA1313
    using System;

    using Sexiled.Events.EventArgs;
    using Sexiled.Events.Handlers;

    using HarmonyLib;

    /// <summary>
    /// Patches <see cref="Inventory.CallCmdSetUnic(int)"/>.
    /// Adds the <see cref="Handlers.Player.ChangingItem"/> event.
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

                Handlers.Player.OnChangingItem(ev);

                oldItemIndex = __instance.GetItemIndex();

                if (oldItemIndex != -1)
                    __instance.items[oldItemIndex] = ev.OldItem;
            }
            catch (Exception e)
            {
                Log.Error($"Sexiled.Events.Patches.Events.Player.ChangingItem: {e}\n{e.StackTrace}");
            }
        }
    }
}
