// -----------------------------------------------------------------------
// <copyright file="ThrowingRequestEventArgs.cs" company="Exiled Team">
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
    /// Contains all information before receiving a throwing request.
    /// </summary>
    public class ThrowingRequestEventArgs : IPlayerEvent, IItemEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ThrowingRequestEventArgs"/> class.
        /// </summary>
        /// <param name="player"><inheritdoc cref="Player"/></param>
        /// <param name="item"><inheritdoc cref="Throwable"/></param>
        /// <param name="request"><inheritdoc cref="RequestType"/></param>
        public ThrowingRequestEventArgs(Player player, ThrowableItem item, ThrowableNetworkHandler.RequestType request)
        {
            Player = player;
            Throwable = (Throwable)Item.Get(item);
            RequestType = (ThrowRequest)request;
        }

        /// <summary>
        /// Gets the player who's sending the request.
        /// </summary>
        public Player Player { get; }

        /// <summary>
        /// Gets or sets the item being thrown.
        /// </summary>
        public Throwable Throwable { get; set; }

        /// <inheritdoc/>
        public Item Item => Throwable;

        /// <summary>
        ///  Gets or sets the type of throw being requested.
        /// </summary>
        public ThrowRequest RequestType { get; set; }
    }
}
