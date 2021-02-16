// -----------------------------------------------------------------------
// <copyright file="StaticItemSpawn.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomItems.API
{
    using UnityEngine;

    /// <summary>
    /// Handles static spawn locations.
    /// </summary>
    public class StaticItemSpawn : CustomItemSpawn
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StaticItemSpawn"/> class.
        /// </summary>
        /// <param name="location">The <see cref="SpawnLocation"/> for the item.</param>
        /// <param name="chance">The spawn chance for this location.</param>
        public StaticItemSpawn(SpawnLocation location, float chance)
            : base(location.TryGetLocation(), chance, $"{location}")
        {
        }
    }
}
