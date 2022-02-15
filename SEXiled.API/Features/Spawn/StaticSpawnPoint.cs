// -----------------------------------------------------------------------
// <copyright file="StaticSpawnPoint.cs" company="SEXiled Team">
// Copyright (c) SEXiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace SEXiled.API.Features.Spawn
{
    using UnityEngine;

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
        public override Vector3 Position { get; set; }
    }
}
