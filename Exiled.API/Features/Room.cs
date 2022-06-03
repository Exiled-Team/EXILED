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
        internal static readonly List<Room> RoomsValue = new(250);

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
        public IEnumerable<Player> Players => Player.List.Where(player => player.IsAlive && !(player.CurrentRoom is null) && player.CurrentRoom.Transform == Transform);

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
        /// Gets a <see cref="Room"/> given the specified <see cref="Vector3"/>.
        /// </summary>
        /// <param name="position">The <see cref="Vector3"/> to search for.</param>
        /// <returns>The <see cref="Room"/> with the given <see cref="Vector3"/> or <see langword="null"/> if not found.</returns>
        public static Room Get(Vector3 position) => List.FirstOrDefault(x => x.RoomIdentifier.UniqueId == RoomIdUtils.RoomAtPosition(position).UniqueId)
            ?? List.FirstOrDefault(x => x.RoomIdentifier.UniqueId == RoomIdUtils.RoomAtPositionRaycasts(position).UniqueId);

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
            List<Room> rooms = zoneType is not ZoneType.Unspecified ? Get(r => r.Zone == zoneType).ToList() : RoomsValue;
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
            return rawName switch
            {
                "LCZ_Armory" => RoomType.LczArmory,
                "LCZ_Curve" => RoomType.LczCurve,
                "LCZ_Straight" => RoomType.LczStraight,
                "LCZ_330" => RoomType.Lcz330,
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
                "HCZ_Straight" => RoomType.HczStraight,
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
                "EZ_ThreeWay" => RoomType.EzTCross,
                "PocketWorld" => RoomType.Pocket,
                "Outside" => RoomType.Surface,
                _ => RoomType.Unknown,
            };
        }

        private static ZoneType FindZone(GameObject gameObject)
        {
            Transform transform = gameObject.transform;

            if (transform.parent is null)
                return ZoneType.Surface;

            return transform.parent.name switch
            {
                "HeavyRooms" => ZoneType.HeavyContainment,
                "LightRooms" => ZoneType.LightContainment,
                "EntranceRooms" => ZoneType.Entrance,
                _ => transform.position.y > 900 ? ZoneType.Surface : ZoneType.Unspecified,
            };
        }

        private void FindObjectsInRoom(out List<Camera079> cameraList, out List<Door> doors, out TeslaGate teslaGate, out FlickerableLightController flickerableLightController)
        {
            cameraList = new List<Camera079>();
            doors = new List<Door>();
            teslaGate = null;
            flickerableLightController = null;

            if (Scp079Interactable.InteractablesByRoomId.ContainsKey(RoomIdentifier.UniqueId))
            {
                foreach (Scp079Interactable scp079Interactable in Scp079Interactable.InteractablesByRoomId[RoomIdentifier.UniqueId])
                {
                    try
                    {
                        if (scp079Interactable is not null)
                        {
                            switch (scp079Interactable.type)
                            {
                                case Scp079Interactable.InteractableType.Door:
                                {
                                    if (scp079Interactable.TryGetComponent(out DoorVariant doorVariant))
                                        doors.Add(Door.Get(doorVariant, this));
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
                                    if (scp079Interactable.TryGetComponent(
                                            out FlickerableLightController lightController))
                                        flickerableLightController = lightController;
                                    break;
                                }

                                case Scp079Interactable.InteractableType.Tesla:
                                {
                                    if (scp079Interactable.TryGetComponent(out global::TeslaGate tesla))
                                        teslaGate = TeslaGate.Get(tesla);
                                    break;
                                }
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Log.Error($"{nameof(FindObjectsInRoom)}: Exception cause {e.Message}\n{scp079Interactable is null} {scp079Interactable?.type is null}");
                    }
                }
            }

            if (flickerableLightController is null && gameObject.transform.position.y > 900)
            {
                flickerableLightController = FlickerableLightController.Instances.Single(x => x.transform.position.y > 900);
            }
        }

        private void Awake()
        {
            Zone = FindZone(gameObject);
            Type = FindType(gameObject.name);
            RoomIdentifier = gameObject.GetComponent<RoomIdentifier>();

            FindObjectsInRoom(out List<Camera079> cameras, out List<Door> doors, out TeslaGate teslagate, out FlickerableLightController flickerableLightController);
            Doors = doors;
            Cameras = Camera.Get(cameras);
            TeslaGate = teslagate;
            if (flickerableLightController is null)
            {
                if (!gameObject.TryGetComponent(out flickerableLightController))
                    flickerableLightController = gameObject.AddComponent<FlickerableLightController>();
            }

            FlickerableLightController = flickerableLightController;
        }
    }
}
