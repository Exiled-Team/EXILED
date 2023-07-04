// -----------------------------------------------------------------------
// <copyright file="Room.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Enums;

    using Exiled.API.Extensions;
    using Exiled.API.Features.Pickups;
    using Exiled.API.Interfaces;
    using Interactables.Interobjects.DoorUtils;

    using MapGeneration;

    using MEC;

    using Mirror;

    using PlayerRoles.PlayableScps.Scp079;
    using RelativePositioning;
    using UnityEngine;

    /// <summary>
    /// The in-game room.
    /// </summary>
    public class Room : MonoBehaviour, IWorldSpace
    {
        /// <summary>
        /// A <see cref="Dictionary{TKey,TValue}"/> containing all known <see cref="RoomIdentifier"/>s and their corresponding <see cref="Room"/>.
        /// </summary>
        internal static readonly Dictionary<RoomIdentifier, Room> RoomIdentifierToRoom = new(250);

        /// <summary>
        /// Gets a <see cref="IEnumerable{T}"/> of <see cref="Room"/> which contains all the <see cref="Room"/> instances.
        /// </summary>
        public static IEnumerable<Room> List => RoomIdentifierToRoom.Values;

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
        /// Gets the <see cref="Room"/> rotation.
        /// </summary>
        public Quaternion Rotation => transform.rotation;

        /// <summary>
        /// Gets the <see cref="ZoneType"/> in which the room is located.
        /// </summary>
        public ZoneType Zone { get; private set; } = ZoneType.Unspecified;

        /// <summary>
        /// Gets the <see cref="MapGeneration.RoomName"/> enum representing this room.
        /// </summary>
        /// <remarks>This property is the internal <see cref="MapGeneration.RoomName"/> of the room. For the actual string of the Room's name, see <see cref="Name"/>.</remarks>
        /// <seealso cref="Name"/>
        public RoomName RoomName => Identifier.Name;

        /// <summary>
        /// Gets the <see cref="RoomType"/>.
        /// </summary>
        public RoomType Type { get; private set; } = RoomType.Unknown;

        /// <summary>
        /// Gets a reference to the room's <see cref="RoomIdentifier"/>.
        /// </summary>
        public RoomIdentifier Identifier { get; private set; }

        /// <summary>
        /// Gets a reference to the <see cref="global::TeslaGate"/> in the room, or <see langword="null"/> if this room does not contain one.
        /// </summary>
        public TeslaGate TeslaGate { get; private set; }

        /// <summary>
        /// Gets a <see cref="IEnumerable{T}"/> of <see cref="Player"/> in the <see cref="Room"/>.
        /// </summary>
        public IEnumerable<Player> Players => Player.List.Where(player => player.IsAlive && player.CurrentRoom is not null && (player.CurrentRoom.Transform == Transform));

        /// <summary>
        /// Gets a <see cref="IEnumerable{T}"/> of <see cref="Door"/> in the <see cref="Room"/>.
        /// </summary>
        public IEnumerable<Door> Doors { get; private set; } = Enumerable.Empty<Door>();

        /// <summary>
        /// Gets a <see cref="IEnumerable{T}"/> of <see cref="Scp079Speaker"/> in the <see cref="Room"/>.
        /// </summary>
        public IEnumerable<Scp079Speaker> Speaker { get; private set; } = Enumerable.Empty<Scp079Speaker>();

        /// <summary>
        /// Gets a <see cref="IEnumerable{T}"/> of <see cref="Pickup"/> in the <see cref="Room"/>.
        /// </summary>
        public IEnumerable<Pickup> Pickups
        {
            get
            {
                List<Pickup> pickups = new();
                foreach (Pickup pickup in Pickup.List)
                {
                    if (Room.FindParentRoom(pickup.GameObject) == this)
                        pickups.Add(pickup);
                }

                return pickups;
            }
        }

        /// <summary>
        /// Gets or sets the color of the room's lights by changing the warhead color.
        /// </summary>
        public Color Color
        {
            get => RoomLightController.NetworkOverrideColor;
            set
            {
                if (RoomLightController)
                {
                    RoomLightController.NetworkOverrideColor = value;
                }
            }
        }

        /// <summary>
        /// Gets a <see cref="IEnumerable{T}"/> of <see cref="Camera"/> in the <see cref="Room"/>.
        /// </summary>
        public IEnumerable<Camera> Cameras { get; private set; } = Enumerable.Empty<Camera>();

        /// <summary>
        /// Gets or sets a value indicating whether or not the lights in this room are currently off.
        /// </summary>
        public bool AreLightsOff
        {
            get => RoomLightController && !RoomLightController.NetworkLightsEnabled;
            set
            {
                if (RoomLightController)
                    RoomLightController.NetworkLightsEnabled = !value;
            }
        }

        /// <summary>
        /// Gets the FlickerableLightController's NetworkIdentity.
        /// </summary>
        public NetworkIdentity RoomLightControllerNetIdentity => RoomLightController.netIdentity;

        /// <summary>
        /// Gets the room's FlickerableLightController.
        /// </summary>
        public RoomLightController RoomLightController { get; private set; }

        /// <summary>
        /// Gets a <see cref="Room"/> given the specified <see cref="RoomType"/>.
        /// </summary>
        /// <param name="roomType">The <see cref="RoomType"/> to search for.</param>
        /// <returns>The <see cref="Room"/> with the given <see cref="RoomType"/> or <see langword="null"/> if not found.</returns>
        public static Room Get(RoomType roomType) => Get(room => room.Type == roomType).FirstOrDefault();

        /// <summary>
        /// Gets a <see cref="Room"/> from a given <see cref="Identifier"/>.
        /// </summary>
        /// <param name="roomIdentifier">The <see cref="Identifier"/> to search with.</param>
        /// <returns>The <see cref="Room"/> of the given identified, if any. Can be <see langword="null"/>.</returns>
        public static Room Get(RoomIdentifier roomIdentifier) => roomIdentifier == null ? null :
            RoomIdentifierToRoom.TryGetValue(roomIdentifier, out Room room) ? room : null;

        /// <summary>
        /// Gets a <see cref="Room"/> from a given <see cref="RoomIdentifier"/>.
        /// </summary>
        /// <param name="flickerableLightController">The <see cref="RoomLightController"/> to search with.</param>
        /// <returns>The <see cref="Room"/> of the given identified, if any. Can be <see langword="null"/>.</returns>
        public static Room Get(RoomLightController flickerableLightController) => flickerableLightController.GetComponentInParent<Room>();

        /// <summary>
        /// Gets a <see cref="Room"/> given the specified <see cref="Vector3"/>.
        /// </summary>
        /// <param name="position">The <see cref="Vector3"/> to search for.</param>
        /// <returns>The <see cref="Room"/> with the given <see cref="Vector3"/> or <see langword="null"/> if not found.</returns>
        public static Room Get(Vector3 position) => RoomIdUtils.RoomAtPositionRaycasts(position, false) is RoomIdentifier identifier ? Get(identifier) : null;

        /// <summary>
        /// Gets a <see cref="Room"/> given the specified <see cref="RelativePosition"/>.
        /// </summary>
        /// <param name="position">The <see cref="RelativePosition"/> to search for.</param>
        /// <returns>The <see cref="Room"/> with the given <see cref="RelativePosition"/> or <see langword="null"/> if not found.</returns>
        public static Room Get(RelativePosition position) => Get(position.Position);

        /// <summary>
        /// Gets a <see cref="IEnumerable{T}"/> of <see cref="Room"/> given the specified <see cref="ZoneType"/>.
        /// </summary>
        /// <param name="zoneType">The <see cref="ZoneType"/> to search for.</param>
        /// <returns>The <see cref="Room"/> with the given <see cref="ZoneType"/> or <see langword="null"/> if not found.</returns>
        public static IEnumerable<Room> Get(ZoneType zoneType) => Get(room => room.Zone.HasFlag(zoneType));

        /// <summary>
        /// Gets a <see cref="IEnumerable{T}"/> of <see cref="Room"/> filtered based on a predicate.
        /// </summary>
        /// <param name="predicate">The condition to satify.</param>
        /// <returns>A <see cref="IEnumerable{T}"/> of <see cref="Room"/> which contains elements that satify the condition.</returns>
        public static IEnumerable<Room> Get(Func<Room, bool> predicate) => List.Where(predicate);

        /// <summary>
        /// Tries to find the room that a <see cref="GameObject"/> is inside, first using the <see cref="Transform"/>'s parents, then using a Raycast if no room was found.
        /// </summary>
        /// <param name="objectInRoom">The <see cref="GameObject"/> inside the room.</param>
        /// <returns>The <see cref="Room"/> that the <see cref="GameObject"/> is located inside. Can be <see langword="null"/>.</returns>
        /// <seealso cref="Get(Vector3)"/>
        public static Room FindParentRoom(GameObject objectInRoom)
        {
            if (objectInRoom == null)
                return default;

            Room room = null;

            const string playerTag = "Player";

            // First try to find the room owner quickly.
            if (!objectInRoom.CompareTag(playerTag))
            {
                room = objectInRoom.GetComponentInParent<Room>();
            }
            else
            {
                // Check for SCP-079 if it's a player
                Player ply = Player.Get(objectInRoom);

                // Raycasting doesn't make sense,
                // SCP-079 position is constant,
                // let it be 'Outside' instead
                if (ply.Role.Is(out Roles.Scp079Role role))
                    room = FindParentRoom(role.Camera.GameObject);
            }

            // Finally, try for objects that aren't children, like players and pickups.
            return room ?? Get(objectInRoom.transform.position) ?? default;
        }

        /// <summary>
        /// Gets a random <see cref="Room"/>.
        /// </summary>
        /// <param name="zoneType">Filters by <see cref="ZoneType"/>.</param>
        /// <returns><see cref="Room"/> object.</returns>
        public static Room Random(ZoneType zoneType = ZoneType.Unspecified)
        {
            IEnumerable<Room> rooms = zoneType is not ZoneType.Unspecified ? Get(r => r.Zone.HasFlag(zoneType)) : List;

            return rooms.ElementAtOrDefault(UnityEngine.Random.Range(0, rooms.Count()));
        }

        /// <summary>
        /// Returns the local space position, based on a world space position.
        /// </summary>
        /// <param name="position">World position.</param>
        /// <returns>Local position, based on the room.</returns>
        public Vector3 LocalPosition(Vector3 position) => Transform.TransformPoint(position);

        /// <summary>
        /// Returns the World position, based on a local space position.
        /// </summary>
        /// <param name="offset">Local position.</param>
        /// <returns>World position, based on the room.</returns>
        public Vector3 PositionFromRoom(Vector3 offset) => Transform.InverseTransformPoint(offset);

        /// <summary>
        /// Flickers the room's lights off for a duration.
        /// </summary>
        /// <param name="duration">Duration in seconds.</param>
        public void TurnOffLights(float duration) => RoomLightController?.ServerFlickerLights(duration);

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

            Timing.CallDelayed(duration, UnlockAll);
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
            if (!RoomLightController)
                return;

            RoomLightController.NetworkOverrideColor = Color.clear;
        }

        /// <summary>
        /// Returns the Room in a human-readable format.
        /// </summary>
        /// <returns>A string containing Room-related data.</returns>
        public override string ToString() => $"{Type} ({Zone}) [{Doors?.Count()}] *{Cameras?.Count()}* |{TeslaGate}|";

        /// <summary>
        /// Factory method to create and add a <see cref="Room"/> component to a Transform.
        /// We can add parameters to be set privately here.
        /// </summary>
        /// <param name="roomGameObject">The Game Object to attach the Room component to.</param>
        /// <returns>The Room component that was instantiated onto the Game Object.</returns>
        internal static Room CreateComponent(GameObject roomGameObject) => roomGameObject.AddComponent<Room>();

        private static RoomType FindType(GameObject gameObject)
        {
            // Try to remove brackets if they exist.
            return gameObject.name.RemoveBracketsOnEndOfName() switch
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
                "LCZ_ChkpB" => RoomType.LczCheckpointB,
                "LCZ_372" => RoomType.LczGlassBox,
                "LCZ_ChkpA" => RoomType.LczCheckpointA,
                "HCZ_079" => RoomType.Hcz079,
                "HCZ_Room3ar" => RoomType.HczArmory,
                "HCZ_Testroom" => RoomType.HczTestRoom,
                "HCZ_Hid" => RoomType.HczHid,
                "HCZ_049" => RoomType.Hcz049,
                "HCZ_Crossing" => RoomType.HczCrossing,
                "HCZ_106" => RoomType.Hcz106,
                "HCZ_Nuke" => RoomType.HczNuke,
                "HCZ_Tesla" => RoomType.HczTesla,
                "HCZ_Servers" => RoomType.HczServers,
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
                "HCZ_939" => RoomType.Hcz939,
                "EZ Part" => RoomType.EzCheckpointHallway,
                "HCZ_ChkpA" => RoomType.HczElevatorA,
                "HCZ_ChkpB" => RoomType.HczElevatorB,
                "HCZ Part" => gameObject.transform.parent.name switch
                {
                    "HCZ_EZ_Checkpoint (A)" => RoomType.HczEzCheckpointA,
                    "HCZ_EZ_Checkpoint (B)" => RoomType.HczEzCheckpointB,
                    _ => RoomType.Unknown
                },
                _ => RoomType.Unknown,
            };
        }

        private static ZoneType FindZone(GameObject gameObject)
        {
            Transform transform = gameObject.transform;

            return transform.parent?.name.RemoveBracketsOnEndOfName() switch
            {
                "HeavyRooms" => ZoneType.HeavyContainment,
                "LightRooms" => ZoneType.LightContainment,
                "EntranceRooms" => ZoneType.Entrance,
                "HCZ_EZ_Checkpoint" => ZoneType.HeavyContainment | ZoneType.Entrance,
                _ => transform.position.y > 900 ? ZoneType.Surface : ZoneType.Unspecified,
            };
        }

        private void Awake()
        {
            Zone = FindZone(gameObject);
            Type = FindType(gameObject);

            Identifier = gameObject.GetComponent<RoomIdentifier>();
            RoomLightController = gameObject.GetComponentInChildren<RoomLightController>();

            Doors = DoorVariant.DoorsByRoom.ContainsKey(Identifier) ? DoorVariant.DoorsByRoom[Identifier].Select(x => Door.Get(x, this)).ToList() : new();
            Cameras = Camera.List.Where(x => x.Base.Room == Identifier).ToList();
            Speaker = Scp079Speaker.SpeakersInRooms.ContainsKey(Identifier) ? Scp079Speaker.SpeakersInRooms[Identifier] : new();

            if (Type is RoomType.HczTesla)
                TeslaGate = TeslaGate.List.Single(x => this == x.Room);
        }
    }
}
