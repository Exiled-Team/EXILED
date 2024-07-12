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
    using Exiled.API.Features.Core;
    using Exiled.API.Features.Doors;
    using Exiled.API.Features.Pickups;
    using MapGeneration;
    using Mirror;
    using PlayerRoles.PlayableScps.Scp079;
    using RelativePositioning;
    using UnityEngine;

    /// <summary>
    /// The in-game room.
    /// </summary>
    public sealed class Room : GameEntity
    {
        /// <summary>
        /// A <see cref="Dictionary{TKey,TValue}"/> containing all known <see cref="RoomIdentifier"/>s and their corresponding <see cref="Room"/>.
        /// </summary>
        internal static readonly Dictionary<RoomIdentifier, Room> RoomIdentifierToRoom = new(250, new ComponentsEqualityComparer());

        /// <summary>
        /// Initializes a new instance of the <see cref="Room"/> class.
        /// </summary>
        /// <param name="roomIdentifier">The room's <see cref="RoomIdentifier"/>.</param>
        internal Room(RoomIdentifier roomIdentifier)
            : base(roomIdentifier.gameObject)
        {
            Identifier = roomIdentifier;
            GameObject = Identifier.gameObject;

            RoomIdentifierToRoom.Add(Identifier, this);

            Zone = FindZone(GameObject);
#if Debug
            if (Type is RoomType.Unknown)
                Log.Error($"[ZONETYPE UNKNOWN] {this}");
#endif
            Type = FindType(GameObject);
#if Debug
            if (Type is RoomType.Unknown)
                Log.Error($"[ROOMTYPE UNKNOWN] {this}");
#endif

            RoomLightControllersValue.AddRange(GameObject.GetComponentsInChildren<RoomLightController>());

            RoomLightControllers = RoomLightControllersValue.AsReadOnly();

            GameObject.GetComponentsInChildren<BreakableWindow>().ForEach(x => WindowsValue.Add(new(x, this)));

            if (GameObject.GetComponentInChildren<global::TeslaGate>() is global::TeslaGate tesla)
                TeslaGate = new TeslaGate(tesla, this);

            Windows = WindowsValue.AsReadOnly();
            Doors = DoorsValue.AsReadOnly();
            Rooms = RoomsValue.AsReadOnly();
            Speakers = SpeakersValue.AsReadOnly();
            Cameras = CamerasValue.AsReadOnly();
        }

        /// <summary>
        /// Gets a <see cref="IEnumerable{T}"/> of <see cref="Room"/> which contains all the <see cref="Room"/> instances.
        /// </summary>
        public static new IReadOnlyCollection<Room> List => RoomIdentifierToRoom.Values;

        /// <summary>
        /// Gets the <see cref="Room"/> name.
        /// </summary>
        public string Name => GameObject.name;

        /// <summary>
        /// Gets the <see cref="Room"/>'s position.
        /// </summary>
        public override Vector3 Position => Transform.position;

        /// <summary>
        /// Gets the <see cref="Room"/>'s rotation.
        /// </summary>
        public override Quaternion Rotation => Transform.rotation;

        /// <summary>
        /// Gets the <see cref="ZoneType"/> in which the room is located.
        /// </summary>
        public ZoneType Zone { get; private set; }

        /// <summary>
        /// Gets the <see cref="MapGeneration.RoomName"/> enum representing this room.
        /// </summary>
        /// <remarks>This property is the internal <see cref="MapGeneration.RoomName"/> of the room. For the actual string of the Room's name, see <see cref="Name"/>.</remarks>
        /// <seealso cref="Name"/>
        public RoomName RoomName => Identifier.Name;

        /// <summary>
        /// Gets the room's <see cref="MapGeneration.RoomShape"/>.
        /// </summary>
        public RoomShape RoomShape => Identifier.Shape;

        /// <summary>
        /// Gets the <see cref="RoomType"/>.
        /// </summary>
        public RoomType Type { get; private set; }

        /// <summary>
        /// Gets a reference to the room's <see cref="RoomIdentifier"/>.
        /// </summary>
        public RoomIdentifier Identifier { get; private set; }

        /// <summary>
        /// Gets a reference to the <see cref="global::TeslaGate"/> in the room, or <see langword="null"/> if this room does not contain one.
        /// </summary>
        public TeslaGate TeslaGate { get; internal set; }

        /// <summary>
        /// Gets a <see cref="IEnumerable{T}"/> of <see cref="Player"/> in the <see cref="Room"/>.
        /// </summary>
        public IEnumerable<Player> Players => Player.List.Where(player => player.IsAlive && player.CurrentRoom is not null && (player.CurrentRoom.Transform == Transform));

        /// <summary>
        /// Gets a <see cref="IReadOnlyList{T}"/> of <see cref="Window"/> in the <see cref="Room"/>.
        /// </summary>
        public IReadOnlyList<Window> Windows { get; private set; }

        /// <summary>
        /// Gets a <see cref="IReadOnlyList{T}"/> of <see cref="Door"/> in the <see cref="Room"/>.
        /// </summary>
        public IReadOnlyList<Door> Doors { get; private set; }

        /// <summary>
        /// Gets a <see cref="IReadOnlyList{T}"/> of <see cref="Scp079Speaker"/> in the <see cref="Room"/>.
        /// </summary>
        public IReadOnlyList<Scp079Speaker> Speakers { get; private set; }

        /// <summary>
        /// Gets a <see cref="IReadOnlyList{T}"/> of <see cref="Camera"/> in the <see cref="Room"/>.
        /// </summary>
        public IReadOnlyList<Camera> Cameras { get; private set; }

        /// <summary>
        /// Gets a <see cref="IReadOnlyList{T}"/> of <see cref="RoomLightController"/> in the <see cref="Room"/>.
        /// </summary>
        /// <remarks>
        /// Using that will make sense only for rooms with more than one light controller, in other cases better to use <see cref="RoomLightController"/>.
        /// </remarks>
        public IReadOnlyList<RoomLightController> RoomLightControllers { get; private set; }

        /// <summary>
        /// Gets a <see cref="IReadOnlyList{T}"/> of <see cref="Room"/> around the <see cref="Room"/>.
        /// </summary>
        public IReadOnlyList<Room> NearestRooms { get; private set; }

        /// <summary>
        /// Gets a <see cref="IEnumerable{T}"/> of <see cref="Pickup"/> in the <see cref="Room"/>.
        /// </summary>
        public IEnumerable<Pickup> Pickups => Pickup.List.Where(pickup => FindParentRoom(pickup.GameObject) == this);

        /// <summary>
        /// Gets or sets the color of the room's lights by changing the warhead color.
        /// </summary>
        /// <remarks>Will return <see cref="Color.clear"/> when <see cref="RoomLightController"/> is <see langword="null"/>.</remarks>
        public Color Color
        {
            get => RoomLightController == null ? Color.clear : RoomLightController.NetworkOverrideColor;
            set
            {
                foreach (RoomLightController light in RoomLightControllers)
                {
                    light.NetworkOverrideColor = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not the lights in this room are currently off.
        /// </summary>
        public bool AreLightsOff
        {
            get => RoomLightController != null && !RoomLightController.NetworkLightsEnabled;
            set
            {
                foreach (RoomLightController light in RoomLightControllers)
                {
                    light.NetworkLightsEnabled = !value;
                }
            }
        }

        /// <summary>
        /// Gets the FlickerableLightController's NetworkIdentity.
        /// </summary>
        public NetworkIdentity RoomLightControllerNetIdentity => RoomLightController ? RoomLightController.netIdentity : null;

        /// <summary>
        /// Gets the room's FlickerableLightController.
        /// </summary>
        public RoomLightController RoomLightController => RoomLightControllers.FirstOrDefault();

        /// <summary>
        /// Gets a <see cref="List{T}"/> containing all known <see cref="Window"/>s in that <see cref="Room"/>.
        /// </summary>
        internal List<Window> WindowsValue { get; } = new();

        /// <summary>
        /// Gets a <see cref="List{T}"/> containing all known <see cref="Door"/>s in that <see cref="Room"/>.
        /// </summary>
        internal List<Door> DoorsValue { get; } = new();

        /// <summary>
        /// Gets a <see cref="List{T}"/> containing all known <see cref="Scp079Speaker"/>s in that <see cref="Room"/>.
        /// </summary>
        internal List<Scp079Speaker> SpeakersValue { get; } = new();

        /// <summary>
        /// Gets a <see cref="List{T}"/> containing all known <see cref="Camera"/>s in that <see cref="Room"/>.
        /// </summary>
        internal List<Camera> CamerasValue { get; } = new();

        /// <summary>
        /// Gets a <see cref="List{T}"/> containing all known <see cref="RoomLightController"/>s in that <see cref="Room"/>.
        /// </summary>
        internal List<RoomLightController> RoomLightControllersValue { get; } = new();

        /// <summary>
        /// Gets a <see cref="List{T}"/> containing all known <see cref="Room"/>s around that <see cref="Room"/>.
        /// </summary>
        internal List<Room> RoomsValue { get; } = new();

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
            RoomIdentifierToRoom.TryGetValue(roomIdentifier, out Room room) ? room : new Room(roomIdentifier);

        /// <summary>
        /// Gets a <see cref="Room"/> from a given <see cref="RoomIdentifier"/>.
        /// </summary>
        /// <param name="flickerableLightController">The <see cref="RoomLightController"/> to search with.</param>
        /// <returns>The <see cref="Room"/> of the given identified, if any. Can be <see langword="null"/>.</returns>
        public static Room Get(RoomLightController flickerableLightController) => List.FirstOrDefault(r => r.RoomLightControllers.Contains(flickerableLightController));

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
        /// <param name="predicate">The condition to satisfy.</param>
        /// <returns>A <see cref="IEnumerable{T}"/> of <see cref="Room"/> which contains elements that satisfy the condition.</returns>
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
                room = Get(objectInRoom.GetComponentInParent<RoomIdentifier>());
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
            return room ? room : Get(objectInRoom.transform.position) ?? default;
        }

        /// <summary>
        /// Gets a random <see cref="Room"/>.
        /// </summary>
        /// <param name="zoneType">Filters by <see cref="ZoneType"/>.</param>
        /// <returns><see cref="Room"/> object.</returns>
        public static Room Random(ZoneType zoneType = ZoneType.Unspecified) => (zoneType is not ZoneType.Unspecified ? Get(r => r.Zone.HasFlag(zoneType)) : List).Random();

        /// <summary>
        /// Flickers the room's lights off.
        /// </summary>
        public void TurnOffLights()
        {
            foreach (RoomLightController light in RoomLightControllers)
            {
                light.SetLights(false);
            }
        }

        /// <summary>
        /// Flickers the room's lights off for a duration.
        /// </summary>
        /// <param name="duration">Duration in seconds.</param>
        public void TurnOffLights(float duration)
        {
            foreach (RoomLightController light in RoomLightControllers)
            {
                light.ServerFlickerLights(duration);
            }
        }

        /// <summary>
        /// Locks all the doors in the room.
        /// </summary>
        /// <param name="lockType">DoorLockType of the lockdown.</param>
        public void LockDown(DoorLockType lockType = DoorLockType.Regular079)
        {
            foreach (Door door in Doors)
            {
                door.ChangeLock(lockType);
                door.IsOpen = false;
            }
        }

        /// <summary>
        /// Locks all the doors in the room.
        /// </summary>
        /// <param name="duration">Duration in seconds.</param>
        /// <param name="lockType">DoorLockType of the lockdown.</param>
        public void LockDown(float duration, DoorLockType lockType = DoorLockType.Regular079)
        {
            foreach (Door door in Doors)
            {
                door.ChangeLock(lockType);
                door.IsOpen = false;
                door.Unlock(duration, lockType);
            }
        }

        /// <summary>
        /// Locks all the doors and turns off all lights in the room.
        /// </summary>
        /// <param name="lockType">DoorLockType of the blackout.</param>
        /// <seealso cref="Map.TurnOffAllLights(float, ZoneType)"/>
        /// <seealso cref="Map.TurnOffAllLights(float, IEnumerable{ZoneType})"/>
        public void Blackout(DoorLockType lockType = DoorLockType.Regular079)
        {
            LockDown(lockType);
            TurnOffLights();
        }

        /// <summary>
        /// Locks all the doors and turns off all lights in the room.
        /// </summary>
        /// <param name="duration">Duration in seconds.</param>
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
        public void ResetColor() => Color = Color.clear;

        /// <summary>
        /// Returns the Room in a human-readable format.
        /// </summary>
        /// <returns>A string containing Room-related data.</returns>
        public override string ToString() => $"{Type} ({Zone}) [{Doors.Count}] *{Cameras.Count}* |{TeslaGate != null}|";

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

            if (transform && transform.parent)
            {
                return transform.parent.name.RemoveBracketsOnEndOfName() switch
                {
                    "HeavyRooms" => ZoneType.HeavyContainment,
                    "LightRooms" => ZoneType.LightContainment,
                    "EntranceRooms" => ZoneType.Entrance,
                    "HCZ_EZ_Checkpoint" => ZoneType.HeavyContainment | ZoneType.Entrance,
                    _ => transform.position.y > 900 ? ZoneType.Surface : ZoneType.Unspecified,
                };
            }

            return ZoneType.Unspecified;
        }
    }
}
