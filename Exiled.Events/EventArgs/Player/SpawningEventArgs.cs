// -----------------------------------------------------------------------
// <copyright file="SpawningEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Player
{
    using Exiled.API.Features;
    using Exiled.Events.EventArgs.Interfaces;

    using UnityEngine;

    /// <summary>
    ///     Contains all information before spawning a player(called only when possibly to change position).
    ///     use <see cref="SpawnedEventArgs"/> or <see cref="ChangingRoleEventArgs"/>for all class changes.
    /// </summary>
    public class SpawningEventArgs : IPlayerEvent
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="SpawningEventArgs" /> class.
        /// </summary>
        /// <param name="player">
        ///     <inheritdoc cref="Player" />
        /// </param>
        /// <param name="position">
        ///     <inheritdoc cref="Position" />
        /// </param>
        public SpawningEventArgs(Player player, Vector3 position)
        {
            Player = player;
            Position = position;
        }

        /// <summary>
        ///     Gets the spawning <see cref="Player"/>.
        /// </summary>
        public Player Player { get; }

        /// <summary>
        ///     Gets or sets the <see cref="Player"/>'s spawning position.
        /// </summary>
        public Vector3 Position { get; set; }
    }
}