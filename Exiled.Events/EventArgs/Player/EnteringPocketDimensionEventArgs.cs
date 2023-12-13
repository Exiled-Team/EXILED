// -----------------------------------------------------------------------
// <copyright file="EnteringPocketDimensionEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Player
{
    using API.Features;

    using Interfaces;

    /// <summary>
    /// Contains all information before a player enters the pocket dimension.
    /// </summary>
    public class EnteringPocketDimensionEventArgs : IPlayerEvent, IDeniableEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EnteringPocketDimensionEventArgs" /> class.
        /// </summary>
        /// <param name="player">
        /// <inheritdoc cref="Player" />
        /// </param>
        /// <param name="scp106">
        /// <inheritdoc cref="Scp106" />
        /// </param>
        /// <param name="isAllowed">
        /// <inheritdoc cref="IsAllowed" />
        /// </param>
        public EnteringPocketDimensionEventArgs(Player player, Player scp106, bool isAllowed = true)
        {
            Player = player;
            Scp106 = scp106;
            IsAllowed = isAllowed;
        }

        /// <summary>
        /// Gets the SCP-106 who sent the player to the pocket dimension.
        /// </summary>
        public Player Scp106 { get; }

        /// <summary>
        /// Gets or sets a value indicating whether or not the player can enter the pocket dimension.
        /// </summary>
        public bool IsAllowed { get; set; }

        /// <summary>
        /// Gets the player who's entering the pocket dimension.
        /// </summary>
        public Player Player { get; }
    }
}