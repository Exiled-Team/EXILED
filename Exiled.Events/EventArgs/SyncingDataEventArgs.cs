// -----------------------------------------------------------------------
// <copyright file="SyncingDataEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs {
    using System;

    using Exiled.API.Features;

    using UnityEngine;

    /// <summary>
    /// Contains all informations before syncing player's data with the server.
    /// </summary>
    public class SyncingDataEventArgs : EventArgs {
        /// <summary>
        /// Initializes a new instance of the <see cref="SyncingDataEventArgs"/> class.
        /// </summary>
        /// <param name="player"><inheritdoc cref="Player"/></param>
        /// <param name="speed"><inheritdoc cref="Speed"/></param>
        /// <param name="currentAnimation"><inheritdoc cref="CurrentAnimation"/></param>
        /// <param name="isAllowed"><inheritdoc cref="IsAllowed"/></param>
        public SyncingDataEventArgs(Player player, Vector2 speed, byte currentAnimation, bool isAllowed = true) {
            Player = player;
            Speed = speed;
            CurrentAnimation = currentAnimation;
            IsAllowed = isAllowed;
        }

        /// <summary>
        /// Gets the player of the syncing data.
        /// </summary>
        public Player Player { get; }

        /// <summary>
        /// Gets the player's speed.
        /// </summary>
        public Vector2 Speed { get; }

        /// <summary>
        /// Gets or sets the current player's animation.
        /// </summary>
        public byte CurrentAnimation { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether or not the player's data can be synced with the server.
        /// </summary>
        public bool IsAllowed { get; set; }
    }
}
