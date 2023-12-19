// -----------------------------------------------------------------------
// <copyright file="StaticSpawnPoint.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Spawn
{
    using System.Diagnostics;

    using UnityEngine;

    /// <summary>
    /// Handles static spawn locations.
    /// </summary>
    [DebuggerDisplay("StaticSpawn Name = {Name} Chance = {Chance} Position = {Position}")]
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