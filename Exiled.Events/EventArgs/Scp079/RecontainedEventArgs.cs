// -----------------------------------------------------------------------
// <copyright file="RecontainedEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Scp079
{
    using API.Features;
    using Exiled.API.Features.Roles;
    using Interfaces;

    /// <summary>
    ///     Contains information after SCP-079 gets recontained.
    /// </summary>
    public class RecontainedEventArgs : IScp079Event
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="RecontainedEventArgs" /> class.
        /// </summary>
        /// <param name="target">
        ///     <inheritdoc cref="Player" />
        /// </param>
        public RecontainedEventArgs(Player target)
        {
            Player = target;
            Scp079 = Player.Role.As<Scp079Role>();
        }

        /// <summary>
        ///     Gets the player that previously controlled SCP-079.
        /// </summary>
        public Player Player { get; }

        /// <inheritdoc/>
        public Scp079Role Scp079 { get; }
    }
}