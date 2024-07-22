// -----------------------------------------------------------------------
// <copyright file="LightSettings.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Structs
{
    using UnityEngine;

    /// <summary>
    /// Settings for lights.
    /// </summary>
    public struct LightSettings
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LightSettings"/> struct.
        /// </summary>
        /// <param name="position">The position of the light.</param>
        /// <param name="rotation">The rotation of the light.</param>
        /// <param name="scale">The scale of the light.</param>
        /// <param name="color">The color of the light.</param>
        /// <param name="spawn">Whether or not the light should be spawned.</param>
        public LightSettings(Vector3? position = null, Quaternion? rotation = null, Vector3? scale = null, Color? color = null, bool spawn = true)
        {
            Position = position ?? Vector3.one;
            Rotation = rotation ?? Quaternion.identity;
            Scale = scale ?? Vector3.one;
            Color = color ?? Color.gray;
            ShouldSpawn = spawn;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LightSettings"/> struct.
        /// </summary>
        /// <param name="light">The light to copy properties of.</param>
        public LightSettings(Features.Toys.Light light)
        {
            Position = light.Position;
            Rotation = light.Rotation;
            Scale = light.Scale;
            Color = light.Color;
            ShouldSpawn = true;
        }

        /// <summary>
        /// Gets or sets the light position.
        /// </summary>
        public Vector3 Position { get; set; }

        /// <summary>
        /// Gets or sets the light rotation.
        /// </summary>
        public Quaternion Rotation { get; set; }

        /// <summary>
        /// Gets or sets the light scale.
        /// </summary>
        public Vector3 Scale { get; set; }

        /// <summary>
        /// Gets or sets the light color.
        /// </summary>
        public Color Color { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether or not the light should be spawned.
        /// </summary>
        public bool ShouldSpawn { get; set; }
    }
}