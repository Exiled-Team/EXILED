// -----------------------------------------------------------------------
// <copyright file="CustomItemSpawn.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomItems.API
{
    using UnityEngine;

    /// <summary>
    /// Handles custom item spawn locations.
    /// </summary>
    public abstract class CustomItemSpawn
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CustomItemSpawn"/> class.
        /// </summary>
        /// <param name="pos">The <see cref="Vector3"/> to spawn the item.</param>
        /// <param name="chance">The spawn chance for this location.</param>
        /// <param name="name">The <see cref="string"/> name of the item.</param>
        protected CustomItemSpawn(Vector3 pos, float chance, string name)
        {
            Position = pos;
            Chance = chance;
            Name = name;
        }

        /// <summary>
        /// Gets a value indicating where in the map to spawn the item.
        /// </summary>
        public Vector3 Position { get; }

        /// <summary>
        /// Gets a value indicating the spawn chance in this location.
        /// </summary>
        public float Chance { get; }

        /// <summary>
        /// Gets the name of this spawn location.
        /// </summary>
        public string Name { get; }
    }
}
