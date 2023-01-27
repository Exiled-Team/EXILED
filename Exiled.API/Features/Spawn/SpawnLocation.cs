// -----------------------------------------------------------------------
// <copyright file="SpawnLocation.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Spawn
{
    using System;

    using Exiled.API.Interfaces;
    using PlayerRoles;
    using UnityEngine;

    /// <summary>
    /// Represents a spawn location for a <see cref="Roles.Role"/>.
    /// </summary>
    public class SpawnLocation : IPosition
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SpawnLocation"/> class.
        /// </summary>
        /// <param name="roleType">The <see cref="RoleTypeId"/> this spawn is for.</param>
        /// <param name="position">The <see cref="Vector3"/> position of the spawn.</param>
        /// <param name="rotationMin">The minimum horizontal rotation of the spawn.</param>
        /// <param name="rotationMax">The maximum horizontal rotation of the spawn.</param>
        public SpawnLocation(RoleTypeId roleType, Vector3 position, float rotationMin, float rotationMax)
        {
            RoleType = roleType;
            Position = position;
            MinHorizontalRotation = rotationMin;
            MaxHorizontalRotation = rotationMax;
        }

        /// <summary>
        /// Gets the <see cref="RoleTypeId"/> the spawn is for.
        /// </summary>
        public RoleTypeId RoleType { get; }

        /// <summary>
        /// Gets the position of the spawn.
        /// </summary>
        public Vector3 Position { get; }

        /// <summary>
        /// Gets the aproximative horizontal rotation of this spawn.
        /// </summary>
        public float HorizontalRotation => MaxHorizontalRotation - MinHorizontalRotation;

        /// <summary>
        /// Gets the minimum horizontal rotation of this spawn.
        /// </summary>
        public float MinHorizontalRotation { get; }

        /// <summary>
        /// Gets the maximum horizontal rotation of this spawn.
        /// </summary>
        public float MaxHorizontalRotation { get; }
    }
}
