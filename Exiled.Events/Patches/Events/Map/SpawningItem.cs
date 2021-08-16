// -----------------------------------------------------------------------
// <copyright file="SpawningItem.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Map
{
#pragma warning disable SA1118
    using System.Collections.Generic;
    using System.Reflection;
    using System.Reflection.Emit;

    using Exiled.Events.EventArgs;

    using HarmonyLib;

    using InventorySystem.Items.Pickups;

    using Mirror;

    using NorthwoodLib.Pools;

    using UnityEngine;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="MapGeneration.Distributors.ItemDistributor.SpawnPickup"/>.
    /// Adds the <see cref="Handlers.Map.SpawningItem"/> and <see cref="Handlers.Map.SpawningItem"/> events.
    /// </summary>
    [HarmonyPatch(typeof(MapGeneration.Distributors.ItemDistributor), nameof(MapGeneration.Distributors.ItemDistributor.SpawnPickup))]
    internal static class SpawningItem
    {
        private static bool Prefix(ItemPickupBase ipb)
        {
            if (ipb != null)
            {
                var ev = new SpawningItemEventArgs(ipb, true);
                Handlers.Map.OnSpawningItem(ev);
                return ev.IsAllowed;
            }

            return false;
        }
    }
}
