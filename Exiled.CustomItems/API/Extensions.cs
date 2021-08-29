// -----------------------------------------------------------------------
// <copyright file="Extensions.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomItems.API
{
    using System;
    using System.Collections.Generic;

    using Exiled.API.Features;
    using Exiled.API.Features.Items;
    using Exiled.CustomItems.API.Features;

    using Interactables.Interobjects.DoorUtils;

    using UnityEngine;

    /// <summary>
    /// A collection of API methods.
    /// </summary>
    public static class Extensions
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
        /// Resets the player's inventory to the provided list of items and/or customitems names, clearing any items it already possess.
        /// </summary>
        /// <param name="player">The player to which items will be given.</param>
        /// <param name="newItems">The new items that have to be added to the inventory.</param>
        /// <param name="displayMessage">Indicates a value whether <see cref="CustomItem.ShowPickedUpMessage"/> will be called when the player receives the <see cref="CustomItem"/> or not.</param>
        public static void ResetInventory(this Player player, List<string> newItems, bool displayMessage = false)
        {
            foreach (Item item in player.Items)
            {
                if (CustomItem.TryGet(item, out CustomItem customItem))
                    customItem.TrackedSerials.Remove(item.Serial);
            }

            player.ClearInventory();

            foreach (string item in newItems)
            {
                if (Enum.TryParse(item, true, out ItemType parsedItem))
                {
                    player.AddItem(parsedItem);
                }
                else if (!CustomItem.TryGive(player, item, displayMessage))
                {
                    Log.Debug($"\"{item}\" is not a valid item name, nor a custom item name.", CustomItems.Instance.Config.Debug);
                }
            }
        }

        /// <summary>
        /// Tries to get the <see cref="Transform"/> of the door used for a specific <see cref="SpawnLocation"/>.
        /// </summary>
        /// <param name="location">The <see cref="SpawnLocation"/> to check.</param>
        /// <returns>The <see cref="Transform"/> used for that spawn location. Can be null.</returns>
        public static Transform GetDoor(this SpawnLocation location)
        {
            string doorName = location.GetDoorName();

            if (string.IsNullOrEmpty(doorName))
                return null;

            return DoorNametagExtension.NamedDoors.TryGetValue(doorName, out var nametag) ? nametag.transform : null;
        }

        /// <summary>
        /// Tries to get the <see cref="Vector3"/> used for a specific <see cref="SpawnLocation"/>.
        /// </summary>
        /// <param name="location">The <see cref="SpawnLocation"/> to check.</param>
        /// <returns>The <see cref="Vector3"/> used for that spawn location. Can be <see cref="Vector3.zero"/>.</returns>
        public static Vector3 GetPosition(this SpawnLocation location)
        {
            Transform transform = location.GetDoor();

            if (transform == null)
                return default;

            return transform.position + (Vector3.up * 1.5f) + (transform.forward * (ReversedLocations.Contains(location) ? -3f : 3f));
        }

        /// <summary>
        /// Registers an <see cref="IEnumerable{T}"/> of <see cref="CustomItem"/>s.
        /// </summary>
        /// <param name="customItems"><see cref="CustomItem"/>s to be registered.</param>
        public static void Register(this IEnumerable<CustomItem> customItems)
        {
            if (customItems == null)
                throw new ArgumentNullException("customItems");

            foreach (CustomItem customItem in customItems)
                customItem.TryRegister();
        }

        /// <summary>
        /// Unregisters an <see cref="IEnumerable{T}"/> of <see cref="CustomItem"/>s.
        /// </summary>
        /// <param name="customItems"><see cref="CustomItem"/>s to be unregistered.</param>
        public static void Unregister(this IEnumerable<CustomItem> customItems)
        {
            if (customItems == null)
                throw new ArgumentNullException("customItems");

            foreach (CustomItem customItem in customItems)
                customItem.TryUnregister();
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
