// -----------------------------------------------------------------------
// <copyright file="StranglingEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Scp3114
{
    using Exiled.API.Features;
    using Exiled.API.Features.Roles;
    using Exiled.Events.EventArgs.Interfaces;

    /// <summary>
    /// Contains all information before strangling a player.
    /// </summary>
    public class StranglingEventArgs : IScp3114Event, IDeniableEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StranglingEventArgs" /> class.
        /// </summary>
        /// <param name="player">The <see cref="Player"/> instance which this is being instantiated from.</param>
        /// <param name="target">The <see cref="API.Features.Player"/> being targeted.</param>
        /// <param name="isAllowed"><see cref="IsAllowed"/>.</param>
        public StranglingEventArgs(Player player, Player target, bool isAllowed = true)
        {
            Player = player;
            Scp3114 = Player.Role.As<Scp3114Role>();
            Target = target;
            IsAllowed = isAllowed;
        }

        /// <inheritdoc/>
        public Player Player { get; }

        /// <inheritdoc/>
        public Scp3114Role Scp3114 { get; }

        /// <summary>
        /// Gets the <see cref="Player"/> being strangled.
        /// </summary>
        public Player Target { get; }

        /// <inheritdoc/>
        public bool IsAllowed { get; set; }
    }
}