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
        /// Gets or sets the name of this spawn location.
        /// </summary>
        public abstract string Name { get; set; }

        /// <summary>
        /// Gets or sets a value indicating the spawn chance in this location.
        /// </summary>
        public abstract float Chance { get; set; }

        /// <summary>
        /// Gets or sets a value indicating where in the map to spawn the item.
        /// </summary>
        public abstract Vector Position { get; set; }
    }
}
