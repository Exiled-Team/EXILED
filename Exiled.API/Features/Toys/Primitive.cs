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
    /// A wrapper class for <see cref="AdminToys.PrimitiveObjectToy"/>.
    /// </summary>
    public class Primitive
    {
        private bool collidable = true;

        /// <summary>
        /// Initializes a new instance of the <see cref="Primitive"/> class.
        /// </summary>
        public Primitive()
        {
            Base = UnityEngine.Object.Instantiate(ToysHelper.PrimitiveBaseObject);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Primitive"/> class from a <see cref="AdminToys.PrimitiveObjectToy"/>.
        /// </summary>
        /// <param name="toy">The <see cref="AdminToys.PrimitiveObjectToy"/> to be wrapped.</param>
        public Primitive(PrimitiveObjectToy toy)
        {
            Base = toy;

            var position = toy.transform.position;
            Collidable = position.x < 0 && position.y < 0 && position.z < 0;
        }

        /// <summary>
        /// Gets the base <see cref="AdminToys.PrimitiveObjectToy"/>.
        /// </summary>
        public PrimitiveObjectToy Base { get; }

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
            get => Base.transform.position;
            set => Base.transform.position = value;
        }

        /// <summary>
        /// Gets or sets the rotation of the primitive.
        /// </summary>
        public Quaternion Rotation
        {
            get => Base.transform.rotation;
            set => Base.transform.rotation = value;
        }

        /// <summary>
        /// Gets or sets the scale of the primitive.
        /// </summary>
        public Vector3 Scale
        {
            get => Base.transform.localScale;
            set
            {
                Base.transform.localScale = value;

                RefreshCollidable();
            }
        }

        /// <summary>
        /// Gets or sets the amount of movement smoothening on the primitive.
        /// <para>Higher values mean less smooth movement, and 60 is an ideal value.</para>
        /// </summary>
        public byte Smoothing
        {
            get => Base.NetworkMovementSmoothing;
            set => Base.NetworkMovementSmoothing = value;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the primitive can be collided with.
        /// </summary>
        public bool Collidable
        {
            get => collidable;
            set
            {
                collidable = value;
                RefreshCollidable();
            }
        }

        /// <summary>
        /// Spawns the primitive.
        /// </summary>
        public void Spawn()
        {
            var transform = Base.transform;
            transform.position = Position;
            transform.rotation = Rotation;

            RefreshCollidable();

            NetworkServer.Spawn(Base.gameObject);
        }

        /// <summary>
        /// Removes the primitive from the game. Use <see cref="Spawn"/> to bring it back.
        /// </summary>
        public void UnSpawn()
        {
            NetworkServer.UnSpawn(Base.gameObject);
        }

        private void RefreshCollidable()
        {
            if (Collidable)
            {
                Base.transform.localScale = new Vector3(Math.Abs(Scale.x), Math.Abs(Scale.y), Math.Abs(Scale.z));
            }
            else
            {
                Base.transform.localScale = new Vector3(-Math.Abs(Scale.x), -Math.Abs(Scale.y), -Math.Abs(Scale.z));
            }
        }
    }
}
