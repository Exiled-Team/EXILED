// -----------------------------------------------------------------------
// <copyright file="DroppingNothingEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Player
{
    using API.Features;

    using Interfaces;

    /// <summary>
    /// Contains all information before a player drops a null item.
    /// </summary>
    public class DroppingNothingEventArgs : IPlayerEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DroppingNothingEventArgs" /> class.
        /// </summary>
        /// <param name="player">
        /// <inheritdoc cref="Player" />
        /// </param>
        public DroppingNothingEventArgs(Player player) => Player = player;

        /// <summary>
        /// Gets the player who's dropping the null item.
        /// </summary>
        public Player Player { get; }
    }
}