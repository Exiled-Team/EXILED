// -----------------------------------------------------------------------
// <copyright file="CameraExtensions.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Extensions {
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    using Exiled.API.Enums;
    using Exiled.API.Features;

    /// <summary>
    /// Contains an extension method to get <see cref="CameraType"/> from <see cref="Camera079"/>, as well as additional methods to get the <see cref="Room"/> and <see cref="ZoneType"/> of a camera.
    /// Internal class <see cref="RegisterCameraInfoOnLevelLoad"/> to cache the <see cref="CameraType"/> and <see cref="Room"/> on level load.
    /// </summary>
    public static class CameraExtensions {
        private static readonly Dictionary<int, CameraType> OrderedCameraTypes = new Dictionary<int, CameraType>();
        private static readonly Dictionary<int, Room> OrderedCameraRooms = new Dictionary<int, Room>();

        /// <summary>
        /// Returns the <see cref="Room"/> the camera is in, or null if not found.
        /// </summary>
        /// <param name="camera">The <see cref="Camera079"/> to check.</param>
        /// <returns>A <see cref="Room"/>, or null if not found.</returns>
        public static Room Room(this Camera079 camera) => OrderedCameraRooms.TryGetValue(camera.GetInstanceID(), out Room room) ? room : null;

        /// <summary>
        /// Gets the <see cref="CameraType"/>.
        /// </summary>
        /// <param name="camera">The <see cref="Camera079"/> to check.</param>
        /// <returns>The <see cref="CameraType"/>.</returns>
        public static CameraType Type(this Camera079 camera) => OrderedCameraTypes.TryGetValue(camera.GetInstanceID(), out CameraType cameraType) ? cameraType : CameraType.Unknown;

        /// <summary>
        /// Returns the <see cref="ZoneType"/> the camera is in.
        /// </summary>
        /// <param name="camera">The <see cref="Camera079"/> to check.</param>
        /// <returns>The <see cref="ZoneType"/> of the zone the camera is in.</returns>
        public static ZoneType Zone(this Camera079 camera) => Room(camera)?.Zone ?? ZoneType.Unspecified;

        /// <summary>
        /// Gets all the <see cref="CameraType"/> and <see cref="Room"/> values for for the <see cref="Camera079"/> instances using <see cref="Camera079.cameraId"/> and <see cref="UnityEngine.GameObject"/> name.
        /// </summary>
        internal static void RegisterCameraInfoOnLevelLoad() {
            OrderedCameraTypes.Clear();

            ReadOnlyCollection<Camera079> cameras = Map.Cameras;

            int cameraCount = cameras.Count;
            for (int i = 0; i < cameraCount; i++) {
                Camera079 camera = cameras[i];
                int cameraID = camera.GetInstanceID();

                CameraType cameraType = (CameraType)cameraID;
                Room room = Map.FindParentRoom(camera.gameObject);

                if (OrderedCameraTypes.ContainsKey(cameraID))
                    OrderedCameraTypes.Remove(cameraID);
                if (OrderedCameraRooms.ContainsKey(cameraID))
                    OrderedCameraRooms.Remove(cameraID);

                OrderedCameraTypes.Add(cameraID, cameraType);
                OrderedCameraRooms.Add(cameraID, room);
            }
        }
    }
}
