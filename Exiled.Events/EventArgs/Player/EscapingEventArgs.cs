// -----------------------------------------------------------------------
// <copyright file="EscapingEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Player
{
    using API.Features;

    using Interfaces;

    using PlayerRoles;

    /// <summary>
    ///     Contains all information before a player escapes.
    /// </summary>
    public class EscapingEventArgs : IPlayerEvent, IDeniableEvent
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="EscapingEventArgs" /> class.
        /// </summary>
        /// <param name="player">
        ///     <inheritdoc cref="Player" />
        /// </param>
        /// <param name="newRole">
        ///     <inheritdoc cref="NewRole" />
        /// </param>
        public EscapingEventArgs(Player player, RoleTypeId newRole)
        {
            Player = player;
            NewRole = newRole;
        }

        /// <summary>
        ///     Gets the player who's escaping.
        /// </summary>
        public Player Player { get; }

        /// <summary>
        ///     Gets or sets the role that will be assigned when the player escapes.
        /// </summary>
        public RoleTypeId NewRole { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether or not the player can escape.
        /// </summary>
        public bool IsAllowed { get; set; } = true;
    }
}