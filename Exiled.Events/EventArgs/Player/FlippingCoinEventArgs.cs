// -----------------------------------------------------------------------
// <copyright file="FlippingCoinEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Player
{
    using API.Features;
    using API.Features.Items;
    using Interfaces;
    using InventorySystem.Items.Coin;

    /// <summary>
    /// Contains all information before a player flips a coin.
    /// </summary>
    public class FlippingCoinEventArgs : IPlayerEvent, IDeniableEvent, IItemEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FlippingCoinEventArgs" /> class.
        /// </summary>
        /// <param name="player">
        /// <inheritdoc cref="Player" />
        /// </param>
        /// <param name="coin">
        /// <inheritdoc cref="Item" />
        /// </param>
        /// <param name="isTails">
        /// <inheritdoc cref="IsTails" />
        /// </param>
        public FlippingCoinEventArgs(Player player, Coin coin, bool isTails)
        {
            Player = player;
            Item = Item.Get(coin);
            IsTails = isTails;
        }

        /// <summary>
        /// Gets the player who's flipping the coin.
        /// </summary>
        public Player Player { get; }

        /// <inheritdoc/>
        public Item Item { get; }

        /// <summary>
        /// Gets or sets a value indicating whether the coin is landing on tails.
        /// </summary>
        public bool IsTails { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the coin can be flipped.
        /// </summary>
        public bool IsAllowed { get; set; } = true;
    }
}