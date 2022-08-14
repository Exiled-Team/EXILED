// -----------------------------------------------------------------------
// <copyright file="ThrowingItemEventArgs.cs" company="Exiled Team">
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
    /// Contains all information before a player throws a grenade.
    /// </summary>
    /// <remarks>
    /// This event does not include IsAllowed property.
    /// 
    /// <br>use <see cref="ThrowingRequestEventArgs.IsAllowed"/>.</br>
    /// </remarks>
    public class ThrowingItemEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ThrowingItemEventArgs"/> class.
        /// </summary>
        /// <param name="player"><inheritdoc cref="Player"/></param>
        /// <param name="item"><inheritdoc cref="Item"/></param>
        /// <param name="projectile"><inheritdoc cref="Grenade"/></param>
        public ThrowingItemEventArgs(Player player, ThrowableItem item, ThrownProjectile projectile)
        {
            Player = player;
            Item = (Throwable)API.Features.Items.Item.Get(item);
            Grenade = (Projectile)Pickup.Get(projectile);
        }

        /// <summary>
        /// Gets the player who's throwing the grenade.
        /// </summary>
        public Player Player { get; }

        /// <summary>
        /// Gets the item being thrown.
        /// </summary>
        public Throwable Item { get; }

        /// <summary>
        /// Gets the grenade thats will thrown.
        /// </summary>
        public Projectile Grenade { get; }
    }
}
