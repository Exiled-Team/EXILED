// -----------------------------------------------------------------------
// <copyright file="AttackingEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Scp0492
{
    using API.Features;
    using Exiled.API.Features.Roles;
    using Exiled.Events.EventArgs.Interfaces;

    /// <summary>
    /// Contains all information before a Scp-0492 attacks a player.
    /// </summary>
    public class AttackingEventArgs : IScp0492Event, IDeniableEvent
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
            Scp0492 = Player.Role.As<Scp0492Role>();
            Target = target;
            IsAllowed = isAllowed;
        }

        /// <inheritdoc/>
        public Player Player { get; }

        /// <inheritdoc/>
        public Scp0492Role Scp0492 { get; }

        /// <summary>
        /// Gets the player that is going to damaged by a SCP-0492.
        /// </summary>
        public Player Target { get; }

        /// <inheritdoc/>
        public bool IsAllowed { get; set; }
    }
}