// -----------------------------------------------------------------------
// <copyright file="FlippingCoinEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Player
{
    using API.Features;

    using Interfaces;

    /// <summary>
    ///     Contains all information before a player flips a coin.
    /// </summary>
    public class FlippingCoinEventArgs : IPlayerEvent, IDeniableEvent
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="FlippingCoinEventArgs" /> class.
        /// </summary>
        /// <param name="player">
        ///     <inheritdoc cref="Player" />
        /// </param>
        /// <param name="isAllowed">
        ///     <inheritdoc cref="IsAllowed" />
        /// </param>
        public FlippingCoinEventArgs(Player player, bool isAllowed = true)
        {
            Player = player;
            IsTails = UnityEngine.Random.value >= 0.5f;
            IsAllowed = isAllowed;
        }

        /// <summary>
        ///     Gets the player who's flipping the coin.
        /// </summary>
        public Player Player { get; }

        /// <summary>
        ///     Gets or sets a value indicating whether or not the coin is landing on tails.
        /// </summary>
        public bool IsTails { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether or not the coin can be flipped.
        /// </summary>
        public bool IsAllowed { get; set; }
    }
}