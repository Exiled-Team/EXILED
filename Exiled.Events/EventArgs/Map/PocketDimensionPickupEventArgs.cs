// -----------------------------------------------------------------------
// <copyright file="PocketDimensionPickupEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Map
{
    using Exiled.API.Features.Pickups;
    using Exiled.Events.EventArgs.Interfaces;
    using InventorySystem.Items.Pickups;
    using MapGeneration;

    using UnityEngine;

    /// <summary>
    /// Contains information about items in the pocket dimension.
    /// </summary>
    public class PocketDimensionPickupEventArgs : IDeniableEvent, IPickupEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PocketDimensionPickupEventArgs"/> class.
        /// </summary>
        /// <param name="pickupBase"><inheritdoc cref="Pickup"/></param>
        /// <param name="roomName"><inheritdoc cref="RoomIdentifier"/></param>
        /// <param name="isAllowed"><inheritdoc cref="IsAllowed"/></param>
        /// <param name="remove">Handles removing pickups from the pocket dimension.</param>
        /// <param name="warningSent">Handles sending the warning sound before a pickup leaves the pocket dimension.</param>
        /// <param name="hasFlagFast">Handles the pickup having the item tier flag.</param>
        /// <param name="position"><inheritdoc/></param>
        public PocketDimensionPickupEventArgs(ItemPickupBase pickupBase, Vector3 position, RoomName roomName, bool isAllowed = true, bool remove = false, bool warningSent = false, bool hasFlagFast = false)
        {
            Pickup = Pickup.Get(pickupBase);
            if (roomName is not RoomName.Pocket)
                return;

            HasFlagFast = hasFlagFast;
            ShouldWarning = warningSent;
            ShouldRemove = remove;
            IsAllowed = isAllowed;
            Position = position;
        }

        /// <inheritdoc/>
        public Pickup Pickup { get; }

        /// <summary>
        /// Gets or sets a value indicating whether the pickup should be removed from the Pocket Dimension.
        /// </summary>
        public bool ShouldRemove { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the warning sound for the pickup should be sent.
        /// </summary>
        public bool ShouldWarning { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the pickup has the item tier flag.
        /// </summary>
        public bool HasFlagFast { get; set; }

        /// <summary>
        /// Gets the location where the pickup will drop onto the map.
        /// </summary>
        public Vector3 Position { get; }

        /// <inheritdoc/>
        public bool IsAllowed { get; set; }
    }
}