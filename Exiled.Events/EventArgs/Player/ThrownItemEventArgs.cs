// -----------------------------------------------------------------------
// <copyright file="ThrownItemEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Player
{
    using Exiled.API.Features;
    using Exiled.API.Features.Items;
    using Exiled.API.Features.Pickups;
    using Exiled.API.Features.Pickups.Projectiles;
    using Exiled.Events.EventArgs.Interfaces;

    using InventorySystem.Items.ThrowableProjectiles;

    /// <summary>
    /// Contains all information after a player throws a grenade.
    /// </summary>
    public class ThrownItemEventArgs : IPlayerEvent, IItemEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ThrownItemEventArgs"/> class.
        /// </summary>
        /// <param name="player"><inheritdoc cref="Player"/></param>
        /// <param name="item"><inheritdoc cref="Item"/></param>
        /// <param name="projectile"><inheritdoc cref="Projectile"/></param>
        public ThrownItemEventArgs(Player player, ThrowableItem item, ThrownProjectile projectile)
        {
            Player = player;
            Item = Item.Get(item);
            Projectile = (Projectile)Pickup.Get(projectile);
        }

        /// <summary>
        /// Gets the player who's thrown the grenade.
        /// </summary>
        public Player Player { get; }

        /// <summary>
        /// Gets the item being thrown.
        /// </summary>
        public Item Item { get; }

        /// <summary>
        /// Gets the thrown grenade.
        /// </summary>
        public Projectile Projectile { get; }
    }
}
