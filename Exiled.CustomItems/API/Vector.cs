// -----------------------------------------------------------------------
// <copyright file="Vector.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomItems.API
{
    using UnityEngine;

    /// <summary>
    /// A yaml-serializable vector object.
    /// </summary>
    public readonly struct Vector
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Vector"/> struct.
        /// </summary>
        /// <param name="x">X coordinates.</param>
        /// <param name="y">Y coordinates.</param>
        /// <param name="z">Z coordinates.</param>
        public Vector(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        /// <summary>
        /// Gets a value indicating the x axis coordinates.
        /// </summary>
        public float X { get; }

        /// <summary>
        /// Gets a value indicating the y axis coordinates.
        /// </summary>
        public float Y { get; }

        /// <summary>
        /// Gets a value indicating the z axis coordinates.
        /// </summary>
        public float Z { get; }

        /// <summary>
        /// Gets a conversion this into a <see cref="Vector3"/>.
        /// </summary>
        /// <returns>The converted <see cref="Vector3"/>.</returns>
        public Vector3 ToVector3 => new Vector3(X, Y, Z);
    }
}
