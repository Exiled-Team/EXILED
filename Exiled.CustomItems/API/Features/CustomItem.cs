// -----------------------------------------------------------------------
// <copyright file="CustomItem.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomItems.API.Features
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using Exiled.API.Enums;
    using Exiled.API.Extensions;
    using Exiled.API.Features;
    using Exiled.API.Features.Attributes;
    using Exiled.API.Features.Items;
    using Exiled.API.Features.Spawn;
    using Exiled.API.Interfaces;
    using Exiled.CustomItems.API.EventArgs;
    using Exiled.Events.EventArgs;
    using Exiled.Loader;

    using InventorySystem.Items;
    using InventorySystem.Items.Firearms;
    using InventorySystem.Items.Pickups;

    using MapGeneration.Distributors;

    using MEC;

    using UnityEngine;

    using YamlDotNet.Serialization;

    using static CustomItems;

    using Firearm = Exiled.API.Features.Items.Firearm;
    using Item = Exiled.API.Features.Items.Item;
    using Map = Exiled.API.Features.Map;
    using Player = Exiled.API.Features.Player;

    /// <summary>
    /// The Custom Item base class.
    /// </summary>
    public abstract class CustomItem
    {
        private ItemType type = ItemType.None;

        /// <summary>
        /// Gets the list of current Item Managers.
        /// </summary>
        public static HashSet<CustomItem> Registered { get; } = new();

        /// <summary>
        /// Gets or sets the custom ItemID of the item.
        /// </summary>
        public abstract uint Id { get; set; }

        /// <summary>
        /// Gets or sets the name of the item.
        /// </summary>
        public abstract string Name { get; set; }

        /// <summary>
        /// Gets or sets the description of the item.
        /// </summary>
        public abstract string Description { get; set; }

        /// <summary>
        /// Gets or sets the weight of the item.
        /// </summary>
        public abstract float Weight { get; set; }

        /// <summary>
        /// Gets or sets the list of spawn locations and chances for each one.
        /// </summary>
        public abstract SpawnProperties SpawnProperties { get; set; }

        /// <summary>
        /// Gets or sets the ItemType to use for this item.
        /// </summary>
        public virtual ItemType Type
        {
            get => type;
            set
            {
                if (!Enum.IsDefined(typeof(ItemType), value))
                    throw new ArgumentOutOfRangeException("Type", value, "Invalid Item type.");

                type = value;
            }
        }

        /// <summary>
        /// Gets the list of custom items inside players' inventory being tracked as the current item.
        /// </summary>
        [YamlIgnore]
        public HashSet<int> TrackedSerials { get; } = new();

        /// <summary>
        /// Gets a value indicating whether or not this item causes things to happen that may be considered hacks, and thus be shown to global moderators as being present in a player's inventory when they gban them.
        /// </summary>
        [YamlIgnore]
        public virtual bool ShouldMessageOnGban { get; } = false;

        /// <summary>
        /// Gets a <see cref="CustomItem"/> with a specific ID.
        /// </summary>
        /// <param name="id">The <see cref="CustomItem"/> ID.</param>
        /// <returns>The <see cref="CustomItem"/> matching the search, <see langword="null"/> if not registered.</returns>
        public static CustomItem Get(int id) => Registered?.FirstOrDefault(tempCustomItem => tempCustomItem.Id == id);

        /// <summary>
        /// Gets a <see cref="CustomItem"/> with a specific name.
        /// </summary>
        /// <param name="name">The <see cref="CustomItem"/> name.</param>
        /// <returns>The <see cref="CustomItem"/> matching the search, <see langword="null"/> if not registered.</returns>
        public static CustomItem Get(string name) => Registered?.FirstOrDefault(tempCustomItem => tempCustomItem.Name == name);

        /// <summary>
        /// Tries to get a <see cref="CustomItem"/> with a specific ID.
        /// </summary>
        /// <param name="id">The <see cref="CustomItem"/> ID to look for.</param>
        /// <param name="customItem">The found <see cref="CustomItem"/>, <see langword="null"/> if not registered.</param>
        /// <returns>Returns a value indicating whether the <see cref="CustomItem"/> was found or not.</returns>
        public static bool TryGet(int id, out CustomItem customItem)
        {
            customItem = Get(id);

            return customItem is not null;
        }

        /// <summary>
        /// Tries to get a <see cref="CustomItem"/> with a specific name.
        /// </summary>
        /// <param name="name">The <see cref="CustomItem"/> name to look for.</param>
        /// <param name="customItem">The found <see cref="CustomItem"/>, <see langword="null"/> if not registered.</param>
        /// <returns>Returns a value indicating whether the <see cref="CustomItem"/> was found or not.</returns>
        public static bool TryGet(string name, out CustomItem customItem)
        {
            if (name is null)
                throw new ArgumentNullException(nameof(name));

            customItem = int.TryParse(name, out int id) ? Get(id) : Get(name);

            return customItem is not null;
        }

        /// <summary>
        /// Tries to get the player's current <see cref="CustomItem"/> in their hand.
        /// </summary>
        /// <param name="player">The <see cref="Player"/> to check.</param>
        /// <param name="customItem">The <see cref="CustomItem"/> in their hand.</param>
        /// <returns>Returns a value indicating whether the <see cref="Player"/> has a <see cref="CustomItem"/> in their hand or not.</returns>
        public static bool TryGet(Player player, out CustomItem customItem)
        {
            if (player is null)
                throw new ArgumentNullException(nameof(player));

            customItem = Registered?.FirstOrDefault(tempCustomItem => tempCustomItem.Check(player.CurrentItem));

            return customItem is not null;
        }

        /// <summary>
        /// Tries to get the player's <see cref="IEnumerable{T}"/> of <see cref="CustomItem"/>.
        /// </summary>
        /// <param name="player">The <see cref="Player"/> to check.</param>
        /// <param name="customItems">The player's <see cref="IEnumerable{T}"/> of <see cref="CustomItem"/>.</param>
        /// <returns>Returns a value indicating whether the <see cref="Player"/> has a <see cref="CustomItem"/> in their hand or not.</returns>
        public static bool TryGet(Player player, out IEnumerable<CustomItem> customItems)
        {
            if (player is null)
                throw new ArgumentNullException(nameof(player));

            customItems = Registered?.Where(tempCustomItem => player.Items.Any(item => tempCustomItem.Check(item)));

            return customItems?.Any() ?? false;
        }

        /// <summary>
        /// Checks to see if this item is a custom item.
        /// </summary>
        /// <param name="item">The <see cref="Events.Handlers.Item"/> to check.</param>
        /// <param name="customItem">The <see cref="CustomItem"/> this item is.</param>
        /// <returns>True if the item is a custom item.</returns>
        public static bool TryGet(Item item, out CustomItem customItem)
        {
            customItem = Registered?.FirstOrDefault(tempCustomItem => tempCustomItem.TrackedSerials.Contains(item.Serial));

            return customItem is not null;
        }

        /// <summary>
        /// Checks if this pickup is a custom item.
        /// </summary>
        /// <param name="pickup">The <see cref="ItemPickupBase"/> to check.</param>
        /// <param name="customItem">The <see cref="CustomItem"/> this pickup is.</param>
        /// <returns>True if the pickup is a custom item.</returns>
        public static bool TryGet(Pickup pickup, out CustomItem customItem)
        {
            customItem = Registered?.FirstOrDefault(tempCustomItem => tempCustomItem.TrackedSerials.Contains(pickup.Serial));

            return customItem is not null;
        }

        /// <summary>
        /// Tries to spawn a specific <see cref="CustomItem"/> at a specific <see cref="Vector3"/> position.
        /// </summary>
        /// <param name="id">The ID of the <see cref="CustomItem"/> to spawn.</param>
        /// <param name="position">The <see cref="Vector3"/> location to spawn the item.</param>
        /// <param name="pickup">The <see cref="ItemPickupBase"/> instance of the <see cref="CustomItem"/>.</param>
        /// <returns>Returns a value indicating whether the <see cref="CustomItem"/> was spawned or not.</returns>
        public static bool TrySpawn(int id, Vector3 position, out Pickup pickup)
        {
            pickup = default;

            if (!TryGet(id, out CustomItem item))
                return false;

            pickup = item.Spawn(position, (Player)null);

            return true;
        }

        /// <summary>
        /// Tries to spawn a specific <see cref="CustomItem"/> at a specific <see cref="Vector3"/> position.
        /// </summary>
        /// <param name="name">The name of the <see cref="CustomItem"/> to spawn.</param>
        /// <param name="position">The <see cref="Vector3"/> location to spawn the item.</param>
        /// <param name="pickup">The <see cref="ItemPickupBase"/> instance of the <see cref="CustomItem"/>.</param>
        /// <returns>Returns a value indicating whether the <see cref="CustomItem"/> was spawned or not.</returns>
        public static bool TrySpawn(string name, Vector3 position, out Pickup pickup)
        {
            pickup = default;

            if (!TryGet(name, out CustomItem item))
                return false;

            pickup = item.Spawn(position, (Player)null);

            return true;
        }

        /// <summary>
        /// Gives to a specific <see cref="Player"/> a specic <see cref="CustomItem"/>.
        /// </summary>
        /// <param name="player">The <see cref="Player"/> to give the item to.</param>
        /// <param name="name">The name of the <see cref="CustomItem"/> to give.</param>
        /// <param name="displayMessage">Indicates a value whether <see cref="ShowPickedUpMessage"/> will be called when the player receives the <see cref="CustomItem"/> or not.</param>
        /// <returns>Returns a value indicating if the player was given the <see cref="CustomItem"/> or not.</returns>
        public static bool TryGive(Player player, string name, bool displayMessage = true)
        {
            if (!TryGet(name, out CustomItem item))
                return false;

            item.Give(player, displayMessage);

            return true;
        }

        /// <summary>
        /// Gives to a specific <see cref="Player"/> a specic <see cref="CustomItem"/>.
        /// </summary>
        /// <param name="player">The <see cref="Player"/> to give the item to.</param>
        /// <param name="id">The IDs of the <see cref="CustomItem"/> to give.</param>
        /// <param name="displayMessage">Indicates a value whether <see cref="ShowPickedUpMessage"/> will be called when the player receives the <see cref="CustomItem"/> or not.</param>
        /// <returns>Returns a value indicating if the player was given the <see cref="CustomItem"/> or not.</returns>
        public static bool TryGive(Player player, int id, bool displayMessage = true)
        {
            if (!TryGet(id, out CustomItem item))
                return false;

            item.Give(player, displayMessage);

            return true;
        }

        /// <summary>
        /// Registers all the <see cref="CustomItem"/>'s present in the current assembly.
        /// </summary>
        /// <param name="skipReflection">Whether or not reflection is skipped (more efficient if you are not using your custom item classes as config objects).</param>
        /// <param name="overrideClass">The class to search properties for, if different from the plugin's config class.</param>
        /// <returns>A <see cref="IEnumerable{T}"/> of <see cref="CustomItem"/> which contains all registered <see cref="CustomItem"/>'s.</returns>
        public static IEnumerable<CustomItem> RegisterItems(bool skipReflection = false, object overrideClass = null)
        {
            List<CustomItem> items = new();
            Assembly assembly = Assembly.GetCallingAssembly();
            foreach (Type type in assembly.GetTypes())
            {
                if ((type.BaseType != typeof(CustomItem) && !type.IsSubclassOf(typeof(CustomItem))) || type.GetCustomAttribute(typeof(CustomItemAttribute)) is null)
                    continue;

                foreach (Attribute attribute in type.GetCustomAttributes(typeof(CustomItemAttribute), true))
                {
                    CustomItem customItem = null;
                    bool flag = false;

                    if (!skipReflection && Loader.PluginAssemblies.ContainsKey(assembly))
                    {
                        IPlugin<IConfig> plugin = Loader.PluginAssemblies[assembly];
                        foreach (PropertyInfo property in overrideClass?.GetType().GetProperties() ?? plugin.Config.GetType().GetProperties())
                        {
                            if (property.PropertyType != type)
                            {
                                if (property.GetValue(overrideClass ?? plugin.Config) is IEnumerable enumerable)
                                {
                                    List<CustomItem> list = new();
                                    foreach (object item in enumerable)
                                    {
                                        if (item is CustomItem ci)
                                            list.Add(ci);
                                    }

                                    foreach (CustomItem item in list)
                                    {
                                        if (item.GetType() != type)
                                            break;

                                        if (item.Type == ItemType.None)
                                            item.Type = ((CustomItemAttribute)attribute).ItemType;

                                        if (!item.TryRegister())
                                            continue;

                                        flag = true;
                                        items.Add(item);
                                    }
                                }

                                continue;
                            }

                            customItem = property.GetValue(overrideClass ?? plugin.Config) as CustomItem;
                        }
                    }

                    if (flag)
                        continue;

                    customItem ??= (CustomItem)Activator.CreateInstance(type);

                    if (customItem.Type == ItemType.None)
                        customItem.Type = ((CustomItemAttribute)attribute).ItemType;

                    if (customItem.TryRegister())
                        items.Add(customItem);
                }
            }

            return items;
        }

        /// <summary>
        /// Registers all the <see cref="CustomItem"/>'s present in the current assembly.
        /// </summary>
        /// <param name="targetTypes">The <see cref="IEnumerable{T}"/> of <see cref="System.Type"/> containing the target types.</param>
        /// <param name="isIgnored">A value indicating whether the target types should be ignored.</param>
        /// <param name="skipReflection">Whether or not reflection is skipped (more efficient if you are not using your custom item classes as config objects).</param>
        /// <param name="overrideClass">The class to search properties for, if different from the plugin's config class.</param>
        /// <returns>A <see cref="IEnumerable{T}"/> of <see cref="CustomItem"/> which contains all registered <see cref="CustomItem"/>'s.</returns>
        public static IEnumerable<CustomItem> RegisterItems(IEnumerable<Type> targetTypes, bool isIgnored = false, bool skipReflection = false, object overrideClass = null)
        {
            List<CustomItem> items = new();
            Assembly assembly = Assembly.GetCallingAssembly();
            foreach (Type type in assembly.GetTypes())
            {
                if ((type.BaseType != typeof(CustomItem) && !type.IsSubclassOf(typeof(CustomItem))) || type.GetCustomAttribute(typeof(CustomItemAttribute)) is null ||
                    (isIgnored && targetTypes.Contains(type)) || (!isIgnored && !targetTypes.Contains(type)))
                    continue;

                foreach (Attribute attribute in type.GetCustomAttributes(typeof(CustomItemAttribute), true))
                {
                    CustomItem customItem = null;

                    if (!skipReflection && Loader.PluginAssemblies.ContainsKey(assembly))
                    {
                        IPlugin<IConfig> plugin = Loader.PluginAssemblies[assembly];

                        foreach (PropertyInfo property in overrideClass?.GetType().GetProperties() ?? plugin.Config.GetType().GetProperties())
                        {
                            if (property.PropertyType != type)
                                continue;

                            customItem = property.GetValue(overrideClass ?? plugin.Config) as CustomItem;
                            break;
                        }
                    }

                    customItem ??= (CustomItem)Activator.CreateInstance(type);

                    if (customItem.Type == ItemType.None)
                        customItem.Type = ((CustomItemAttribute)attribute).ItemType;

                    if (customItem.TryRegister())
                        items.Add(customItem);
                }
            }

            return items;
        }

        /// <summary>
        /// Unregisters all the <see cref="CustomItem"/>'s present in the current assembly.
        /// </summary>
        /// <returns>A <see cref="IEnumerable{T}"/> of <see cref="CustomItem"/> which contains all unregistered <see cref="CustomItem"/>'s.</returns>
        public static IEnumerable<CustomItem> UnregisterItems()
        {
            List<CustomItem> unregisteredItems = new();

            foreach (CustomItem customItem in Registered)
            {
                customItem.TryUnregister();
                unregisteredItems.Add(customItem);
            }

            return unregisteredItems;
        }

        /// <summary>
        /// Unregisters all the <see cref="CustomItem"/>'s present in the current assembly.
        /// </summary>
        /// <param name="targetTypes">The <see cref="IEnumerable{T}"/> of <see cref="System.Type"/> containing the target types.</param>
        /// <param name="isIgnored">A value indicating whether the target types should be ignored.</param>
        /// <returns>A <see cref="IEnumerable{T}"/> of <see cref="CustomItem"/> which contains all unregistered <see cref="CustomItem"/>'s.</returns>
        public static IEnumerable<CustomItem> UnregisterItems(IEnumerable<Type> targetTypes, bool isIgnored = false)
        {
            List<CustomItem> unregisteredItems = new();

            foreach (CustomItem customItem in Registered)
            {
                if ((targetTypes.Contains(customItem.GetType()) && isIgnored) || (!targetTypes.Contains(customItem.GetType()) && !isIgnored))
                    continue;

                customItem.TryUnregister();
                unregisteredItems.Add(customItem);
            }

            return unregisteredItems;
        }

        /// <summary>
        /// Unregisters all the <see cref="CustomItem"/>'s present in the current assembly.
        /// </summary>
        /// <param name="targetItems">The <see cref="IEnumerable{T}"/> of <see cref="CustomItem"/> containing the target items.</param>
        /// <param name="isIgnored">A value indicating whether the target items should be ignored.</param>
        /// <returns>A <see cref="IEnumerable{T}"/> of <see cref="CustomItem"/> which contains all unregistered <see cref="CustomItem"/>'s.</returns>
        public static IEnumerable<CustomItem> UnregisterItems(IEnumerable<CustomItem> targetItems, bool isIgnored = false) => UnregisterItems(targetItems.Select(x => x.GetType()), isIgnored);

        /// <summary>
        /// Spawns the <see cref="CustomItem"/> in a specific location.
        /// </summary>
        /// <param name="x">The x coordinate.</param>
        /// <param name="y">The y coordinate.</param>
        /// <param name="z">The z coordinate.</param>
        /// <returns>The <see cref="Pickup"/> wrapper of the spawned <see cref="CustomItem"/>.</returns>
        public virtual Pickup Spawn(float x, float y, float z) => Spawn(new Vector3(x, y, z), (Player)null);

        /// <summary>
        /// Spawns a <see cref="Item"/> as a <see cref="CustomItem"/> in a specific location.
        /// </summary>
        /// <param name="x">The x coordinate.</param>
        /// <param name="y">The y coordinate.</param>
        /// <param name="z">The z coordinate.</param>
        /// <param name="item">The <see cref="Item"/> to be spawned as a <see cref="CustomItem"/>.</param>
        /// <returns>The <see cref="Pickup"/> wrapper of the spawned <see cref="CustomItem"/>.</returns>
        public virtual Pickup Spawn(float x, float y, float z, Item item) => Spawn(new Vector3(x, y, z), item, null);

        /// <summary>
        /// Spawns the <see cref="CustomItem"/> where a specific <see cref="Player"/> is.
        /// </summary>
        /// <param name="player">The <see cref="Player"/> position where the <see cref="CustomItem"/> will be spawned.</param>
        /// <returns>The <see cref="Pickup"/> wrapper of the spawned <see cref="CustomItem"/>.</returns>
        [Obsolete("Use Spawn(Player, Player) instead.", true)]
        public virtual Pickup Spawn(Player player) => Spawn(player.Position);

        /// <summary>
        /// Spawns the <see cref="CustomItem"/> where a specific <see cref="Player"/> is, and optionally sets the previous owner.
        /// </summary>
        /// <param name="player">The <see cref="Player"/> position where the <see cref="CustomItem"/> will be spawned.</param>
        /// <param name="previousOwner">The previous owner of the pickup, can be null.</param>
        /// <returns>The <see cref="Pickup"/> of the spawned <see cref="CustomItem"/>.</returns>
        public virtual Pickup Spawn(Player player, Player previousOwner = null) => Spawn(player.Position, previousOwner);

        /// <summary>
        /// Spawns a <see cref="Item"/> as a <see cref="CustomItem"/> where a specific <see cref="Player"/> is, and optionally sets the previous owner.
        /// </summary>
        /// <param name="player">The <see cref="Player"/> position where the <see cref="CustomItem"/> will be spawned.</param>
        /// <param name="item">The <see cref="Item"/> to be spawned as a <see cref="CustomItem"/>.</param>
        /// <param name="previousOwner">The previous owner of the pickup, can be null.</param>
        /// <returns>The <see cref="Pickup"/> of the spawned <see cref="CustomItem"/>.</returns>
        public virtual Pickup Spawn(Player player, Item item, Player previousOwner = null) => Spawn(player.Position, item, previousOwner);

        /// <summary>
        /// Spawns a <see cref="Item"/> as a <see cref="CustomItem"/> where a specific <see cref="Player"/> is.
        /// </summary>
        /// <param name="player">The <see cref="Player"/> position where the <see cref="CustomItem"/> will be spawned.</param>
        /// <param name="item">The <see cref="Item"/> to be spawned as a <see cref="CustomItem"/>.</param>
        /// <returns>The <see cref="Pickup"/> wrapper of the spawned <see cref="CustomItem"/>.</returns>
        [Obsolete("Use Spawn(Player, Item, Player) instead.", true)]
        public virtual Pickup Spawn(Player player, Item item)
        {
            Pickup pickup = Spawn(player.Position, item);
            return pickup;
        }

        /// <summary>
        /// Spawns the <see cref="CustomItem"/> in a specific position.
        /// </summary>
        /// <param name="position">The <see cref="Vector3"/> where the <see cref="CustomItem"/> will be spawned.</param>
        /// <param name="previousOwner">The <see cref="Pickup.PreviousOwner"/> of the item. Can be null.</param>
        /// <returns>The <see cref="Pickup"/> of the spawned <see cref="CustomItem"/>.</returns>
        public virtual Pickup Spawn(Vector3 position, Player previousOwner = null) => Spawn(position, CreateCorrectItem(), previousOwner);

        /// <summary>
        /// Spawns the <see cref="CustomItem"/> in a specific position.
        /// </summary>
        /// <param name="position">The <see cref="Vector3"/> where the <see cref="CustomItem"/> will be spawned.</param>
        /// <returns>The <see cref="Pickup"/> wrapper of the spawned <see cref="CustomItem"/>.</returns>
        [Obsolete("Use Spawn(Vector3, Player) instead.", true)]
        public virtual Pickup Spawn(Vector3 position)
        {
            Pickup pickup = CreateCorrectItem().Spawn(position);
            pickup.Weight = Weight;
            TrackedSerials.Add(pickup.Serial);

            return pickup;
        }

        /// <summary>
        /// Spawns the <see cref="CustomItem"/> in a specific position.
        /// </summary>
        /// <param name="position">The <see cref="Vector3"/> where the <see cref="CustomItem"/> will be spawned.</param>
        /// <param name="item">The <see cref="Item"/> to be spawned as a <see cref="CustomItem"/>.</param>
        /// <param name="previousOwner">The <see cref="Pickup.PreviousOwner"/> of the item. Can be null.</param>
        /// <returns>The <see cref="Pickup"/> of the spawned <see cref="CustomItem"/>.</returns>
        public virtual Pickup Spawn(Vector3 position, Item item, Player previousOwner = null)
        {
            Pickup pickup = item.Spawn(position);
            pickup.Weight = Weight;
            if (previousOwner is not null)
                pickup.PreviousOwner = previousOwner;
            TrackedSerials.Add(pickup.Serial);

            return pickup;
        }

        /// <summary>
        /// Spawns a <see cref="Item"/> as a <see cref="CustomItem"/> in a specific position.
        /// </summary>
        /// <param name="position">The <see cref="Vector3"/> where the <see cref="CustomItem"/> will be spawned.</param>
        /// <param name="item">The <see cref="Item"/> to be spawned as a <see cref="CustomItem"/>.</param>
        /// <returns>The <see cref="Pickup"/> wrapper of the spawned <see cref="CustomItem"/>.</returns>
        [Obsolete("Use Spawn(Vector3, Item, Player) instead.", true)]
        public virtual Pickup Spawn(Vector3 position, Item item)
        {
            Pickup pickup = item.Spawn(position);
            TrackedSerials.Add(pickup.Serial);

            return pickup;
        }

        /// <summary>
        /// Spawns <see cref="CustomItem"/>s inside <paramref name="spawnPoints"/>.
        /// </summary>
        /// <param name="spawnPoints">The spawn points <see cref="IEnumerable{T}"/>.</param>
        /// <param name="limit">The spawn limit.</param>
        /// <returns>Returns the number of spawned items.</returns>
        public virtual uint Spawn(IEnumerable<SpawnPoint> spawnPoints, uint limit)
        {
            uint spawned = 0;

            foreach (SpawnPoint spawnPoint in spawnPoints)
            {
                Log.Debug($"Attempting to spawn {Name} at {spawnPoint.Position}.", Instance.Config.Debug);

                if (UnityEngine.Random.Range(1, 101) >= spawnPoint.Chance || (limit > 0 && spawned >= limit))
                    continue;

                spawned++;

                if (spawnPoint is DynamicSpawnPoint dynamicSpawnPoint && dynamicSpawnPoint.Location == SpawnLocation.InsideLocker)
                {
                    for (int i = 0; i < 50; i++)
                    {
                        if (Map.Lockers is null)
                        {
                            Log.Debug($"{nameof(Spawn)}: Locker list is null.", Instance.Config.Debug);
                            continue;
                        }

                        Locker locker =
                            Map.Lockers[
                                Loader.Random.Next(Map.Lockers.Count)];

                        if (locker is null)
                        {
                            Log.Debug($"{nameof(Spawn)}: Selected locker is null.", Instance.Config.Debug);
                            continue;
                        }

                        if (locker.Loot is null)
                        {
                            Log.Debug($"{nameof(Spawn)}: Invalid locker location. Attempting to find a new one..", Instance.Config.Debug);
                            continue;
                        }

                        if (locker.Chambers is null)
                        {
                            Log.Debug($"{nameof(Spawn)}: Locker chambers is null", Instance.Config.Debug);
                            continue;
                        }

                        LockerChamber chamber = locker.Chambers[Loader.Random.Next(Mathf.Max(0, locker.Chambers.Length - 1))];

                        if (chamber is null)
                        {
                            Log.Debug($"{nameof(Spawn)}: chamber is null", Instance.Config.Debug);
                            continue;
                        }

                        Vector3 position = chamber._spawnpoint.transform.position;
                        Spawn(position, (Player)null);
                        Log.Debug($"Spawned {Name} at {position} ({spawnPoint.Name})", Instance.Config.Debug);

                        break;
                    }
                }
                else if (spawnPoint is RoleSpawnPoint roleSpawnPoint)
                {
                    Vector3 position = roleSpawnPoint.Role.GetRandomSpawnProperties().Item1;
                    Spawn(position, (Player)null);
                }
                else
                {
                    Pickup pickup = Spawn(spawnPoint.Position, (Player)null);
                    if (pickup.Base is FirearmPickup firearmPickup && this is CustomWeapon customWeapon)
                    {
                        firearmPickup.Status = new FirearmStatus(customWeapon.ClipSize, firearmPickup.Status.Flags, firearmPickup.Status.Attachments);
                        firearmPickup.NetworkStatus = firearmPickup.Status;
                    }

                    Log.Debug($"Spawned {Name} at {spawnPoint.Position} ({spawnPoint.Name})", Instance.Config.Debug);
                }
            }

            return spawned;
        }

        /// <summary>
        /// Spawns all items at their dynamic and static positions.
        /// </summary>
        public virtual void SpawnAll()
        {
            if (SpawnProperties is null)
                return;

            // This will go over each spawn property type (static, dynamic and role) to try and spawn the item.
            // It will attempt to spawn in role-based locations, and then dynamic ones, and finally static.
            // Math.Min is used here to ensure that our recursive Spawn() calls do not result in exceeding the spawn limit config.
            // This is the same as:
            // int spawned = 0;
            // spawned += Spawn(SpawnProperties.RoleSpawnPoints, SpawnProperties.Limit);
            // if (spawned < SpawnProperties.Limit)
            //    spawned += Spawn(SpawnProperties.DynamicSpawnPoints, SpawnProperties.Limit - spawned);
            // if (spawned < SpawnProperties.Limit)
            //    Spawn(SpawnProperties.StaticSpawnPoints, SpawnProperties.Limit - spawned);
            Spawn(SpawnProperties.StaticSpawnPoints, Math.Min(0, SpawnProperties.Limit - Math.Min(0, Spawn(SpawnProperties.DynamicSpawnPoints, SpawnProperties.Limit) - Spawn(SpawnProperties.RoleSpawnPoints, SpawnProperties.Limit))));
        }

        /// <summary>
        /// Gives an <see cref="Item"/> as a <see cref="CustomItem"/> to a <see cref="Player"/>.
        /// </summary>
        /// <param name="player">The <see cref="Player"/> who will receive the item.</param>
        /// <param name="item">The <see cref="Item"/> to be given.</param>
        /// <param name="displayMessage">Indicates whether or not <see cref="ShowPickedUpMessage"/> will be called when the player receives the item.</param>
        public virtual void Give(Player player, Item item, bool displayMessage = true)
        {
            try
            {
                Log.Debug($"{Name}.{nameof(Give)}: Item Serial: {item.Serial} Ammo: {(item is Firearm firearm ? firearm.Ammo : -1)}", Instance.Config.Debug);

                player.AddItem(item);

                Log.Debug($"{nameof(Give)}: Adding {item.Serial} to tracker.", Instance.Config.Debug);
                if (!TrackedSerials.Contains(item.Serial))
                    TrackedSerials.Add(item.Serial);

                if (displayMessage)
                    ShowPickedUpMessage(player);

                Timing.CallDelayed(0.05f, () => OnAcquired(player));
            }
            catch (Exception e)
            {
                Log.Error($"{nameof(Give)}: {e}");
            }
        }

        /// <summary>
        /// Gives a <see cref="ItemPickupBase"/> as a <see cref="CustomItem"/> to a <see cref="Player"/>.
        /// </summary>
        /// <param name="player">The <see cref="Player"/> who will receive the item.</param>
        /// <param name="pickup">The <see cref="ItemPickupBase"/> to be given.</param>
        /// <param name="displayMessage">Indicates whether or not <see cref="ShowPickedUpMessage"/> will be called when the player receives the item.</param>
        public virtual void Give(Player player, Pickup pickup, bool displayMessage = true) => Give(player, player.AddItem(pickup), displayMessage);

        /// <summary>
        /// Gives the <see cref="CustomItem"/> to a player.
        /// </summary>
        /// <param name="player">The <see cref="Player"/> who will receive the item.</param>
        /// <param name="displayMessage">Indicates whether or not <see cref="ShowPickedUpMessage"/> will be called when the player receives the item.</param>
        public virtual void Give(Player player, bool displayMessage = true) => Give(player, CreateCorrectItem(player.Inventory.CreateItemInstance(Type, true)), displayMessage);

        /// <summary>
        /// Called when the item is registered.
        /// </summary>
        public virtual void Init() => SubscribeEvents();

        /// <summary>
        /// Called when the item is unregistered.
        /// </summary>
        public virtual void Destroy() => UnsubscribeEvents();

        /// <summary>
        /// Checks the specified pickup to see if it is a custom item.
        /// </summary>
        /// <param name="pickup">The <see cref="Pickup"/> to check.</param>
        /// <returns>True if it is a custom item.</returns>
        public virtual bool Check(Pickup pickup) => TrackedSerials.Contains(pickup.Serial);

        /// <summary>
        /// Checks the specified inventory item to see if it is a custom item.
        /// </summary>
        /// <param name="item">The <see cref="Item"/> to check.</param>
        /// <returns>True if it is a custom item.</returns>
        public virtual bool Check(Item item) => item is not null && TrackedSerials.Contains(item.Serial);

        /// <summary>
        /// Checks the specified player's current item to see if it is a custom item.
        /// </summary>
        /// <param name="player">The <see cref="Player"/> who's current item should be checked.</param>
        /// <returns>True if it is a custom item.</returns>
        public virtual bool Check(Player player) => Check(player?.CurrentItem);

        /// <inheritdoc/>
        public override string ToString() => $"[{Name} ({Type}) | {Id}] {Description}";

        /// <summary>
        /// Registers a <see cref="CustomItem"/>.
        /// </summary>
        /// <returns>Returns a value indicating whether the <see cref="CustomItem"/> was registered or not.</returns>
        internal bool TryRegister()
        {
            if (!Instance.Config.IsEnabled)
                return false;

            Log.Debug($"Trying to register {Name} ({Id}).", Instance.Config.Debug);
            if (!Registered.Contains(this))
            {
                Log.Debug("Registered items doesn't contain this item yet..", Instance.Config.Debug);
                if (Registered.Any(customItem => customItem.Id == Id))
                {
                    Log.Warn($"{Name} has tried to register with the same custom item ID as another item: {Id}. It will not be registered.");

                    return false;
                }

                Log.Debug("Adding item to registered list..", Instance.Config.Debug);
                Registered.Add(this);

                Init();

                Log.Debug($"{Name} ({Id}) [{Type}] has been successfully registered.", Instance.Config.Debug);

                return true;
            }

            Log.Warn($"Couldn't register {Name} ({Id}) [{Type}] as it already exists.");

            return false;
        }

        /// <summary>
        /// Tries to unregister a <see cref="CustomItem"/>.
        /// </summary>
        /// <returns>Returns a value indicating whether the <see cref="CustomItem"/> was unregistered or not.</returns>
        internal bool TryUnregister()
        {
            Destroy();

            if (!Registered.Remove(this))
            {
                Log.Warn($"Cannot unregister {Name} ({Id}) [{Type}], it hasn't been registered yet.");

                return false;
            }

            return true;
        }

        /// <summary>
        /// Called after the manager is initialized, to allow loading of special event handlers.
        /// </summary>
        protected virtual void SubscribeEvents()
        {
            Events.Handlers.Player.Dying += OnInternalOwnerDying;
            Events.Handlers.Player.DroppingItem += OnInternalDropping;
            Events.Handlers.Player.ChangingItem += OnInternalChanging;
            Events.Handlers.Player.Escaping += OnInternalOwnerEscaping;
            Events.Handlers.Player.PickingUpItem += OnInternalPickingUp;
            Events.Handlers.Scp914.UpgradingItem += OnInternalUpgradingItem;
            Events.Handlers.Server.WaitingForPlayers += OnWaitingForPlayers;
            Events.Handlers.Player.Handcuffing += OnInternalOwnerHandcuffing;
            Events.Handlers.Player.ChangingRole += OnInternalOwnerChangingRole;
            Events.Handlers.Scp914.UpgradingInventoryItem += OnInternalUpgradingInventoryItem;
        }

        /// <summary>
        /// Called when the manager is being destroyed, to allow unloading of special event handlers.
        /// </summary>
        protected virtual void UnsubscribeEvents()
        {
            Events.Handlers.Player.Dying -= OnInternalOwnerDying;
            Events.Handlers.Player.DroppingItem -= OnInternalDropping;
            Events.Handlers.Player.ChangingItem -= OnInternalChanging;
            Events.Handlers.Player.Escaping -= OnInternalOwnerEscaping;
            Events.Handlers.Player.PickingUpItem -= OnInternalPickingUp;
            Events.Handlers.Scp914.UpgradingItem -= OnInternalUpgradingItem;
            Events.Handlers.Server.WaitingForPlayers -= OnWaitingForPlayers;
            Events.Handlers.Player.Handcuffing -= OnInternalOwnerHandcuffing;
            Events.Handlers.Player.ChangingRole -= OnInternalOwnerChangingRole;
            Events.Handlers.Scp914.UpgradingInventoryItem -= OnInternalUpgradingInventoryItem;
        }

        /// <summary>
        /// Handles tracking items when they a player changes their role.
        /// </summary>
        /// <param name="ev"><see cref="OwnerChangingRoleEventArgs"/>.</param>
        protected virtual void OnOwnerChangingRole(OwnerChangingRoleEventArgs ev)
        {
        }

        /// <summary>
        /// Handles making sure custom items are not "lost" when a player dies.
        /// </summary>
        /// <param name="ev"><see cref="OwnerDyingEventArgs"/>.</param>
        protected virtual void OnOwnerDying(OwnerDyingEventArgs ev)
        {
        }

        /// <summary>
        /// Handles making sure custom items are not "lost" when a player escapes.
        /// </summary>
        /// <param name="ev"><see cref="OwnerEscapingEventArgs"/>.</param>
        protected virtual void OnOwnerEscaping(OwnerEscapingEventArgs ev)
        {
        }

        /// <summary>
        /// Handles making sure custom items are not "lost" when being handcuffed.
        /// </summary>
        /// <param name="ev"><see cref="OwnerHandcuffingEventArgs"/>.</param>
        protected virtual void OnOwnerHandcuffing(OwnerHandcuffingEventArgs ev)
        {
        }

        /// <summary>
        /// Handles tracking items when they are dropped by a player.
        /// </summary>
        /// <param name="ev"><see cref="DroppingItemEventArgs"/>.</param>
        protected virtual void OnDropping(DroppingItemEventArgs ev)
        {
        }

        /// <summary>
        /// Handles tracking items when they are picked up by a player.
        /// </summary>
        /// <param name="ev"><see cref="PickingUpItemEventArgs"/>.</param>
        protected virtual void OnPickingUp(PickingUpItemEventArgs ev)
        {
        }

        /// <summary>
        /// Handles tracking items when they are selected in the player's inventory.
        /// </summary>
        /// <param name="ev"><see cref="ChangingItemEventArgs"/>.</param>
        protected virtual void OnChanging(ChangingItemEventArgs ev) => ShowSelectedMessage(ev.Player);

        /// <summary>
        /// Handles making sure custom items are not affected by SCP-914.
        /// </summary>
        /// <param name="ev"><see cref="UpgradingEventArgs"/>.</param>
        protected virtual void OnUpgrading(UpgradingEventArgs ev)
        {
        }

        /// <inheritdoc cref="OnUpgrading(UpgradingEventArgs)"/>
        protected virtual void OnUpgrading(API.EventArgs.UpgradingItemEventArgs ev)
        {
        }

        /// <summary>
        /// Called anytime the item enters a player's inventory by any means.
        /// </summary>
        /// <param name="player">The <see cref="Player"/> acquiring the item.</param>
        protected virtual void OnAcquired(Player player)
        {
        }

        /// <summary>
        /// Clears the lists of item uniqIDs and Pickups since any still in the list will be invalid.
        /// </summary>
        protected virtual void OnWaitingForPlayers()
        {
            TrackedSerials.Clear();
        }

        /// <summary>
        /// Shows a message to the player when he pickups a custom item.
        /// </summary>
        /// <param name="player">The <see cref="Player"/> who will be shown the message.</param>
        protected virtual void ShowPickedUpMessage(Player player)
        {
            player.ShowHint(string.Format(Instance.Config.PickedUpHint.Content, Name, Description), Instance.Config.PickedUpHint.Duration);
        }

        /// <summary>
        /// Shows a message to the player when he selects a custom item.
        /// </summary>
        /// <param name="player">The <see cref="Player"/> who will be shown the message.</param>
        protected virtual void ShowSelectedMessage(Player player)
        {
            player.ShowHint(string.Format(Instance.Config.SelectedHint.Content, Name, Description), Instance.Config.PickedUpHint.Duration);
        }

        /// <summary>
        /// This method will take the item's <see cref="Type"/> and create a new <see cref="Item"/> of the correct subtype for the <see cref="ItemType"/>.
        /// </summary>
        /// <param name="itemBase">The <see cref="ItemBase"/> to be used for creation, if any.</param>
        /// <returns>The <see cref="Item"/> created.</returns>
        protected Item CreateCorrectItem(ItemBase itemBase = null)
        {
            if (itemBase is null)
                itemBase = Server.Host.Inventory.CreateItemInstance(Type, false);
            return Item.Get(itemBase);
        }

        private void OnInternalOwnerChangingRole(ChangingRoleEventArgs ev)
        {
            if (ev.Reason == SpawnReason.Escaped)
                return;

            foreach (Item item in ev.Player.Items.ToList())
            {
                if (!Check(item))
                    continue;

                OnOwnerChangingRole(new OwnerChangingRoleEventArgs(item.Base, ev));

                TrackedSerials.Remove(item.Serial);

                ev.Player.RemoveItem(item);

                Spawn(ev.Player, item, null);

                MirrorExtensions.ResyncSyncVar(ev.Player.ReferenceHub.networkIdentity, typeof(NicknameSync), nameof(NicknameSync.Network_myNickSync));
            }
        }

        private void OnInternalOwnerDying(DyingEventArgs ev)
        {
            foreach (Item item in ev.Target.Items.ToList())
            {
                if (!Check(item))
                    continue;

                OnOwnerDying(new OwnerDyingEventArgs(item, ev));

                if (!ev.IsAllowed)
                    continue;

                ev.Target.RemoveItem(item);

                TrackedSerials.Remove(item.Serial);

                Spawn(ev.Target, item, null);

                MirrorExtensions.ResyncSyncVar(ev.Target.ReferenceHub.networkIdentity, typeof(NicknameSync), nameof(NicknameSync.Network_myNickSync));
            }
        }

        private void OnInternalOwnerEscaping(EscapingEventArgs ev)
        {
            foreach (Item item in ev.Player.Items.ToList())
            {
                if (!Check(item))
                    continue;

                OnOwnerEscaping(new OwnerEscapingEventArgs(item, ev));

                if (!ev.IsAllowed)
                    continue;

                ev.Player.RemoveItem(item);

                TrackedSerials.Remove(item.Serial);

                Timing.CallDelayed(1.5f, () => Spawn(ev.NewRole.GetRandomSpawnProperties().Item1, item, null));

                MirrorExtensions.ResyncSyncVar(ev.Player.ReferenceHub.networkIdentity, typeof(NicknameSync), nameof(NicknameSync.Network_myNickSync));
            }
        }

        private void OnInternalOwnerHandcuffing(HandcuffingEventArgs ev)
        {
            foreach (Item item in ev.Target.Items.ToList())
            {
                if (!Check(item))
                    continue;

                OnOwnerHandcuffing(new OwnerHandcuffingEventArgs(item, ev));

                if (!ev.IsAllowed)
                    continue;

                ev.Target.RemoveItem(item);

                TrackedSerials.Remove(item.Serial);

                Spawn(ev.Target, item, null);
            }
        }

        private void OnInternalDropping(DroppingItemEventArgs ev)
        {
            if (!Check(ev.Item))
                return;

            OnDropping(ev);

            if (!ev.IsAllowed)
                return;

            ev.IsAllowed = false;

#if DEBUG
            Log.Debug($"{ev.Player.Nickname} is dropping a {Name} ({ev.Item.Serial})");
#endif
            TrackedSerials.Remove(ev.Item.Serial);

            ev.Player.RemoveItem(ev.Item);
            if (ev.Player.Inventory.UserInventory.Items.ContainsKey(ev.Item.Serial))
            {
                ev.Player.Inventory.UserInventory.Items.Remove(ev.Item.Serial);
                ev.Player.Inventory.SendItemsNextFrame = true;
            }

            Pickup pickup = Spawn(ev.Player, ev.Item, null);
            if (pickup.Base.Rb is not null && ev.IsThrown)
            {
                Vector3 vector = (ev.Player.ReferenceHub.playerMovementSync.PlayerVelocity / 3f) + (ev.Player.ReferenceHub.PlayerCameraReference.forward * 6f * (Mathf.Clamp01(Mathf.InverseLerp(7f, 0.1f, pickup.Base.Rb.mass)) + 0.3f));
                vector.x = Mathf.Max(Mathf.Abs(ev.Player.ReferenceHub.playerMovementSync.PlayerVelocity.x), Mathf.Abs(vector.x)) * (float)((vector.x < 0f) ? -1 : 1);
                vector.y = Mathf.Max(Mathf.Abs(ev.Player.ReferenceHub.playerMovementSync.PlayerVelocity.y), Mathf.Abs(vector.y)) * (float)((vector.y < 0f) ? -1 : 1);
                vector.z = Mathf.Max(Mathf.Abs(ev.Player.ReferenceHub.playerMovementSync.PlayerVelocity.z), Mathf.Abs(vector.z)) * (float)((vector.z < 0f) ? -1 : 1);
                pickup.Base.Rb.position = ev.Player.ReferenceHub.PlayerCameraReference.position;
                pickup.Base.Rb.velocity = vector;
                pickup.Base.Rb.angularVelocity = Vector3.Lerp(ev.Item.Base.ThrowSettings.RandomTorqueA, ev.Item.Base.ThrowSettings.RandomTorqueB, UnityEngine.Random.value);
                float magnitude = pickup.Base.Rb.angularVelocity.magnitude;
                if (magnitude > pickup.Base.Rb.maxAngularVelocity)
                {
                    pickup.Base.Rb.maxAngularVelocity = magnitude;
                }
            }
        }

        private void OnInternalPickingUp(PickingUpItemEventArgs ev)
        {
            if (!Check(ev.Pickup) || ev.Player.Items.Count >= 8)
                return;

            OnPickingUp(ev);

            if (!ev.IsAllowed)
                return;

            if (!TrackedSerials.Contains(ev.Pickup.Serial))
                TrackedSerials.Add(ev.Pickup.Serial);

            Timing.CallDelayed(0.05f, () => OnAcquired(ev.Player));
        }

        private void OnInternalChanging(ChangingItemEventArgs ev)
        {
            if (!Check(ev.NewItem))
            {
                MirrorExtensions.ResyncSyncVar(ev.Player.ReferenceHub.networkIdentity, typeof(NicknameSync), nameof(NicknameSync.Network_displayName));
                return;
            }

            if (ShouldMessageOnGban)
            {
                foreach (Player player in Player.Get(RoleType.Spectator))
                {
                    Timing.CallDelayed(0.5f, () => player.SendFakeSyncVar(ev.Player.ReferenceHub.networkIdentity, typeof(NicknameSync), nameof(NicknameSync.Network_displayName), $"{ev.Player.Nickname} (CustomItem: {Name})"));
                }
            }

            OnChanging(ev);
        }

        private void OnInternalUpgradingInventoryItem(UpgradingInventoryItemEventArgs ev)
        {
            if (!Check(ev.Item))
                return;

            ev.IsAllowed = false;
            OnUpgrading(new API.EventArgs.UpgradingItemEventArgs(ev.Player, ev.Item.Base, ev.KnobSetting));
        }

        private void OnInternalUpgradingItem(Events.EventArgs.UpgradingItemEventArgs ev)
        {
            if (!Check(ev.Item))
                return;

            ev.IsAllowed = false;

            Timing.CallDelayed(3.5f, () =>
            {
                ev.Item.Position = ev.OutputPosition;
                OnUpgrading(new UpgradingEventArgs(ev.Item.Base, ev.OutputPosition, ev.KnobSetting));
            });
        }
    }
}
