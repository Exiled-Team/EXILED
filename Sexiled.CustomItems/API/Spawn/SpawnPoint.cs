// -----------------------------------------------------------------------
// <copyright file="SpawnPoint.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

using Sexiled.CustomItems.API.Features;

namespace Sexiled.CustomItems.API.Spawn
{
    using Sexiled.CustomItems.API.Features;

    /// <summary>
    /// Defines item spawn properties.
    /// </summary>
    public abstract class SpawnPoint
    {
        /// <summary>
        /// Gets or sets this spawn point name.
        /// </summary>
        public abstract string Name { get; set; }

        /// <summary>
        /// Gets or sets the spawn chance.
        /// </summary>
        public abstract float Chance { get; set; }

        /// <summary>
        /// Gets or sets this spawn point position.
        /// </summary>
        public abstract Vector Position { get; set; }
    }
}
