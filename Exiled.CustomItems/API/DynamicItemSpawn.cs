// -----------------------------------------------------------------------
// <copyright file="DynamicItemSpawn.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomItems.API
{
    using System;
    using UnityEngine;
    using YamlDotNet.Serialization;

    /// <summary>
    /// Handles dynamic spawn locations.
    /// </summary>
    public class DynamicItemSpawn
    {
        /// <summary>
        /// Gets or sets the <see cref="SpawnLocation"/> for this item.
        /// </summary>
        public SpawnLocation Location { get; set; }

        /// <summary>
        /// Gets or sets this location's spawn chance.
        /// </summary>
        public float Chance { get; set; }

        /// <summary>
        /// Gets this location's name.
        /// </summary>
        [YamlIgnore]
        public string Name => Location.ToString();
    }
}
