// -----------------------------------------------------------------------
// <copyright file="Door.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Exiled.API.Enums;
    using Exiled.API.Extensions;

    using Interactables.Interobjects;
    using Interactables.Interobjects.DoorUtils;

    using MEC;
    using Mirror;

    using UnityEngine;

    /// <summary>
    /// A wrapper class for <see cref="DoorVariant"/>.
    /// </summary>
    public class Door
    {
        /// <summary>
        /// A <see cref="Dictionary{TKey,TValue}"/> containing all known <see cref="DoorVariant"/>s and their corresponding <see cref="Door"/>.
        /// </summary>
        internal static readonly Dictionary<DoorVariant, Door> DoorVariantToDoor = new();

        /// <summary>
        /// Initializes a new instance of the <see cref="Door"/> class.
        /// </summary>
        /// <param name="door">The base <see cref="DoorVariant"/> for this door.</param>
        [Obsolete("Use new Door(DoorVariant, Room) instead.", true)]
        public Door(DoorVariant door)
        {
            DoorVariantToDoor.Add(door, this);
            Base = door;
            Room = door.GetComponentInParent<Room>();
            Type = GetDoorType();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Door"/> class.
        /// </summary>
        /// <param name="door">The base <see cref="DoorVariant"/> for this door.</param>
        /// <param name="room">The <see cref="Room"/> for this door.</param>
        public Door(DoorVariant door, Room room)
        {
            DoorVariantToDoor.Add(door, this);
            Base = door;
            Room = room;
            Type = GetDoorType();
        }

        /// <summary>
        /// Gets a <see cref="IEnumerable{T}"/> of <see cref="Door"/> which contains all the <see cref="Door"/> instances.
        /// </summary>
        public static IEnumerable<Door> List => DoorVariantToDoor.Values.ToList().AsReadOnly();

        /// <summary>
        /// Gets a <see cref="List{T}"/> of <see cref="Door"/> which contains all the <see cref="Door"/> instances.
        /// </summary>
        /// <summary>
        /// Gets the base-game <see cref="DoorVariant"/> for this door.
        /// </summary>
        public DoorVariant Base { get; }

        /// <summary>
        /// Gets the <see cref="UnityEngine.GameObject"/> of the door.
        /// </summary>
        public GameObject GameObject => Base.gameObject;

        /// <summary>
        /// Gets the <see cref="DoorType"/>.
        /// </summary>
        public DoorType Type { get; }

        /// <summary>
        /// Gets the <see cref="Room"/>.
        /// </summary>
        public Room Room { get; }

        /// <summary>
        /// Gets or sets a value indicating whether the door is open.
        /// </summary>
        public bool IsOpen
        {
            get => Base.IsConsideredOpen();
            set => Base.NetworkTargetState = value;
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
        /// Gets or sets a value indicating whether SCP-106 can walk through the door.
        /// </summary>
        public bool AllowsScp106
        {
            get => Base.UsedBy106;
            set => Base.UsedBy106 = value;
        }

        /// <summary>
        /// Gets a value indicating whether the door is locked.
        /// </summary>
        public bool IsLocked => DoorLockType > 0;

        /// <summary>
        /// Gets or the door lock type.
        /// </summary>
        public DoorLockType DoorLockType => (DoorLockType)Base.NetworkActiveLocks;

        /// <summary>
        /// Gets a value indicating whether or not this door is breakable.
        /// </summary>
        public bool IsBreakable => Base is IDamageableDoor dDoor && !dDoor.IsDestroyed;

        /// <summary>
        /// Gets a value indicating whether or not this door is broken.
        /// </summary>
        public bool IsBroken => Base is IDamageableDoor dDoor && dDoor.IsDestroyed;

        /// <summary>
        /// Gets the door's Instance ID.
        /// </summary>
        public int InstanceId => Base.GetInstanceID();

        /// <summary>
        /// Gets a nametag of a door.
        /// </summary>
        public string Nametag => Base.TryGetComponent(out DoorNametagExtension name) ? name.GetName : null;

        /// <summary>
        /// Gets or sets the required permissions to open the door.
        /// </summary>
        public DoorPermissions RequiredPermissions
        {
            get => Base.RequiredPermissions;
            set => Base.RequiredPermissions = value;
        }

        /// <summary>
        /// Gets or sets the max health of the door. No effect if the door cannot be broken.
        /// </summary>
        public float MaxHealth
        {
            get => Base is BreakableDoor breakable ? breakable._maxHealth : float.NaN;
            set
            {
                if (Base is BreakableDoor breakable)
                    breakable._maxHealth = value;
            }
        }

        /// <summary>
        /// Gets or sets the door's remaining health. No effect if the door cannot be broken.
        /// </summary>
        public float Health
        {
            get => Base is BreakableDoor breakable ? breakable._remainingHealth : float.NaN;
            set
            {
                if (Base is BreakableDoor breakable)
                    breakable._remainingHealth = value;
            }
        }

        /// <summary>
        /// Gets or sets the damage types this door ignores, if it is breakable.
        /// </summary>
        public DoorDamageType IgnoredDamageTypes
        {
            get => Base is BreakableDoor breakable ? breakable._ignoredDamageSources : DoorDamageType.None;
            set
            {
                if (Base is BreakableDoor breakable)
                    breakable._ignoredDamageSources = value;
            }
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
        /// Gets the door object associated with a specific <see cref="DoorVariant"/>, or creates a new one if there isn't one.
        /// </summary>
        /// <param name="doorVariant">The base-game <see cref="DoorVariant"/>.</param>
        /// <returns>A <see cref="Door"/> wrapper object.</returns>
        public static Door Get(DoorVariant doorVariant) => DoorVariantToDoor.ContainsKey(doorVariant)
            ? DoorVariantToDoor[doorVariant]
            : new Door(doorVariant, doorVariant.GetComponentInParent<Room>());

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
        /// Gets a random <see cref="Door"/>.
        /// </summary>
        /// <param name="type">Filters by <see cref="ZoneType"/>.</param>
        /// <param name="onlyUnbroken">A value indicating whether it filters broken doors.</param>
        /// <returns><see cref="Door"/> object.</returns>
        public static Door Random(ZoneType type = ZoneType.Unspecified, bool onlyUnbroken = false)
        {
            List<Door> doors = onlyUnbroken || type is not ZoneType.Unspecified ? Get(x => (x.Room is null || x.Room.Zone == type || type == ZoneType.Unspecified) && (!x.IsBroken || !onlyUnbroken)).ToList() : DoorVariantToDoor.Values.ToList();
            return doors[UnityEngine.Random.Range(0, doors.Count)];
        }

        /// <summary>
        /// Locks all <see cref="Door">doors</see> given the specified <see cref="ZoneType"/>.
        /// </summary>
        /// <param name="duration">The duration of the lockdown.</param>
        /// <param name="zoneType">The <see cref="ZoneType"/> to affect.</param>
        /// <param name="lockType">The specified <see cref="DoorLockType"/>.</param>
        public static void LockAll(float duration, ZoneType zoneType = ZoneType.Unspecified, DoorLockType lockType = DoorLockType.Regular079)
        {
            foreach (Door door in Get(door => zoneType is not ZoneType.Unspecified && door.Zone == zoneType))
            {
                door.IsOpen = false;
                door.ChangeLock(lockType);
                Timing.CallDelayed(duration, () => door.ChangeLock(DoorLockType.None));
            }
        }

        /// <summary>
        /// Locks all <see cref="Door">doors</see> given the specified <see cref="ZoneType"/>.
        /// </summary>
        /// <param name="duration">The duration of the lockdown.</param>
        /// <param name="zoneTypes">The <see cref="ZoneType"/>s to affect.</param>
        /// <param name="lockType">The specified <see cref="DoorLockType"/>.</param>
        public static void LockAll(float duration, IEnumerable<ZoneType> zoneTypes, DoorLockType lockType = DoorLockType.Regular079)
        {
            foreach (ZoneType zone in zoneTypes)
                LockAll(duration, zone, lockType);
        }

        /// <summary>
        /// Locks all <see cref="Door">doors</see> in the facility.
        /// </summary>
        /// <param name="duration">The duration of the lockdown.</param>
        /// <param name="lockType">The specified <see cref="DoorLockType"/>.</param>
        public static void LockAll(float duration, DoorLockType lockType = DoorLockType.Regular079)
        {
            foreach (Door door in Door.List)
            {
                door.IsOpen = false;
                door.ChangeLock(lockType);
                Timing.CallDelayed(duration, () => door.ChangeLock(DoorLockType.None));
            }
        }

        /// <summary>
        /// Unlocks all <see cref="Door">doors</see> in the facility.
        /// </summary>
        public static void UnlockAll()
        {
            foreach (Door door in List)
                door.ChangeLock(DoorLockType.None);
        }

        /// <summary>
        /// Unlocks all <see cref="Door">doors</see> in the facility.
        /// </summary>
        /// <param name="zoneType">The <see cref="ZoneType"/> to affect.</param>
        public static void UnlockAll(ZoneType zoneType) => UnlockAll(door => door.Zone == zoneType);

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
                door.ChangeLock(DoorLockType.None);
        }

        /// <summary>
        /// Breaks the specified door. No effect if the door cannot be broken, or if it is already broken.
        /// </summary>
        /// <returns><see langword="true"/> if the door was broken, <see langword="false"/> if it was unable to be broken, or was already broken before.</returns>
        public bool BreakDoor()
        {
            if (Base is IDamageableDoor dmg && !dmg.IsDestroyed)
            {
                dmg.ServerDamage(ushort.MaxValue, DoorDamageType.ServerCommand);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Damages the door. No effect if the door cannot be broken.
        /// </summary>
        /// <param name="amount">The amount of damage to deal.</param>
        /// <param name="type">The damage type to use.</param>
        /// <returns><see langword="true"/> if the door was damaged.</returns>
        public bool DamageDoor(float amount, DoorDamageType type = DoorDamageType.ServerCommand) => Base is BreakableDoor breakable && breakable.ServerDamage(amount, type);

        /// <summary>
        /// Tries to pry the door open. No effect if the door cannot be pried.
        /// </summary>
        /// <returns><see langword="true"/> if the door was able to be pried open.</returns>
        public bool TryPryOpen() => Base is PryableDoor pryable && pryable.TryPryGate();

        /// <summary>
        /// Makes the door play a beep sound.
        /// </summary>
        /// <param name="beep">The beep sound to play.</param>
        public void PlaySound(DoorBeepType beep)
        {
            switch (Base)
            {
                case BasicDoor basic:
                    basic.RpcPlayBeepSound(beep != DoorBeepType.InteractionAllowed);
                    break;
                case CheckpointDoor chkPt:
                    chkPt.RpcPlayBeepSound((byte)Mathf.Min((int)beep, 3));
                    break;
            }
        }

        /// <summary>
        /// Locks the door with the given lock type.
        /// </summary>
        /// <param name="lockType">The <see cref="Enums.DoorLockType"/> to use.</param>
        public void ChangeLock(DoorLockType lockType)
        {
            if (lockType == DoorLockType.None)
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
        /// Unlocks and clears all active locks on the door.
        /// </summary>
        public void Unlock() => ChangeLock(DoorLockType.None);

        /// <summary>
        /// Unlocks and clears all active locks on the door after a specified length of time.
        /// </summary>
        /// <param name="time">The amount of time that must pass before unlocking the door.</param>
        /// <param name="flagsToUnlock">The door.</param>
        public void Unlock(float time, DoorLockType flagsToUnlock) => DoorScheduledUnlocker.UnlockLater(Base, time, (DoorLockReason)flagsToUnlock);

        /// <summary>
        /// Locks all active locks on the door, and then reverts back any changes after a specified length of time.
        /// </summary>
        /// <param name="time">The amount of time that must pass before unlocking the door.</param>
        /// <param name="flagsToUnlock">The door.</param>
        public void Lock(float time, DoorLockType flagsToUnlock)
        {
            ChangeLock(flagsToUnlock);
            DoorScheduledUnlocker.UnlockLater(Base, time, (DoorLockReason)flagsToUnlock);
        }

        /// <summary>
        /// Gets the door object associated with a specific <see cref="DoorVariant"/>, or creates a new one if there isn't one.
        /// </summary>
        /// <param name="doorVariant">The base-game <see cref="DoorVariant"/>.</param>
        /// <param name="room">The <see cref="Room"/> this door is in.</param>
        /// <remarks>The 'room' parameter is only used if a new door wrapper needs to be created.</remarks>
        /// <returns>A <see cref="Door"/> wrapper object.</returns>
        internal static Door Get(DoorVariant doorVariant, Room room) => DoorVariantToDoor.ContainsKey(doorVariant)
            ? DoorVariantToDoor[doorVariant]
            : new Door(doorVariant, room);

        private DoorType GetDoorType()
        {
            if (Nametag is null)
            {
                string doorName = GameObject.name.GetBefore(' ');
                return doorName switch
                {
                    "LCZ" => DoorType.LightContainmentDoor,
                    "HCZ" => DoorType.HeavyContainmentDoor,
                    "EZ" => DoorType.EntranceDoor,
                    "Prison" => DoorType.PrisonDoor,
                    "914" => DoorType.Scp914Door,
                    "Unsecured" => DoorType.Scp049Gate,
                    _ => DoorType.UnknownDoor,
                };
            }

            return Nametag.RemoveBracketsOnEndOfName() switch
            {
                // Doors contains the DoorNameTagExtension component
                "CHECKPOINT_LCZ_A" => DoorType.CheckpointLczA,
                "CHECKPOINT_EZ_HCZ" => DoorType.CheckpointEntrance,
                "CHECKPOINT_LCZ_B" => DoorType.CheckpointLczB,
                "106_PRIMARY" => DoorType.Scp106Primary,
                "106_SECONDARY" => DoorType.Scp106Secondary,
                "106_BOTTOM" => DoorType.Scp106Bottom,
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
