// -----------------------------------------------------------------------
// <copyright file="PickingUpItem.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
#pragma warning disable SA1313
    using System;

    using Exiled.API.Features;
    using Exiled.Events.EventArgs;

    using HarmonyLib;

    using InventorySystem.Items.Firearms.Ammo;

    /// <summary>
    /// Patches <see cref="InventorySystem.Searching.ItemSearchCompletor.Complete"/>.
    /// Adds the <see cref="Handlers.Player.PickingUpItem"/> event.
    /// </summary>
    [HarmonyPatch(typeof(InventorySystem.Searching.ItemSearchCompletor), nameof(InventorySystem.Searching.ItemSearchCompletor.Complete))]
    internal static class PickingUpItem
    {
        private static bool Prefix(InventorySystem.Searching.ItemSearchCompletor __instance)
        {
            try
            {
                PickingUpItemEventArgs ev =
                    new PickingUpItemEventArgs(Player.Get(__instance.Hub), __instance.TargetPickup);

                Handlers.Player.OnPickingUpItem(ev);
                __instance.TargetPickup.Info.Serial = ev.Pickup.Serial;

                // Allow future pick up of this item
                if (!ev.IsAllowed)
                {
                    __instance.TargetPickup.Info.InUse = false;
                    __instance.TargetPickup.NetworkInfo = __instance.TargetPickup.Info;
                }

                return ev.IsAllowed;
            }
            catch (Exception exception)
            {
                Log.Error($"{typeof(PickingUpItem).FullName}\n{exception}");

                return true;
            }
        }
    }
}
