// -----------------------------------------------------------------------
// <copyright file="StaticSpawnPoint.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Spawn
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

        /// <summary>
        /// Deconstructs the class into usable variables.
        /// </summary>
        /// <param name="chance"><inheritdoc cref="Chance"/></param>
        /// <param name="position"><inheritdoc cref="Position"/></param>
        public void Deconstruct(out float chance, out Vector3 position)
        {
            chance = Chance;
            position = Position;
        }
    }
}
