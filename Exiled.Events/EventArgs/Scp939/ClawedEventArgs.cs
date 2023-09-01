// -----------------------------------------------------------------------
// <copyright file="ClawedEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Scp939
{
    using API.Features;
    using Exiled.API.Features.Roles;
    using Exiled.Events.EventArgs.Interfaces;

    /// <summary>
    /// Contains all information after SCP-939 attacks.
    /// </summary>
    public class ClawedEventArgs : IScp939Event
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ClawedEventArgs"/> class.
        /// </summary>
        /// <param name="player"><inheritdoc cref="Player"/></param>
        public ClawedEventArgs(Player player)
        {
            Player = player;
            Scp939 = Player.Role.As<Scp939Role>();
        }

        /// <summary>
        /// Gets the SCP-939.
        /// </summary>
        public Player Player { get; }

        /// <inheritdoc/>
        public Scp939Role Scp939 { get; }
    }
}