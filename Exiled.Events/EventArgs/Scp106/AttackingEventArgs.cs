// -----------------------------------------------------------------------
// <copyright file="AttackingEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Scp106
{
    using Exiled.API.Features;
    using Exiled.API.Features.Roles;
    using Exiled.Events.EventArgs.Interfaces;

    /// <summary>
    /// Contains all information before SCP-106 attacks player.
    /// </summary>
    public class AttackingEventArgs : IScp106Event, IDeniableEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AttackingEventArgs"/> class.
        /// </summary>
        /// <param name="player"><inheritdoc cref="Player"/></param>
        /// <param name="target"><inheritdoc cref="Target"/></param>
        /// <param name="isAllowed"><inheritdoc cref="IsAllowed"/></param>
        public AttackingEventArgs(Player player, Player target, bool isAllowed = true)
        {
            Player = player;
            Scp106 = Player.Role.As<Scp106Role>();
            Target = target;
            IsAllowed = isAllowed;
        }

        /// <summary>
        /// Gets the player controlling SCP-106.
        /// </summary>
        public Player Player { get; }

        /// <inheritdoc/>
        public Scp106Role Scp106 { get; }

        /// <summary>
        /// Gets the target of attack.
        /// </summary>
        public Player Target { get; }

        /// <summary>
        /// Gets or sets a value indicating whether or not target can be attacked.
        /// </summary>
        public bool IsAllowed { get; set; }
    }
}