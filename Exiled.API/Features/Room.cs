// -----------------------------------------------------------------------
// <copyright file="Room.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features
{
    using System;
#pragma warning disable 1584
    using System.Collections.Generic;
    using System.Linq;

    using Exiled.API.Enums;
    using Exiled.API.Extensions;

    using Interactables.Interobjects.DoorUtils;

    using MapGeneration;

    using Mirror;

    using UnityEngine;

    /// <summary>
    /// The in-game room.
    /// </summary>
    public class Room : MonoBehaviour
    {
        /// <summary>
        /// A <see cref="List{T}"/> of <see cref="Room"/>s on the map.
        /// </summary>
        internal static readonly List<Room> RoomsValue = new List<Room>(250);

        /// <summary>
        /// Gets a <see cref="IEnumerable{T}"/> of <see cref="Room"/> which contains all the <see cref="Room"/> instances.
        /// </summary>
        public static IEnumerable<Room> List => RoomsValue;

        /// <summary>
        /// Gets the <see cref="Room"/> name.
        /// </summary>
        public string Name => name;

        /// <summary>
        /// Gets the <see cref="Room"/> <see cref="UnityEngine.GameObject"/>.
        /// </summary>
        public GameObject GameObject => gameObject;

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
        /// Gets a reference to the room's <see cref="MapGeneration.RoomIdentifier"/>.
        /// </summary>
        public RoomIdentifier RoomIdentifier { get; private set; }

        /// <summary>
        /// Gets a reference to the <see cref="global::TeslaGate"/> in the room, or <see langword="null"/> if this room does not contain one.
        /// </summary>
        public TeslaGate TeslaGate { get; private set; }

        /// <summary>
        /// Gets a <see cref="IEnumerable{T}"/> of <see cref="Player"/> in the <see cref="Room"/>.
        /// </summary>
        public IEnumerable<Player> Players => Player.List.Where(player => player.IsAlive && player.CurrentRoom.Transform == Transform);

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
            set
            {
                if (FlickerableLightController)
                    FlickerableLightController.Network_lightIntensityMultiplier = value;
            }
        }

        /// <summary>
        /// Gets or sets the color of the room's lights by changing the warhead color.
        /// </summary>
        public Color Color
        {
            get => (Color)FlickerableLightController?.WarheadLightColor;
            set
            {
                if (FlickerableLightController)
                {
                    FlickerableLightController.WarheadLightColor = value;
                    FlickerableLightController.WarheadLightOverride = true;
                }
            }
        }

        /// <summary>
        /// Gets a <see cref="IEnumerable{T}"/> of <see cref="Camera"/> in the <see cref="Room"/>.
        /// </summary>
        public IEnumerable<Camera> Cameras { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether or not the lights in this room are currently flickered on.
        /// </summary>
        public bool LightsOn
        {
            get => FlickerableLightController && FlickerableLightController.NetworkLightsEnabled;
            set
            {
                if (FlickerableLightController)
                    FlickerableLightController.NetworkLightsEnabled = value;
            }
        }

        /// <summary>
        /// Gets the FlickerableLightController's NetworkIdentity.
        /// </summary>
        public NetworkIdentity FlickerableLightControllerNetIdentity => FlickerableLightController.netIdentity;

        /// <summary>
        /// Gets the room's FlickerableLightController.
        /// </summary>
        public FlickerableLightController FlickerableLightController { get; private set; }

        /// <summary>
        /// Gets a <see cref="Room"/> given the specified <see cref="RoomType"/>.
        /// </summary>
        /// <param name="roomType">The <see cref="RoomType"/> to search for.</param>
        /// <returns>The <see cref="Room"/> with the given <see cref="RoomType"/> or <see langword="null"/> if not found.</returns>
        public static Room Get(RoomType roomType) => Get(room => room.Type == roomType).FirstOrDefault();

        /// <summary>
        /// Gets a <see cref="Room"/> given the specified <see cref="ZoneType"/>.
        /// </summary>
        /// <param name="zoneType">The <see cref="ZoneType"/> to search for.</param>
        /// <returns>The <see cref="Room"/> with the given <see cref="ZoneType"/> or <see langword="null"/> if not found.</returns>
        public static IEnumerable<Room> Get(ZoneType zoneType) => Get(room => room.Zone == zoneType);

        /// <summary>
        /// Gets a <see cref="IEnumerable{T}"/> of <see cref="Room"/> filtered based on a predicate.
        /// </summary>
        /// <param name="predicate">The condition to satify.</param>
        /// <returns>A <see cref="IEnumerable{T}"/> of <see cref="Room"/> which contains elements that satify the condition.</returns>
        public static IEnumerable<Room> Get(Func<Room, bool> predicate) => List.Where(predicate);

        /// <summary>
        /// Gets a random <see cref="Room"/>.
        /// </summary>
        /// <param name="zoneType">Filters by <see cref="ZoneType"/>.</param>
        /// <returns><see cref="Room"/> object.</returns>
        public static Room Random(ZoneType zoneType = ZoneType.Unspecified)
        {
            List<Room> rooms = zoneType != ZoneType.Unspecified ? Room.Get(r => r.Zone == zoneType).ToList() : Room.RoomsValue;
            return rooms[UnityEngine.Random.Range(0, rooms.Count)];
        }

        /// <summary>
        /// Flickers the room's lights off for a duration.
        /// </summary>
        /// <param name="duration">Duration in seconds.</param>
        public void TurnOffLights(float duration) => FlickerableLightController?.ServerFlickerLights(duration);

        /// <summary>
        /// Locks all the doors in the room.
        /// </summary>
        /// <param name="duration">Duration in seconds, or <c>-1</c> for permanent lockdown.</param>
        /// <param name="lockType">DoorLockType of the lockdown.</param>
        /// <seealso cref="Door.LockAll(float, ZoneType, DoorLockType)"/>
        /// <seealso cref="Door.LockAll(float, IEnumerable{ZoneType}, DoorLockType)"/>
        public void LockDown(float duration, DoorLockType lockType = DoorLockType.Regular079)
        {
            foreach (Door door in Doors)
            {
                door.ChangeLock(lockType);
                door.IsOpen = false;
            }

            if (duration < 0)
                return;
            MEC.Timing.CallDelayed(duration, UnlockAll);
        }

        /// <summary>
        /// Locks all the doors and turns off all lights in the room.
        /// </summary>
        /// <param name="duration">Duration in seconds, or <c>-1</c> for permanent blackout.</param>
        /// <param name="lockType">DoorLockType of the blackout.</param>
        /// <seealso cref="Map.TurnOffAllLights(float, ZoneType)"/>
        /// <seealso cref="Map.TurnOffAllLights(float, IEnumerable{ZoneType})"/>
        public void Blackout(float duration, DoorLockType lockType = DoorLockType.Regular079)
        {
            LockDown(duration, lockType);
            TurnOffLights(duration);
        }

        /// <summary>
        /// Unlocks all the doors in the room.
        /// </summary>
        /// <seealso cref="Door.UnlockAll()"/>
        /// <seealso cref="Door.UnlockAll(ZoneType)"/>
        /// <seealso cref="Door.UnlockAll(IEnumerable{ZoneType})"/>
        /// <seealso cref="Door.UnlockAll(Func{Door, bool})"/>
        public void UnlockAll()
        {
            foreach (Door door in Doors)
                door.Unlock();
        }

        /// <summary>
        /// Resets the room color to default.
        /// </summary>
        public void ResetColor()
        {
            FlickerableLightController.WarheadLightColor = FlickerableLightController.DefaultWarheadColor;
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

            switch (rawName)
            {
                case "LCZ_Armory":
                    return RoomType.LczArmory;
                case "LCZ_Curve":
                    return RoomType.LczCurve;
                case "LCZ_Straight":
                    return RoomType.LczStraight;
                case "LCZ_330":
                    return RoomType.Lcz330;
                case "LCZ_914":
                    return RoomType.Lcz914;
                case "LCZ_Crossing":
                    return RoomType.LczCrossing;
                case "LCZ_TCross":
                    return RoomType.LczTCross;
                case "LCZ_Cafe":
                    return RoomType.LczCafe;
                case "LCZ_Plants":
                    return RoomType.LczPlants;
                case "LCZ_Toilets":
                    return RoomType.LczToilets;
                case "LCZ_Airlock":
                    return RoomType.LczAirlock;
                case "LCZ_173":
                    return RoomType.Lcz173;
                case "LCZ_ClassDSpawn":
                    return RoomType.LczClassDSpawn;
                case "LCZ_ChkpB":
                    return RoomType.LczChkpB;
                case "LCZ_372":
                    return RoomType.LczGlassBox;
                case "LCZ_ChkpA":
                    return RoomType.LczChkpA;
                case "HCZ_079":
                    return RoomType.Hcz079;
                case "HCZ_EZ_Checkpoint":
                    return RoomType.HczEzCheckpoint;
                case "HCZ_Room3ar":
                    return RoomType.HczArmory;
                case "HCZ_Testroom":
                    return RoomType.Hcz939;
                case "HCZ_Hid":
                    return RoomType.HczHid;
                case "HCZ_049":
                    return RoomType.Hcz049;
                case "HCZ_ChkpA":
                    return RoomType.HczChkpA;
                case "HCZ_Crossing":
                    return RoomType.HczCrossing;
                case "HCZ_106":
                    return RoomType.Hcz106;
                case "HCZ_Nuke":
                    return RoomType.HczNuke;
                case "HCZ_Tesla":
                    return RoomType.HczTesla;
                case "HCZ_Servers":
                    return RoomType.HczServers;
                case "HCZ_ChkpB":
                    return RoomType.HczChkpB;
                case "HCZ_Room3":
                    return RoomType.HczTCross;
                case "HCZ_457":
                    return RoomType.Hcz096;
                case "HCZ_Curve":
                    return RoomType.HczCurve;
                case "HCZ_Straight":
                    return RoomType.HczStraight;
                case "EZ_Endoof":
                    return RoomType.EzVent;
                case "EZ_Intercom":
                    return RoomType.EzIntercom;
                case "EZ_GateA":
                    return RoomType.EzGateA;
                case "EZ_PCs_small":
                    return RoomType.EzDownstairsPcs;
                case "EZ_Curve":
                    return RoomType.EzCurve;
                case "EZ_PCs":
                    return RoomType.EzPcs;
                case "EZ_Crossing":
                    return RoomType.EzCrossing;
                case "EZ_CollapsedTunnel":
                    return RoomType.EzCollapsedTunnel;
                case "EZ_Smallrooms2":
                    return RoomType.EzConference;
                case "EZ_Straight":
                    return RoomType.EzStraight;
                case "EZ_Cafeteria":
                    return RoomType.EzCafeteria;
                case "EZ_upstairs":
                    return RoomType.EzUpstairsPcs;
                case "EZ_GateB":
                    return RoomType.EzGateB;
                case "EZ_Shelter":
                    return RoomType.EzShelter;
                case "EZ_ThreeWay":
                    return RoomType.EzTCross;
                case "PocketWorld":
                    return RoomType.Pocket;
                case "Outside":
                    return RoomType.Surface;
                default:
                    return RoomType.Unknown;
            }
        }

        private static ZoneType FindZone(GameObject gameObject)
        {
            Transform transform = gameObject.transform;

            if (transform.parent == null)
                return ZoneType.Surface;

            switch (transform.parent.name)
            {
                case "HeavyRooms":
                    return ZoneType.HeavyContainment;
                case "LightRooms":
                    return ZoneType.LightContainment;
                case "EntranceRooms":
                    return ZoneType.Entrance;
                default:
                    return transform.position.y > 900 ? ZoneType.Surface : ZoneType.Unspecified;
            }
        }

        private void FindObjectsInRoom(out List<Camera079> cameraList, out List<Door> doors, out FlickerableLightController flickerableLightController)
        {
            cameraList = new List<Camera079>();
            doors = new List<Door>();
            flickerableLightController = null;

            if (Scp079Interactable.InteractablesByRoomId.ContainsKey(RoomIdentifier.UniqueId))
            {
                foreach (Scp079Interactable scp079Interactable in Scp079Interactable.InteractablesByRoomId[
                    gameObject.GetComponent<RoomIdentifier>().UniqueId])
                {
                    if (scp079Interactable != null)
                    {
                        switch (scp079Interactable.type)
                        {
                            case Scp079Interactable.InteractableType.Door:
                            {
                                if (scp079Interactable.TryGetComponent(out DoorVariant doorVariant))
                                    doors.Add(Door.Get(doorVariant));
                                break;
                            }

                            case Scp079Interactable.InteractableType.Camera:
                            {
                                if (scp079Interactable.TryGetComponent(out Camera079 camera))
                                    cameraList.Add(camera);
                                break;
                            }

                            case Scp079Interactable.InteractableType.LightController:
                            {
                                if (scp079Interactable.TryGetComponent(out FlickerableLightController lightController))
                                    flickerableLightController = lightController;
                                break;
                            }
                        }
                    }
                }
            }

            if (flickerableLightController == null && gameObject.transform.position.y > 900)
            {
                flickerableLightController = FlickerableLightController.Instances.Single(x => x.transform.position.y > 900);
            }
        }

        private void Start()
        {
            Zone = FindZone(gameObject);
            Type = FindType(gameObject.name);
            RoomIdentifier = gameObject.GetComponent<RoomIdentifier>();
            TeslaGate = gameObject.GetComponentInChildren<TeslaGate>();

            FindObjectsInRoom(out List<Camera079> cameras, out List<Door> doors, out FlickerableLightController flickerableLightController);
            Doors = doors;
            Cameras = Camera.Get(cameras);
            if (flickerableLightController == null)
            {
                if (!gameObject.TryGetComponent(out flickerableLightController))
                    flickerableLightController = gameObject.AddComponent<FlickerableLightController>();
            }

            FlickerableLightController = flickerableLightController;
        }
    }
}
