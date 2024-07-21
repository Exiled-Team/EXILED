// -----------------------------------------------------------------------
// <copyright file="AttackingEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Scp173
{
    using API.Features;
    using Exiled.API.Features.Roles;
    using Exiled.Events.EventArgs.Interfaces;

    /// <summary>
    /// Contains all information before a Scp-173 snaps a player neck.
    /// </summary>
    public class AttackingEventArgs : IScp173Event, IDeniableEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AttackingEventArgs" /> class.
        /// </summary>
        /// <param name="player"><see cref="Player"/>.</param>
        /// <param name="target"><see cref="Target"/>.</param>
        /// <param name="isAllowed"><see cref="IsAllowed"/>.</param>
        public AttackingEventArgs(Player player, Player target, bool isAllowed = true)
        {
            Player = player;
            Scp173 = Player.Role.As<Scp173Role>();
            Target = target;
            IsAllowed = isAllowed;
        }

        /// <inheritdoc/>
        public Player Player { get; }

        /// <inheritdoc/>
        public Scp173Role Scp173 { get; }

        /// <summary>
        /// Gets the player that is going to be snapped.
        /// </summary>
        public Player Target { get; }

        /// <inheritdoc/>
        public bool IsAllowed { get; set; }
    }
}