// -----------------------------------------------------------------------
// <copyright file="RoleSpawnPoint.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Spawn
{
    using System;
    using System.Diagnostics;

    using Extensions;

    using PlayerRoles;

    using UnityEngine;

    using YamlDotNet.Serialization;

    /// <summary>
    /// Defines a spawn point that follows a base-game role spawn point.
    /// </summary>
    [DebuggerDisplay("RoleSpawn Name = {Name} Chance = {Chance} Position = {Position}")]
    public class RoleSpawnPoint : SpawnPoint
    {
        /// <summary>
        /// Gets or sets the role type used for this spawn.
        /// </summary>
        public RoleTypeId Role { get; set; }

        /// <inheritdoc/>
        public override float Chance { get; set; }

        /// <inheritdoc/>
        [YamlIgnore]
        public override string Name
        {
            get => Role.ToString();
            set => throw new InvalidOperationException("The name of this type of SpawnPoint cannot be changed.");
        }

        /// <inheritdoc/>
        [YamlIgnore]
        public override Vector3 Position
        {
            get => Role.GetRandomSpawnLocation().Position;
            set => throw new InvalidOperationException("The position of this type of SpawnPoint cannot be changed.");
        }
    }
}