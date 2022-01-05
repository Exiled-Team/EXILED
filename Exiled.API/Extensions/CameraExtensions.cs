// -----------------------------------------------------------------------
// <copyright file="CameraExtensions.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Extensions
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    using Exiled.API.Enums;
    using Exiled.API.Features;

    /// <summary>
    /// Contains an extension method to get <see cref="CameraType"/> from <see cref="Camera079"/>, as well as additional methods to get the <see cref="Room"/> and <see cref="ZoneType"/> of a camera.
    /// Internal class <see cref="RegisterCameraInfoOnLevelLoad"/> to cache the <see cref="CameraType"/> and <see cref="Room"/> on level load.
    /// </summary>
    public static class CameraExtensions
    {
        // private static readonly Dictionary<int, CameraType> OrderedCameraTypes = new Dictionary<int, CameraType>();
        private static readonly Dictionary<int, Room> OrderedCameraRooms = new Dictionary<int, Room>();
        private static readonly Dictionary<string, CameraType> NameToCameraType = new Dictionary<string, CameraType>
        {
            // Light Containment
            ["173 hallway"] = CameraType.Lcz173Hallway,
            ["173 gunroom"] = CameraType.Lcz173Armory,
            ["914 hallway"] = CameraType.Lcz914Hallway,
            ["airlock"] = CameraType.LczAirlock,
            ["armory"] = CameraType.LczArmory,
            ["d cells"] = CameraType.LczClassDSpawn,
            ["etrcp @ a"] = CameraType.LczBEntrance, // For some reason, this one and the next three are backwards lol
            ["etrcp @ b"] = CameraType.LczAEntrance,
            ["ex @ a"] = CameraType.HczBEntrance,
            ["ex @ b"] = CameraType.HczAEntrance,
            ["glassroom"] = CameraType.LczGlassRoom,
            ["greenhouse"] = CameraType.LczGreenhouse,
            ["lcz @ a"] = CameraType.LczALifts,
            ["lcz @ b"] = CameraType.LczBLifts,
            ["scp-173 stairs"] = CameraType.Lcz173Bottom,
            ["scp-914"] = CameraType.Lcz914,
            ["tc-01 chamber"] = CameraType.Lcz330,
            ["tc-01 hall"] = CameraType.Lcz330Hall,
            ["wc"] = CameraType.WC,

            // Heavy Containment
            ["049 hall 1"] = CameraType.Hcz049Elevator,
            ["049 hall 5"] = CameraType.Hcz049Armory,
            ["106 ent a"] = CameraType.Hcz106Primary,
            ["106 ent b"] = CameraType.Hcz106Secondary,
            ["106 stairway"] = CameraType.Hcz106Stairs,
            ["downservs"] = CameraType.HczServerBottom,
            ["ez entrance"] = CameraType.EzEntrance,
            ["hcz @ a"] = CameraType.HczALifts,
            ["hcz @ b"] = CameraType.HczBLifts,
            ["hcz armory"] = CameraType.HczArmory,
            ["hcz entrance"] = CameraType.HczEntrance,
            ["hallway cam"] = CameraType.Hcz079Hallway,
            ["head armory"] = CameraType.HczWarheadArmory,
            ["head panel"] = CameraType.HczWarheadSwitch,
            ["head top"] = CameraType.HczWarheadRoom,
            ["hid hall"] = CameraType.HczHidHall,
            ["hid interior"] = CameraType.HczHidInterior,
            ["pre-hallway cam"] = CameraType.Hcz079PreHallway,
            ["sacrificer"] = CameraType.Hcz106Recontainer, // i love this camera name
            ["servers"] = CameraType.HczServerTop,
            ["servhall"] = CameraType.HczServerHall,
            ["scp-049 hall"] = CameraType.Hcz049Hall,
            ["scp-079 main cam"] = CameraType.Hcz079Main,
            ["scp-096 cr"] = CameraType.Hcz096,
            ["scp-106 main cam"] = CameraType.Hcz106First,
            ["scp-106 second"] = CameraType.Hcz106Second,
            ["scp-939 cr"] = CameraType.Hcz939,
            ["tesla gate"] = CameraType.HczTeslaGate,
            ["warhead hall"] = CameraType.HczWarheadHall,

            // Entrance Zone
            ["hallway"] = CameraType.EzHall,
            ["icom hall"] = CameraType.EzIntercomHall,
            ["icom room"] = CameraType.EzIntercomInterior,
            ["intercom"] = CameraType.EzIntercomHall,
            ["t intersection"] = CameraType.EzTIntersection,
            ["x intersection"] = CameraType.EzXIntersection,

            // Surface
            ["bridge"] = CameraType.Bridge,
            ["backstreet"] = CameraType.Backstreet,
            ["exit"] = CameraType.Exit,
            ["escape zone"] = CameraType.EscapeZone,
            ["helipad"] = CameraType.Helipad,
            ["streetcam"] = CameraType.Streetcam,
            ["tower"] = CameraType.Tower,

            // Unspecified
            ["corner"] = CameraType.Corner,
            ["x-type inters"] = CameraType.XIntersection,
            ["t-type inters"] = CameraType.TIntersection,
            ["straight"] = CameraType.Hallway,
            ["offices"] = CameraType.Office,
            ["gate a"] = CameraType.GateA,
            ["gate b"] = CameraType.GateB,
        };

        /// <summary>
        /// Returns the <see cref="Room"/> the camera is in, or <see langword="null"/> if not found.
        /// </summary>
        /// <param name="camera">The <see cref="Camera079"/> to check.</param>
        /// <returns>A <see cref="Room"/>, or <see langword="null"/> if not found.</returns>
        public static Room Room(this Camera079 camera) => OrderedCameraRooms.TryGetValue(camera.GetInstanceID(), out Room room) ? room : null;

        /// <summary>
        /// Given the camera, returns the appropriate <see cref="CameraType"/>, based on its <see cref="Camera079.cameraName">name</see> and <see cref="Room.Zone">zone</see>.
        /// </summary>
        /// <param name="camera">The <see cref="Camera079"/> to check.</param>
        /// <returns>The <see cref="CameraType"/>.</returns>
        public static CameraType Type(this Camera079 camera)
        {
            string cameraName = camera.cameraName.ToLower();
            Room room = camera.Room();

            if (room != null)
            {
                if (cameraName == "corner")
                {
                    switch (room.Zone)
                    {
                        case ZoneType.LightContainment:
                            return CameraType.LczCorner;
                        case ZoneType.HeavyContainment:
                            return CameraType.HczCorner;
                        case ZoneType.Entrance:
                            return CameraType.EzCorner;
                    }
                }
                else if (cameraName == "x-type inters")
                {
                    switch (room.Zone)
                    {
                        case ZoneType.LightContainment:
                            return CameraType.LczXIntersection;
                        case ZoneType.HeavyContainment:
                            return CameraType.HczXIntersection;
                    }
                }
                else if (cameraName == "t-type inters")
                {
                    switch (room.Zone)
                    {
                        case ZoneType.LightContainment:
                            return CameraType.LczTIntersection;
                        case ZoneType.HeavyContainment:
                            return CameraType.LczTIntersection;
                    }
                }
                else if (cameraName == "straight")
                {
                    switch (room.Zone)
                    {
                        case ZoneType.LightContainment:
                            return CameraType.LczHall;
                        case ZoneType.HeavyContainment:
                            return CameraType.HczHall;
                    }
                }
                else if (cameraName == "offices")
                {
                    switch (room.Zone)
                    {
                        case ZoneType.LightContainment:
                            return CameraType.LczCafe;
                        case ZoneType.Entrance:
                            return CameraType.EzOffice;
                    }
                }
                else if (cameraName == "gate a")
                {
                    switch (room.Zone)
                    {
                        case ZoneType.Entrance:
                            return CameraType.EzGateA;
                        case ZoneType.Surface:
                            return CameraType.SurfaceGateA;
                    }
                }
                else if (cameraName == "gate b")
                {
                    switch (room.Zone)
                    {
                        case ZoneType.Entrance:
                            return CameraType.EzGateB;
                        case ZoneType.Surface:
                            return CameraType.SurfaceGate;
                    }
                }
            }

            // If it's not a room name shared by multiple zones, or the given room is null, look in the dictionary.
            // Entrance Zone T-halls, X-halls, and straight halls are named differently than LCZ and HCZ.
            if (NameToCameraType.ContainsKey(cameraName))
                return NameToCameraType[cameraName];
            return CameraType.Unknown;
        }

        /// <summary>
        /// Returns the <see cref="ZoneType"/> the camera is in.
        /// </summary>
        /// <param name="camera">The <see cref="Camera079"/> to check.</param>
        /// <returns>The <see cref="ZoneType"/> of the zone the camera is in.</returns>
        public static ZoneType Zone(this Camera079 camera) => Room(camera)?.Zone ?? ZoneType.Unspecified;

        /// <summary>
        /// Gets all the <see cref="CameraType"/> and <see cref="Room"/> values for for the <see cref="Camera079"/> instances using <see cref="Camera079.cameraId"/> and <see cref="UnityEngine.GameObject"/> name.
        /// </summary>
        internal static void RegisterCameraInfoOnLevelLoad()
        {
            // OrderedCameraTypes.Clear();
            ReadOnlyCollection<Camera079> cameras = Map.Cameras;

            int cameraCount = cameras.Count;
            for (int i = 0; i < cameraCount; i++)
            {
                Camera079 camera = cameras[i];
                int cameraID = camera.GetInstanceID();

                CameraType cameraType = (CameraType)cameraID;
                Room room = Map.FindParentRoom(camera.gameObject);

                // if (OrderedCameraTypes.ContainsKey(cameraID))
                //    OrderedCameraTypes.Remove(cameraID);
                if (OrderedCameraRooms.ContainsKey(cameraID))
                    OrderedCameraRooms.Remove(cameraID);

                // OrderedCameraTypes.Add(cameraID, cameraType);
                OrderedCameraRooms.Add(cameraID, room);
            }
        }
    }
}
