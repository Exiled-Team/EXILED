// -----------------------------------------------------------------------
// <copyright file="AddingScanTargetEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Scp079
{
    using Exiled.API.Features;
    using Exiled.API.Features.Roles;
    using Exiled.Events.EventArgs.Interfaces;

    /// <summary>
    /// Contains all information before adding a target to the SCP-079 scan.
    /// </summary>
    public class AddingScanTargetEventArgs : IScp079Event, IDeniableEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AddingScanTargetEventArgs" /> class.
        /// </summary>
        /// <param name="player">
        /// <inheritdoc cref="Player" />
        /// </param>
        /// <param name="target">
        /// <inheritdoc cref="Target" />
        /// </param>
        public AddingScanTargetEventArgs(Player player, Player target)
        {
            Scp079 = player.Role.As<Scp079Role>();
            Player = player;
            Target = target;
        }

        /// <inheritdoc />
        public Scp079Role Scp079 { get; }

        /// <summary>
        /// Gets the player who's controlling SCP-079.
        /// </summary>
        public Player Player { get; }

        /// <summary>
        /// Gets the player who will be added to the scan.
        /// </summary>
        public Player Target { get; }

        /// <inheritdoc />
        public bool IsAllowed { get; set; } = true;
    }
}