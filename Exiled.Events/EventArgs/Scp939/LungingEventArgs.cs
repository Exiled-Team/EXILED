// -----------------------------------------------------------------------
// <copyright file="LungingEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Scp939
{
    using System;

    using API.Features;
    using Exiled.API.Features.Roles;
    using Interfaces;

    /// <summary>
    ///     Contains all information before SCP-939 uses its lunge ability.
    /// </summary>
    public class LungingEventArgs : IScp939Event
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="LungingEventArgs" /> class.
        /// </summary>
        /// <param name="player">
        ///     <inheritdoc cref="Player" />
        /// </param>
        public LungingEventArgs(ReferenceHub player)
        {
            Player = Player.Get(player);
            Scp939 = Player.Role.As<Scp939Role>();
        }

        /// <summary>
        ///     Gets the player who's controlling SCP-939.
        /// </summary>
        public Player Player { get; }

        /// <inheritdoc/>
        public Scp939Role Scp939 { get; }
    }
}
