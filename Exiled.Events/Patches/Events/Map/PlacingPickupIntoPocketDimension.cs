// -----------------------------------------------------------------------
// <copyright file="PlacingPickupIntoPocketDimension.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Map
{
    using System.Collections.Generic;

    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs.Map;
    using Handlers;
    using HarmonyLib;
    using InventorySystem.Items.Pickups;
    using Mirror;
    using PlayerRoles.PlayableScps.Scp106;

    using static PlayerRoles.PlayableScps.Scp106.Scp106PocketItemManager;

    /// <summary>
    ///     Patches <see cref="Scp106PocketItemManager"/>
    ///     Adds the <see cref="Map.PlacingPickupIntoPocketDimension" /> event.
    /// </summary>
    [EventPatch(typeof(Map), nameof(Map.PlacingPickupIntoPocketDimension))]
    [HarmonyPatch(typeof(Scp106PocketItemManager), nameof(Scp106PocketItemManager.OnAdded))]
    internal static class PlacingPickupIntoPocketDimension
    {
        private static void Postfix(ItemPickupBase ipb)
        {
            if (TrackedItems.TryGetValue(ipb, out PocketItem pocketItem))
            {
                PlacingPickupIntoPocketDimensionEventArgs ev = new(ipb, pocketItem, true);
                Map.OnPlacingPickupIntoPocketDimension(ev);

                if (!ev.IsAllowed)
                {
                    TrackedItems.Remove(ipb);
                }
            }
        }
    }
}
