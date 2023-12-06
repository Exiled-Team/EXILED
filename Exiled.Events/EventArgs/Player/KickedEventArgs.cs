// -----------------------------------------------------------------------
// <copyright file="KickedEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Player
{
    using API.Features;

    using Interfaces;

    /// <summary>
    /// Contains all information after kicking a player from the server.
    /// </summary>
    public class KickedEventArgs : IPlayerEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="KickedEventArgs" /> class.
        /// </summary>
        /// <param name="target">
        /// <inheritdoc cref="Player" />
        /// </param>
        /// <param name="reason">
        /// <inheritdoc cref="Reason" />
        /// </param>
        public KickedEventArgs(Player target, string reason)
        {
            Player = target;
            Reason = reason;
        }

        /// <summary>
        /// Gets the kick reason.
        /// </summary>
        public string Reason { get; }

        /// <summary>
        /// Gets the kicked player.
        /// </summary>
        public Player Player { get; }
    }
}