// -----------------------------------------------------------------------
// <copyright file="PrimitiveSettings.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Structs
{
    using AdminToys;
    using Exiled.API.Features.Toys;
    using UnityEngine;

    /// <summary>
    /// Settings for primitives.
    /// </summary>
    public struct PrimitiveSettings
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PrimitiveSettings"/> struct.
        /// </summary>
        /// <param name="primitiveType">The type of the primitive.</param>
        /// <param name="flags">The <see cref="PrimitiveFlags"/> to apply.</param>
        /// <param name="color">The color of the primitive.</param>
        /// <param name="position">The position of the primitive.</param>
        /// <param name="rotation">The rotation of the primitive.</param>
        /// <param name="scale">The scale of the primitive.</param>
        /// <param name="spawn">Whether or not the primitive should be spawned.</param>
        /// <param name="isStatic">Whether or not the primitive should be static.</param>
        public PrimitiveSettings(PrimitiveType primitiveType, Color? color, Vector3? position, PrimitiveFlags flags = PrimitiveFlags.Visible | PrimitiveFlags.Collidable, Quaternion? rotation = null, Vector3? scale = null, bool isStatic = false, bool spawn = true)
        {
            PrimitiveType = primitiveType;
            Color = color ?? Color.gray;
            Position = position ?? Vector3.one;
            Flags = flags;
            Rotation = rotation ?? Quaternion.identity;
            Scale = scale ?? Vector3.one;
            IsStatic = isStatic;
            ShouldSpawn = spawn;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PrimitiveSettings"/> struct.
        /// </summary>
        /// <param name="primitive">The primitive to copy properties of.</param>
        public PrimitiveSettings(Primitive primitive)
        {
            PrimitiveType = primitive.Type;
            Flags = primitive.Flags;
            Color = primitive.Color;
            Position = primitive.Position;
            Rotation = primitive.Rotation;
            Scale = primitive.Scale;
            IsStatic = primitive.IsStatic;
            ShouldSpawn = true;
        }

        /// <summary>
        /// Gets or sets the primitive type.
        /// </summary>
        public PrimitiveType PrimitiveType { get; set;  }

        /// <summary>
        /// Gets or sets the primitive flags.
        /// </summary>
        public PrimitiveFlags Flags { get; set;  }

        /// <summary>
        /// Gets or sets the primitive color.
        /// </summary>
        public Color Color { get; set; }

        /// <summary>
        /// Gets or sets the primitive position.
        /// </summary>
        public Vector3 Position { get; set; }

        /// <summary>
        /// Gets or sets the primitive rotation.
        /// </summary>
        public Quaternion Rotation { get; set; }

        /// <summary>
        /// Gets or sets the primitive scale.
        /// </summary>
        public Vector3 Scale { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether or not the primitive should be spawned.
        /// </summary>
        public bool IsStatic { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether or not the primitive should be spawned.
        /// </summary>
        public bool ShouldSpawn { get; set; }
    }
}