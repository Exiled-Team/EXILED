// -----------------------------------------------------------------------
// <copyright file="LeftEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Player
{
    using API.Features;
    using Exiled.Events.EventArgs.Interfaces;

    /// <summary>
    /// Contains all information after a <see cref="API.Features.Player"/> disconnects from the server.
    /// </summary>
    public class LeftEventArgs : IPlayerEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LeftEventArgs"/> class.
        /// </summary>
        /// <param name="player">The player who left the server.</param>
        public LeftEventArgs(Player player) => Player = player;

        /// <summary>
        /// Gets the left player.
        /// </summary>
        public Player Player { get; }
    }
}
