// -----------------------------------------------------------------------
// <copyright file="SpawnLocation.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Spawn
{
    using System.Diagnostics;

    using Exiled.API.Interfaces;
    using PlayerRoles;

    using UnityEngine;

    /// <summary>
    /// Represents a spawn location for a <see cref="Roles.Role"/>.
    /// </summary>
    [DebuggerDisplay("RoleType = {RoleType} Position = {Position}")]
    public class SpawnLocation : IPosition
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SpawnLocation"/> class.
        /// </summary>
        /// <param name="roleType">The <see cref="RoleTypeId"/> this spawn is for.</param>
        /// <param name="position">The <see cref="Vector3"/> position of the spawn.</param>
        /// <param name="horizontalRotation">The horizontal rotation of the spawn.</param>
        public SpawnLocation(RoleTypeId roleType, Vector3 position, float horizontalRotation)
        {
            RoleType = roleType;
            Position = position;
            HorizontalRotation = horizontalRotation;
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
        /// Gets the horizontal rotation of the spawn.
        /// </summary>
        public float HorizontalRotation { get; }
    }
}
