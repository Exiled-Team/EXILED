// -----------------------------------------------------------------------
// <copyright file="SpawnExtensions.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Extensions
{
    using Exiled.CustomItems.API;

    using Interactables.Interobjects.DoorUtils;

    using UnityEngine;

    /// <summary>
    /// A set of extensions for <see cref="SpawnLocation"/>.
    /// </summary>
    public static class SpawnExtensions
    {
        /// <summary>
        /// The names of spawn locations who's positions are on the opposite side of their door, and must be corrected.
        /// </summary>
        public static readonly SpawnLocation[] ReversedLocations =
        {
            SpawnLocation.InsideServersBottom,
            SpawnLocation.InsideHczArmory,
            SpawnLocation.Inside079First,
            SpawnLocation.InsideHidRight,
            SpawnLocation.Inside173Gate,
            SpawnLocation.InsideHidLeft,
            SpawnLocation.InsideGateA,
            SpawnLocation.InsideGateB,
            SpawnLocation.InsideLczWc,
            SpawnLocation.InsideGr18,
            SpawnLocation.Inside914,
            SpawnLocation.InsideHid,
        };

        /// <summary>
        /// Tries to get the <see cref="Transform"/> of the door used for a specific <see cref="SpawnLocation"/>.
        /// </summary>
        /// <param name="location">The <see cref="SpawnLocation"/> to check.</param>
        /// <returns>The <see cref="Transform"/> used for that spawn location. Can be <see langword="null"/>.</returns>
        public static Transform GetDoor(this SpawnLocation location)
        {
            string doorName = location.GetDoorName();

            if (string.IsNullOrEmpty(doorName))
                return null;

            return DoorNametagExtension.NamedDoors.TryGetValue(doorName, out DoorNametagExtension nametag) ? nametag.transform : null;
        }

        /// <summary>
        /// Tries to get the <see cref="Vector3"/> used for a specific <see cref="SpawnLocation"/>.
        /// </summary>
        /// <param name="location">The <see cref="SpawnLocation"/> to check.</param>
        /// <returns>The <see cref="Vector3"/> used for that spawn location. Can be <see cref="Vector3.zero"/>.</returns>
        public static Vector3 GetPosition(this SpawnLocation location)
        {
            Transform transform = location.GetDoor();

            if (transform is null)
                return default;

            // Returns a value that is offset from the door's location.
            // The vector3.up * 1.5 is added to ensure they do not spawn inside the floor and get stuck.
            // The transform.forward is added to make them actually spawn INSIDE the room instead of inside the door.
            // ReversedLocations is a list of doors which are facing the wrong way, putting transform.forward outside the room, instead of inside, which means we need to take the negative of that offset to be where we want.
            return transform.position + (Vector3.up * 1.5f) + (transform.forward * (ReversedLocations.Contains(location) ? -3f : 3f));
        }

        /// <summary>
        /// The names of the doors attached to each spawn location.
        /// </summary>
        /// <param name="spawnLocation">The <see cref="SpawnLocation"/>.</param>
        /// <returns>Returns the door name.</returns>
        public static string GetDoorName(this SpawnLocation spawnLocation)
        {
            switch (spawnLocation)
            {
                case SpawnLocation.Inside012:
                    return "012";
                case SpawnLocation.Inside096:
                    return "096";
                case SpawnLocation.Inside914:
                    return "914";
                case SpawnLocation.InsideHid:
                    return "HID";
                case SpawnLocation.InsideGr18:
                    return "GR18";
                case SpawnLocation.InsideGateA:
                    return "GATE_A";
                case SpawnLocation.InsideGateB:
                    return "GATE_B";
                case SpawnLocation.InsideLczWc:
                    return "LCZ_WC";
                case SpawnLocation.InsideHidLeft:
                    return "HID_LEFT";
                case SpawnLocation.InsideLczCafe:
                    return "LCZ_CAFE";
                case SpawnLocation.Inside173Gate:
                    return "173_GATE";
                case SpawnLocation.InsideIntercom:
                    return "INTERCOM";
                case SpawnLocation.InsideHidRight:
                    return "HID_RIGHT";
                case SpawnLocation.Inside079First:
                    return "079_FIRST";
                case SpawnLocation.Inside012Bottom:
                    return "012_BOTTOM";
                case SpawnLocation.Inside012Locker:
                    return "012_LOCKER";
                case SpawnLocation.Inside049Armory:
                    return "049_ARMORY";
                case SpawnLocation.Inside173Armory:
                    return "173_ARMORY";
                case SpawnLocation.Inside173Bottom:
                    return "173_BOTTOM";
                case SpawnLocation.InsideLczArmory:
                    return "LCZ_ARMORY";
                case SpawnLocation.InsideHczArmory:
                    return "HCZ_ARMORY";
                case SpawnLocation.InsideNukeArmory:
                    return "NUKE_ARMORY";
                case SpawnLocation.InsideSurfaceNuke:
                    return "SURFACE_NUKE";
                case SpawnLocation.Inside079Secondary:
                    return "079_SECOND";
                case SpawnLocation.Inside173Connector:
                    return "173_CONNECTOR";
                case SpawnLocation.InsideServersBottom:
                    return "SERVERS_BOTTOM";
                case SpawnLocation.InsideEscapePrimary:
                    return "ESCAPE_PRIMARY";
                case SpawnLocation.InsideEscapeSecondary:
                    return "ESCAPE_SECONDARY";
                default:
                    return default;
            }
        }
    }
}
