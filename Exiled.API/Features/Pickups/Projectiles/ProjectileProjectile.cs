// -----------------------------------------------------------------------
// <copyright file="ProjectilePickup.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Pickups.Projectiles
{
    using InventorySystem.Items.ThrowableProjectiles;

    /// <summary>
    /// A wrapper class for Projectile.
    /// </summary>
    public class ProjectileProjectile : Pickup
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectileProjectile"/> class.
        /// </summary>
        /// <param name="pickupBase">The base <see cref="ThrownProjectile"/> class.</param>
        internal ProjectileProjectile(ThrownProjectile pickupBase)
            : base(pickupBase)
        {
            Base = pickupBase;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectileProjectile"/> class.
        /// </summary>
        /// <param name="type">The <see cref="ItemType"/> of the pickup.</param>
        internal ProjectileProjectile(ItemType type)
            : base(type)
        {
            Base = (ThrownProjectile)((Pickup)this).Base;
        }

        /// <summary>
        /// Gets the <see cref="ThrownProjectile"/> that this class is encapsulating.
        /// </summary>
        public new ThrownProjectile Base { get; }

        /// <summary>
        /// Returns the ProjectilePickup in a human readable format.
        /// </summary>
        /// <returns>A string containing ProjectilePickup-related data.</returns>
        public override string ToString() => $"{Type} ({Serial}) [{Weight}] *{Scale}* |{Position}| -{Locked}- ={InUse}=";
    }
}
