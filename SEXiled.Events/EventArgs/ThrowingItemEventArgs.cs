// -----------------------------------------------------------------------
// <copyright file="ThrowingItemEventArgs.cs" company="SEXiled Team">
// Copyright (c) SEXiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace SEXiled.Events.EventArgs
{
    using System;

    using SEXiled.API.Enums;
    using SEXiled.API.Features;
    using SEXiled.API.Features.Items;

    using InventorySystem.Items.ThrowableProjectiles;

    /// <summary>
    /// Contains all information before a player throws a grenade.
    /// </summary>
    public class ThrowingItemEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ThrowingItemEventArgs"/> class.
        /// </summary>
        /// <param name="player"><inheritdoc cref="Player"/></param>
        /// <param name="item"><inheritdoc cref="Item"/></param>
        /// <param name="request"><inheritdoc cref="RequestType"/></param>
        /// <param name="isAllowed"><inheritdoc cref="IsAllowed"/></param>
        public ThrowingItemEventArgs(Player player, ThrowableItem item, ThrowableNetworkHandler.RequestType request, bool isAllowed = true)
        {
            Player = player;
            Item = (Throwable)API.Features.Items.Item.Get(item);
            RequestType = (ThrowRequest)request;
            IsAllowed = isAllowed;
        }

        /// <summary>
        /// Gets the player who's throwing the grenade.
        /// </summary>
        public Player Player { get; }

        /// <summary>
        /// Gets or sets the item being thrown.
        /// </summary>
        public Throwable Item { get; set; }

        /// <summary>
        ///  Gets or sets the type of throw being requested.
        /// </summary>
        public ThrowRequest RequestType { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether or not the grenade can be thrown.
        /// </summary>
        public bool IsAllowed { get; set; } = true;
    }
}
