// -----------------------------------------------------------------------
// <copyright file="SpawnProperties.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomItems.API
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
        public int Limit { get; set; }

        /// <summary>
        /// Gets or sets a <see cref="List{T}"/> of possible dynamic spawn locations.
        /// </summary>
        public List<DynamicItemSpawn> DynamicSpawnLocations { get; set; } = new List<DynamicItemSpawn>();

        /// <summary>
        /// Gets or sets a <see cref="List{T}"/> of possible static spawn locations.
        /// </summary>
        public List<StaticItemSpawn> StaticSpawnLocations { get; set; } = new List<StaticItemSpawn>();
    }
}
