// -----------------------------------------------------------------------
// <copyright file="ChangedIntoGrenadeEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Map
{
    using System;

    using Exiled.API.Features.Pickups;
    using Exiled.API.Features.Pickups.Projectiles;
    using Exiled.Events.EventArgs.Interfaces;
    using InventorySystem.Items.ThrowableProjectiles;

    /// <summary>
    /// Contains all information after a <see cref="GrenadePickup"/> was changed into a live grenade by an explosion.
    /// </summary>
    public class ChangedIntoGrenadeEventArgs : IExiledEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ChangedIntoGrenadeEventArgs"/> class.
        /// </summary>
        /// <param name="pickup">The <see cref="API.Features.Pickups.Pickup"/> being changed.</param>
        /// <param name="projectile">The <see cref="TimeGrenadeProjectile"/>.</param>
        public ChangedIntoGrenadeEventArgs(TimedGrenadePickup pickup, ThrownProjectile projectile)
        {
            Pickup = (GrenadePickup)API.Features.Pickups.Pickup.Get(pickup);
            Projectile = (Projectile)API.Features.Pickups.Pickup.Get(projectile);
        }

        /// <summary>
        /// Gets a value indicating the pickup that was changed into a grenade.
        /// </summary>
        public GrenadePickup Pickup { get; }

        /// <summary>
        /// Gets a value indicating the projectile that was spawned.
        /// </summary>
        public Projectile Projectile { get; }
    }
}
