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
    using System.Linq;

    using Exiled.API.Extensions;
    using Exiled.API.Features;
    using Exiled.CustomItems.API.Spawn;
    using Exiled.Events.EventArgs;

    using MEC;

    using UnityEngine;

    using static CustomItems;

    /// <summary>
    /// The Custom Item base class.
    /// </summary>
    public abstract class CustomItem
    {
        private ItemType type;

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomItem"/> class.
        /// </summary>
        /// <param name="type">The <see cref="ItemType"/> to be used.</param>
        /// <param name="id">The <see cref="uint"/> custom item ID to be used.</param>
        protected CustomItem(ItemType type, uint id)
        {
            Type = type;
            Id = id;
        }

        /// <summary>
        /// Gets the name of the item.
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        /// Gets the description of the item.
        /// </summary>
        public abstract string Description { get; }

        /// <summary>
        /// Gets or sets the custom ItemID of the item.
        /// </summary>
        public virtual uint Id { get; protected set; }

        /// <summary>
        /// Gets or sets the ItemType to use for this item.
        /// </summary>
        public virtual ItemType Type
        {
            get => type;
            protected set
            {
                if (!Enum.IsDefined(typeof(ItemType), value))
                    throw new ArgumentOutOfRangeException("Type", value, "Invalid Item type.");

                type = value;
            }
        }

        /// <summary>
        /// Gets or sets the item durability.
        /// </summary>
        public virtual float Durability { get; protected set; }

        /// <summary>
        /// Gets or sets the list of spawn locations and chances for each one.
        /// </summary>
        public virtual SpawnProperties SpawnProperties { get; set; }

        /// <summary>
        /// Gets the list of custom items inside players' inventory being tracked as the current item.
        /// </summary>
        protected List<int> InsideInventories { get; } = new List<int>();

        /// <summary>
        /// Gets the list of spawned custom items being tracked as the current item.
        /// </summary>
        protected List<Pickup> Pickups { get; } = new List<Pickup>();

        /// <summary>
        /// Spawns the item in a specific location.
        /// </summary>
        /// <param name="x">The x coordinate.</param>
        /// <param name="y">The y coordinate.</param>
        /// <param name="z">The z coordinate.</param>
        public virtual void Spawn(float x, float y, float z) => Spawn(new Vector3(x, y, z));

        /// <summary>
        /// Spawns the item in a specific location.
        /// </summary>
        /// <param name="position">The <see cref="Vector3"/> where the item will be spawned.</param>
        public virtual void Spawn(Vector3 position) => Pickups.Add(Item.Spawn(Type, Durability, position));

        /// <summary>
        /// Spawns items inside <paramref name="spawnPoints"/>.
        /// </summary>
        /// <param name="spawnPoints">The spawn points <see cref="IEnumerable{T}"/>.</param>
        public virtual void Spawn(IEnumerable<SpawnPoint> spawnPoints)
        {
            int spawned = 0;

            foreach (SpawnPoint spawnPoint in spawnPoints)
            {
                Log.Debug($"Attempting to spawn {Name} at {spawnPoint.Position}.", Instance.Config.Debug);

                if (UnityEngine.Random.Range(1, 101) >= spawnPoint.Chance || (SpawnProperties.Limit > 0 && spawned >= SpawnProperties.Limit))
                    continue;

                spawned++;

                Spawn(spawnPoint.Position.ToVector3);

                Log.Debug($"Spawned {Name} at {spawnPoint.Position} ({spawnPoint.Name})", Instance.Config.Debug);
            }
        }

        /// <summary>
        /// Spawns all items at their dynamic and static positions.
        /// </summary>
        public virtual void SpawnAll()
        {
            if (SpawnProperties == null)
                return;

            Spawn(SpawnProperties.DynamicSpawnPoints);
            Spawn(SpawnProperties.StaticSpawnPoints);
        }

        /// <summary>
        /// Gives the item to a player.
        /// </summary>
        /// <param name="player">The <see cref="Player"/> who will recieve the item.</param>
        /// <param name="displayMessage">Indicates whether or not <see cref="ShowPickedUpMessage"/> will be called when the player received the item.</param>
        public virtual void Give(Player player, bool displayMessage = true)
        {
            Inventory.SyncItemInfo syncItemInfo = new Inventory.SyncItemInfo()
            {
                durability = Durability,
                id = Type,
                uniq = ++Inventory._uniqId,
            };

            player.Inventory.items.Add(syncItemInfo);

            InsideInventories.Add(syncItemInfo.uniq);

            if (displayMessage)
                ShowPickedUpMessage(player);
        }

        /// <summary>
        /// Unregisters a <see cref="CustomItem"/> manager.
        /// </summary>
        public void Unregister()
        {
            if (!Instance.ItemManagers.Contains(this))
                return;

            Destroy();

            Instance.ItemManagers.Remove(this);
        }

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
        public virtual bool Check(Pickup pickup) => Pickups.Contains(pickup);

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
            Events.Handlers.Player.Dying += OnDying;
            Events.Handlers.Player.Escaping += OnEscaping;
            Events.Handlers.Player.Handcuffing += OnHandcuffing;
            Events.Handlers.Player.DroppingItem += OnDropping;
            Events.Handlers.Player.PickingUpItem += OnPickingUp;
            Events.Handlers.Player.ChangingItem += OnChanging;
            Events.Handlers.Scp914.UpgradingItems += OnUpgrading;
            Events.Handlers.Server.WaitingForPlayers += OnWaitingForPlayers;
        }

        /// <summary>
        /// Called when the manager is being destroyed, to allow unloading of special event handlers.
        /// </summary>
        protected virtual void UnsubscribeEvents()
        {
            Events.Handlers.Player.Dying -= OnDying;
            Events.Handlers.Player.Escaping -= OnEscaping;
            Events.Handlers.Player.Handcuffing -= OnHandcuffing;
            Events.Handlers.Player.DroppingItem -= OnDropping;
            Events.Handlers.Player.PickingUpItem -= OnPickingUp;
            Events.Handlers.Player.ChangingItem -= OnChanging;
            Events.Handlers.Scp914.UpgradingItems -= OnUpgrading;
            Events.Handlers.Server.WaitingForPlayers -= OnWaitingForPlayers;
        }

        /// <summary>
        /// Clears the lists of item uniqIDs and Pickups since any still in the list will be invalid.
        /// </summary>
        protected virtual void OnWaitingForPlayers()
        {
            InsideInventories.Clear();
            Pickups.Clear();
        }

        /// <summary>
        /// Handles tracking items when they are dropped by a player.
        /// </summary>
        /// <param name="ev"><see cref="DroppingItemEventArgs"/>.</param>
        protected virtual void OnDropping(DroppingItemEventArgs ev)
        {
            if (Check(ev.Item))
            {
                ev.IsAllowed = false;

                Pickups.Add(Item.Spawn(ev.Item.id, ev.Item.durability, ev.Player.Position, default, ev.Item.modSight, ev.Item.modBarrel, ev.Item.modOther));

                ev.Player.RemoveItem(ev.Item);
            }
        }

        /// <summary>
        /// Handles tracking items when they are picked up by a player.
        /// </summary>
        /// <param name="ev"><see cref="PickingUpItemEventArgs"/>.</param>
        protected virtual void OnPickingUp(PickingUpItemEventArgs ev)
        {
            if (Check(ev.Pickup) && ev.Player.Inventory.items.Count < 8)
            {
                ev.IsAllowed = false;

                Give(ev.Player);

                ev.Pickup.Delete();
            }
        }

        /// <summary>
        /// Handles making sure custom items are not affected by SCP-914.
        /// </summary>
        /// <param name="ev"><see cref="UpgradingItemsEventArgs"/>.</param>
        protected virtual void OnUpgrading(UpgradingItemsEventArgs ev)
        {
            foreach (Pickup pickup in ev.Items.ToList())
            {
                if (!Check(pickup))
                    continue;

                pickup.transform.position = ev.Scp914.output.position;

                ev.Items.Remove(pickup);
            }

            // It could be handled differently
            Dictionary<Player, Inventory.SyncItemInfo> itemsToSave = new Dictionary<Player, Inventory.SyncItemInfo>();

            foreach (Player player in ev.Players)
            {
                foreach (Inventory.SyncItemInfo item in player.Inventory.items.ToList())
                {
                    if (!Check(item))
                        continue;

                    itemsToSave.Add(player, item);

                    player.Inventory.items.Remove(item);
                }
            }

            Timing.CallDelayed(3.5f, () =>
            {
                foreach (KeyValuePair<Player, Inventory.SyncItemInfo> kvp in itemsToSave)
                    kvp.Key.Inventory.items.Add(kvp.Value);
            });
        }

        /// <summary>
        /// Handles making sure custom items are not "lost" when being handcuffed.
        /// </summary>
        /// <param name="ev"><see cref="HandcuffingEventArgs"/>.</param>
        protected virtual void OnHandcuffing(HandcuffingEventArgs ev)
        {
            foreach (Inventory.SyncItemInfo item in ev.Target.Inventory.items.ToList())
            {
                if (!Check(item))
                    continue;

                Spawn(ev.Target.Position);

                ev.Target.RemoveItem(item);
            }
        }

        /// <summary>
        /// Handles making sure custom items are not "lost" when a player dies.
        /// </summary>
        /// <param name="ev"><see cref="DyingEventArgs"/>.</param>
        protected virtual void OnDying(DyingEventArgs ev)
        {
            foreach (Inventory.SyncItemInfo item in ev.Target.Inventory.items.ToList())
            {
                if (!Check(item))
                    continue;

                Spawn(ev.Target.Position);

                ev.Target.RemoveItem(item);
            }
        }

        /// <summary>
        /// Handles making sure custom items are not "lost" when a player escapes.
        /// </summary>
        /// <param name="ev"><see cref="EscapingEventArgs"/>.</param>
        protected virtual void OnEscaping(EscapingEventArgs ev)
        {
            foreach (Inventory.SyncItemInfo item in ev.Player.Inventory.items.ToList())
            {
                if (!Check(item))
                    continue;

                Spawn(ev.NewRole.GetRandomSpawnPoint());

                ev.Player.RemoveItem(item);
            }
        }

        /// <summary>
        /// Handles tracking items when they are selected in the player's inventory.
        /// </summary>
        /// <param name="ev"><see cref="ChangingItemEventArgs"/>.</param>
        protected virtual void OnChanging(ChangingItemEventArgs ev)
        {
            if (Check(ev.NewItem))
                ShowSelectedMessage(ev.Player);
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
    }
}
