// -----------------------------------------------------------------------
// <copyright file="StranglingEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Scp3114
{
    using Exiled.API.Features;
    using Exiled.Events.EventArgs.Interfaces;

    using Scp3114Role = API.Features.Roles.Scp3114Role;

    /// <summary>
    /// Contains all information before strangling a player.
    /// </summary>
    public class StranglingEventArgs : IScp3114Event, IDeniableEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StranglingEventArgs"/> class.
        /// </summary>
        /// <param name="player">The <see cref="API.Features.Player"/> triggering the event.</param>
        /// <param name="target">The <see cref="API.Features.Player"/> being targeted.</param>
        public StranglingEventArgs(Player player, Player target)
        {
            Scp3114 = player.Role.As<Scp3114Role>();
            Player = player;
            Target = target;
        }

        /// <inheritdoc />
        public Scp3114Role Scp3114 { get; }

        /// <inheritdoc />
        public Player Player { get; }

        /// <summary>
        /// Gets the target that the skeleton is strangling.
        /// </summary>
        public Player Target { get; }

        /// <inheritdoc />
        public bool IsAllowed { get; set; } = true;
    }
}