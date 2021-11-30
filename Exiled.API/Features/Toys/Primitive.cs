// -----------------------------------------------------------------------
// <copyright file="Primitive.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Toys
{
    using System;
    using AdminToys;
    using Mirror;
    using UnityEngine;

    /// <summary>
    /// A wrapper class for primitive Admin Toys.
    /// </summary>
    public class Primitive
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Primitive"/> class.
        /// </summary>
        public Primitive()
        {
            Base = UnityEngine.Object.Instantiate(ToysHelper.PrimitiveBaseObject);
        }

        /// <summary>
        /// Gets the base <see cref="PrimitiveObjectToy"/>.
        /// </summary>
        public PrimitiveObjectToy Base { get; } = null;

        /// <summary>
        /// Gets or sets the material color of the primitive.
        /// </summary>
        public Color Color
        {
            get => Base.NetworkMaterialColor;
            set => Base.NetworkMaterialColor = value;
        }

        /// <summary>
        /// Gets or sets the type of the primitive.
        /// </summary>
        public PrimitiveType Type
        {
            get => Base.NetworkPrimitiveType;
            set => Base.NetworkPrimitiveType = value;
        }

        /// <summary>
        /// Gets or sets the position of the primitive.
        /// </summary>
        public Vector3 Position
        {
            get => Base.NetworkPosition;
            set => Base.NetworkPosition = value;
        }

        /// <summary>
        /// Gets or sets the rotation of the primitive.
        /// </summary>
        public Quaternion Rotation
        {
            get => Base.NetworkRotation.Value;
            set => Base.NetworkRotation = new LowPrecisionQuaternion(value);
        }

        /// <summary>
        /// Gets or sets the scale of the primitive.
        /// </summary>
        public Vector3 Scale
        {
            get => Base.NetworkScale;
            set => Base.NetworkScale = value;
        }

        /// <summary>
        /// Gets or sets a value indicating whether gets or sets if the primitive can be collided with.
        /// </summary>
        public bool Collidable { get; set; } = true;

        /// <summary>
        /// Spawns the primitive.
        /// </summary>
        public void Spawn()
        {
            if (!Collidable)
            {
                Scale = new Vector3(-Math.Abs(Scale.x), -Math.Abs(Scale.y), -Math.Abs(Scale.z));
            }
            else
            {
                Scale = new Vector3(Math.Abs(Scale.x), Math.Abs(Scale.y), Math.Abs(Scale.z));
            }

            var transform = Base.transform;
            transform.position = Position;
            transform.rotation = Rotation;
            transform.localScale = Scale;

            NetworkServer.Spawn(Base.gameObject);
        }

        /// <summary>
        /// Removes the primitive from the game. Use <see cref="Spawn"/> to bring it back.
        /// </summary>
        public void Destroy()
        {
            NetworkServer.UnSpawn(Base.gameObject);
        }
    }
}
