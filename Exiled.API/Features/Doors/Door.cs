// -----------------------------------------------------------------------
// <copyright file="Door.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Doors
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.Linq;

    using Exiled.API.Enums;
    using Exiled.API.Extensions;
    using Exiled.API.Features.Core;
    using Exiled.API.Interfaces;
    using Interactables.Interobjects;
    using Interactables.Interobjects.DoorUtils;
    using MEC;
    using Mirror;
    using UnityEngine;

    using static Interactables.Interobjects.ElevatorManager;

    using BaseBreakableDoor = Interactables.Interobjects.BreakableDoor;
    using BaseKeycardPermissions = Interactables.Interobjects.DoorUtils.KeycardPermissions;
    using Breakable = BreakableDoor;
    using Checkpoint = CheckpointDoor;
    using Elevator = ElevatorDoor;
    using KeycardPermissions = Enums.KeycardPermissions;

    /// <summary>
    /// A wrapper class for <see cref="DoorVariant"/>.
    /// </summary>
    public class Door : TypeCastObject<Door>, IWrapper<DoorVariant>, IWorldSpace
    {
        /// <summary>
        /// A <see cref="Dictionary{TKey,TValue}"/> containing all known <see cref="DoorVariant"/>'s and their corresponding <see cref="Door"/>.
        /// </summary>
        internal static readonly Dictionary<DoorVariant, Door> DoorVariantToDoor = new();

        /// <summary>
        /// Initializes a new instance of the <see cref="Door"/> class.
        /// </summary>
        /// <param name="door">The base <see cref="DoorVariant"/> for this door.</param>
        /// <param name="rooms">The <see cref="List{T}"/> of <see cref="Features.Room"/>'s for this door.</param>
        internal Door(DoorVariant door, List<Room> rooms)
        {
            Base = door;

            if (rooms != null)
            {
                DoorVariantToDoor.Add(door, this);
                RoomsValue = rooms;
                Rooms = RoomsValue.AsReadOnly();
            }

            Type = GetDoorType();
#if Debug
            if (Type is DoorType.Unknown)
                Log.Error($"[DOORTYPE UNKNOWN] {this}");
#endif
        }

        /// <summary>
        /// Gets a <see cref="IEnumerable{T}"/> of <see cref="Door"/> which contains all the <see cref="Door"/> instances.
        /// </summary>
        public static IReadOnlyCollection<Door> List => DoorVariantToDoor.Values;

        /// <summary>
        /// Gets the base-game <see cref="DoorVariant"/> corresponding with this door.
        /// </summary>
        public DoorVariant Base { get; }

        /// <summary>
        /// Gets the door's <see cref="UnityEngine.GameObject"/>.
        /// </summary>
        public GameObject GameObject => Base.gameObject;

        /// <summary>
        /// Gets the door's <see cref="UnityEngine.Transform"/>.
        /// </summary>
        public Transform Transform => Base.transform;

        /// <summary>
        /// Gets the door's <see cref="DoorType"/>.
        /// </summary>
        public DoorType Type { get; }

        /// <summary>
        /// Gets the <see cref="Features.Room"/> that the door is located in.
        /// </summary>
        public Room Room => Rooms?.FirstOrDefault();

        /// <summary>
        /// Gets the <see cref="Features.Room"/>'s that the door is located in.
        /// </summary>
        public IReadOnlyCollection<Room> Rooms { get; }

        /// <summary>
        /// Gets a value indicating whether or not the door is fully closed.
        /// </summary>
        public bool IsFullyClosed => Is(out Gate gate) ? !IsOpen && gate.RemainingPryCooldown <= 0 : ExactState is 0;

        /// <summary>
        /// Gets a value indicating whether the door is fully open.
        /// </summary>
        public bool IsFullyOpen => Is(out Gate gate) ? !IsOpen && gate.RemainingPryCooldown <= 0 : ExactState is 1;

        /// <summary>
        /// Gets a value indicating whether or not the door is currently moving.
        /// </summary>
        public bool IsMoving => Is(out Gate gate) ? !IsOpen && gate.RemainingPryCooldown > 0 : ExactState is not(0 or 1);

        /// <summary>
        /// Gets a value indicating the precise state of the door, from <c>0-1</c>. A value of <c>0</c> indicates the door is fully closed, while a value of <c>1</c> indicates the door is fully open. Values in-between represent the door's animation progress.
        /// </summary>
        public float ExactState => Base.GetExactState();

        /// <summary>
        /// Gets or sets a value indicating whether the door is open.
        /// </summary>
        public bool IsOpen
        {
            get => Base.IsConsideredOpen();
            set => Base.NetworkTargetState = value;
        }

        /// <summary>
        /// Gets a value indicating whether or not this door is a gate.
        /// </summary>
        public bool IsGate => this is Gate;

        /// <summary>
        /// Gets a value indicating whether or not this door is a checkpoint door.
        /// </summary>
        public bool IsCheckpoint => this is Checkpoint;

        /// <summary>
        /// Gets a value indicating whether or not this door is an elevator door.
        /// </summary>
        public bool IsElevator => this is Elevator;

        /// <summary>
        /// Gets a value indicating whether or not this door can be damaged.
        /// </summary>
        public bool IsDamageable => this is Interfaces.IDamageableDoor;

        /// <summary>
        /// Gets a value indicating whether or not this door is non-interactable.
        /// </summary>
        public bool IsNonInteractable => this is Interfaces.INonInteractableDoor;

        /// <summary>
        /// Gets a value indicating whether or not this door is subdoor belonging to a checkpoint.
        /// </summary>
        public bool IsPartOfCheckpoint => List.Any(x => x is Checkpoint checkpoint && checkpoint.Subdoors.Contains(this));

        /// <summary>
        /// Gets a value indicating whether or not this door requires a keycard to open.
        /// </summary>
        /// <remarks>
        /// This value is <see langword="false"/> if <see cref="KeycardPermissions"/> is equal to <see cref="KeycardPermissions.None"/>.
        /// </remarks>
        public bool IsKeycardDoor => KeycardPermissions is not KeycardPermissions.None;

        /// <summary>
        /// Gets or sets the keycard permissions required to open the door.
        /// </summary>
        /// <remarks>
        /// Setting this value to <see cref="KeycardPermissions.None"/> will allow this door to be opened without a keycard.
        /// </remarks>
        public KeycardPermissions KeycardPermissions
        {
            get => (KeycardPermissions)RequiredPermissions.RequiredPermissions;
            set => RequiredPermissions.RequiredPermissions = (BaseKeycardPermissions)value;
        }

        /// <summary>
        /// Gets or sets the door's position.
        /// </summary>
        public Vector3 Position
        {
            get => GameObject.transform.position;
            set
            {
                NetworkServer.UnSpawn(GameObject);
                GameObject.transform.position = value;
                NetworkServer.Spawn(GameObject);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not SCP-106 can walk through the door.
        /// </summary>
        public bool AllowsScp106
        {
            get => Base is IScp106PassableDoor door && door.IsScp106Passable;
            set
            {
                if (Base is IScp106PassableDoor door)
                    door.IsScp106Passable = value;
            }
        }

        /// <summary>
        /// Gets a value indicating whether or not the door is locked.
        /// </summary>
        public bool IsLocked => DoorLockType > 0;

        /// <summary>
        /// Gets or sets the door lock type.
        /// </summary>
        public DoorLockType DoorLockType
        {
            get => (DoorLockType)Base.NetworkActiveLocks;
            set => ChangeLock(value);
        }

        /// <summary>
        /// Gets the door's Instance ID.
        /// </summary>
        public int InstanceId => Base.GetInstanceID();

        /// <summary>
        /// Gets a nametag of a door.
        /// </summary>
        public DoorNametagExtension Nametag => Base.GetComponent<DoorNametagExtension>();

        /// <summary>
        /// Gets the name of this door.
        /// </summary>
        public string Name => Nametag == null ? GameObject.name.GetBefore(' ') : Nametag.GetName.RemoveBracketsOnEndOfName();

        /// <summary>
        /// Gets or sets the required permissions to open the door.
        /// </summary>
        public DoorPermissions RequiredPermissions
        {
            get => Base.RequiredPermissions;
            set => Base.RequiredPermissions = value;
        }

        /// <summary>
        /// Gets or sets the door's rotation.
        /// </summary>
        public Quaternion Rotation
        {
            get => GameObject.transform.rotation;
            set
            {
                NetworkServer.UnSpawn(GameObject);
                GameObject.transform.rotation = value;
                NetworkServer.Spawn(GameObject);
            }
        }

        /// <summary>
        /// Gets or sets the size scale of the door.
        /// </summary>
        public Vector3 Scale
        {
            get => GameObject.transform.localScale;
            set
            {
                NetworkServer.UnSpawn(GameObject);
                GameObject.transform.localScale = value;
                NetworkServer.Spawn(GameObject);
            }
        }

        /// <summary>
        /// Gets the door's <see cref="ZoneType"/>.
        /// </summary>
        public ZoneType Zone => Room?.Zone ?? ZoneType.Unspecified;

        /// <summary>
        /// Gets a <see cref="List{T}"/> containing all <see cref="Features.Room"/>'s that are connected with <see cref="Door"/>.
        /// </summary>
        internal List<Room> RoomsValue { get; } = new List<Room>();

        /// <summary>
        /// Gets the door object associated with a specific <see cref="DoorVariant"/>, or creates a new one if there isn't one.
        /// </summary>
        /// <param name="doorVariant">The base-game <see cref="DoorVariant"/>.</param>
        /// <returns>A <see cref="Door"/> wrapper object.</returns>
        public static Door Get(DoorVariant doorVariant)
        {
            if (doorVariant == null)
                return null;

            if (doorVariant.Rooms == null)
            {
                doorVariant.RegisterRooms();
            }

            // Exiled door must be created after the `RegisterRooms` call
            return DoorVariantToDoor[doorVariant];
        }

        /// <summary>
        /// Gets a <see cref="Door"/> given the specified name.
        /// </summary>
        /// <param name="name">The name to search for.</param>
        /// <returns>The <see cref="Door"/> with the given name or <see langword="null"/> if not found.</returns>
        public static Door Get(string name)
        {
            DoorNametagExtension.NamedDoors.TryGetValue(name, out DoorNametagExtension nameExtension);
            return nameExtension is null ? null : Get(nameExtension.TargetDoor);
        }

        /// <summary>
        /// Gets the door object associated with a specific <see cref="UnityEngine.GameObject"/>, or creates a new one if there isn't one.
        /// </summary>
        /// <param name="gameObject">The base-game <see cref="UnityEngine.GameObject"/>.</param>
        /// <returns>The <see cref="Door"/> with the given name or <see langword="null"/> if not found.</returns>
        public static Door Get(GameObject gameObject) => gameObject is null ? null : Get(gameObject.GetComponentInChildren<DoorVariant>());

        /// <summary>
        /// Gets a <see cref="IEnumerable{T}"/> of <see cref="Door"/> filtered based on a predicate.
        /// </summary>
        /// <param name="predicate">The condition to satify.</param>
        /// <returns>A <see cref="IEnumerable{T}"/> of <see cref="Door"/> which contains elements that satify the condition.</returns>
        public static IEnumerable<Door> Get(Func<Door, bool> predicate) => List.Where(predicate);

        /// <summary>
        /// Gets a <see cref="Door"/> given the specified <see cref="DoorType"/>.
        /// </summary>
        /// <param name="doorType">The <see cref="DoorType"/> to search for.</param>
        /// <returns>The <see cref="Door"/> with the given <see cref="DoorType"/> or <see langword="null"/> if not found.</returns>
        public static Door Get(DoorType doorType) => List.FirstOrDefault(x => x.Type == doorType);

        /// <summary>
        /// Returns the closest <see cref="Door"/> to the given <paramref name="position"/>.
        /// </summary>
        /// <param name="position">The <see cref="Vector3">position</see> to find the closest door to.</param>
        /// <param name="distance">The distance between the door and the point.</param>
        /// <returns>The door closest to the provided position.</returns>
        public static Door GetClosest(Vector3 position, out float distance)
        {
            Door doorToReturn = List.OrderBy(door => Vector3.Distance(position, door.Position)).FirstOrDefault();

            distance = Vector3.Distance(position, doorToReturn.Position);
            return doorToReturn;
        }

        /// <summary>
        /// Gets a random <see cref="Door"/>.
        /// </summary>
        /// <param name="type">Filters by <see cref="ZoneType"/>.</param>
        /// <param name="onlyUnbroken">A value indicating whether it filters broken doors.</param>
        /// <returns><see cref="Door"/> object.</returns>
        public static Door Random(ZoneType type = ZoneType.Unspecified, bool onlyUnbroken = false)
        {
            List<Door> doors = onlyUnbroken || type is not ZoneType.Unspecified ? Get(x => (x.Room is null || x.Room.Zone.HasFlag(type) || type == ZoneType.Unspecified) && (x is Breakable { IsDestroyed: true } || !onlyUnbroken)).ToList() : DoorVariantToDoor.Values.ToList();
            return doors[UnityEngine.Random.Range(0, doors.Count)];
        }

        /// <summary>
        /// Locks all <see cref="Door">doors</see> given the specified <see cref="ZoneType"/>.
        /// </summary>
        /// <param name="duration">The duration of the lockdown.</param>
        /// <param name="zoneType">The <see cref="ZoneType"/> to affect.</param>
        /// <param name="lockType">The specified <see cref="Enums.DoorLockType"/>.</param>
        public static void LockAll(float duration, ZoneType zoneType = ZoneType.Unspecified, DoorLockType lockType = DoorLockType.Regular079)
        {
            foreach (Door door in Get(door => zoneType is not ZoneType.Unspecified && door.Zone.HasFlag(zoneType)))
            {
                door.IsOpen = false;
                door.ChangeLock(lockType);
                Timing.CallDelayed(duration, () => door.Unlock());
            }
        }

        /// <summary>
        /// Locks all <see cref="Door">doors</see> given the specified <see cref="ZoneType"/>.
        /// </summary>
        /// <param name="duration">The duration of the lockdown.</param>
        /// <param name="zoneTypes">The <see cref="ZoneType"/>s to affect.</param>
        /// <param name="lockType">The specified <see cref="Enums.DoorLockType"/>.</param>
        public static void LockAll(float duration, IEnumerable<ZoneType> zoneTypes, DoorLockType lockType = DoorLockType.Regular079)
        {
            foreach (ZoneType zone in zoneTypes)
                LockAll(duration, zone, lockType);
        }

        /// <summary>
        /// Locks all <see cref="Door">doors</see> in the facility.
        /// </summary>
        /// <param name="duration">The duration of the lockdown.</param>
        /// <param name="lockType">The specified <see cref="Enums.DoorLockType"/>.</param>
        public static void LockAll(float duration, DoorLockType lockType = DoorLockType.Regular079)
        {
            foreach (Door door in List)
            {
                door.IsOpen = false;
                door.ChangeLock(lockType);
                Timing.CallDelayed(duration, () => door.Unlock());
            }
        }

        /// <summary>
        /// Unlocks all <see cref="Door">doors</see> in the facility.
        /// </summary>
        public static void UnlockAll()
        {
            foreach (Door door in List)
                door.Unlock();
        }

        /// <summary>
        /// Unlocks all <see cref="Door">doors</see> in the facility.
        /// </summary>
        /// <param name="zoneType">The <see cref="ZoneType"/> to affect.</param>
        public static void UnlockAll(ZoneType zoneType) => UnlockAll(door => door.Zone.HasFlag(zoneType));

        /// <summary>
        /// Unlocks all <see cref="Door">doors</see> in the facility.
        /// </summary>
        /// <param name="zoneTypes">The <see cref="ZoneType"/>s to affect.</param>
        public static void UnlockAll(IEnumerable<ZoneType> zoneTypes) => UnlockAll(door => zoneTypes.Contains(door.Zone));

        /// <summary>
        /// Unlocks all <see cref="Door">doors</see> in the facility.
        /// </summary>
        /// <param name="predicate">The condition to satify.</param>
        public static void UnlockAll(Func<Door, bool> predicate)
        {
            foreach (Door door in Get(predicate))
                door.Unlock();
        }

        /// <summary>
        /// Makes the door play a beep sound.
        /// </summary>
        /// <param name="beep">The beep sound to play.</param>
        public void PlaySound(DoorBeepType beep)
        {
            switch (Base)
            {
                case Interactables.Interobjects.BasicDoor basic:
                    basic.RpcPlayBeepSound(beep is not DoorBeepType.InteractionAllowed);
                    break;
                case Interactables.Interobjects.CheckpointDoor chkPt:
                    chkPt.RpcPlayBeepSound((byte)Mathf.Min((int)beep, 3));
                    break;
            }
        }

        /// <summary>
        /// Change the door lock with the given lock type.
        /// </summary>
        /// <param name="lockType">The <see cref="Enums.DoorLockType"/> to use.</param>
        public void ChangeLock(DoorLockType lockType)
        {
            if (lockType is DoorLockType.None)
            {
                Base.NetworkActiveLocks = 0;
            }
            else
            {
                DoorLockType locks = DoorLockType;
                if (locks.HasFlag(lockType))
                    locks &= ~lockType;
                else
                    locks |= lockType;

                Base.NetworkActiveLocks = (ushort)locks;
            }

            DoorEvents.TriggerAction(Base, IsLocked ? DoorAction.Locked : DoorAction.Unlocked, null);
        }

        /// <summary>
        /// Locks all active locks on the door, and then reverts back any changes after a specified length of time.
        /// </summary>
        /// <param name="time">The amount of time that must pass before unlocking the door.</param>
        /// <param name="lockType">The <see cref="Enums.DoorLockType"/> of the lockdown.</param>
        public void Lock(float time, DoorLockType lockType)
        {
            ChangeLock(lockType);
            Unlock(time, lockType);
        }

        /// <summary>
        /// Unlocks and clears all active locks on the door.
        /// </summary>
        public void Unlock() => ChangeLock(DoorLockType.None);

        /// <summary>
        /// Unlocks and clears all active locks on the door after a specified length of time.
        /// </summary>
        /// <param name="time">The amount of time that must pass before unlocking the door.</param>
        /// <param name="flagsToUnlock">The <see cref="Enums.DoorLockType"/> of the lockdown.</param>
        public void Unlock(float time, DoorLockType flagsToUnlock) => DoorScheduledUnlocker.UnlockLater(Base, time, (DoorLockReason)flagsToUnlock);

        /// <summary>
        /// Checks if specified <see cref="Player"/> can interact with the door.
        /// </summary>
        /// <param name="player">Player to check.</param>
        /// <returns><see langword="true"/> if the specified player can interact with the door. Otherwise, <see langword="false"/>.</returns>
        public bool IsAllowToInteract(Player player = null) => Base.AllowInteracting(player?.ReferenceHub, 0);

        /// <summary>
        /// Returns the Door in a human-readable format.
        /// </summary>
        /// <returns>A string containing Door-related data.</returns>
        public override string ToString() => $"{Type} ({Zone}) [{Room}] *{DoorLockType}* ={RequiredPermissions.RequiredPermissions}=";

        /// <summary>
        /// Creates the door object associated with a specific <see cref="DoorVariant"/>.
        /// </summary>
        /// <param name="doorVariant">The base-game <see cref="DoorVariant"/>.</param>
        /// <param name="rooms">Target door <see cref="Rooms"/>.</param>
        /// <returns>A <see cref="Door"/> wrapper object.</returns>
        internal static Door Create(DoorVariant doorVariant, List<Room> rooms)
        {
            if (doorVariant == null)
            {
                return null;
            }

            return doorVariant switch
            {
                Interactables.Interobjects.CheckpointDoor chkpt => new Checkpoint(chkpt, rooms),
                BaseBreakableDoor brkbl => new Breakable(brkbl, rooms),
                Interactables.Interobjects.ElevatorDoor elvtr => new Elevator(elvtr, rooms),
                PryableDoor prbl => new Gate(prbl, rooms),
                Interactables.Interobjects.BasicNonInteractableDoor nonInteractableDoor => new BasicNonInteractableDoor(nonInteractableDoor, rooms),
                Interactables.Interobjects.BasicDoor basicDoor => new BasicDoor(basicDoor, rooms),
                _ => new Door(doorVariant, rooms)
            };
        }

        private DoorType GetDoorType()
        {
            if (Nametag is null)
            {
                string doorName = GameObject.name.GetBefore(' ');
                return doorName switch
                {
                    "LCZ" => Room?.Type switch
                    {
                        RoomType.LczAirlock => (Base.GetComponentInParent<Interactables.Interobjects.AirlockController>() != null) ? DoorType.Airlock : DoorType.LightContainmentDoor,
                        _ => DoorType.LightContainmentDoor,
                    },
                    "HCZ" => DoorType.HeavyContainmentDoor,
                    "EZ" => DoorType.EntranceDoor,
                    "Prison" => DoorType.PrisonDoor,
                    "914" => DoorType.Scp914Door,
                    "Intercom" => Room?.Type switch
                    {
                        RoomType.HczEzCheckpointA => DoorType.CheckpointArmoryA,
                        RoomType.HczEzCheckpointB => DoorType.CheckpointArmoryB,
                        _ => DoorType.UnknownDoor,
                    },
                    "Unsecured" => Room?.Type switch
                    {
                        RoomType.EzCheckpointHallway => DoorType.CheckpointGate,
                        RoomType.Hcz049 => Position.y < -805 ? DoorType.Scp049Gate : DoorType.Scp173NewGate,
                        _ => DoorType.UnknownGate,
                    },
                    "Elevator" => (Base as Interactables.Interobjects.ElevatorDoor)?.Group switch
                    {
                        ElevatorGroup.Nuke => DoorType.ElevatorNuke,
                        ElevatorGroup.Scp049 => DoorType.ElevatorScp049,
                        ElevatorGroup.GateB => DoorType.ElevatorGateB,
                        ElevatorGroup.GateA => DoorType.ElevatorGateA,
                        ElevatorGroup.LczA01 or ElevatorGroup.LczA02 => DoorType.ElevatorLczA,
                        ElevatorGroup.LczB01 or ElevatorGroup.LczB02 => DoorType.ElevatorLczB,
                        _ => DoorType.UnknownElevator,
                    },
                    _ => DoorType.UnknownDoor,
                };
            }

            return Name.RemoveBracketsOnEndOfName() switch
            {
                // Doors contains the DoorNameTagExtension component
                "CHECKPOINT_LCZ_A" => DoorType.CheckpointLczA,
                "CHECKPOINT_LCZ_B" => DoorType.CheckpointLczB,
                "CHECKPOINT_EZ_HCZ_A" => DoorType.CheckpointEzHczA,
                "CHECKPOINT_EZ_HCZ_B" => DoorType.CheckpointEzHczB,
                "106_PRIMARY" => DoorType.Scp106Primary,
                "106_SECONDARY" => DoorType.Scp106Secondary,
                "ESCAPE_PRIMARY" => DoorType.EscapePrimary,
                "ESCAPE_SECONDARY" => DoorType.EscapeSecondary,
                "INTERCOM" => DoorType.Intercom,
                "NUKE_ARMORY" => DoorType.NukeArmory,
                "LCZ_ARMORY" => DoorType.LczArmory,
                "SURFACE_NUKE" => DoorType.NukeSurface,
                "HID" => DoorType.HID,
                "HCZ_ARMORY" => DoorType.HczArmory,
                "096" => DoorType.Scp096,
                "049_ARMORY" => DoorType.Scp049Armory,
                "079_ARMORY" => DoorType.Scp079Armory,
                "914" => DoorType.Scp914Gate,
                "GATE_A" => DoorType.GateA,
                "079_FIRST" => DoorType.Scp079First,
                "GATE_B" => DoorType.GateB,
                "079_SECOND" => DoorType.Scp079Second,
                "SERVERS_BOTTOM" => DoorType.ServersBottom,
                "173_CONNECTOR" => DoorType.Scp173Connector,
                "LCZ_WC" => DoorType.LczWc,
                "HID_RIGHT" => DoorType.HIDRight,
                "HID_LEFT" => DoorType.HIDLeft,
                "173_ARMORY" => DoorType.Scp173Armory,
                "173_GATE" => DoorType.Scp173Gate,
                "GR18" => DoorType.GR18Gate,
                "SURFACE_GATE" => DoorType.SurfaceGate,
                "330" => DoorType.Scp330,
                "330_CHAMBER" => DoorType.Scp330Chamber,
                "GR18_INNER" => DoorType.GR18Inner,
                "939_CRYO" => DoorType.Scp939Cryo,

                // Doors spawned by the DoorSpawnPoint component
                "LCZ_CAFE" => DoorType.LczCafe,
                "173_BOTTOM" => DoorType.Scp173Bottom,

                // Doors contains the Door component,
                // also gameobject names
                "LightContainmentDoor" => DoorType.LightContainmentDoor,
                "EntrDoor" => DoorType.EntranceDoor,

                _ => DoorType.UnknownDoor,
            };
        }
    }
}
