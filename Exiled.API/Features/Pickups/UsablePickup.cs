// -----------------------------------------------------------------------
// <copyright file="UsablePickup.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Pickups
{
    using Exiled.API.Features.Items;
    using InventorySystem.Items.Pickups;

    /// <summary>
    /// A wrapper class for dropped Usable Pickup.
    /// </summary>
    public class UsablePickup : Pickup
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UsablePickup"/> class.
        /// </summary>
        /// <param name="pickupBase">.</param>
        internal UsablePickup(ItemPickupBase pickupBase)
            : base(pickupBase)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UsablePickup"/> class.
        /// </summary>
        /// <param name="type">.</param>
        internal UsablePickup(ItemType type)
            : base(type)
        {
        }

        /// <summary>
        /// Gets or sets how long it takes to use the item.
        /// </summary>
        public float UseTime { get; set; }

        /// <summary>
        /// Gets or sets how long after using starts a player has to cancel using the item.
        /// </summary>
        public float MaxCancellableTime { get; set; }

        /// <inheritdoc/>
        internal override void ReadItemInfo(Item item)
        {
            base.ReadItemInfo(item);
            if (item is Usable usableitem)
            {
                UseTime = usableitem.UseTime;
                MaxCancellableTime = usableitem.MaxCancellableTime;
            }
        }
    }
}
