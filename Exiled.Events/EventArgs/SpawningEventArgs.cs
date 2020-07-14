// -----------------------------------------------------------------------
// <copyright file="SpawningEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs
{
    using System;
    using Exiled.API.Features;
    using UnityEngine;

    /// <summary>
    /// Contains all informations before spawning a player.
    /// </summary>
    public class SpawningEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SpawningEventArgs"/> class.
        /// </summary>
        /// <param name="player"><inheritdoc cref="Player"/></param>
        /// <param name="roleType"><inheritdoc cref="RoleType"/></param>
        /// <param name="position"><inheritdoc cref="Position"/></param>
        /// <param name="rotationY"><inheritdoc cref="RotationY"/></param>
        public SpawningEventArgs(Player player, RoleType roleType, Vector3 position, float rotationY)
        {
            Player = player;
            RoleType = roleType;
            Position = position;
            RotationY = rotationY;
        }

        /// <summary>
        /// Gets the spawning player.
        /// </summary>
        public Player Player { get; private set; }

        /// <summary>
        /// Gets the player role type.
        /// </summary>
        public RoleType RoleType { get; private set; }

        /// <summary>
        /// Gets or sets the player's spawning position.
        /// </summary>
        public Vector3 Position { get; set; }

        /// <summary>
        /// Gets or sets the rotation y axis of the player.
        /// </summary>
        public float RotationY { get; set; }
    }
}