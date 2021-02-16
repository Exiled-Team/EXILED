// -----------------------------------------------------------------------
// <copyright file="DynamicSpawn.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomItems.API
{
    using UnityEngine;

    /// <summary>
    /// Handles dynamic spawn locations.
    /// </summary>
    public class DynamicSpawn : CustomItemSpawn
    {
        /// <summary>
        /// The <see cref="SpawnLocation"/> for this item.
        /// </summary>
        private readonly SpawnLocation location;

        /// <summary>
        /// Initializes a new instance of the <see cref="DynamicSpawn"/> class.
        /// </summary>
        /// <param name="location">The <see cref="SpawnLocation"/> to spawn the item.</param>
        /// <param name="chance">The spawn chance for this location.</param>
        public DynamicSpawn(SpawnLocation location, float chance)
            : base(chance, $"{location}") => this.location = location;

        /// <inheritdoc />
        public override Vector Position
        {
            get
            {
                Vector3 pos = location.TryGetLocation();
                return new Vector(pos.x, pos.y, pos.z);
            }
        }
    }
}
