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
        private PrimitiveObjectToy toy = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="Primitive"/> class.
        /// </summary>
        public Primitive()
        {
            toy = UnityEngine.Object.Instantiate(ToysHelper.BaseObject);
        }

        /// <summary>
        /// Gets the base <see cref="PrimitiveObjectToy"/>.
        /// </summary>
        public PrimitiveObjectToy Base => toy;

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

            NetworkServer.Spawn(toy.gameObject);
        }
    }
}
