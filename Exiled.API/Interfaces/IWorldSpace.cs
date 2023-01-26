// -----------------------------------------------------------------------
// <copyright file="IWorldSpace.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Interfaces
{
    using UnityEngine;

    /// <summary>
    /// Represents an object with a <see cref="Vector3"/> position and a <see cref="Quaternion"/> rotation.
    /// </summary>
    public interface IWorldSpace : IPosition, IRotation
    {
    }
}
