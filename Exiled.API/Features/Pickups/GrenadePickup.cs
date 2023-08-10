// -----------------------------------------------------------------------
// <copyright file="GrenadePickup.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Pickups
{
    using Exiled.API.Enums;
    using Exiled.API.Extensions;
    using Exiled.API.Interfaces;

    using InventorySystem.Items.ThrowableProjectiles;

    using Footprinting;

    /// <summary>
    /// A wrapper class for a grenade pickup.
    /// </summary>
    public class GrenadePickup : Pickup, IWrapper<TimedGrenadePickup>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GrenadePickup"/> class.
        /// </summary>
        /// <param name="pickupBase">The base <see cref="TimedGrenadePickup"/> class.</param>
        internal GrenadePickup(TimedGrenadePickup pickupBase)
            : base(pickupBase)
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
        /// Gets the <see cref="Enums.ProjectileType"/> of the item.
        /// </summary>
        public ProjectileType ProjectileType => Type.GetProjectileType();

        /// <summary>
        /// Gets the <see cref="TimedGrenadePickup"/> that this class is encapsulating.
        /// </summary>
        public new TimedGrenadePickup Base { get; }

        /// <summary>
        /// Trigger the grenade to make it Explode.
        /// </summary>
        public void Explode() => Explode(Base.PreviousOwner);

        /// <summary>
        /// Trigger the grenade to make it Explode.
        /// </summary>
        /// <param name="attacker">The <see cref="Footprint"/> of the explosion.</param> 
        public void Explode(Footprint attacker) 
        {
            Base._replaceNextFrame = true;
            Base._attacker = attacker;
        }
    }
}
