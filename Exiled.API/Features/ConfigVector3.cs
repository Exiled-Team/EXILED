// -----------------------------------------------------------------------
// <copyright file="ConfigVector3.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features
{
    using UnityEngine;

    using YamlDotNet.Serialization;

    /// <summary>
    /// A helper class used for storing <see cref="Vector3"/>s in config.
    /// </summary>
    public class ConfigVector3
    {
        private Vector3 value = default;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigVector3"/> class.
        /// </summary>
        /// <param name="x">The x coordinate.</param>
        /// <param name="y">The y coordinate.</param>
        /// <param name="z">The z coordinate.</param>
        public ConfigVector3(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigVector3"/> class.
        /// </summary>
        /// <param name="vector">The <see cref="Vector3"/> containing the coordinates.</param>
        public ConfigVector3(Vector3 vector)
        {
            X = vector.x;
            Y = vector.y;
            Z = vector.z;
        }

        /// <summary>
        /// Gets the x value of the <see cref="Vector3"/>.
        /// </summary>
        public float X { get; private set; }

        /// <summary>
        /// Gets the y value of the <see cref="Vector3"/>.
        /// </summary>
        public float Y { get; private set; }

        /// <summary>
        /// Gets the z value of the <see cref="Vector3"/>.
        /// </summary>
        public float Z { get; private set; }

        /// <summary>
        /// Gets the <see cref="Vector3"/>.
        /// </summary>
        [YamlIgnore]
        public Vector3 Value
        {
            get
            {
                if (_value == default)
                    _value = new Vector3(X, Y, Z);

                return _value;
            }
        }
    }
}
