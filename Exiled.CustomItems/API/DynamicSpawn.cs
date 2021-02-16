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
        /// Initializes a new instance of the <see cref="DynamicSpawn"/> class.
        /// </summary>
        /// <param name="pos">The <see cref="Vector3"/> position to spawn the item.</param>
        /// <param name="chance">The spawn chance for this location.</param>
        /// <param name="name">The name of this location.</param>
        public DynamicSpawn(Vector3 pos, float chance, string name)
            : base(pos, chance, name)
        {
        }
    }
}
