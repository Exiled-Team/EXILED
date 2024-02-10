// -----------------------------------------------------------------------
// <copyright file="SlappedEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Scp3114
{
    using API.Features;
    using Exiled.API.Features.Roles;
    using Interfaces;
    using Mono.Cecil;

    /// <summary>
    /// Contains all information after SCP-3114 slaps.
    /// </summary>
    public class SlappedEventArgs : IScp3114Event
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SlappedEventArgs" /> class.
        /// </summary>
        /// /// <param name="player">
        /// <inheritdoc cref="Player" />
        /// </param>
        public SlappedEventArgs(Player player)
        {
            Player = player;
            Scp3114 = Player.Role.As<Scp3114Role>();
        }

        /// <summary>
        /// Gets the SCP-3114.
        /// </summary>
        public Player Player { get; }

        /// <inheritdoc/>
        public Scp3114Role Scp3114 { get; }
    }
}