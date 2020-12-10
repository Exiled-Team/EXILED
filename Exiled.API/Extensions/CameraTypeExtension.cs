// -----------------------------------------------------------------------
// <copyright file="CameraTypeExtension.cs" company="Exiled Team">
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
    /// Contains an extension method to get <see cref="CameraType"/> from <see cref="Camera079"/>.
    /// Internal class <see cref="RegisterCameraTypesOnLevelLoad"/> to cache the <see cref="CameraType"/> on level load.
    /// </summary>
    public static class CameraTypeExtension
    {
        private static readonly Dictionary<int, CameraType> OrderedCameraTypes = new Dictionary<int, CameraType>();

        /// <summary>
        /// Gets the <see cref="CameraType"/>.
        /// </summary>
        /// <param name="camera">The <see cref="Camera079"/> to check.</param>
        /// <returns>The <see cref="CameraType"/>.</returns>
        public static CameraType Type(this Camera079 camera) => OrderedCameraTypes.TryGetValue(camera.GetInstanceID(), out var cameraType) ? cameraType : CameraType.Unknown;

        /// <summary>
        /// Gets all the <see cref="CameraType"/> values for for the <see cref="Camera079"/> instances using <see cref="Camera079.cameraId"/> and <see cref="UnityEngine.GameObject"/> name.
        /// </summary>
        internal static void RegisterCameraTypesOnLevelLoad()
        {
            OrderedCameraTypes.Clear();

            var cameras = Map.Cameras;

            if (cameras == null)
                return;

            var cameraCount = cameras.Length;
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
