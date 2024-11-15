// -----------------------------------------------------------------------
// <copyright file="SpawnProperties.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Spawn
{
    using System.Collections.Generic;
    using System.ComponentModel;

    using YamlDotNet.Serialization;

    /// <summary>
    /// Handles special properties of spawning an entity.
    /// </summary>
    public class SpawnProperties
    {
        /// <summary>
        /// Gets or sets a value indicating how many items can be spawned when the round starts.
        /// </summary>
        [Description("Indicates how many items can be spawned when the round starts.")]
        public uint Limit { get; set; }

        /// <summary>
        /// Gets or sets a <see cref="List{T}"/> of possible dynamic spawn points.
        /// </summary>
        [Description("A list of possible dynamic spawn points.")]
        public List<DynamicSpawnPoint> DynamicSpawnPoints { get; set; } = new();

        /// <summary>
        /// Gets or sets a <see cref="List{T}"/> of possible static spawn points.
        /// </summary>
        [Description("A list of possible static spawn points.")]
        public List<StaticSpawnPoint> StaticSpawnPoints { get; set; } = new();

        /// <summary>
        /// Gets or sets a <see cref="List{T}"/> of possible role-based spawn points.
        /// </summary>
        [Description("A list of possible role-based spawn points.")]
        public List<RoleSpawnPoint> RoleSpawnPoints { get; set; } = new();

        /// <summary>
        /// Gets a value indicating whether spawn points count is zero.
        /// </summary>
        [YamlIgnore]
        public bool IsEmpty => Length == 0;

        /// <summary>
        /// Gets the amount of spawn points in this instance.
        /// </summary>
        /// <returns>The amount of existing spawn points.</returns>
        [YamlIgnore]
        public int Length => DynamicSpawnPoints.Count + StaticSpawnPoints.Count + RoleSpawnPoints.Count;
    }
}