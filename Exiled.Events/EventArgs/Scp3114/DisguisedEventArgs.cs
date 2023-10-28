// -----------------------------------------------------------------------
// <copyright file="DisguisedEventArgs.cs" company="Exiled Team">
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
    ///     Contains all information before SCP-3114 changes its target focus.
    /// </summary>
    public class DisguisedEventArgs : IScp3114Event
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="DisguisedEventArgs" /> class.
        /// </summary>
        /// <param name="player">
        ///     <inheritdoc cref="Player" />
        /// </param>
        public DisguisedEventArgs(ReferenceHub player)
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