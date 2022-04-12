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
        public static string GetDoorName(this SpawnLocation spawnLocation) => spawnLocation switch
        {
            SpawnLocation.Inside012 => "012",
            SpawnLocation.Inside096 => "096",
            SpawnLocation.Inside914 => "914",
            SpawnLocation.InsideHid => "HID",
            SpawnLocation.InsideGr18 => "GR18",
            SpawnLocation.InsideGateA => "GATE_A",
            SpawnLocation.InsideGateB => "GATE_B",
            SpawnLocation.InsideLczWc => "LCZ_WC",
            SpawnLocation.InsideHidLeft => "HID_LEFT",
            SpawnLocation.InsideLczCafe => "LCZ_CAFE",
            SpawnLocation.Inside173Gate => "173_GATE",
            SpawnLocation.InsideIntercom => "INTERCOM",
            SpawnLocation.InsideHidRight => "HID_RIGHT",
            SpawnLocation.Inside079First => "079_FIRST",
            SpawnLocation.Inside012Bottom => "012_BOTTOM",
            SpawnLocation.Inside012Locker => "012_LOCKER",
            SpawnLocation.Inside049Armory => "049_ARMORY",
            SpawnLocation.Inside173Armory => "173_ARMORY",
            SpawnLocation.Inside173Bottom => "173_BOTTOM",
            SpawnLocation.InsideLczArmory => "LCZ_ARMORY",
            SpawnLocation.InsideHczArmory => "HCZ_ARMORY",
            SpawnLocation.InsideNukeArmory => "NUKE_ARMORY",
            SpawnLocation.InsideSurfaceNuke => "SURFACE_NUKE",
            SpawnLocation.Inside079Secondary => "079_SECOND",
            SpawnLocation.Inside173Connector => "173_CONNECTOR",
            SpawnLocation.InsideServersBottom => "SERVERS_BOTTOM",
            SpawnLocation.InsideEscapePrimary => "ESCAPE_PRIMARY",
            SpawnLocation.InsideEscapeSecondary => "ESCAPE_SECONDARY",
            _ => default,
        };
    }
}
