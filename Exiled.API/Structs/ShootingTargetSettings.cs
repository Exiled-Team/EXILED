// -----------------------------------------------------------------------
// <copyright file="ShootingTargetSettings.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Structs
{
    using Exiled.API.Enums;
    using Exiled.API.Features.Toys;
    using UnityEngine;

    /// <summary>
    /// Settings for shooting targets.
    /// </summary>
    public struct ShootingTargetSettings
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ShootingTargetSettings"/> struct.
        /// </summary>
        /// <param name="targetType">The type of the shooting target.</param>
        /// <param name="position">The position of the shooting target.</param>
        /// <param name="rotation">The rotation of the shooting target.</param>
        /// <param name="scale">The scale of the shooting target.</param>
        /// <param name="spawn">Whether or not the shooting target should be spawned.</param>
        public ShootingTargetSettings(ShootingTargetType targetType, Vector3? position = null, Quaternion? rotation = null, Vector3? scale = null, bool spawn = true)
        {
            ShootingTargetType = targetType;
            Position = position ?? Vector3.one;
            Rotation = rotation ?? Quaternion.identity;
            Scale = scale ?? Vector3.one;
            ShouldSpawn = spawn;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ShootingTargetSettings"/> struct.
        /// </summary>
        /// <param name="shootingTarget">The shooting target to copy properties of.</param>
        public ShootingTargetSettings(ShootingTargetToy shootingTarget)
        {
            ShootingTargetType = shootingTarget.Type;
            Position = shootingTarget.Position;
            Rotation = shootingTarget.Rotation;
            Scale = shootingTarget.Scale;
            ShouldSpawn = true;
        }

        /// <summary>
        /// Gets or sets the shooting target type.
        /// </summary>
        public ShootingTargetType ShootingTargetType { get; set;  }

        /// <summary>
        /// Gets or sets the shooting target position.
        /// </summary>
        public Vector3 Position { get; set; }

        /// <summary>
        /// Gets or sets the shooting target rotation.
        /// </summary>
        public Quaternion Rotation { get; set; }

        /// <summary>
        /// Gets or sets the shooting target scale.
        /// </summary>
        public Vector3 Scale { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether or not the shooting target should be spawned.
        /// </summary>
        public bool ShouldSpawn { get; set; }
    }
}