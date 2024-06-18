// -----------------------------------------------------------------------
// <copyright file="HealedEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Player
{
    using Exiled.API.Features;
    using Exiled.Events.EventArgs.Interfaces;

    /// <summary>
    /// Invoked after a <see cref="API.Features.Player"/> has healed.
    /// </summary>
    public class HealedEventArgs : IPlayerEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HealedEventArgs"/> class.
        /// </summary>
        /// <param name="player">
        /// <inheritdoc cref="Player" />
        /// </param>
        /// <param name="lastAmount">
        /// <inheritdoc cref="LastAmount" />
        /// </param>
        public HealedEventArgs(Player player, float lastAmount)
        {
            Player = player;
            LastAmount = lastAmount;
        }

        /// <inheritdoc/>
        public Player Player { get; }

        /// <summary>
        /// Gets the player's last recorded amount of health.
        /// </summary>
        public float LastAmount { get; }

        /// <summary>
        /// Gets the amount of health healed.
        /// </summary>
        public float HealedAmount => Player.Health - LastAmount;
    }
}
