// -----------------------------------------------------------------------
// <copyright file="StaticSpawnPoint.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomItems.API.Spawn
{
    using Exiled.CustomItems.API.Features;

    /// <summary>
    /// Handles static spawn locations.
    /// </summary>
    public class StaticSpawnPoint : SpawnPoint
    {
        /// <summary>
        /// Gets or sets this spawn location's name.
        /// </summary>
        public override string Name { get; set; }

        /// <summary>
        /// Gets or sets this spawn location's spawn chance.
        /// </summary>
        public override float Chance { get; set; }

        /// <summary>
        /// Gets or sets this spawn location's <see cref="Vector"/>.
        /// </summary>
        public override Vector Position { get; set; }
    }
}
