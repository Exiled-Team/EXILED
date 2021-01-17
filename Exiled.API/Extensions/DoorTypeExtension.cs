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

    using Interactables.Interobjects.DoorUtils;

#pragma warning disable SA1123 // Do not place regions within elements

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
        public static DoorType Type(this DoorVariant door) => OrderedDoorTypes.TryGetValue(door.GetInstanceID(), out var doorType) ? doorType : DoorType.UnknownDoor;

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

                var doorNameTag = door.GetComponent<DoorNametagExtension>();
                var doorName = doorNameTag == null ? door.name.RemoveBracketsOnEndOfName() : doorNameTag.GetName;

                var doorType = GetDoorType(doorName);

                OrderedDoorTypes.Add(doorID, doorType);
            }
        }

        private static DoorType GetDoorType(string doorName)
        {
            switch (doorName)
            {
                #region GameObject names

                case "Prison BreakableDoor":
                    return DoorType.PrisonDoor;

                #endregion

                // Doors contains the DoorNameTagExtension component
                #region Door names

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

                #endregion

                // Doors spawned by the DoorSpawnPoint component
                #region DoorSpawnPoint names

                case "LCZ_CAFE":
                    return DoorType.LczCafe;
                case "173_BOTTOM":
                    return DoorType.Scp173Bottom;

                #endregion

                // Doors contains the Door component,
                // also gameobject names
                #region Outdated doors

                case "LightContainmentDoor":
                    return DoorType.LightContainmentDoor;
                case "EntrDoor":
                    return DoorType.EntranceDoor;

                #endregion

                default:
                    // All door gameobject names are separated by a whitespace
                    doorName = doorName.GetBefore(' ');
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
