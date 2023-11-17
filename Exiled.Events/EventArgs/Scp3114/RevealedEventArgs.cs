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
    ///     Contains all information before SCP-3114 changes its target focus.
    /// </summary>
    public class RevealedEventArgs : IScp3114Event
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="RevealedEventArgs" /> class.
        /// </summary>
        /// <param name="player">
        ///     <inheritdoc cref="Player" />
        /// </param>
        /// <param name="isManualReveal">
        ///     <inheritdoc cref="IsManualReveal" />
        /// </param>
        public RevealedEventArgs(Player player, bool isManualReveal)
        {
            Player = player;
            Scp3114 = Player.Role.As<Scp3114Role>();
            IsManualReveal = isManualReveal;
        }

        /// <inheritdoc/>
        public Player Player { get; }

        /// <inheritdoc/>
        public Scp3114Role Scp3114 { get; }

        /// <summary>
        ///     Gets a value indicating whether the reveal is manual or not.
        /// </summary>
        public bool IsManualReveal { get; }
    }
}