// -----------------------------------------------------------------------
// <copyright file="TeleportingEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Scp106
{
    using API.Features;
    using Exiled.API.Features.Roles;
    using Interfaces;

    using UnityEngine;

    /// <summary>
    /// Contains all information before SCP-106 teleports using hunter atlas.
    /// </summary>
    public class TeleportingEventArgs : IScp106Event, IDeniableEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TeleportingEventArgs" /> class.
        /// </summary>
        /// <param name="player">
        /// <inheritdoc cref="Player" />
        /// </param>
        /// <param name="position">
        /// <inheritdoc cref="Position" />
        /// </param>
        /// <param name="isAllowed">
        /// <inheritdoc cref="IsAllowed" />
        /// </param>
        public TeleportingEventArgs(Player player, Vector3 position, bool isAllowed = true)
        {
            Player = player;
            Scp106 = player.Role.As<Scp106Role>();
            Position = position;
            IsAllowed = isAllowed;
        }

        /// <summary>
        /// Gets or sets the teleporting position.
        /// </summary>
        public Vector3 Position { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether SCP-106 can teleport using a portal.
        /// </summary>
        public bool IsAllowed { get; set; }

        /// <summary>
        /// Gets the player who's controlling SCP-106.
        /// </summary>
        public Player Player { get; }

        /// <inheritdoc/>
        public Scp106Role Scp106 { get; }
    }
}