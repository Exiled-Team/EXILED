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

    using UnityEngine;

    using static PlayerRoles.PlayableScps.Scp106.Scp106PocketItemManager;

    /// <summary>
    /// Contains information about items in the pocket dimension.
    /// </summary>
    public class PocketDimensionPickupEventArgs : IDeniableEvent, IPickupEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PocketDimensionPickupEventArgs"/> class.
        /// </summary>
        /// <param name="pickupBase"><inheritdoc cref="Pickup"/></param>
        /// <param name="pocketItem"><inheritdoc cref="PocketItem"/></param>
        /// <param name="isAllowed"><inheritdoc cref="IsAllowed"/></param>
        public PocketDimensionPickupEventArgs(ItemPickupBase pickupBase, PocketItem pocketItem, bool isAllowed)
        {
            Pickup = Pickup.Get(pickupBase);
            PocketItem = pocketItem;
            IsAllowed = isAllowed;
        }

        /// <inheritdoc/>
        public Pickup Pickup { get; }

        /// <summary>
        /// Gets the value of the PocketItem.
        /// </summary>
        public PocketItem PocketItem { get; }

        /// <summary>
        /// Gets or sets a value indicating when the Pickup will be dropped onto the map.
        /// </summary>
        public double DropTime
        {
            get => PocketItem.TriggerTime;
            set => PocketItem.TriggerTime = value;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the pickup should be removed from the Pocket Dimension.
        /// </summary>
        public bool ShouldRemove
        {
            get => PocketItem.Remove;
            set => PocketItem.Remove = value;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the warning sound for the pickup should be sent.
        /// </summary>
        public bool ShouldWarn
        {
            get => !PocketItem.WarningSent;
            set => PocketItem.WarningSent = !value;
        }

        /// <summary>
        /// Gets or sets the location where the pickup will drop onto the map.
        /// </summary>
        public Vector3 Position
        {
            get => PocketItem.DropPosition.Position;
            set => PocketItem.DropPosition = new(value);
        }

        /// <inheritdoc/>
        public bool IsAllowed { get; set; }
    }
}