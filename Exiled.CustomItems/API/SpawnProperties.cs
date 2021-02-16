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
        /// Initializes a new instance of the <see cref="SpawnProperties"/> class.
        /// </summary>
        /// <param name="spawnLocations">The list of <see cref="CustomItemSpawn"/>'s for this item.</param>
        /// <param name="spawnLimit">How many of this item are allowed to spawn on the map.</param>
        public SpawnProperties(List<CustomItemSpawn> spawnLocations, int spawnLimit)
        {
            Limit = spawnLimit;
            SpawnLocations = spawnLocations;
        }

        /// <summary>
        /// Gets a value indicating how many of the item can be spawned when the round starts.
        /// </summary>
        public int Limit { get; }

        /// <summary>
        /// Gets a <see cref="Dictionary{TKey,TValue}"/> of possible spawn locations and their chance to spawn.
        /// </summary>
        public List<CustomItemSpawn> SpawnLocations { get; }
    }
}
