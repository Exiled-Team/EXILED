// -----------------------------------------------------------------------
// <copyright file="SpawningItemEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Map
{
    using Exiled.API.Features;
    using Exiled.API.Features.Doors;
    using Exiled.API.Features.Pickups;
    using Exiled.Events.EventArgs.Interfaces;
    using Interactables.Interobjects.DoorUtils;
    using InventorySystem.Items.Pickups;

    /// <summary>
    ///     Contains all information before the server spawns an item.
    /// </summary>
    public class SpawningItemEventArgs : IDeniableEvent, IPickupEvent
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="SpawningItemEventArgs" /> class.
        /// </summary>
        /// <param name="pickupBase">
        ///     <inheritdoc cref="Pickup" />
        /// </param>
        /// <param name="shouldInitiallySpawn">
        ///     <inheritdoc cref="ShouldInitiallySpawn" />
        /// </param>
        /// <param name="door">
        ///     <inheritdoc cref="Door" />
        /// </param>
        public SpawningItemEventArgs(ItemPickupBase pickupBase, bool shouldInitiallySpawn, DoorVariant door)
        {
            Pickup = Pickup.Get(pickupBase);
            TriggerDoor = Door.Get(door);
            ShouldInitiallySpawn = shouldInitiallySpawn;
        }

        /// <summary>
        ///     Gets a value indicating the pickup being spawned.
        /// </summary>
        public Pickup Pickup { get; }

        /// <summary>
        ///     Gets or sets a value indicating whether or not the item will be initially spawned.
        /// </summary>
        public bool ShouldInitiallySpawn { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating the trigger door for pickup.
        /// </summary>
        /// <remarks>
        ///     Works only when <see cref="ShouldInitiallySpawn"/> is false.
        ///     null when <see cref="ShouldInitiallySpawn"/> is true.
        ///     Can be not fully initialized.
        /// </remarks>
        public Door TriggerDoor { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether or not the item can be spawned.
        /// </summary>
        public bool IsAllowed { get; set; } = true;
    }
}