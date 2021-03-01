// -----------------------------------------------------------------------
// <copyright file="SpawnProperties.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomItems.API.Spawn
{
    using System.Collections.Generic;

    /// <summary>
    /// Handles special properties of spawning an item.
    /// </summary>
    public class SpawnProperties
    {
        /// <summary>
        /// Gets or sets a value indicating how many of the item can be spawned when the round starts.
        /// </summary>
        public uint Limit { get; set; }

        /// <summary>
        /// Gets or sets a <see cref="List{T}"/> of possible dynamic spawn points.
        /// </summary>
        public List<DynamicSpawnPoint> DynamicSpawnPoints { get; set; } = new List<DynamicSpawnPoint>();

        /// <summary>
        /// Gets or sets a <see cref="List{T}"/> of possible static spawn points.
        /// </summary>
        public List<StaticSpawnPoint> StaticSpawnPoints { get; set; } = new List<StaticSpawnPoint>();
    }
}
