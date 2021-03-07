// -----------------------------------------------------------------------
// <copyright file="CustomItem.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomItems.API.Features
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;

    using Exiled.API.Extensions;
    using Exiled.API.Features;
    using Exiled.CustomItems.API.EventArgs;
    using Exiled.CustomItems.API.Spawn;
    using Exiled.Events.EventArgs;

    using MEC;

    using UnityEngine;

    using YamlDotNet.Serialization;

    using static CustomItems;

    /// <summary>
    /// The Custom Item base class.
    /// </summary>
    public abstract class CustomItem
    {
        private ItemType type;

        /// <summary>
        /// Gets the list of current Item Managers.
        /// </summary>
        public static HashSet<CustomItem> Registered { get; } = new HashSet<CustomItem>();

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
        /// Gets or sets the list of spawn locations and chances for each one.
        /// </summary>
        public abstract SpawnProperties SpawnProperties { get; set; }

        /// <summary>
        /// Gets or sets the item durability.
        /// </summary>
        [YamlIgnore]
        public virtual float Durability { get; set; }

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
        public HashSet<int> InsideInventories { get; } = new HashSet<int>();

        /// <summary>
        /// Gets the list of spawned custom items being tracked as the current item.
        /// </summary>
        [YamlIgnore]
        public HashSet<Pickup> Spawned { get; } = new HashSet<Pickup>();

        /// <summary>
        /// Gets a <see cref="CustomItem"/> with a specific ID.
        /// </summary>
        /// <param name="id">The <see cref="CustomItem"/> ID.</param>
        /// <returns>The <see cref="CustomItem"/> matching the search, null if not registered.</returns>
        public static CustomItem Get(int id) => Registered?.FirstOrDefault(tempCustomItem => tempCustomItem.Id == id);

        /// <summary>
        /// Gets a <see cref="CustomItem"/> with a specific name.
        /// </summary>
        /// <param name="name">The <see cref="CustomItem"/> name.</param>
        /// <returns>The <see cref="CustomItem"/> matching the search, null if not registered.</returns>
        public static CustomItem Get(string name) => Registered?.FirstOrDefault(tempCustomItem => tempCustomItem.Name == name);

        /// <summary>
        /// Tries to get a <see cref="CustomItem"/> with a specific ID.
        /// </summary>
        /// <param name="id">The <see cref="CustomItem"/> ID to look for.</param>
        /// <param name="customItem">The found <see cref="CustomItem"/>, null if not registered.</param>
        /// <returns>Returns a value indicating whether the <see cref="CustomItem"/> was found or not.</returns>
        public static bool TryGet(int id, out CustomItem customItem)
        {
            customItem = Get(id);

            return customItem != null;
        }

        /// <summary>
        /// Tries to get a <see cref="CustomItem"/> with a specific name.
        /// </summary>
        /// <param name="name">The <see cref="CustomItem"/> name to look for.</param>
        /// <param name="customItem">The found <see cref="CustomItem"/>, null if not registered.</param>
        /// <returns>Returns a value indicating whether the <see cref="CustomItem"/> was found or not.</returns>
        public static bool TryGet(string name, out CustomItem customItem)
        {
            if (name == null)
                throw new ArgumentNullException("name");

            customItem = int.TryParse(name, out int id) ? Get(id) : Get(name);

            return customItem != null;
        }

        /// <summary>
        /// Tries to get the player's current <see cref="CustomItem"/> in their hand.
        /// </summary>
        /// <param name="player">The <see cref="Player"/> to check.</param>
        /// <param name="customItem">The <see cref="CustomItem"/> in their hand.</param>
        /// <returns>Returns a value indicating whether the <see cref="Player"/> has a <see cref="CustomItem"/> in their hand or not.</returns>
        public static bool TryGet(Player player, out CustomItem customItem)
        {
            if (player == null)
                throw new ArgumentNullException("player");

            customItem = Registered?.FirstOrDefault(tempCustomItem => tempCustomItem.Check(player.CurrentItem));

            return customItem != null;
        }

        /// <summary>
        /// Tries to get the player's <see cref="IEnumerable{T}"/> of <see cref="CustomItem"/>.
        /// </summary>
        /// <param name="player">The <see cref="Player"/> to check.</param>
        /// <param name="customItems">The player's <see cref="IEnumerable{T}"/> of <see cref="CustomItem"/>.</param>
        /// <returns>Returns a value indicating whether the <see cref="Player"/> has a <see cref="CustomItem"/> in their hand or not.</returns>
        public static bool TryGet(Player player, out IEnumerable<CustomItem> customItems)
        {
            if (player == null)
                throw new ArgumentNullException("player");

            customItems = Registered?.Where(tempCustomItem => player.Inventory.items.Any(item => tempCustomItem.Check(item)));

            return customItems?.Any() ?? false;
        }

        /// <summary>
        /// Checks to see if this item is a custom item.
        /// </summary>
        /// <param name="item">The <see cref="Inventory.SyncItemInfo"/> to check.</param>
        /// <param name="customItem">The <see cref="CustomItem"/> this item is.</param>
        /// <returns>True if the item is a custom item.</returns>
        public static bool TryGet(Inventory.SyncItemInfo item, out CustomItem customItem)
        {
            customItem = Registered?.FirstOrDefault(tempCustomItem => tempCustomItem.InsideInventories.Contains(item.uniq));

            return customItem != null;
        }

        /// <summary>
        /// Checks if this pickup is a custom item.
        /// </summary>
        /// <param name="pickup">The <see cref="Pickup"/> to check.</param>
        /// <param name="customItem">The <see cref="CustomItem"/> this pickup is.</param>
        /// <returns>True if the pickup is a custom item.</returns>
        public static bool TryGet(Pickup pickup, out CustomItem customItem)
        {
            customItem = Registered?.FirstOrDefault(tempCustomItem => tempCustomItem.Spawned.Contains(pickup));

            return customItem != null;
        }

        /// <summary>
        /// Tries to spawn a specific <see cref="CustomItem"/> at a specific <see cref="Vector3"/> position.
        /// </summary>
        /// <param name="id">The ID of the <see cref="CustomItem"/> to spawn.</param>
        /// <param name="position">The <see cref="Vector3"/> location to spawn the item.</param>
        /// <returns>Returns a value indicating whether the <see cref="CustomItem"/> was spawned or not.</returns>
        public static bool TrySpawn(int id, Vector3 position)
        {
            if (!TryGet(id, out CustomItem item))
                return false;

            item.Spawn(position);

            return true;
        }

        /// <summary>
        /// Tries to spawn a specific <see cref="CustomItem"/> at a specific <see cref="Vector3"/> position.
        /// </summary>
        /// <param name="name">The name of the <see cref="CustomItem"/> to spawn.</param>
        /// <param name="position">The <see cref="Vector3"/> location to spawn the item.</param>
        /// <returns>Returns a value indicating whether the <see cref="CustomItem"/> was spawned or not.</returns>
        public static bool TrySpawn(string name, Vector3 position)
        {
            if (!TryGet(name, out CustomItem item))
                return false;

            item.Spawn(position);

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
        /// Registers a <see cref="CustomItem"/>.
        /// </summary>
        /// <returns>Returns a value indicating whether the <see cref="CustomItem"/> was registered or not.</returns>
        public bool TryRegister()
        {
            if (!Registered.Contains(this))
            {
                if (Registered.Any(customItem => customItem.Id == Id))
                {
                    Log.Warn($"{Name} has tried to register with the same custom item ID as another item: {Id}. It will not be registered.");

                    return false;
                }

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
        public bool TryUnregister()
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
        /// Spawns the <see cref="CustomItem"/> in a specific location.
        /// </summary>
        /// <param name="x">The x coordinate.</param>
        /// <param name="y">The y coordinate.</param>
        /// <param name="z">The z coordinate.</param>
        public virtual void Spawn(float x, float y, float z) => Spawn(new Vector3(x, y, z));

        /// <summary>
        /// Spawns a <see cref="Inventory.SyncItemInfo"/> as a <see cref="CustomItem"/> in a specific location.
        /// </summary>
        /// <param name="x">The x coordinate.</param>
        /// <param name="y">The y coordinate.</param>
        /// <param name="z">The z coordinate.</param>
        /// <param name="item">The <see cref="Inventory.SyncItemInfo"/> to be spawned as a <see cref="CustomItem"/>.</param>
        public virtual void Spawn(float x, float y, float z, Inventory.SyncItemInfo item) => Spawn(new Vector3(x, y, z), item);

        /// <summary>
        /// Spawns the <see cref="CustomItem"/> where a specific <see cref="Player"/> is.
        /// </summary>
        /// <param name="player">The <see cref="Player"/> position where the <see cref="CustomItem"/> will be spawned.</param>
        public virtual void Spawn(Player player) => Spawn(player.Position);

        /// <summary>
        /// Spawns a <see cref="Inventory.SyncItemInfo"/> as a <see cref="CustomItem"/> where a specific <see cref="Player"/> is.
        /// </summary>
        /// <param name="player">The <see cref="Player"/> position where the <see cref="CustomItem"/> will be spawned.</param>
        /// <param name="item">The <see cref="Inventory.SyncItemInfo"/> to be spawned as a <see cref="CustomItem"/>.</param>
        public virtual void Spawn(Player player, Inventory.SyncItemInfo item) => Spawn(player.Position, item);

        /// <summary>
        /// Spawns the <see cref="CustomItem"/> in a specific position.
        /// </summary>
        /// <param name="position">The <see cref="Vector3"/> where the <see cref="CustomItem"/> will be spawned.</param>
        public virtual void Spawn(Vector3 position) => Spawned.Add(Item.Spawn(Type, Durability, position));

        /// <summary>
        /// Spawns a <see cref="Inventory.SyncItemInfo"/> as a <see cref="CustomItem"/> in a specific position.
        /// </summary>
        /// <param name="position">The <see cref="Vector3"/> where the <see cref="CustomItem"/> will be spawned.</param>
        /// <param name="item">The <see cref="Inventory.SyncItemInfo"/> to be spawned as a <see cref="CustomItem"/>.</param>
        public virtual void Spawn(Vector3 position, Inventory.SyncItemInfo item) => Spawned.Add(Item.Spawn(item, position));

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

                Spawn(spawnPoint.Position.ToVector3);

                Log.Debug($"Spawned {Name} at {spawnPoint.Position} ({spawnPoint.Name})", Instance.Config.Debug);
            }

            return spawned;
        }

        /// <summary>
        /// Spawns all items at their dynamic and static positions.
        /// </summary>
        public virtual void SpawnAll()
        {
            if (SpawnProperties == null)
                return;

            Spawn(SpawnProperties.StaticSpawnPoints, Math.Min(0, SpawnProperties.Limit - Spawn(SpawnProperties.DynamicSpawnPoints, SpawnProperties.Limit)));
        }

        /// <summary>
        /// Gives an <see cref="Inventory.SyncItemInfo"/> as a <see cref="CustomItem"/> to a <see cref="Player"/>.
        /// </summary>
        /// <param name="player">The <see cref="Player"/> who will receive the item.</param>
        /// <param name="item">The <see cref="Inventory.SyncItemInfo"/> to be given.</param>
        /// <param name="displayMessage">Indicates whether or not <see cref="ShowPickedUpMessage"/> will be called when the player receives the item.</param>
        public virtual void Give(Player player, Inventory.SyncItemInfo item, bool displayMessage = true)
        {
            player.Inventory.items.Add(item);

            InsideInventories.Add(item.uniq);

            if (displayMessage)
                ShowPickedUpMessage(player);
        }

        /// <summary>
        /// Gives a <see cref="Pickup"/> as a <see cref="CustomItem"/> to a <see cref="Player"/>.
        /// </summary>
        /// <param name="player">The <see cref="Player"/> who will receive the item.</param>
        /// <param name="pickup">The <see cref="Pickup"/> to be given.</param>
        /// <param name="displayMessage">Indicates whether or not <see cref="ShowPickedUpMessage"/> will be called when the player receives the item.</param>
        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1117:Parameters should be on same line or separate lines", Justification = "Mind your own business.")]
        public virtual void Give(Player player, Pickup pickup, bool displayMessage = true) => Give(player, new Inventory.SyncItemInfo()
        {
            durability = pickup.durability,
            id = pickup.itemId,
            modBarrel = pickup.weaponMods.Barrel,
            modSight = pickup.weaponMods.Sight,
            modOther = pickup.weaponMods.Other,
            uniq = ++Inventory._uniqId,
        }, displayMessage);

        /// <summary>
        /// Gives the <see cref="CustomItem"/> to a player.
        /// </summary>
        /// <param name="player">The <see cref="Player"/> who will receive the item.</param>
        /// <param name="displayMessage">Indicates whether or not <see cref="ShowPickedUpMessage"/> will be called when the player receives the item.</param>
        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1117:Parameters should be on same line or separate lines", Justification = "Mind your own business.")]
        public virtual void Give(Player player, bool displayMessage = true) => Give(player, new Inventory.SyncItemInfo()
        {
            durability = Durability,
            id = Type,
            uniq = ++Inventory._uniqId,
        }, displayMessage);

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
        public virtual bool Check(Pickup pickup) => Spawned.Contains(pickup);

        /// <summary>
        /// Checks the specified inventory item to see if it is a custom item.
        /// </summary>
        /// <param name="item">The <see cref="Inventory.SyncItemInfo"/> to check.</param>
        /// <returns>True if it is a custom item.</returns>
        public virtual bool Check(Inventory.SyncItemInfo item) => InsideInventories.Contains(item.uniq);

        /// <inheritdoc/>
        public override string ToString() => $"[{Name} ({Type}) | {Id}] {Description}";

        /// <summary>
        /// Called after the manager is initialized, to allow loading of special event handlers.
        /// </summary>
        protected virtual void SubscribeEvents()
        {
            Events.Handlers.Player.ChangingRole += OnInternalOwnerChangingRole;
            Events.Handlers.Player.Dying += OnInternalOwnerDying;
            Events.Handlers.Player.Escaping += OnInternalOwnerEscaping;
            Events.Handlers.Player.Handcuffing += OnInternalOwnerHandcuffing;
            Events.Handlers.Player.DroppingItem += OnInternalDropping;
            Events.Handlers.Player.PickingUpItem += OnInternalPickingUp;
            Events.Handlers.Player.ChangingItem += OnInternalChanging;
            Events.Handlers.Scp914.UpgradingItems += OnInternalUpgrading;
            Events.Handlers.Server.WaitingForPlayers += OnWaitingForPlayers;
        }

        /// <summary>
        /// Called when the manager is being destroyed, to allow unloading of special event handlers.
        /// </summary>
        protected virtual void UnsubscribeEvents()
        {
            Events.Handlers.Player.ChangingRole -= OnInternalOwnerChangingRole;
            Events.Handlers.Player.Dying -= OnInternalOwnerDying;
            Events.Handlers.Player.Escaping -= OnInternalOwnerEscaping;
            Events.Handlers.Player.Handcuffing -= OnInternalOwnerHandcuffing;
            Events.Handlers.Player.DroppingItem -= OnInternalDropping;
            Events.Handlers.Player.PickingUpItem -= OnInternalPickingUp;
            Events.Handlers.Player.ChangingItem -= OnInternalChanging;
            Events.Handlers.Scp914.UpgradingItems -= OnInternalUpgrading;
            Events.Handlers.Server.WaitingForPlayers -= OnWaitingForPlayers;
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

        /// <summary>
        /// Clears the lists of item uniqIDs and Pickups since any still in the list will be invalid.
        /// </summary>
        protected virtual void OnWaitingForPlayers()
        {
            InsideInventories.Clear();
            Spawned.Clear();
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

        private void OnInternalOwnerChangingRole(ChangingRoleEventArgs ev)
        {
            foreach (Inventory.SyncItemInfo item in ev.Player.Inventory.items.ToList())
            {
                if (!Check(item))
                    continue;

                OnOwnerChangingRole(new OwnerChangingRoleEventArgs(item, ev));

                InsideInventories.Remove(item.uniq);

                ev.Player.RemoveItem(item);

                Spawn(ev.Player, item);
            }
        }

        private void OnInternalOwnerDying(DyingEventArgs ev)
        {
            foreach (Inventory.SyncItemInfo item in ev.Target.Inventory.items.ToList())
            {
                if (!Check(item))
                    continue;

                OnOwnerDying(new OwnerDyingEventArgs(item, ev));

                if (!ev.IsAllowed)
                    continue;

                ev.Target.RemoveItem(item);

                InsideInventories.Remove(item.uniq);

                Spawn(ev.Target, item);
            }
        }

        private void OnInternalOwnerEscaping(EscapingEventArgs ev)
        {
            foreach (Inventory.SyncItemInfo item in ev.Player.Inventory.items.ToList())
            {
                if (!Check(item))
                    continue;

                OnOwnerEscaping(new OwnerEscapingEventArgs(item, ev));

                if (!ev.IsAllowed)
                    continue;

                ev.Player.RemoveItem(item);

                InsideInventories.Remove(item.uniq);

                Spawn(ev.NewRole.GetRandomSpawnPoint(), item);
            }
        }

        private void OnInternalOwnerHandcuffing(HandcuffingEventArgs ev)
        {
            foreach (Inventory.SyncItemInfo item in ev.Target.Inventory.items.ToList())
            {
                if (!Check(item))
                    continue;

                OnOwnerHandcuffing(new OwnerHandcuffingEventArgs(item, ev));

                if (!ev.IsAllowed)
                    continue;

                ev.Target.RemoveItem(item);

                InsideInventories.Remove(item.uniq);

                Spawn(ev.Target, item);
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

            InsideInventories.Remove(ev.Item.uniq);

            ev.Player.RemoveItem(ev.Item);

            Spawn(ev.Player, ev.Item);
        }

        private void OnInternalPickingUp(PickingUpItemEventArgs ev)
        {
            if (!Check(ev.Pickup) || ev.Player.Inventory.items.Count >= 8)
                return;

            OnPickingUp(ev);

            if (!ev.IsAllowed)
                return;

            ev.IsAllowed = false;

            Give(ev.Player, ev.Pickup);

            Spawned.Remove(ev.Pickup);

            ev.Pickup.Delete();
        }

        private void OnInternalChanging(ChangingItemEventArgs ev)
        {
            if (!Check(ev.NewItem))
                return;

            OnChanging(ev);
        }

        private void OnInternalUpgrading(UpgradingItemsEventArgs ev)
        {
            foreach (Pickup pickup in ev.Items.ToList())
            {
                if (!Check(pickup))
                    continue;

                pickup.transform.position = ev.Scp914.output.position;

                ev.Items.Remove(pickup);
            }

            Dictionary<Player, List<Inventory.SyncItemInfo>> playerToItems = new Dictionary<Player, List<Inventory.SyncItemInfo>>();

            foreach (Player player in ev.Players)
            {
                playerToItems.Add(player, new List<Inventory.SyncItemInfo>());

                foreach (Inventory.SyncItemInfo item in player.Inventory.items.ToList())
                {
                    if (!Check(item))
                        continue;

                    OnUpgrading(new UpgradingEventArgs(item, ev));

                    playerToItems[player].Add(item);

                    player.Inventory.items.Remove(item);
                }
            }

            Timing.CallDelayed(3.5f, () =>
            {
                foreach (KeyValuePair<Player, List<Inventory.SyncItemInfo>> playerToItemsPair in playerToItems)
                {
                    foreach (Inventory.SyncItemInfo item in playerToItemsPair.Value)
                    {
                        if (playerToItemsPair.Key.Inventory.items.Count >= 8)
                        {
                            InsideInventories.Remove(item.uniq);

                            Spawn(playerToItemsPair.Key, item);

                            continue;
                        }

                        playerToItemsPair.Key.Inventory.items.Add(item);
                    }
                }
            });
        }
    }
}
