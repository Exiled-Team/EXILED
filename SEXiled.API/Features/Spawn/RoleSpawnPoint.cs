// -----------------------------------------------------------------------
// <copyright file="RoleSpawnPoint.cs" company="SEXiled Team">
// Copyright (c) SEXiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace SEXiled.API.Features.Spawn
{
    using System;

    using SEXiled.API.Extensions;

    using UnityEngine;

    using YamlDotNet.Serialization;

    /// <summary>
    /// Defines a spawn point that follows a base-game role spawn point.
    /// </summary>
    public class RoleSpawnPoint : SpawnPoint
    {
        /// <summary>
        /// Gets or sets the role type used for this spawn.
        /// </summary>
        public RoleType Role { get; set; }

        /// <inheritdoc/>
        public override float Chance { get; set; }

        /// <inheritdoc/>
        [YamlIgnore]
        public override string Name
        {
            get => Role.ToString();
            set => throw new InvalidOperationException("You cannot change the name of this type of SpawnPoint.");
        }

        /// <inheritdoc/>
        [YamlIgnore]
        public override Vector3 Position
        {
            get => Role.GetRandomSpawnProperties().Item1;
            set => throw new InvalidOperationException("You cannot change the position of this type of SpawnPoint.");
        }
    }
}
