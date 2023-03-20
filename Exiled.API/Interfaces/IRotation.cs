// -----------------------------------------------------------------------
// <copyright file="IRotation.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Interfaces
{
    using UnityEngine;

    /// <summary>
    /// Represents an object with a <see cref="Quaternion"/> rotation.
    /// </summary>
    public interface IRotation
    {
        /// <summary>
        /// Gets the rotation of this object.
        /// </summary>
        public Quaternion Rotation { get; }
    }
}
