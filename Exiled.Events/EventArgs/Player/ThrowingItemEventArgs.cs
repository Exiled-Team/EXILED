// -----------------------------------------------------------------------
// <copyright file="ThrowingItemEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Player
{
    using Exiled.API.Enums;
    using Exiled.API.Features;
    using Exiled.API.Features.Items;
    using Exiled.Events.EventArgs.Interfaces;

    using InventorySystem.Items.ThrowableProjectiles;

    /// <summary>
    ///     Contains all information before a player throws an item.
    /// </summary>
    public class ThrowingItemEventArgs : IPlayerEvent, IDeniableEvent
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="ThrowingItemEventArgs" /> class.
        /// </summary>
        /// <param name="player">
        ///     <inheritdoc cref="Player" />
        /// </param>
        /// <param name="item">
        ///     <inheritdoc cref="Item" />
        /// </param>
        /// <param name="request">
        ///     <inheritdoc cref="RequestType" />
        /// </param>
        /// <param name="isAllowed">
        ///     <inheritdoc cref="IsAllowed" />
        /// </param>
        public ThrowingItemEventArgs(Player player, ThrowableItem item, ThrowableNetworkHandler.RequestType request, bool isAllowed = true)
        {
            Player = player;
            Item = (Throwable)API.Features.Items.Item.Get(item);
            RequestType = (ThrowRequest)request;
            IsAllowed = isAllowed;
        }

        /// <summary>
        ///     Gets or sets the item being thrown.
        /// </summary>
        public Throwable Item { get; set; }

        /// <summary>
        ///     Gets or sets the type of throw being requested.
        /// </summary>
        public ThrowRequest RequestType { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether or not the grenade can be thrown.
        /// </summary>
        public bool IsAllowed { get; set; } = true;

        /// <summary>
        ///     Gets the player who's throwing the grenade.
        /// </summary>
        public Player Player { get; }
    }
}