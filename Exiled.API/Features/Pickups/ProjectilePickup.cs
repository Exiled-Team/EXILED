// -----------------------------------------------------------------------
// <copyright file="ProjectilePickup.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Pickups
{
    using InventorySystem.Items.ThrowableProjectiles;

    /// <summary>
    /// A wrapper class for Projectile.
    /// </summary>
    public class ProjectilePickup : Pickup
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectilePickup"/> class.
        /// </summary>
        /// <param name="itemBase">The base <see cref="ThrownProjectile"/> class.</param>
        public ProjectilePickup(ThrownProjectile itemBase)
            : base(itemBase)
        {
            Base = itemBase;
        }

        /// <summary>
        /// Gets the <see cref="ThrownProjectile"/> that this class is encapsulating.
        /// </summary>
        public new ThrownProjectile Base { get; }
    }
}
