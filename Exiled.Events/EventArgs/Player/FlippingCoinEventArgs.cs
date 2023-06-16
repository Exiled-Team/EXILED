// -----------------------------------------------------------------------
// <copyright file="FlippingCoinEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Player
{
    using API.Features;
    using Exiled.API.Features.Items;
    using Interfaces;
    using InventorySystem.Items.Coin;

    /// <summary>
    ///     Contains all information before a player flips a coin.
    /// </summary>
    public class FlippingCoinEventArgs : IPlayerEvent, IItemEvent, IDeniableEvent
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="FlippingCoinEventArgs" /> class.
        /// </summary>
        /// <param name="coin">
        ///     <inheritdoc cref="Item" />
        /// </param>
        /// <param name="isTails">
        ///     <inheritdoc cref="IsTails" />
        /// </param>
        public FlippingCoinEventArgs(Coin coin, bool isTails)
        {
            Item = Item.Get(coin);
            Player = Item.Owner;
            IsTails = isTails;
        }

        /// <inheritdoc />
        public Player Player { get; }

        /// <inheritdoc />
        public Item Item { get; }

        /// <summary>
        ///     Gets or sets a value indicating whether or not the coin is landing on tails.
        /// </summary>
        public bool IsTails { get; set; }

        /// <inheritdoc />
        public bool IsAllowed { get; set; } = true;
    }
}