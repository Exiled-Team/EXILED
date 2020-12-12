// -----------------------------------------------------------------------
// <copyright file="CameraExtensions.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Extensions
{
    using System;
    using System.Linq;

    using Exiled.API.Enums;
    using Exiled.API.Features;

    using UnityEngine;

    /// <summary>
    /// A set of extensions for <see cref="ItemType"/>.
    /// </summary>
    public static class CameraExtensions
    {
        /// <summary>
        /// Returns the <see cref="Room"/> the camera is in.
        /// </summary>
        /// <param name="camera">The <see cref="Camera079"/> to check.</param>
        /// <returns>A <see cref="Room"/>.</returns>
        public static Room GetRoom(this Camera079 camera) => Map.FindParentRoom(camera.gameObject);

        /// <summary>
        /// Returns the <see cref="ZoneType"/> the camera is in.
        /// </summary>
        /// <param name="camera">The <see cref="Camera079"/> to check.</param>
        /// <returns>The <see cref="ZoneType"/> of the zone the camera is in.</returns>
        public static ZoneType GetZone(this Camera079 camera) => GetRoom(camera).Zone;
    }
}
