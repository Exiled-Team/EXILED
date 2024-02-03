// -----------------------------------------------------------------------
// <copyright file="InteractedEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Player
{
    using API.Features;

    using Interfaces;

    /// <summary>
    /// Contains all information after a player has interacted with an interactable.
    /// </summary>
    public class InteractedEventArgs : IPlayerEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InteractedEventArgs" /> class.
        /// </summary>
        /// <param name="player">
        /// <inheritdoc cref="Player" />
        /// </param>
        public InteractedEventArgs(Player player)
        {
            Player = player;
        }

        /// <summary>
        /// Gets the player who interacted.
        /// </summary>
        public Player Player { get; }
    }
}