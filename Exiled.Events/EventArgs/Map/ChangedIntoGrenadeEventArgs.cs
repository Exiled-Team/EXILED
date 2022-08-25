// -----------------------------------------------------------------------
// <copyright file="ChangedIntoGrenadeEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Map
{
    using System;

    using Exiled.API.Features;
    using Exiled.API.Features.Pickups;
    using Exiled.API.Features.Pickups.Projectiles;
    using Exiled.Events.EventArgs.Interfaces;

    using InventorySystem.Items.ThrowableProjectiles;

    /// <summary>
    /// Contains all information for when the server is turned a pickup into a live grenade.
    /// </summary>
    public class ChangedIntoGrenadeEventArgs : IExiledEvent, IPickupEvent, IProjectileEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ChangedIntoGrenadeEventArgs"/> class.
        /// </summary>
        /// <param name="pickup">The <see cref="API.Features.Pickups.Pickup"/> being changed.</param>
        /// <param name="projectile">The <see cref="TimeGrenadeProjectile"/>.</param>
        public ChangedIntoGrenadeEventArgs(TimedGrenadePickup pickup, ThrownProjectile projectile)
        {
            if (pickup is null)
                Log.Error($"{nameof(ChangedIntoGrenadeEventArgs)}: Pickup is null!");
            Pickup = Pickup.Get(pickup);
            Projectile = (Projectile)Pickup.Get(projectile);
            FuseTime = (Projectile as TimeGrenadeProjectile)?.FuseTime ?? 0f;
        }

        /// <summary>
        /// Gets a value indicating the pickup that changed.
        /// </summary>
        public Pickup Pickup { get; }

        /// <summary>
        /// Gets a value indicating the projectile that spawned.
        /// </summary>
        public Projectile Projectile { get; }

        /// <summary>
        /// Gets or sets a value indicating how long the fuse of the changed grenade will be.
        /// </summary>
        public float FuseTime { get; set; }
    }
}
