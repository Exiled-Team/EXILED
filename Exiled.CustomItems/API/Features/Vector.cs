// -----------------------------------------------------------------------
// <copyright file="Vector.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomItems.API.Features
{
    using System;

    using UnityEngine;

    using YamlDotNet.Serialization;

    /// <summary>
    /// A yaml-serializable vector object.
    /// </summary>
    [Obsolete("Use UnityEngine.Vector3 instead.", false)]
    public struct Vector
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
        public float X { get; private set; }

        /// <summary>
        /// Gets a value indicating the y axis coordinates.
        /// </summary>
        public float Y { get; private set; }

        /// <summary>
        /// Gets a value indicating the z axis coordinates.
        /// </summary>
        public float Z { get; private set; }

        /// <summary>
        /// Gets a conversion this into a <see cref="Vector3"/>.
        /// </summary>
        /// <returns>The converted <see cref="Vector3"/>.</returns>
        [YamlIgnore]
        public Vector3 ToVector3 => new Vector3(X, Y, Z);

        /// <summary>
        /// Overloads the == operator.
        /// </summary>
        /// <param name="left">The left <see cref="Vector"/>.</param>
        /// <param name="right">The right <see cref="Vector"/>.</param>
        /// <returns>Returns a value indicating whether the two <see cref="Vector"/> are equal or not.</returns>
        public static bool operator ==(Vector left, Vector right) => left.X == right.X && left.Y == right.Y && left.Z == right.Z;

        /// <summary>
        /// Overloads the != operator.
        /// </summary>
        /// <param name="left">The left <see cref="Vector"/>.</param>
        /// <param name="right">The right <see cref="Vector"/>.</param>
        /// <returns>Returns a value indicating whether the two <see cref="Vector"/> aren't equal or not.</returns>
        public static bool operator !=(Vector left, Vector right) => !(left == right);

        /// <summary>
        /// Overloads the * operator.
        /// </summary>
        /// <param name="left">The left <see cref="Vector"/>.</param>
        /// <param name="right">The right <see cref="Vector"/>.</param>
        /// <returns>Returns a new <see cref="Vector"/> with the result of the multiplication.</returns>
        public static Vector operator *(Vector left, Vector right) => new Vector(left.X * right.X, left.Y * right.Y, left.Z * right.Z);

        /// <summary>
        /// Overloads the * operator.
        /// </summary>
        /// <param name="left">The left <see cref="Vector"/>.</param>
        /// <param name="right">The right <see cref="float"/>.</param>
        /// <returns>Returns a new <see cref="Vector"/> with the result of the multiplication.</returns>
        public static Vector operator *(Vector left, float right) => new Vector(left.X * right, left.Y * right, left.Z * right);

        /// <summary>
        /// Overloads the * operator.
        /// </summary>
        /// <param name="left">The left <see cref="float"/>.</param>
        /// <param name="right">The right <see cref="Vector"/>.</param>
        /// <returns>Returns a new <see cref="Vector"/> with the result of the multiplication.</returns>
        public static Vector operator *(float left, Vector right) => right * left;

        /// <summary>
        /// Overloads the / operator.
        /// </summary>
        /// <param name="left">The left <see cref="Vector"/>.</param>
        /// <param name="right">The right <see cref="Vector"/>.</param>
        /// <returns>Returns a new <see cref="Vector"/> with the result of the division.</returns>
        public static Vector operator /(Vector left, Vector right)
        {
            if (right.X == 0 || right.Y == 0 || right.Z == 0)
                throw new DivideByZeroException();

            return new Vector(left.X / right.X, left.Y / right.Y, left.Z / right.Z);
        }

        /// <summary>
        /// Overloads the / operator.
        /// </summary>
        /// <param name="left">The left <see cref="Vector"/>.</param>
        /// <param name="right">The right <see cref="float"/>.</param>
        /// <returns>Returns a new <see cref="Vector"/> with the result of the division.</returns>
        public static Vector operator /(Vector left, float right)
        {
            if (right == 0)
                throw new DivideByZeroException();

            return new Vector(left.X / right, left.Y / right, left.Z / right);
        }

        /// <summary>
        /// Overloads the / operator.
        /// </summary>
        /// <param name="left">The left <see cref="Vector"/>.</param>
        /// <param name="right">The right <see cref="Vector"/>.</param>
        /// <returns>Returns a new <see cref="Vector"/> with the result of the division.</returns>
        public static Vector operator /(float left, Vector right) => right / left;

        /// <summary>
        /// Overloads the + operator.
        /// </summary>
        /// <param name="left">The left <see cref="Vector"/>.</param>
        /// <param name="right">The right <see cref="Vector"/>.</param>
        /// <returns>Returns a new <see cref="Vector"/> with the result of the addition.</returns>
        public static Vector operator +(Vector left, Vector right) => new Vector(left.X + right.X, left.Y + right.Y, left.Z + right.Z);

        /// <summary>
        /// Overloads the - operator.
        /// </summary>
        /// <param name="left">The left <see cref="Vector"/>.</param>
        /// <param name="right">The right <see cref="Vector"/>.</param>
        /// <returns>Returns a new <see cref="Vector"/> with the result of the subtraction.</returns>
        public static Vector operator -(Vector left, Vector right) => left + (-right);

        /// <summary>
        /// Overloads the - operator.
        /// </summary>
        /// <param name="right">The right <see cref="Vector"/>.</param>
        /// <returns>Returns a new <see cref="Vector"/> with the result of the subtraction.</returns>
        public static Vector operator -(Vector right) => new Vector(-right.X, -right.Y, -right.Z);

        /// <summary>
        /// Overloads the > operator.
        /// </summary>
        /// <param name="left">The left <see cref="Vector"/>.</param>
        /// <param name="right">The right <see cref="Vector"/>.</param>
        /// <returns>Returns a value indicating if the left <see cref="Vector"/> is greater than the right one or not.</returns>
        public static bool operator >(Vector left, Vector right) => left.X > right.X && left.Y > right.Y && left.Z > right.Z;

        /// <summary>
        /// Overloads the >= operator.
        /// </summary>
        /// <param name="left">The left <see cref="Vector"/>.</param>
        /// <param name="right">The right <see cref="Vector"/>.</param>
        /// <returns>Returns a value indicating if the left <see cref="Vector"/> is greater or equal than the right one or not.</returns>
        public static bool operator >=(Vector left, Vector right) => !(left < right);

        /// <summary>
        /// Overloads the &lt; operator.
        /// </summary>
        /// <param name="left">The left <see cref="Vector"/>.</param>
        /// <param name="right">The right <see cref="Vector"/>.</param>
        /// <returns>Returns a value indicating if the left <see cref="Vector"/> is lower than the right one or not.</returns>
        public static bool operator <(Vector left, Vector right) => left.X < right.X && left.Y < right.Y && left.Z < right.Z;

        /// <summary>
        /// Overloads the &lt;= operator.
        /// </summary>
        /// <param name="left">The left <see cref="Vector"/>.</param>
        /// <param name="right">The right <see cref="Vector"/>.</param>
        /// <returns>Returns a value indicating if the left <see cref="Vector"/> is lower or equal than the right one or not.</returns>
        public static bool operator <=(Vector left, Vector right) => !(left > right);

        /// <inheritdoc/>
        public override bool Equals(object obj) => obj is Vector vector && X == vector.X && Y == vector.Y && Z == vector.Z && ToVector3.Equals(vector.ToVector3);

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            int hashCode = 1943678824;

            hashCode = (hashCode * -1521134295) + X.GetHashCode();
            hashCode = (hashCode * -1521134295) + Y.GetHashCode();
            hashCode = (hashCode * -1521134295) + Z.GetHashCode();
            hashCode = (hashCode * -1521134295) + ToVector3.GetHashCode();

            return hashCode;
        }

        /// <inheritdoc/>
        public override string ToString() => $"({X}, {Y}, {Z})";
    }
}
