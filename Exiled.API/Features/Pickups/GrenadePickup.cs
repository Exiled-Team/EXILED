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
    using Exiled.API.Features.Pickups.Projectiles;
    using Exiled.API.Interfaces;

    using Footprinting;

    using InventorySystem.Items.ThrowableProjectiles;

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
        /// Gets or sets how long the fuse will last.
        /// </summary>
        public float FuseTime { get; set; }

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

        /// <summary>
        /// Returns the Projectile with the according property from the Pickup.
        /// </summary>
        /// <param name="projectile"> Pickup-related data to give to the Projectile.</param>
        /// <returns>A Projectile containing the Pickup-related data.</returns>
        internal virtual Pickup GetPickupInfo(Projectile projectile)
        {
            if (projectile is TimeGrenadeProjectile timeGrenadeProjectile)
            {
                timeGrenadeProjectile.FuseTime = FuseTime;
            }

            return projectile;
        }
    }
}
