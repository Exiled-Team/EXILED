// -----------------------------------------------------------------------
// <copyright file="DoorTypeExtension.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Extensions
{
    using System.Collections.Generic;

    using Exiled.API.Enums;
    using Exiled.API.Features;

    /// <summary>
    /// Contains an extension method to get <see cref="DoorType"/> from <see cref="Door"/>.
    /// Internal class <see cref="RegisterDoorTypesOnLevelLoad"/> to cache the <see cref="DoorType"/> on level load.
    /// </summary>
    public static class DoorTypeExtension
    {
        private static readonly Dictionary<int, DoorType> OrderedDoorTypes = new Dictionary<int, DoorType>();

        /// <summary>
        /// Gets the <see cref="DoorType"/>.
        /// </summary>
        /// <param name="door">The Door to check.</param>
        /// <returns>The <see cref="DoorType"/>.</returns>
        public static DoorType Type(this Door door) => OrderedDoorTypes.TryGetValue(door.GetInstanceID(), out var doorType) ? doorType : DoorType.UnknownDoor;

        /// <summary>
        /// Gets all the <see cref="DoorType"/> values for for the <see cref="Door"/> instances using <see cref="Door.DoorName"/> and <see cref="UnityEngine.GameObject"/> name.
        /// </summary>
        internal static void RegisterDoorTypesOnLevelLoad()
        {
            OrderedDoorTypes.Clear();

            var doors = Map.Doors;

            if (doors == null)
                return;

            var doorCount = doors.Count;
            for (int i = 0; i < doorCount; i++)
            {
                var door = doors[i];
                var doorID = door.GetInstanceID();

                var doorName = string.IsNullOrWhiteSpace(door.DoorName) ? door.name.RemoveBracketsOnEndOfName() : door.DoorName;

                var doorType = GetDoorType(doorName);

                OrderedDoorTypes.Add(doorID, doorType);
            }
        }

        private static DoorType GetDoorType(string doorName)
        {
            switch (doorName)
            {
                case "012":
                    return DoorType.Scp012;
                case "012_BOTTOM":
                    return DoorType.Scp012Bottom;
                case "012_LOCKER":
                    return DoorType.Scp012Locker;
                case "049_ARMORY":
                    return DoorType.Scp049Armory;
                case "079_FIRST":
                    return DoorType.Scp079First;
                case "079_SECOND":
                    return DoorType.Scp079Second;
                case "096":
                    return DoorType.Scp096;
                case "106_BOTTOM":
                    return DoorType.Scp106Bottom;
                case "106_PRIMARY":
                    return DoorType.Scp106Primary;
                case "106_SECONDARY":
                    return DoorType.Scp106Secondary;
                case "173":
                    return DoorType.Scp173;
                case "173_ARMORY":
                    return DoorType.Scp173Armory;
                case "173_BOTTOM":
                    return DoorType.Scp173Bottom;
                case "372":
                    return DoorType.Scp372;
                case "914":
                    return DoorType.Scp914;
                case "Airlocks":
                    return DoorType.Airlocks;
                case "CHECKPOINT_ENT":
                    return DoorType.CheckpointEntrance;
                case "CHECKPOINT_LCZ_A":
                    return DoorType.CheckpointLczA;
                case "CHECKPOINT_LCZ_B":
                    return DoorType.CheckpointLczB;
                case "ContDoor":
                    return DoorType.ContDoor;
                case "EntrDoor":
                    return DoorType.EntranceDoor;
                case "ESCAPE":
                    return DoorType.Escape;
                case "ESCAPE_INNER":
                    return DoorType.EscapeInner;
                case "GATE_A":
                    return DoorType.GateA;
                case "GATE_B":
                    return DoorType.GateB;
                case "HCZ_ARMORY":
                    return DoorType.HczArmory;
                case "HeavyContainmentDoor":
                    return DoorType.HeavyContainmentDoor;
                case "HID":
                    return DoorType.HID;
                case "HID_LEFT":
                    return DoorType.HIDLeft;
                case "HID_RIGHT":
                    return DoorType.HIDRight;
                case "INTERCOM":
                    return DoorType.Intercom;
                case "LCZ_ARMORY":
                    return DoorType.LczArmory;
                case "LCZ_CAFE":
                    return DoorType.LczCafe;
                case "LCZ_WC":
                    return DoorType.LczWc;
                case "LightContainmentDoor":
                    return DoorType.LightContainmentDoor;
                case "NUKE_ARMORY":
                    return DoorType.NukeArmory;
                case "NUKE_SURFACE":
                    return DoorType.NukeSurface;
                case "PrisonDoor":
                    return DoorType.PrisonDoor;
                case "SURFACE_GATE":
                    return DoorType.SurfaceGate;
                default:
                    return DoorType.UnknownDoor;
            }
        }
    }
}
