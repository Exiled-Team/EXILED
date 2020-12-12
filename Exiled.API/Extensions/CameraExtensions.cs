// -----------------------------------------------------------------------
// <copyright file="CameraExtensions.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Extensions
{
    using System.Collections.Generic;

    using Exiled.API.Enums;
    using Exiled.API.Features;

    /// <summary>
    /// Contains an extension method to get <see cref="CameraType"/> from <see cref="Camera079"/>, as well as additional methods to get the <see cref="Room"/> and <see cref="ZoneType"/> of a camera.
    /// Internal class <see cref="RegisterCameraTypesOnLevelLoad"/> to cache the <see cref="CameraType"/> on level load.
    /// </summary>
    public static class CameraExtensions
    {
        private static readonly Dictionary<int, CameraType> OrderedCameraTypes = new Dictionary<int, CameraType>();

        /// <summary>
        /// Returns the <see cref="Room"/> the camera is in.
        /// </summary>
        /// <param name="camera">The <see cref="Camera079"/> to check.</param>
        /// <returns>A <see cref="Room"/>.</returns>
        public static Room GetRoom(this Camera079 camera) => Map.FindParentRoom(camera.gameObject);

        /// <summary>
        /// Gets the <see cref="CameraType"/>.
        /// </summary>
        /// <param name="camera">The <see cref="Camera079"/> to check.</param>
        /// <returns>The <see cref="CameraType"/>.</returns>
        public static CameraType Type(this Camera079 camera) => OrderedCameraTypes.TryGetValue(camera.GetInstanceID(), out var cameraType) ? cameraType : CameraType.Unknown;

        /// <summary>
        /// Returns the <see cref="ZoneType"/> the camera is in.
        /// </summary>
        /// <param name="camera">The <see cref="Camera079"/> to check.</param>
        /// <returns>The <see cref="ZoneType"/> of the zone the camera is in.</returns>
        public static ZoneType GetZone(this Camera079 camera) => GetRoom(camera).Zone;

        /// <summary>
        /// Gets all the <see cref="CameraType"/> values for for the <see cref="Camera079"/> instances using <see cref="Camera079.cameraId"/> and <see cref="UnityEngine.GameObject"/> name.
        /// </summary>
        internal static void RegisterCameraTypesOnLevelLoad()
        {
            OrderedCameraTypes.Clear();

            var cameras = Map.Cameras;

            if (cameras == null)
                return;

            var cameraCount = cameras.Count;
            for (int i = 0; i < cameraCount; i++)
            {
                var camera = cameras[i];
                var cameraID = camera.GetInstanceID();

                var cameraType = (CameraType)cameraID;

                OrderedCameraTypes.Add(cameraID, cameraType);
            }
        }
    }
}
