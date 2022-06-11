// -----------------------------------------------------------------------
// <copyright file="ChangingIntoGrenadeEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs
{
    using System;

    using Exiled.API.Features;
    using Exiled.API.Features.Items;
    using Exiled.API.Features.Pickups;
    using Exiled.API.Features.Pickups.Projectiles;

    using InventorySystem.Items.ThrowableProjectiles;

    /// <summary>
    /// Contains all information for when the server is turning a pickup into a live grenade.
    /// </summary>
    public class ChangingIntoGrenadeEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ChangingIntoGrenadeEventArgs"/> class.
        /// </summary>
        /// <param name="pickup">The <see cref="Pickup"/> being changed.</param>
        public ChangingIntoGrenadeEventArgs(TimedGrenadePickup pickup)
        {
            if (pickup is null)
                Log.Error($"{nameof(ChangingIntoGrenadeEventArgs)}: Pickup is null!");
            Projectile = (Projectile)Pickup.Get(pickup);
            Type = Projectile.Type;
            FuseTime = Projectile is TimeGrenadeProjectile timeGrenade ? timeGrenade.FuseTime : 3f;
        }

        /// <summary>
        /// Gets a value indicating the pickup being changed.
        /// </summary>
        public Projectile Projectile { get; }

        /// <summary>
        /// Gets or sets a value indicating what type of grenade will be spawned.
        /// </summary>
        public ItemType Type { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the pickup will be changed.
        /// </summary>
        public bool IsAllowed { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating how long the fuse of the changed grenade will be.
        /// </summary>
        public float FuseTime { get; set; }
    }
}
