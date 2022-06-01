// -----------------------------------------------------------------------
// <copyright file="GrenadePickup.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Pickups
{
    using Exiled.API.Features.Items;

    using InventorySystem.Items.ThrowableProjectiles;

    /// <summary>
    /// A wrapper class for Grenade.
    /// </summary>
    public class GrenadePickup : Pickup
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GrenadePickup"/> class.
        /// </summary>
        /// <param name="pickupBase">The base <see cref="TimedGrenadePickup"/> class.</param>
        public GrenadePickup(TimedGrenadePickup pickupBase)
            : base(pickupBase)
        {
            Base = pickupBase;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GrenadePickup"/> class.
        /// </summary>
        /// <param name="scp244Type">.</param>
        internal GrenadePickup(ItemType scp244Type)
           : this((TimedGrenadePickup)new Pickup(scp244Type).Base)
        {
        }

        /// <summary>
        /// Gets the <see cref="TimedGrenadePickup"/> that this class is encapsulating.
        /// </summary>
        public new TimedGrenadePickup Base { get; }
    }
}
