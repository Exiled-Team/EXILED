// -----------------------------------------------------------------------
// <copyright file="Room.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features
{
    using System.Collections.Generic;
    using System.Linq;

    using Exiled.API.Enums;
    using Exiled.API.Extensions;

    using Interactables.Interobjects.DoorUtils;

    using NorthwoodLib.Pools;

    using UnityEngine;

    /// <summary>
    /// The in-game room.
    /// </summary>
    public class Room : MonoBehaviour
    {
        /// <summary>
        /// Gets the <see cref="Room"/> name.
        /// </summary>
        public string Name => name;

        /// <summary>
        /// Gets the <see cref="Room"/> <see cref="UnityEngine.Transform"/>.
        /// </summary>
        public Transform Transform => transform;

        /// <summary>
        /// Gets the <see cref="Room"/> position.
        /// </summary>
        public Vector3 Position => transform.position;

        /// <summary>
        /// Gets the <see cref="ZoneType"/> in which the room is located.
        /// </summary>
        public ZoneType Zone { get; private set; }

        /// <summary>
        /// Gets the <see cref="RoomType"/>.
        /// </summary>
        public RoomType Type { get; private set; }

        /// <summary>
        /// Gets a <see cref="IEnumerable{T}"/> of <see cref="Player"/> in the <see cref="Room"/>.
        /// </summary>
        public IEnumerable<Player> Players => Player.List.Where(player => player.CurrentRoom.Transform == Transform);

        /// <summary>
        /// Gets a <see cref="IEnumerable{T}"/> of <see cref="Door"/> in the <see cref="Room"/>.
        /// </summary>
        public IEnumerable<Door> Doors { get; private set; }

        /// <summary>
        /// Gets or sets the intensity of the lights in the room.
        /// </summary>
        public float LightIntensity
        {
            get => (float)FlickerableLightController?.Network_lightIntensityMultiplier;
            set => FlickerableLightController.Network_lightIntensityMultiplier = value;
        }

        /// <summary>
        /// Gets or sets the color of the room's lights by changing the warhead color.
        /// </summary>
        public Color Color
        {
            get => (Color)FlickerableLightController.WarheadLightColor;
            set
            {
                FlickerableLightController.WarheadLightColor = value;
                FlickerableLightController.WarheadLightOverride = true;
            }
        }

        /// <summary>
        /// Gets a <see cref="IEnumerable{T}"/> of <see cref="Camera079"/> in the <see cref="Room"/>.
        /// </summary>
        public IEnumerable<Camera079> Cameras { get; private set; }

        /// <summary>
        /// Gets a value indicating whether or not the lights in this room are currently flickered off.
        /// </summary>
        public bool LightsOff => FlickerableLightController && FlickerableLightController.IsEnabled();

        private FlickerableLightController FlickerableLightController { get; set; }

        /// <summary>
        /// Flickers the room's lights off for a duration.
        /// </summary>
        /// <param name="duration">Duration in seconds.</param>
        public void TurnOffLights(float duration) => FlickerableLightController?.ServerFlickerLights(duration);

        /// <summary>
        /// Resets the room color to default.
        /// </summary>
        public void ResetColor()
        {
            FlickerableLightController.WarheadLightColor = global::FlickerableLightController.DefaultWarheadColor;
            FlickerableLightController.WarheadLightOverride = false;
        }

        /// <summary>
        /// Factory method to create and add a <see cref="Room"/> component to a Transform.
        /// We can add parameters to be set privately here.
        /// </summary>
        /// <param name="roomGameObject">The Game Object to attach the Room component to.</param>
        /// <returns>The Room component that was instantiated onto the Game Object.</returns>
        internal static Room CreateComponent(GameObject roomGameObject) => roomGameObject.AddComponent<Room>();

        private static RoomType FindType(string rawName)
        {
            // Try to remove brackets if they exist.
            rawName = rawName.RemoveBracketsOnEndOfName();

            return rawName switch
            {
                "LCZ_Armory" => RoomType.LczArmory,
                "LCZ_Curve" => RoomType.LczCurve,
                "LCZ_Straight" => RoomType.LczStraight,
                "LCZ_012" => RoomType.Lcz012,
                "LCZ_914" => RoomType.Lcz914,
                "LCZ_Crossing" => RoomType.LczCrossing,
                "LCZ_TCross" => RoomType.LczTCross,
                "LCZ_Cafe" => RoomType.LczCafe,
                "LCZ_Plants" => RoomType.LczPlants,
                "LCZ_Toilets" => RoomType.LczToilets,
                "LCZ_Airlock" => RoomType.LczAirlock,
                "LCZ_173" => RoomType.Lcz173,
                "LCZ_ClassDSpawn" => RoomType.LczClassDSpawn,
                "LCZ_ChkpB" => RoomType.LczChkpB,
                "LCZ_372" => RoomType.LczGlassBox,
                "LCZ_ChkpA" => RoomType.LczChkpA,
                "HCZ_079" => RoomType.Hcz079,
                "HCZ_EZ_Checkpoint" => RoomType.HczEzCheckpoint,
                "HCZ_Room3ar" => RoomType.HczArmory,
                "HCZ_Testroom" => RoomType.Hcz939,
                "HCZ_Hid" => RoomType.HczHid,
                "HCZ_049" => RoomType.Hcz049,
                "HCZ_ChkpA" => RoomType.HczChkpA,
                "HCZ_Crossing" => RoomType.HczCrossing,
                "HCZ_106" => RoomType.Hcz106,
                "HCZ_Nuke" => RoomType.HczNuke,
                "HCZ_Tesla" => RoomType.HczTesla,
                "HCZ_Servers" => RoomType.HczServers,
                "HCZ_ChkpB" => RoomType.HczChkpB,
                "HCZ_Room3" => RoomType.HczTCross,
                "HCZ_457" => RoomType.Hcz096,
                "HCZ_Curve" => RoomType.HczCurve,
                "EZ_Endoof" => RoomType.EzVent,
                "EZ_Intercom" => RoomType.EzIntercom,
                "EZ_GateA" => RoomType.EzGateA,
                "EZ_PCs_small" => RoomType.EzDownstairsPcs,
                "EZ_Curve" => RoomType.EzCurve,
                "EZ_PCs" => RoomType.EzPcs,
                "EZ_Crossing" => RoomType.EzCrossing,
                "EZ_CollapsedTunnel" => RoomType.EzCollapsedTunnel,
                "EZ_Smallrooms2" => RoomType.EzConference,
                "EZ_Straight" => RoomType.EzStraight,
                "EZ_Cafeteria" => RoomType.EzCafeteria,
                "EZ_upstairs" => RoomType.EzUpstairsPcs,
                "EZ_GateB" => RoomType.EzGateB,
                "EZ_Shelter" => RoomType.EzShelter,
                "PocketWorld" => RoomType.Pocket,
                "Outside" => RoomType.Surface,
                _ => RoomType.Unknown
            };
        }

        private static ZoneType FindZone(GameObject gameObject)
        {
            var transform = gameObject.transform;

            if (transform.parent == null)
                return ZoneType.Unspecified;

            return transform.parent.name switch
            {
                "HeavyRooms" => ZoneType.HeavyContainment,
                "LightRooms" => ZoneType.LightContainment,
                "EntranceRooms" => ZoneType.Entrance,
                _ => transform.position.y > 900 ? ZoneType.Surface : ZoneType.Unspecified
            };
        }

        private static IEnumerable<Door> FindDoors(GameObject gameObject)
        {
            List<Door> doors = new List<Door>();
            foreach (DoorVariant doorVariant in gameObject.GetComponentsInChildren<DoorVariant>())
                doors.Add(Door.Get(doorVariant));
            return doors;
        }

        private static List<Camera079> FindCameras(GameObject gameObject)
        {
            List<Camera079> cameraList = new List<Camera079>();
            foreach (Camera079 camera in Map.Cameras)
            {
                if (camera.Room().gameObject == gameObject)
                {
                    cameraList.Add(camera);
                }
            }

            return cameraList;
        }

        private void Start()
        {
            Zone = FindZone(gameObject);
            Type = FindType(gameObject.name);
            Doors = FindDoors(gameObject);
            Cameras = FindCameras(gameObject);
            FlickerableLightController = GetComponentInChildren<FlickerableLightController>();
        }
    }
}
