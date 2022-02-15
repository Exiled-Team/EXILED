// -----------------------------------------------------------------------
// <copyright file="SpawningItemEventArgs.cs" company="SEXiled Team">
// Copyright (c) SEXiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace SEXiled.Events.EventArgs
{
    using System;

    using SEXiled.API.Features.Items;

    using InventorySystem.Items.Pickups;

    /// <summary>
    /// Contains all information before the server spawns an item.
    /// </summary>
    public class SpawningItemEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SpawningItemEventArgs"/> class.
        /// </summary>
        /// <param name="pickupBase"><inheritdoc cref="Pickup"/></param>
        /// <param name="isAllowed"><inheritdoc cref="IsAllowed"/></param>
        public SpawningItemEventArgs(ItemPickupBase pickupBase, bool isAllowed = true)
        {
            if (pickupBase.Info.Serial > 0)
            {
                pickupBase.Info.Serial = 0;
                pickupBase.NetworkInfo = pickupBase.Info;
            }

            Pickup = Pickup.Get(pickupBase);
            IsAllowed = isAllowed;
        }

        /// <summary>
        /// Gets or sets a value indicating the pickup being spawned.
        /// </summary>
        public Pickup Pickup { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether or not the item can be spawned.
        /// </summary>
        public bool IsAllowed { get; set; }
    }
}
