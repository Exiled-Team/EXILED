// -----------------------------------------------------------------------
// <copyright file="SpawnedItemEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs
{
    using System;

    using InventorySystem.Items.Pickups;

    /// <summary>
    /// Contains all information after the server spawns an item.
    /// </summary>
    public class SpawnedItemEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SpawnedItemEventArgs"/> class.
        /// </summary>
        /// <param name="pickup"><inheritdoc cref="Pickup"/></param>
        public SpawnedItemEventArgs(ItemPickupBase pickup)
        {
            Pickup = pickup;
        }

        /// <summary>
        /// Gets or sets the item pickup.
        /// </summary>
        public ItemPickupBase Pickup { get; set; }
    }
}
