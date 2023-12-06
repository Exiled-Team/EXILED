// -----------------------------------------------------------------------
// <copyright file="RevealedEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Scp3114
{
    using API.Features;
    using Exiled.API.Features.Roles;
    using Interfaces;

    /// <summary>
    /// Contains all information after SCP-3114 reveals.
    /// </summary>
    public class RevealedEventArgs : IScp3114Event
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RevealedEventArgs" /> class.
        /// </summary>
        /// <param name="player">
        /// <inheritdoc cref="Player" />
        /// </param>
        public RevealedEventArgs(ReferenceHub player)
        {
            Player = Player.Get(player);
            Scp3114 = Player.Role.As<Scp3114Role>();
        }

        /// <inheritdoc/>
        public Player Player { get; }

        /// <inheritdoc/>
        public Scp3114Role Scp3114 { get; }
    }
}