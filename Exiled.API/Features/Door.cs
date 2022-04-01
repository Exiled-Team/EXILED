// -----------------------------------------------------------------------
// <copyright file="Door.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features {
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    using Exiled.API.Enums;
    using Exiled.API.Extensions;

    using Interactables.Interobjects;
    using Interactables.Interobjects.DoorUtils;

    using Mirror;

    using UnityEngine;

    /// <summary>
    /// A wrapper class for <see cref="DoorVariant"/>.
    /// </summary>
    public class Door {
        private static readonly Dictionary<int, DoorType> OrderedDoorTypes = new Dictionary<int, DoorType>();
        private static readonly Dictionary<DoorVariant, Door> DoorVariantToDoor = new Dictionary<DoorVariant, Door>();

        /// <summary>
        /// Initializes a new instance of the <see cref="Door"/> class.
        /// </summary>
        /// <param name="door"><inheritdoc cref="Base"/></param>
        public Door(DoorVariant door) {
            DoorVariantToDoor.Add(door, this);
            Base = door;
            Room = door.GetComponentInParent<Room>();
        }

        /// <summary>
        /// Gets the base-game <see cref="DoorVariant"/> for this door.
        /// </summary>
        public DoorVariant Base { get; }

        /// <summary>
        /// Gets the <see cref="DoorType"/>.
        /// </summary>
        public DoorType Type => OrderedDoorTypes.TryGetValue(Base.GetInstanceID(), out DoorType doorType)
            ? doorType
            : DoorType.UnknownDoor;

        /// <summary>
        /// Gets the <see cref="Room"/>.
        /// </summary>
        public Room Room { get; }

        /// <summary>
        /// Gets or sets a value indicating whether the door is open.
        /// </summary>
        public bool IsOpen {
            get => Base.IsConsideredOpen();
            set => Base.NetworkTargetState = value;
        }

        /// <summary>
        /// Gets or sets the door's position.
        /// </summary>
        public Vector3 Position {
            get => Base.gameObject.transform.position;
            set {
                GameObject gameObject = Base.gameObject;
                NetworkServer.UnSpawn(gameObject);
                gameObject.transform.position = value;
                NetworkServer.Spawn(gameObject);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether SCP-106 can walk through the door.
        /// </summary>
        public bool AllowsScp106 {
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
        public DoorPermissions RequiredPermissions {
            get => Base.RequiredPermissions;
            set => Base.RequiredPermissions = value;
        }

        /// <summary>
        /// Gets or sets the max health of the door. No effect if the door cannot be broken.
        /// </summary>
        public float MaxHealth {
            get => Base is BreakableDoor breakable ? breakable._maxHealth : float.NaN;
            set {
                if (Base is BreakableDoor breakable)
                    breakable._maxHealth = value;
            }
        }

        /// <summary>
        /// Gets or sets the door's remaining health. No effect if the door cannot be broken.
        /// </summary>
        public float Health {
            get => Base is BreakableDoor breakable ? breakable._remainingHealth : float.NaN;
            set {
                if (Base is BreakableDoor breakable)
                    breakable._remainingHealth = value;
            }
        }

        /// <summary>
        /// Gets or sets the damage types this door ignores, if it is breakable.
        /// </summary>
        public DoorDamageType IgnoredDamageTypes {
            get => Base is BreakableDoor breakable ? breakable._ignoredDamageSources : DoorDamageType.None;
            set {
                if (Base is BreakableDoor breakable)
                    breakable._ignoredDamageSources = value;
            }
        }

        /// <summary>
        /// Gets or sets the door's rotation.
        /// </summary>
        public Quaternion Rotation {
            get => Base.gameObject.transform.rotation;
            set {
                GameObject gameObject = Base.gameObject;
                NetworkServer.UnSpawn(gameObject);
                gameObject.transform.rotation = value;
                NetworkServer.Spawn(gameObject);
            }
        }

        /// <summary>
        /// Gets or sets the size scale of the door.
        /// </summary>
        public Vector3 Scale {
            get => Base.gameObject.transform.localScale;
            set {
                GameObject gameObject = Base.gameObject;
                NetworkServer.UnSpawn(gameObject);
                gameObject.transform.localScale = value;
                NetworkServer.Spawn(gameObject);
            }
        }

        /// <summary>
        /// Gets the door object associated with a specific <see cref="DoorVariant"/>, or creates a new one if there isn't one.
        /// </summary>
        /// <param name="doorVariant"><inheritdoc cref="Base"/></param>
        /// <returns><inheritdoc cref="Door"/></returns>
        public static Door Get(DoorVariant doorVariant) => DoorVariantToDoor.ContainsKey(doorVariant)
            ? DoorVariantToDoor[doorVariant]
            : new Door(doorVariant);

        /// <summary>
        /// Breaks the specified door. No effect if the door cannot be broken, or if it is already broken.
        /// </summary>
        /// <returns><see langword="true"/> if the door was broken, <see langword="false"/> if it was unable to be broken, or was already broken before.</returns>
        public bool BreakDoor() {
            if (Base is IDamageableDoor dmg && !dmg.IsDestroyed) {
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
        public void PlaySound(DoorBeepType beep) {
            switch (Base) {
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
        /// <param name="lockType"><inheritdoc cref="DoorLockType"/></param>
        public void ChangeLock(DoorLockType lockType) {
            if (lockType == DoorLockType.None) {
                Base.NetworkActiveLocks = 0;
            }
            else {
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
        /// Gets all the <see cref="DoorType"/> values for the <see cref="Door"/> instances using <see cref="Door"/> and <see cref="UnityEngine.GameObject"/> name.
        /// </summary>
        internal static void RegisterDoorTypesOnLevelLoad() {
            OrderedDoorTypes.Clear();
            ReadOnlyCollection<Door> doors = Map.Doors;

            int doorCount = doors.Count;
            for (int i = 0; i < doorCount; i++) {
                Door door = doors[i];
                int doorID = door.InstanceId;

                DoorType doorType = door.GetDoorType();

                OrderedDoorTypes.Add(doorID, doorType);
            }
        }

        private DoorType GetDoorType() {
            if (Nametag == null) {
                string doorName = Base.gameObject.name.GetBefore(' ');
                switch (doorName) {
                    case "LCZ":
                        return DoorType.LightContainmentDoor;
                    case "HCZ":
                        return DoorType.HeavyContainmentDoor;
                    case "EZ":
                        return DoorType.EntranceDoor;
                    case "Prison":
                        return DoorType.PrisonDoor;
                    default:
                        return DoorType.UnknownDoor;
                }
            }

            switch (Nametag.RemoveBracketsOnEndOfName()) {
                // Doors contains the DoorNameTagExtension component
                case "CHECKPOINT_LCZ_A":
                    return DoorType.CheckpointLczA;
                case "CHECKPOINT_EZ_HCZ":
                    return DoorType.CheckpointEntrance;
                case "CHECKPOINT_LCZ_B":
                    return DoorType.CheckpointLczB;
                case "106_PRIMARY":
                    return DoorType.Scp106Primary;
                case "106_SECONDARY":
                    return DoorType.Scp106Secondary;
                case "106_BOTTOM":
                    return DoorType.Scp106Bottom;
                case "ESCAPE_PRIMARY":
                    return DoorType.EscapePrimary;
                case "ESCAPE_SECONDARY":
                    return DoorType.EscapeSecondary;
                case "INTERCOM":
                    return DoorType.Intercom;
                case "NUKE_ARMORY":
                    return DoorType.NukeArmory;
                case "LCZ_ARMORY":
                    return DoorType.LczArmory;
                case "012":
                    return DoorType.Scp012;
                case "SURFACE_NUKE":
                    return DoorType.NukeSurface;
                case "HID":
                    return DoorType.HID;
                case "HCZ_ARMORY":
                    return DoorType.HczArmory;
                case "096":
                    return DoorType.Scp096;
                case "049_ARMORY":
                    return DoorType.Scp049Armory;
                case "914":
                    return DoorType.Scp914;
                case "GATE_A":
                    return DoorType.GateA;
                case "079_FIRST":
                    return DoorType.Scp079First;
                case "GATE_B":
                    return DoorType.GateB;
                case "079_SECOND":
                    return DoorType.Scp079Second;
                case "012_LOCKER":
                    return DoorType.Scp012Locker;
                case "SERVERS_BOTTOM":
                    return DoorType.ServersBottom;
                case "173_CONNECTOR":
                    return DoorType.Scp173Connector;
                case "LCZ_WC":
                    return DoorType.LczWc;
                case "HID_RIGHT":
                    return DoorType.HIDRight;
                case "012_BOTTOM":
                    return DoorType.Scp012Bottom;
                case "HID_LEFT":
                    return DoorType.HIDLeft;
                case "173_ARMORY":
                    return DoorType.Scp173Armory;
                case "173_GATE":
                    return DoorType.Scp173Gate;
                case "GR18":
                    return DoorType.GR18;
                case "SURFACE_GATE":
                    return DoorType.SurfaceGate;

                // Doors spawned by the DoorSpawnPoint component
                case "LCZ_CAFE":
                    return DoorType.LczCafe;
                case "173_BOTTOM":
                    return DoorType.Scp173Bottom;

                // Doors contains the Door component,
                // also gameobject names
                case "LightContainmentDoor":
                    return DoorType.LightContainmentDoor;
                case "EntrDoor":
                    return DoorType.EntranceDoor;
                default:
                    return DoorType.UnknownDoor;
            }
        }
    }
}
