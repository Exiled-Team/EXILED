// -----------------------------------------------------------------------
// <copyright file="RemoveObserverEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Scp173
{
    using API.Features;
    using Exiled.API.Features.Roles;
    using Interfaces;

    /// <summary>
    /// Contains all information after a player stops looking at SCP-173.
    /// </summary>
    public class RemoveObserverEventArgs : IScp173Event
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RemoveObserverEventArgs"/> class.
        /// </summary>
        /// <param name="player">
        /// <inheritdoc cref="Scp173" />
        /// </param>
        /// <param name="target">
        /// <inheritdoc cref="Target" />
        /// </param>
        public RemoveObserverEventArgs(Player player, Player target)
        {
            Scp173 = player.Role.As<Scp173Role>();
            Player = player;
            Target = target;
        }

        /// <inheritdoc />
        public Scp173Role Scp173 { get; }

        /// <summary>
        /// Gets the player who controlling scp 173.
        /// </summary>
        public Player Player { get; }

        /// <summary>
        /// Gets the target who no longer see the scp 173.
        /// </summary>
        public Player Target { get; }
    }
}