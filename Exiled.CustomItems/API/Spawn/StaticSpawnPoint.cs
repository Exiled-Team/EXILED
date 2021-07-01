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
        /// <inheritdoc/>
        public override string Name { get; set; }

        /// <inheritdoc/>
        public override float Chance { get; set; }

        /// <inheritdoc/>
#pragma warning disable CS0618 // Type or member is obsolete
        public override Vector Position { get; set; }
    }
}
