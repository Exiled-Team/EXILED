// -----------------------------------------------------------------------
// <copyright file="Extensions.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomItems.API
{
    using System.Collections.Generic;
    using System.Linq;

    using Exiled.API.Features;
    using Exiled.CustomItems.API.Features;
    using Exiled.CustomItems.API.Spawn;

    using Interactables.Interobjects.DoorUtils;

    using UnityEngine;

    /// <summary>
    /// A collection of API methods.
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Registers a <see cref="CustomItem"/> manager with the plugin.
        /// </summary>
        /// <param name="item">The <see cref="CustomItem"/> to register.</param>
        /// <returns>A <see cref="bool"/> indicating whether or not the item was registered.</returns>
        public static bool RegisterCustomItem(this CustomItem item)
        {
            if (!CustomItems.Instance.ItemManagers.Contains(item))
            {
                if (CustomItems.Instance.ItemManagers.Any(i => i.Id == item.Id))
                {
                    Log.Error($"{item.Name} has tried to register with the same ItemID as another item: {item.Id}. It will not be registered.");

                    return false;
                }

                CustomItems.Instance.ItemManagers.Add(item);

                item.Init();

                Log.Debug($"{item.Name} ({item.Id}) has been successfully registered.", CustomItems.Instance.Config.Debug);

                return true;
            }

            Log.Warn($"Couldn't register {item} as it already exists.");
            return false;
        }

        /// <summary>
        /// Tries to get a <see cref="CustomItem"/> with a particular name.
        /// </summary>
        /// <param name="name">The <see cref="string"/> name of the item.</param>
        /// <param name="item">The <see cref="CustomItem"/> item found.</param>
        /// <returns>The <see cref="CustomItem"/> matching the search. Can be null.</returns>
        public static bool TryGetItem(string name, out CustomItem item)
        {
            foreach (CustomItem cItem in CustomItems.Instance.ItemManagers)
            {
                if (!cItem.Name.Equals(name, System.StringComparison.OrdinalIgnoreCase))
                    continue;

                item = cItem;
                return true;
            }

            item = null;
            return false;
        }

        /// <summary>
        /// Tries to get a <see cref="CustomItem"/> with a particular ID.
        /// </summary>
        /// <param name="id">The <see cref="int"/>ID of the item to look for.</param>
        /// <param name="item">The <see cref="CustomItem"/> found.</param>
        /// <returns>The <see cref="CustomItem"/> matching the search. Can be null.</returns>
        public static bool TryGetItem(int id, out CustomItem item)
        {
            foreach (CustomItem cItem in CustomItems.Instance.ItemManagers)
            {
                if (cItem.Id != id)
                    continue;

                item = cItem;
                return true;
            }

            item = null;
            return false;
        }

        /// <summary>
        /// Gives the specified item to a player.
        /// </summary>
        /// <param name="player">The <see cref="Player"/> to give the item to.</param>
        /// <param name="item">The <see cref="CustomItem"/> to give to the player.</param>
        public static void GiveCustomItem(this Player player, CustomItem item) => player.GiveCustomItem(item, false);

        /// <summary>
        /// Gives the specified item to a player.
        /// </summary>
        /// <param name="player">The <see cref="Player"/> to give the item to.</param>
        /// <param name="item">The <see cref="CustomItem"/> to give to the player.</param>
        /// <param name="displayMessage">Whether or not to show a message when the item is given.</param>
        public static void GiveCustomItem(this Player player, CustomItem item, bool displayMessage) => item.Give(player, displayMessage);

        /// <summary>
        /// Gives the player a specified item.
        /// </summary>
        /// <param name="player">The <see cref="Player"/> to give the item to.</param>
        /// <param name="name">The <see cref="string"/> name of the item to give.</param>
        /// <returns>A <see cref="bool"/> indicating if the player was given the item or not.</returns>
        public static bool TryGiveCustomItem(this Player player, string name)
        {
            if (!TryGetItem(name, out CustomItem item))
                return false;

            player.GiveCustomItem(item);

            return true;
        }

        /// <summary>
        /// Gives the player a specified item.
        /// </summary>
        /// <param name="player">The <see cref="Player"/> to give the item to.</param>
        /// <param name="id">The <see cref="int"/> name of the item to give.</param>
        /// <returns>A <see cref="bool"/> indicating if the player was given the item or not.</returns>
        public static bool TryGiveCustomItem(this Player player, int id)
        {
            if (!TryGetItem(id, out CustomItem item))
                return false;

            player.GiveCustomItem(item);

            return true;
        }

        /// <summary>
        /// Spawns a specified item at a location.
        /// </summary>
        /// <param name="name">The <see cref="string"/> name of the item to spawn.</param>
        /// <param name="position">The <see cref="Vector3"/> location to spawn the item.</param>
        /// <returns>A <see cref="bool"/> value indicating whether or not the item was spawned.</returns>
        public static bool SpawnItem(this string name, Vector3 position)
        {
            if (!TryGetItem(name, out CustomItem item))
                return false;

            item.Spawn(position);

            return true;
        }

        /// <summary>
        /// Spawns a specified item at a location.
        /// </summary>
        /// <param name="id">The <see cref="int"/> ID of the item to spawn.</param>
        /// <param name="position">The <see cref="Vector3"/> location to spawn the item.</param>
        /// <returns>A <see cref="bool"/> value indicating whether or not the item was spawned.</returns>
        public static bool SpawnItem(this int id, Vector3 position)
        {
            if (!TryGetItem(id, out CustomItem item))
                return false;

            item.Spawn(position);

            return true;
        }

        /// <summary>
        /// Gets a list of all currently active Item Managers.
        /// </summary>
        /// <returns>A list of all <see cref="CustomItem"/>s.</returns>
        public static HashSet<CustomItem> GetInstalledItems() => CustomItems.Instance.ItemManagers;

        /// <summary>
        /// Tries to get the <see cref="Transform"/> of the door used for a specific <see cref="SpawnLocation"/>.
        /// </summary>
        /// <param name="location">The <see cref="SpawnLocation"/> to check.</param>
        /// <returns>The <see cref="Transform"/> used for that spawn location. Can be null.</returns>
        public static Transform GetDoor(this SpawnLocation location)
        {
            if (!SpawnLocationData.DoorNames.TryGetValue(location, out string doorName))
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

            float modifier = SpawnLocationData.ReversedLocations.Contains(location) ? -3f : 3f;

            return (transform.position + (Vector3.up * 1.5f)) + (transform.forward * modifier);
        }

        /// <summary>
        /// Checks to see if the player's current item is a custom item.
        /// </summary>
        /// <param name="player">The <see cref="Player"/> to check.</param>
        /// <param name="customItem">The <see cref="CustomItem"/> in their hand.</param>
        /// <returns>True if the player has a custom item in their hand.</returns>
        public static bool TryGetCustomItemInHand(this Player player, out CustomItem customItem)
        {
            foreach (CustomItem installedCustomItem in GetInstalledItems())
            {
                if (!installedCustomItem.Check(player.CurrentItem))
                    continue;

                customItem = installedCustomItem;
                return true;
            }

            customItem = null;
            return false;
        }

        /// <summary>
        /// Checks to see if the player has any custom items in their inventory.
        /// </summary>
        /// <param name="player">The <see cref="Player"/> to check.</param>
        /// <param name="customItems">A <see cref="List{T}"/> of custom items in their possession.</param>
        /// <returns>True if they have any custom items.</returns>
        public static bool TryGetCustomItems(this Player player, out List<CustomItem> customItems)
        {
            customItems = new List<CustomItem>();

            foreach (Inventory.SyncItemInfo item in player.Inventory.items)
            {
                foreach (CustomItem installedCustomItem in GetInstalledItems())
                    customItems.Add(installedCustomItem);
            }

            return customItems.Count > 0;
        }

        /// <summary>
        /// Checks to see if this item is a custom item.
        /// </summary>
        /// <param name="item">The <see cref="Inventory.SyncItemInfo"/> to check.</param>
        /// <param name="cItem">The <see cref="CustomItem"/> this item is.</param>
        /// <returns>True if the item is a custom item.</returns>
        public static bool IsCustomItem(this Inventory.SyncItemInfo item, out CustomItem cItem)
        {
            foreach (CustomItem customItem in GetInstalledItems())
            {
                if (!customItem.Check(item))
                    continue;

                cItem = customItem;
                return true;
            }

            cItem = null;
            return false;
        }

        /// <summary>
        /// Checks if this pickup is a custom item.
        /// </summary>
        /// <param name="pickup">The <see cref="Pickup"/> to check.</param>
        /// <param name="customItem">The <see cref="CustomItem"/> this pickup is.</param>
        /// <returns>True if the pickup is a custom item.</returns>
        public static bool IsCustomItem(this Pickup pickup, out CustomItem customItem)
        {
            foreach (CustomItem installedCustomItem in GetInstalledItems())
            {
                if (!installedCustomItem.Check(pickup))
                    continue;

                customItem = installedCustomItem;
                return true;
            }

            customItem = null;
            return false;
        }

        /// <summary>
        /// Converts a <see cref="Vector3"/> to a <see cref="Vector"/>.
        /// </summary>
        /// <param name="vector3">The <see cref="Vector3"/> to be converted.</param>
        /// <returns>Returns the <see cref="Vector"/>.</returns>
        public static Vector ToVector(this Vector3 vector3) => new Vector(vector3.x, vector3.y, vector3.z);

        /// <summary>
        /// Reloads the current weapon for the player.
        /// </summary>
        /// <param name="player">The <see cref="Player"/> who's weapon should be reloaded.</param>
        public static void ReloadWeapon(this Player player) => player.ReferenceHub.weaponManager.RpcReload(player.ReferenceHub.weaponManager.curWeapon);
    }
}
