// -----------------------------------------------------------------------
// <copyright file="Door.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    using Exiled.API.Enums;
    using Exiled.API.Extensions;

    using Interactables.Interobjects.DoorUtils;

    /// <summary>
    /// A wrapper class for <see cref="DoorVariant"/>.
    /// </summary>
    public class Door
    {
        private static readonly Dictionary<int, DoorType> OrderedDoorTypes = new Dictionary<int, DoorType>();
        private static readonly Dictionary<DoorVariant, Door> DoorVariantToDoor = new Dictionary<DoorVariant, Door>();

        /// <summary>
        /// Initializes a new instance of the <see cref="Door"/> class.
        /// </summary>
        /// <param name="door"><inheritdoc cref="Base"/></param>
        public Door(DoorVariant door)
        {
            DoorVariantToDoor.Add(door, this);
            Base = door;
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
        /// Gets a value indicating whether or not this door is breakable.
        /// </summary>
        public bool IsBreakable => Base is IDamageableDoor dDoor && !dDoor.IsDestroyed;

        /// <summary>
        /// Gets the door's Instance ID.
        /// </summary>
        public int InstanceId => Base.GetInstanceID();

        /// <summary>
        /// Gets a nametag of a door.
        /// </summary>
        public string Nametag => Base.TryGetComponent<DoorNametagExtension>(out var name) ? name.GetName : null;

        /// <summary>
        /// Gets the door object associated with a specific <see cref="DoorVariant"/>, or creates a new one if there isn't one.
        /// </summary>
        /// <param name="doorVariant"><inheritdoc cref="Base"/></param>
        /// <returns><inheritdoc cref="Door"/></returns>
        public static Door Get(DoorVariant doorVariant) => DoorVariantToDoor.ContainsKey(doorVariant)
            ? DoorVariantToDoor[doorVariant]
            : new Door(doorVariant);

        /// <summary>
        /// Breaks the specified door, if it is not already broken.
        /// </summary>
        /// <param name="door">The <see cref="DoorVariant"/> to break.</param>
        /// <returns>True if the door was broken, false if it was unable to be broken, or was already broken before.</returns>
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
        /// Gets all the <see cref="DoorType"/> values for the <see cref="Door"/> instances using <see cref="Door"/> and <see cref="UnityEngine.GameObject"/> name.
        /// </summary>
        internal static void RegisterDoorTypesOnLevelLoad()
        {
            OrderedDoorTypes.Clear();
            ReadOnlyCollection<Door> doors = Map.Doors;

            int doorCount = doors.Count;
            for (int i = 0; i < doorCount; i++)
            {
                Door door = doors[i];
                int doorID = door.InstanceId;

                DoorType doorType = door.GetDoorType();

                OrderedDoorTypes.Add(doorID, doorType);
            }
        }

        private DoorType GetDoorType()
        {
            switch (Nametag.RemoveBracketsOnEndOfName())
            {
                case "Prison BreakableDoor":
                    return DoorType.PrisonDoor;

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
                    // All door gameobject names are separated by a whitespace
                    string doorName = Nametag.GetBefore(' ');
                    switch (doorName)
                    {
                        case "LCZ":
                            return DoorType.LightContainmentDoor;
                        case "HCZ":
                            return DoorType.HeavyContainmentDoor;
                        case "EZ":
                            return DoorType.EntranceDoor;
                        default:
                            return DoorType.UnknownDoor;
                    }
            }
        }
    }
}
