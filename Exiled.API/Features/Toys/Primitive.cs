// -----------------------------------------------------------------------
// <copyright file="Primitive.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Toys
{
    using System;
    using System.Linq;
    using AdminToys;
    using Exiled.API.Enums;
    using Exiled.API.Exceptions;
    using UnityEngine;
    using Object = UnityEngine.Object;

    /// <summary>
    /// A wrapper class for <see cref="PrimitiveObjectToy"/>.
    /// </summary>
    public class Primitive : AdminToy
    {
        private bool collidable = true;

        /// <summary>
        /// Initializes a new instance of the <see cref="Primitive"/> class.
        /// </summary>
        /// <param name="toyAdminToyBase">The <see cref="PrimitiveObjectToy"/> of the toy.</param>
        internal Primitive(PrimitiveObjectToy toyAdminToyBase)
            : base(toyAdminToyBase, AdminToyType.PrimitiveObject)
        {
            Base = toyAdminToyBase;

            Vector3 actualScale = Base.transform.localScale;
            Collidable = actualScale.x > 0f || actualScale.y > 0f || actualScale.z > 0f;
        }

        /// <summary>
        /// Gets the base <see cref="PrimitiveObjectToy"/>.
        /// </summary>
        public PrimitiveObjectToy Base { get; }

        /// <summary>
        /// Gets or sets the type of the primitive.
        /// </summary>
        public PrimitiveType Type
        {
            get => Base.NetworkPrimitiveType;
            set => Base.NetworkPrimitiveType = value;
        }

        /// <summary>
        /// Gets or sets the material color of the primitive.
        /// </summary>
        public Color Color
        {
            get => Base.NetworkMaterialColor;
            set => Base.NetworkMaterialColor = value;
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
        /// Creates a new <see cref="Primitive"/>.
        /// </summary>
        /// <param name="position">The position of the <see cref="Primitive"/>.</param>
        /// <param name="rotation">The rotation of the <see cref="Primitive"/>.</param>
        /// <param name="scale">The scale of the <see cref="Primitive"/>.</param>
        /// <param name="spawn">Whether the <see cref="Primitive"/> should be initially spawned.</param>
        /// <returns>The new <see cref="Primitive"/>.</returns>
        public static Primitive Create(Vector3? position = null, Vector3? rotation = null, Vector3? scale = null, bool spawn = true)
        {
            if (!Server.HeavilyModded)
                throw new HeavilyModdedOperationException("Only Heavily Modded servers can spawn Primitives.");

            Primitive primitve = new(Object.Instantiate(ToysHelper.PrimitiveBaseObject));

            primitve.AdminToyBase.transform.position = position ?? Vector3.zero;
            primitve.AdminToyBase.transform.eulerAngles = rotation ?? Vector3.zero;
            primitve.AdminToyBase.transform.localScale = scale ?? Vector3.one;

            if (spawn)
                primitve.Spawn();

            return primitve;
        }

        /// <summary>
        /// Gets the <see cref="Primitive"/> belonging to the <see cref="PrimitiveObjectToy"/>.
        /// </summary>
        /// <param name="primitveObjectToy">The <see cref="PrimitiveObjectToy"/> instance.</param>
        /// <returns>The corresponding <see cref="Primitive"/> instance.</returns>
        public static Primitive Get(PrimitiveObjectToy primitveObjectToy)
        {
            AdminToy adminToy = Map.Toys.FirstOrDefault(x => x.AdminToyBase == primitveObjectToy);
            return adminToy is not null ? adminToy as Primitive : new Primitive(primitveObjectToy);
        }

        private void RefreshCollidable()
        {
            UnSpawn();

            Vector3 actualScale = Scale;

            if (Collidable)
            {
                Base.transform.localScale = new Vector3(Math.Abs(actualScale.x), Math.Abs(actualScale.y), Math.Abs(actualScale.z));
            }
            else
            {
                Base.transform.localScale = new Vector3(-Math.Abs(actualScale.x), -Math.Abs(actualScale.y), -Math.Abs(actualScale.z));
            }

            Spawn();
        }
    }
}
