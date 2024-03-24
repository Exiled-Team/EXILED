// -----------------------------------------------------------------------
// <copyright file="SpawnProperties.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Spawn
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Handles special properties of spawning an entity.
    /// </summary>
    public class SpawnProperties
    {
        /// <summary>
        /// Gets or sets a value indicating how many items can be spawned when the round starts.
        /// </summary>
        public uint Limit { get; set; }

        /// <summary>
        /// Gets or sets a <see cref="List{T}"/> of possible dynamic spawn points.
        /// </summary>
        public List<DynamicSpawnPoint> DynamicSpawnPoints { get; set; } = new();

        /// <summary>
        /// Gets or sets a <see cref="List{T}"/> of possible static spawn points.
        /// </summary>
        public List<StaticSpawnPoint> StaticSpawnPoints { get; set; } = new();

        /// <summary>
        /// Gets or sets a <see cref="List{T}"/> of possible role-based spawn points.
        /// </summary>
        public List<RoleSpawnPoint> RoleSpawnPoints { get; set; } = new();

        /// <summary>
        /// Gets a value indicating whether spawn points count is zero.
        /// </summary>
        public bool IsEmpty => Length == 0;

        /// <summary>
        /// Gets the amount of spawn points in this instance.
        /// </summary>
        /// <returns>The amount of existing spawn points.</returns>
        public int Length => DynamicSpawnPoints.Count + StaticSpawnPoints.Count + RoleSpawnPoints.Count;
    }
}