// -----------------------------------------------------------------------
// <copyright file="CustomItem.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomModules.API.Features.CustomItems
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using Exiled.API.Enums;
    using Exiled.API.Extensions;
    using Exiled.API.Features;
    using Exiled.API.Features.Core;
    using Exiled.API.Features.Core.Interfaces;
    using Exiled.API.Features.Items;
    using Exiled.API.Features.Pickups;
    using Exiled.API.Features.Spawn;
    using Exiled.CustomModules.API.Enums;
    using Exiled.CustomModules.API.Features.Attributes;
    using Exiled.CustomModules.API.Features.CustomEscapes;
    using Exiled.CustomModules.API.Features.CustomItems.Items;
    using MapGeneration.Distributors;
    using UnityEngine;

    /// <summary>
    /// A class to easily manage item behavior.
    /// </summary>
    public abstract class CustomItem : CustomModule, IAdditiveBehaviour
    {
        private static readonly List<CustomItem> Registered = new();
        private static readonly Dictionary<Item, CustomItem> ItemsValue = new();
        private static readonly Dictionary<Pickup, CustomItem> PickupValue = new();
        private static readonly Dictionary<Type, CustomItem> TypeLookupTable = new();
        private static readonly Dictionary<Type, CustomItem> BehaviourLookupTable = new();
        private static readonly Dictionary<uint, CustomItem> IdLookupTable = new();
        private static readonly Dictionary<string, CustomItem> NameLookupTable = new();

        /// <summary>
        /// Gets all tracked behaviours.
        /// </summary>
#pragma warning disable SA1202 // Elements should be ordered by access
        internal static readonly Dictionary<Pickup, ItemBehaviour> TrackedBehaviours = new();
#pragma warning restore SA1202 // Elements should be ordered by access

        /// <summary>
        /// Gets a <see cref="List{T}"/> which contains all registered <see cref="CustomItem"/>'s.
        /// </summary>
        public static IEnumerable<CustomItem> List => Registered;

        /// <summary>
        /// Gets all Items and their respective <see cref="CustomItem"/>.
        /// </summary>
        public static IReadOnlyDictionary<Item, CustomItem> ItemManager => ItemsValue;

        /// <summary>
        /// Gets all Pickups and their respective <see cref="CustomItem"/>.
        /// </summary>
        public static IReadOnlyDictionary<Pickup, CustomItem> PickupManager => PickupValue;

        /// <summary>
        /// Gets all pickups belonging to a <see cref="CustomItem"/>.
        /// </summary>
        public static HashSet<Pickup> CustomItemsUnhold => PickupManager.Keys.ToHashSet();

        /// <summary>
        /// Gets all items belonging to a <see cref="CustomItem"/>.
        /// </summary>
        public static HashSet<Item> CustomItemsHolded => ItemManager.Keys.ToHashSet();

        /// <summary>
        /// Gets the <see cref="CustomItem"/>'s <see cref="Type"/>.
        /// </summary>
        public virtual Type BehaviourComponent { get; }

        /// <summary>
        /// Gets the <see cref="CustomItem"/>'s name.
        /// </summary>
        public override string Name { get; }

        /// <summary>
        /// Gets or sets the <see cref="CustomItem"/>'s id.
        /// </summary>
        public override uint Id { get; protected set; }

        /// <summary>
        /// Gets a value indicating whether the <see cref="CustomItem"/> is enabled.
        /// </summary>
        public override bool IsEnabled { get; }

        /// <summary>
        /// Gets the <see cref="CustomItem"/>'s description.
        /// </summary>
        public virtual string Description { get; }

        /// <summary>
        /// Gets the <see cref="CustomItem"/>'s <see cref="global::ItemType"/>.
        /// </summary>
        public virtual ItemType ItemType { get; }

        /// <summary>
        /// Gets the <see cref="CustomItem"/>'s <see cref="global::ItemCategory"/>.
        /// </summary>
        public virtual ItemCategory ItemCategory { get; }

        /// <summary>
        /// Gets the <see cref="Settings"/>.
        /// </summary>
        public virtual Settings Settings { get; }

        /// <summary>
        /// Gets a value indicating whether the <see cref="CustomItem"/> is registered.
        /// </summary>
        public virtual bool IsRegistered => Registered.Contains(this);

        /// <summary>
        /// Gets a <see cref="IEnumerable{T}"/> of <see cref="Pickup"/> containing all pickup owning this <see cref="Pickup"/>.
        /// </summary>
        public IEnumerable<Pickup> Pickups => PickupManager.Where(x => x.Value.Id == Id).Select(x => x.Key);

        /// <summary>
        /// Gets a <see cref="IEnumerable{T}"/> of <see cref="Item"/> containing all item owning this <see cref="CustomItem"/>.
        /// </summary>
        public IEnumerable<Item> Items => ItemsValue.Where(x => x.Value.Id == Id).Select(x => x.Key);

        /// <summary>
        /// Gets a <see cref="CustomItem"/> based on the provided id or <see cref="UUCustomItemType"/>.
        /// </summary>
        /// <param name="id">The id or <see cref="UUCustomItemType"/> of the custom item.</param>
        /// <returns>The <see cref="CustomItem"/> with the specified id, or <see langword="null"/> if no item is found.</returns>
        public static CustomItem Get(object id) => id is uint or UUCustomItemType ? Get((uint)id) : null;

        /// <summary>
        /// Retrieves a <see cref="CustomItem"/> instance based on the specified custom item id.
        /// </summary>
        /// <param name="id">The custom item id to retrieve.</param>
        /// <returns>The retrieved <see cref="CustomItem"/> instance if found and enabled; otherwise, <see langword="null"/>.</returns>
        public static CustomItem Get(uint id) => IdLookupTable[id];

        /// <summary>
        /// Retrieves a <see cref="CustomItem"/> instance based on the specified item name.
        /// </summary>
        /// <param name="name">The name of the custom item to retrieve.</param>
        /// <returns>The retrieved <see cref="CustomItem"/> instance if found; otherwise, <see langword="null"/>.</returns>
        public static CustomItem Get(string name) => NameLookupTable[name];

        /// <summary>
        /// Retrieves a <see cref="CustomItem"/> instance based on the specified type.
        /// </summary>
        /// <param name="type">The type to retrieve the custom item for.</param>
        /// <returns>The retrieved <see cref="CustomItem"/> instance if found and enabled; otherwise, <see langword="null"/>.</returns>
        public static CustomItem Get(Type type) =>
            typeof(CustomItem).IsAssignableFrom(type) ? TypeLookupTable[type] :
            typeof(ItemBehaviour).IsAssignableFrom(type) ? BehaviourLookupTable[type] : null;

        /// <summary>
        /// Retrieves a <see cref="CustomItem"/> instance based on the specified <see cref="Item"/> instance.
        /// </summary>
        /// <param name="item">The <see cref="Item"/> instance to retrieve the custom item from.</param>
        /// <returns>The retrieved <see cref="CustomItem"/> instance if found; otherwise, <see langword="null"/>.</returns>
        public static CustomItem Get(Item item)
        {
            CustomItem customItem = default;

            foreach (KeyValuePair<Item, CustomItem> kvp in ItemManager)
            {
                if (kvp.Key != item)
                    continue;

                customItem = kvp.Value;
            }

            return customItem;
        }

        /// <summary>
        /// Retrieves a <see cref="CustomItem"/> instance based on the specified <see cref="Pickup"/> instance.
        /// </summary>
        /// <param name="item">The <see cref="Pickup"/> instance to retrieve the custom item from.</param>
        /// <returns>The retrieved <see cref="CustomItem"/> instance if found; otherwise, <see langword="null"/>.</returns>
        public static CustomItem Get(Pickup item)
        {
            CustomItem customItem = default;

            foreach (KeyValuePair<Pickup, CustomItem> kvp in PickupManager)
            {
                if (kvp.Key != item)
                    continue;

                customItem = kvp.Value;
            }

            return customItem;
        }

        /// <summary>
        /// Attempts to retrieve a <see cref="CustomItem"/> based on the provided id or <see cref="UUCustomItemType"/>.
        /// </summary>
        /// <param name="id">The id or <see cref="UUCustomItemType"/> of the custom item.</param>
        /// <param name="customItem">When this method returns, contains the <see cref="CustomItem"/> associated with the specified id, if the id was found; otherwise, <see langword="null"/>.</param>
        /// <returns><see langword="true"/> if a <see cref="CustomItem"/> was found; otherwise, <see langword="false"/>.</returns>
        public static bool TryGet(object id, out CustomItem customItem) => customItem = Get(id);

        /// <summary>
        /// Tries to retrieve a <see cref="CustomItem"/> instance based on the specified custom item id.
        /// </summary>
        /// <param name="id">The custom item id to retrieve.</param>
        /// <param name="customItem">The retrieved <see cref="CustomItem"/> instance, if successful; otherwise, <see langword="null"/>.</param>
        /// <returns><see langword="true"/> if the retrieval is successful; otherwise, <see langword="false"/>.</returns>
        public static bool TryGet(uint id, out CustomItem customItem) => customItem = Get(id);

        /// <summary>
        /// Tries to retrieve a <see cref="CustomItem"/> instance based on the specified item name.
        /// </summary>
        /// <param name="name">The name of the custom item to retrieve.</param>
        /// <param name="customItem">The retrieved <see cref="CustomItem"/> instance, if successful; otherwise, <see langword="null"/>.</param>
        /// <returns><see langword="true"/> if the retrieval is successful; otherwise, <see langword="false"/>.</returns>
        public static bool TryGet(string name, out CustomItem customItem) => customItem = Get(name);

        /// <summary>
        /// Tries to retrieve a <see cref="CustomItem"/> instance based on the specified <see cref="Item"/> instance.
        /// </summary>
        /// <param name="item">The <see cref="Item"/> instance to retrieve the custom item for.</param>
        /// <param name="customItem">The retrieved <see cref="CustomItem"/> instance, if successful; otherwise, <see langword="null"/>.</param>
        /// <returns><see langword="true"/> if the retrieval is successful; otherwise, <see langword="false"/>.</returns>
        public static bool TryGet(Item item, out CustomItem customItem) => customItem = Get(item);

        /// <summary>
        /// Tries to retrieve a <see cref="CustomItem"/> instance based on the specified <see cref="Pickup"/> instance.
        /// </summary>
        /// <param name="pickup">The <see cref="Pickup"/> instance to retrieve the custom item for.</param>
        /// <param name="customItem">The retrieved <see cref="CustomItem"/> instance, if successful; otherwise, <see langword="null"/>.</param>
        /// <returns><see langword="true"/> if the retrieval is successful; otherwise, <see langword="false"/>.</returns>
        public static bool TryGet(Pickup pickup, out CustomItem customItem) => customItem = Get(pickup);

        /// <summary>
        /// Tries to retrieve a <see cref="CustomItem"/> instance based on the specified type.
        /// </summary>
        /// <param name="type">The type to retrieve the custom item for.</param>
        /// <param name="customItem">The retrieved <see cref="CustomItem"/> instance, if successful; otherwise, <see langword="null"/>.</param>
        /// <returns><see langword="true"/> if the retrieval is successful; otherwise, <see langword="false"/>.</returns>
        public static bool TryGet(Type type, out CustomItem customItem) => customItem = Get(type);

        /// <summary>
        /// Tries to spawn a custom item at the specified position.
        /// </summary>
        /// <param name="position">The position where the item should be spawned.</param>
        /// <param name="customItem">The custom item to spawn.</param>
        /// <param name="pickup">The spawned pickup, if successful; otherwise, <see langword="null"/>.</param>
        /// <returns><see langword="true"/> if the spawn is successful; otherwise, <see langword="false"/>.</returns>
        public static bool TrySpawn(Vector3 position, CustomItem customItem, out Pickup pickup)
        {
            pickup = default;

            if (!customItem)
                return false;

            pickup = customItem.Spawn(position);

            return true;
        }

        /// <summary>
        /// Tries to spawn a custom item at the specified position using the specified custom item id.
        /// </summary>
        /// <param name="position">The position where the item should be spawned.</param>
        /// <param name="id">The custom item id to spawn.</param>
        /// <param name="pickup">The spawned pickup, if successful; otherwise, <see langword="null"/>.</param>
        /// <returns><see langword="true"/> if the spawn is successful; otherwise, <see langword="false"/>.</returns>
        public static bool TrySpawn(Vector3 position, uint id, out Pickup pickup)
        {
            pickup = default;

            if (!TryGet(id, out CustomItem customItem))
                return false;

            TrySpawn(position, customItem, out pickup);

            return true;
        }

        /// <summary>
        /// Tries to spawn a custom item at the specified position using the specified item name.
        /// </summary>
        /// <param name="position">The position where the item should be spawned.</param>
        /// <param name="name">The name of the custom item to spawn.</param>
        /// <param name="pickup">The spawned pickup, if successful; otherwise, <see langword="null"/>.</param>
        /// <returns><see langword="true"/> if the spawn is successful; otherwise, <see langword="false"/>.</returns>
        public static bool TrySpawn(Vector3 position, string name, out Pickup pickup)
        {
            pickup = default;

            if (!TryGet(name, out CustomItem customItem))
                return false;

            TrySpawn(position, customItem, out pickup);

            return true;
        }

        /// <summary>
        /// Tries to give a custom item to a player using the specified item name.
        /// </summary>
        /// <param name="player">The player to give the item to.</param>
        /// <param name="name">The name of the custom item to give.</param>
        /// <param name="displayMessage">Determines whether to display a message to the player.</param>
        /// <returns><see langword="true"/> if the give is successful; otherwise, <see langword="false"/>.</returns>
        public static bool TryGive(Player player, string name, bool displayMessage = true)
        {
            if (!TryGet(name, out CustomItem item))
                return false;

            item.Give(player, displayMessage);

            return true;
        }

        /// <summary>
        /// Tries to give a custom item to a player using the specified item ID.
        /// </summary>
        /// <param name="player">The player to give the item to.</param>
        /// <param name="id">The ID of the custom item to give.</param>
        /// <param name="displayMessage">Determines whether to display a message to the player.</param>
        /// <returns><see langword="true"/> if the give is successful; otherwise, <see langword="false"/>.</returns>
        public static bool TryGive(Player player, uint id, bool displayMessage = true)
        {
            if (!TryGet(id, out CustomItem item))
                return false;

            item.Give(player, displayMessage);

            return true;
        }

        /// <summary>
        /// Tries to give a custom item to a player using the specified item type.
        /// </summary>
        /// <param name="player">The player to give the item to.</param>
        /// <param name="type">The type of the custom item to give.</param>
        /// <param name="displayMessage">Determines whether to display a message to the player.</param>
        /// <returns><see langword="true"/> if the give is successful; otherwise, <see langword="false"/>.</returns>
        public static bool TryGive(Player player, Type type, bool displayMessage = true)
        {
            if (!TryGet(type, out CustomItem item))
                return false;

            item.Give(player, displayMessage);

            return true;
        }

        /// <summary>
        /// Enables all the custom items present in the assembly.
        /// </summary>
        /// <returns>
        /// A <see cref="List{T}"/> of <see cref="CustomItem"/> containing all the enabled custom items.
        /// </returns>
        /// <remarks>
        /// This method dynamically enables all custom items found in the calling assembly. Custom items
        /// must be marked with the <see cref="CustomItemAttribute"/> to be considered for enabling. If
        /// a custom item is enabled successfully, it is added to the returned list.
        /// </remarks>
        public static List<CustomItem> EnableAll() => EnableAll(Assembly.GetCallingAssembly());

        /// <summary>
        /// Enables all the custom items present in the assembly.
        /// </summary>
        /// <param name="assembly">The assembly to enable the items from.</param>
        /// <returns>
        /// A <see cref="List{T}"/> of <see cref="CustomItem"/> containing all the enabled custom items.
        /// </returns>
        /// <remarks>
        /// This method dynamically enables all custom items found in the calling assembly. Custom items
        /// must be marked with the <see cref="CustomItemAttribute"/> to be considered for enabling. If
        /// a custom item is enabled successfully, it is added to the returned list.
        /// </remarks>
        public static List<CustomItem> EnableAll(Assembly assembly)
        {
            List<CustomItem> customItems = new();
            foreach (Type type in assembly.GetTypes())
            {
                CustomItemAttribute attribute = type.GetCustomAttribute<CustomItemAttribute>();
                if (!typeof(CustomItem).IsAssignableFrom(type) || attribute is null)
                    continue;

                CustomItem customItem = Activator.CreateInstance(type) as CustomItem;

                if (!customItem.IsEnabled)
                    continue;

                if (customItem.TryRegister(attribute))
                    customItems.Add(customItem);
            }

            if (customItems.Count != Registered.Count)
                Log.Info($"{customItems.Count} custom items have been successfully registered!");

            return customItems;
        }

        /// <summary>
        /// Disables all the custom items present in the assembly.
        /// </summary>
        /// <returns>
        /// A <see cref="List{T}"/> of <see cref="CustomItem"/> containing all the disabled custom items.
        /// </returns>
        /// <remarks>
        /// This method dynamically disables all custom items found in the calling assembly that were
        /// previously registered. If a custom item is disabled successfully, it is added to the returned list.
        /// </remarks>
        public static List<CustomItem> DisableAll()
        {
            List<CustomItem> customItems = new();
            customItems.AddRange(Registered.Where(customItem => customItem.TryUnregister()));

            Log.Info($"{customItems.Count} custom items have been successfully unregistered!");

            return customItems;
        }

        /// <summary>
        /// Spawns the <see cref="CustomItem"/> in a specific location.
        /// </summary>
        /// <param name="x">The x coordinate.</param>
        /// <param name="y">The y coordinate.</param>
        /// <param name="z">The z coordinate.</param>
        /// <returns>The <see cref="Pickup"/> wrapper of the spawned <see cref="CustomItem"/>.</returns>
        public virtual Pickup Spawn(float x, float y, float z) => Spawn(new Vector3(x, y, z));

        /// <summary>
        /// Spawns a <see cref="Item"/> as a <see cref="CustomItem"/> in a specific location.
        /// </summary>
        /// <param name="x">The x coordinate.</param>
        /// <param name="y">The y coordinate.</param>
        /// <param name="z">The z coordinate.</param>
        /// <param name="item">The <see cref="ItemType"/> to be spawned as a <see cref="CustomItem"/>.</param>
        /// <returns>The <see cref="Pickup"/> wrapper of the spawned <see cref="CustomItem"/>.</returns>
        public virtual Pickup Spawn(float x, float y, float z, Item item) => Spawn(new Vector3(x, y, z), item);

        /// <summary>
        /// Spawns the <see cref="CustomItem"/> where a specific <see cref="Player"/> is, and optionally sets the previous owner.
        /// </summary>
        /// <param name="player">The <see cref="Player"/> position where the <see cref="CustomItem"/> will be spawned.</param>
        /// <param name="previousOwner">The previous owner of the pickup, can be null.</param>
        /// <returns>The <see cref="Pickup"/> of the spawned <see cref="CustomItem"/>.</returns>
        public virtual Pickup Spawn(Player player, Player previousOwner = null) => Spawn(player.Position, previousOwner);

        /// <summary>
        /// Spawns an <see cref="Item"/> as a <see cref="CustomItem"/> where a specific <see cref="Player"/> is, and optionally sets the previous owner.
        /// </summary>
        /// <param name="player">The <see cref="Player"/> position where the <see cref="CustomItem"/> will be spawned.</param>
        /// <param name="item">The <see cref="ItemType"/> to be spawned as a <see cref="CustomItem"/>.</param>
        /// <param name="previousOwner">The previous owner of the pickup, can be null.</param>
        /// <returns>The <see cref="Pickup"/> of the spawned <see cref="CustomItem"/>.</returns>
        public virtual Pickup Spawn(Player player, Item item, Player previousOwner = null) => Spawn(player.Position, item, previousOwner);

        /// <summary>
        /// Spawns the <see cref="CustomItem"/> in a specific position.
        /// </summary>
        /// <param name="position">The <see cref="Vector3"/> where the <see cref="CustomItem"/> will be spawned.</param>
        /// <param name="previousOwner">The <see cref="Pickup.PreviousOwner"/> of the item. Can be null.</param>
        /// <returns>The <see cref="Pickup"/> of the spawned <see cref="CustomItem"/>.</returns>
        public virtual Pickup Spawn(Vector3 position, Player previousOwner = null) => Spawn(position, Item.Create(ItemType), previousOwner);

        /// <summary>
        /// Spawns an <see cref="Item"/> as <see cref="CustomItem"/> in a specific position.
        /// </summary>
        /// <param name="position">The <see cref="Vector3"/> where the <see cref="CustomItem"/> will be spawned.</param>
        /// <param name="item">The <see cref="ItemType"/> to be spawned as a <see cref="CustomItem"/>.</param>
        /// <param name="previousOwner">The <see cref="Pickup.PreviousOwner"/> of the item. Can be null.</param>
        /// <returns>The <see cref="Pickup"/> of the spawned <see cref="CustomItem"/>.</returns>
        public virtual Pickup Spawn(Vector3 position, Item item, Player previousOwner = null)
        {
            item.AddComponent(BehaviourComponent);
            Pickup pickup = item.CreatePickup(position);

            ItemTracker tracker = StaticActor.Get<ItemTracker>();
            tracker.AddOrTrack(item, pickup);
            tracker.Restore(pickup, item);

            pickup.Scale = Settings.Scale;

            if (Settings.Weight != -1)
                pickup.Weight = Settings.Weight;

            if (previousOwner)
                pickup.PreviousOwner = previousOwner;

            PickupValue.Add(pickup, this);

            return pickup;
        }

        /// <summary>
        /// Spawns the specified number of items at given spawn points.
        /// </summary>
        /// <param name="spawnPoints">The collection of spawn points to use.</param>
        /// <param name="limit">The maximum number of items to spawn.</param>
        /// <returns>The total number of items spawned.</returns>
        public virtual uint Spawn(IEnumerable<SpawnPoint> spawnPoints, uint limit)
        {
            uint spawned = 0;

            foreach (SpawnPoint spawnPoint in spawnPoints)
            {
                Log.Debug($"Attempting to spawn {Name} at {spawnPoint.Position}.", true);

                if (Loader.Loader.Random.NextDouble() * 100 >= spawnPoint.Chance || (limit > 0 && spawned >= limit))
                    continue;

                spawned++;

                if (spawnPoint is DynamicSpawnPoint dynamicSpawnPoint && dynamicSpawnPoint.Location == SpawnLocationType.InsideLocker)
                {
                    for (int i = 0; i < 50; i++)
                    {
                        if (Map.Lockers is null)
                            continue;

                        Locker locker = Map.Lockers[Loader.Loader.Random.Next(Map.Lockers.Count)];

                        if (locker is null || locker.Loot is null || locker.Chambers is null)
                            continue;

                        LockerChamber chamber = locker.Chambers[Loader.Loader.Random.Next(Mathf.Max(0, locker.Chambers.Length - 1))];
                        Vector3 position = chamber._spawnpoint.transform.position;
                        Spawn(position, null);

                        Log.Debug($"Spawned {Name} at {position} ({spawnPoint.Name})", true);

                        break;
                    }
                }
                else if (spawnPoint is RoleSpawnPoint roleSpawnPoint)
                {
                    Spawn(roleSpawnPoint.Role.GetRandomSpawnLocation().Position, null);
                }
                else
                {
                    Spawn(spawnPoint.Position, null);

                    Log.Debug($"Spawned {Name} at {spawnPoint.Position} ({spawnPoint.Name})", true);
                }
            }

            return spawned;
        }

        /// <summary>
        /// Spawns all items at their dynamic and static positions.
        /// </summary>
        public virtual void SpawnAll()
        {
            if (Settings is null || Settings.SpawnProperties is not SpawnProperties spawnProperties)
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
            Spawn(
                spawnProperties.StaticSpawnPoints,
                Math.Min(0, spawnProperties.Limit - Math.Min(0, Spawn(spawnProperties.DynamicSpawnPoints, spawnProperties.Limit) - Spawn(spawnProperties.RoleSpawnPoints, spawnProperties.Limit))));
        }

        /// <summary>
        /// Gives a specific <see cref="Item"/> to the specified <see cref="Player"/>.
        /// </summary>
        /// <param name="player">The <see cref="Player"/> to receive the item.</param>
        /// <param name="item">The <see cref="Item"/> to be given to the player.</param>
        /// <param name="displayMessage">Determines whether to display a message for the action (default is true).</param>
        public virtual void Give(Player player, Item item, bool displayMessage = true)
        {
            try
            {
                item.AddComponent(BehaviourComponent);
                StaticActor.Get<ItemTracker>().AddOrTrack(item);
                player.AddItem(item);
                ItemsValue.Add(item, this);
            }
            catch (Exception e)
            {
                Log.Error($"{nameof(Give)}: {e}");
            }
        }

        /// <summary>
        /// Gives the specified <see cref="Pickup"/> to the specified <see cref="Player"/>.
        /// </summary>
        /// <param name="player">The <see cref="Player"/> to receive the pickup.</param>
        /// <param name="pickup">The <see cref="Pickup"/> to be given to the player.</param>
        /// <param name="displayMessage">Determines whether to display a message for the action (default is true).</param>
        public virtual void Give(Player player, Pickup pickup, bool displayMessage = true) => Give(player, player.AddItem(pickup), displayMessage);

        /// <summary>
        /// Gives a new instance of the custom item to the specified <see cref="Player"/>.
        /// </summary>
        /// <param name="player">The <see cref="Player"/> to receive the item.</param>
        /// <param name="displayMessage">Determines whether to display a message for the action (default is true).</param>
        public virtual void Give(Player player, bool displayMessage = true) => Give(player, Item.Create(ItemType), displayMessage);

        /// <summary>
        /// Tries to register a <see cref="CustomItem"/>.
        /// </summary>
        /// <param name="attribute">The specified <see cref="CustomItemAttribute"/>.</param>
        /// <returns><see langword="true"/> if the <see cref="CustomItem"/> was registered; otherwise, <see langword="false"/>.</returns>
        internal bool TryRegister(CustomItemAttribute attribute = null)
        {
            if (!Registered.Contains(this))
            {
                if (attribute is not null && Id == 0)
                {
                    if (attribute.Id != 0)
                        Id = attribute.Id;
                    else
                        throw new ArgumentException($"Unable to register {Name}. The ID 0 is reserved for special use.");
                }

                CustomItem duplicate = Registered.FirstOrDefault(x => x.Id == Id || x.Name == Name || x.BehaviourComponent == BehaviourComponent);
                if (duplicate)
                {
                    Log.Warn($"Unable to register {Name}. Another item with the same ID, Name or Behaviour Component already exists: {duplicate.Name}");

                    return false;
                }

                EObject.RegisterObjectType(BehaviourComponent, Name);
                Registered.Add(this);

                TypeLookupTable.TryAdd(GetType(), this);
                BehaviourLookupTable.TryAdd(BehaviourComponent, this);
                IdLookupTable.TryAdd(Id, this);
                NameLookupTable.TryAdd(Name, this);

                return true;
            }

            Log.Warn($"Unable to register {Name}. Item already exists.");

            return false;
        }

        /// <summary>
        /// Tries to unregister a <see cref="CustomItem"/>.
        /// </summary>
        /// <returns><see langword="true"/> if the <see cref="CustomItem"/> was unregistered; otherwise, <see langword="false"/>.</returns>
        internal bool TryUnregister()
        {
            if (!Registered.Contains(this))
            {
                Log.Debug($"Unable to unregister {Name}. Item is not yet registered.");

                return false;
            }

            EObject.UnregisterObjectType(BehaviourComponent);
            Registered.Remove(this);

            TypeLookupTable.Remove(GetType());
            BehaviourLookupTable.Remove(BehaviourComponent);
            IdLookupTable.Remove(Id);
            NameLookupTable.Remove(Name);

            return true;
        }
    }
}
