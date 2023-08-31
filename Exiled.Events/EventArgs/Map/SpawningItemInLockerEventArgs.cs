// -----------------------------------------------------------------------
// <copyright file="SpawningItemInLockerEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Map
{
    using Exiled.API.Features.Lockers;
    using Exiled.API.Features.Pickups;
    using Exiled.Events.EventArgs.Interfaces;
    using InventorySystem.Items.Pickups;
    using MapGeneration.Distributors;

    /// <summary>
    /// Contains all information before an item spawned in a locker.
    /// </summary>
    public class SpawningItemInLockerEventArgs : IPickupEvent, IDeniableEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SpawningItemInLockerEventArgs"/> class.
        /// </summary>
        /// <param name="chamber"><inheritdoc cref="Chamber"/></param>
        /// <param name="pickupBase"><inheritdoc cref="Pickup"/></param>
        /// <param name="shouldInitiallySpawn"><inheritdoc cref="ShouldInitiallySpawn"/></param>
        /// <param name="isAllowed"><inheritdoc cref="IsAllowed"/></param>
        public SpawningItemInLockerEventArgs(LockerChamber chamber, ItemPickupBase pickupBase, bool shouldInitiallySpawn, bool isAllowed = true)
        {
            Chamber = Chamber.Get(chamber);
            Pickup = Pickup.Get(pickupBase);
            ShouldInitiallySpawn = shouldInitiallySpawn;
            IsAllowed = isAllowed;
        }

        /// <summary>
        /// Gets the <see cref="API.Features.Lockers.Chamber"/> where is spawning <see cref="Pickup"/>.
        /// </summary>
        public Chamber Chamber { get; }

        /// <summary>
        /// Gets or sets a <see cref="API.Features.Pickups.Pickup"/> which is spawning.
        /// </summary>
        public Pickup Pickup { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether or not item should be spawned now.
        /// </summary>
        public bool ShouldInitiallySpawn { get; set; }

        /// <inheritdoc/>
        public bool IsAllowed { get; set; }
    }
}