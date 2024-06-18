// -----------------------------------------------------------------------
// <copyright file="HealingEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Player
{
    using Exiled.API.Features;
    using Exiled.Events.EventArgs.Interfaces;

    /// <summary>
    /// Invoked before a <see cref="API.Features.Player"/> heals.
    /// </summary>
    public class HealingEventArgs : IPlayerEvent, IDeniableEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HealingEventArgs"/> class.
        /// </summary>
        /// <param name="player">
        /// <inheritdoc cref="Player" />
        /// </param>
        /// <param name="amount">
        /// <inheritdoc cref="Amount" />
        /// </param>
        public HealingEventArgs(Player player, float amount)
        {
            Player = player;
            Amount = amount;
        }

        /// <inheritdoc/>
        public Player Player { get; }

        /// <summary>
        /// Gets or sets the player's health.
        /// </summary>
        public float Amount { get; set; }

        /// <inheritdoc/>
        public bool IsAllowed { get; set; } = true;
    }
}
