// -----------------------------------------------------------------------
// <copyright file="JumpingEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Player
{
    using API.Features;

    using Interfaces;

    using UnityEngine;

    /// <summary>
    /// Contains all information before a player jumps.
    /// </summary>
    public class JumpingEventArgs : IPlayerEvent, IDeniableEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="JumpingEventArgs" /> class.
        /// </summary>
        /// <param name="player">
        /// <inheritdoc cref="Player" />
        /// </param>
        /// <param name="direction">
        /// <inheritdoc cref="Direction" />
        /// </param>
        /// <param name="isAllowed">
        /// <inheritdoc cref="IsAllowed" />
        /// </param>
        public JumpingEventArgs(Player player, Vector3 direction, bool isAllowed = true)
        {
            Player = player;
            Direction = direction;
            IsAllowed = isAllowed;
        }

        /// <summary>
        /// Gets the player who's jumping.
        /// </summary>
        public Player Player { get; }

        /// <summary>
        /// Gets or sets the jump direction.
        /// </summary>
        public Vector3 Direction { get; set; }

        /// <summary>
        /// Gets or sets the jump speed.
        /// </summary>
        public float Speed
        {
            get => Direction.y;
            set => Direction += Vector3.up * value;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the client data can be synchronized with the server.
        /// </summary>
        public bool IsAllowed { get; set; }
    }
}