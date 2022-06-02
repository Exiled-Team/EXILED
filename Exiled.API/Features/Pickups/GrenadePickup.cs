// -----------------------------------------------------------------------
// <copyright file="GrenadePickup.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Pickups
{
    using InventorySystem.Items.ThrowableProjectiles;

    /// <summary>
    /// A wrapper class for GrenadePickup(granade pickup, before turning into a grenade).
    /// </summary>
    public class GrenadePickup : Pickup
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GrenadePickup"/> class.
        /// </summary>
        /// <param name="itemBase">The base <see cref="TimedGrenadePickup"/> class.</param>
        internal GrenadePickup(TimedGrenadePickup itemBase)
            : base(itemBase)
        {
            Base = pickupBase;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GrenadePickup"/> class.
        /// </summary>
        /// <param name="type">The <see cref="ItemType"/> of the pickup.</param>
        internal GrenadePickup(ItemType type)
            : base(type)
        {
            Base = (TimedGrenadePickup)((Pickup)this).Base;
        }

        /// <summary>
        /// Gets the <see cref="TimedGrenadePickup"/> that this class is encapsulating.
        /// </summary>
        public new TimedGrenadePickup Base { get; }
    }
}
