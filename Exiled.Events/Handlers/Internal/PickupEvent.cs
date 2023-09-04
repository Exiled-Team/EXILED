// -----------------------------------------------------------------------
// <copyright file="PickupEvent.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Handlers.Internal
{
    using Exiled.API.Features.Pickups;
    using InventorySystem.Items.Pickups;

    /// <summary>
    /// Handles adding and removing from <see cref="Pickup.BaseToPickup"/>.
    /// </summary>
    internal static class PickupEvent
    {
        /// <summary>
        /// Called after a pickup is spawned. Hooked to <see cref="ItemPickupBase.OnPickupAdded"/>.
        /// </summary>
        /// <param name="itemPickupBase">The spawned Pickup.</param>
        public static void OnSpawnedPickup(ItemPickupBase itemPickupBase)
        {
            Handlers.Map.OnPickupAdded(new(itemPickupBase));
        }

        /// <summary>
        /// Called before a pickup is destroyed. Hooked to <see cref="ItemPickupBase.OnPickupDestroyed"/>.
        /// </summary>
        /// <param name="itemPickupBase">The destroyed Pickup.</param>
        public static void OnRemovedPickup(ItemPickupBase itemPickupBase)
        {
            Handlers.Map.OnPickupDestroyed(new(itemPickupBase));
            Pickup.BaseToPickup.Remove(itemPickupBase);
        }
    }
}
