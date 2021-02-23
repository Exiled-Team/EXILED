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
        /// <param name="position">The <see cref="Vector3"/> where the item will be spawned.</param>
        public virtual void Spawn(Vector3 position) => Pickups.Add(Item.Spawn(Type, 1, position));

        /// <summary>
        /// Gives the item to a player.
        /// </summary>
        /// <param name="player">The <see cref="Player"/> who will recieve the item.</param>
        public virtual void Give(Player player) => Give(player, false);

        /// <summary>
        /// Gives the item to a player.
        /// </summary>
        /// <param name="player">The <see cref="Player"/> who will recieve the item.</param>
        /// <param name="displayMessage">Indicates whether or not <see cref="ShowMessage"/> will be called when the player received the item.</param>
        public virtual void Give(Player player, bool displayMessage)
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
                ShowMessage(player);

            ItemGiven(player);
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

                Inventory.SyncItemInfo item = new Inventory.SyncItemInfo()
                {
                    durability = ev.Pickup.durability,
                    id = ev.Pickup.itemId,
                    modBarrel = ev.Pickup.weaponMods.Barrel,
                    modOther = ev.Pickup.weaponMods.Other,
                    modSight = ev.Pickup.weaponMods.Sight,
                    uniq = ++Inventory._uniqId,
                };

                ev.Player.Inventory.items.Add(item);

                InsideInventories.Add(item.uniq);

                ev.Pickup.Delete();

                ShowMessage(ev.Player);
            }
        }

        /// <summary>
        /// Handles making sure custom items are not affected by SCP-914.
        /// </summary>
        /// <param name="ev"><see cref="UpgradingItemsEventArgs"/>.</param>
        protected virtual void OnUpgrading(UpgradingItemsEventArgs ev)
        {
            Vector3 outPosition = ev.Scp914.output.position - ev.Scp914.intake.position;

            foreach (Pickup pickup in ev.Items.ToList())
            {
                if (!Check(pickup))
                    continue;

                pickup.transform.position += outPosition;
                ev.Items.Remove(pickup);
            }

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
        /// Handles making sure custom items are not 'lost' when being handcuffed.
        /// </summary>
        /// <param name="ev"><see cref="HandcuffingEventArgs"/>.</param>
        protected virtual void OnHandcuffing(HandcuffingEventArgs ev)
        {
            foreach (Inventory.SyncItemInfo item in ev.Target.Inventory.items.ToList())
            {
                if (!Check(item))
                    continue;

                Pickups.Add(Item.Spawn(item.id, item.durability, ev.Target.Position, default, item.modSight, item.modBarrel, item.modOther));
                ev.Target.RemoveItem(item);
            }
        }

        /// <summary>
        /// Handles making sure custom items are not 'lost' when a player dies.
        /// </summary>
        /// <param name="ev"><see cref="DyingEventArgs"/>.</param>
        protected virtual void OnDying(DyingEventArgs ev)
        {
            foreach (Inventory.SyncItemInfo item in ev.Target.Inventory.items.ToList())
            {
                if (!Check(item))
                    continue;

                Pickups.Add(Item.Spawn(item.id, item.durability, ev.Target.Position, default, item.modSight, item.modBarrel, item.modOther));
                ev.Target.RemoveItem(item);
            }
        }

        /// <summary>
        /// Handles making sure custom items are not 'lost' when a player escapes.
        /// </summary>
        /// <param name="ev"><see cref="EscapingEventArgs"/>.</param>
        protected virtual void OnEscaping(EscapingEventArgs ev)
        {
            foreach (Inventory.SyncItemInfo item in ev.Player.Inventory.items.ToList())
            {
                if (!Check(item))
                    continue;

                Pickups.Add(Item.Spawn(item.id, item.durability, ev.NewRole.GetRandomSpawnPoint(), default, item.modSight, item.modBarrel, item.modOther));

                ev.Player.RemoveItem(item);
            }
        }

        /// <summary>
        /// Shows a message to the player when they pickup a custom item.
        /// </summary>
        /// <param name="player">The <see cref="Player"/> who will be shown the message.</param>
        protected virtual void ShowMessage(Player player)
        {
            player.ShowHint(string.Format(Instance.Config.PickedUpBroadcast.Content, Name, Description), Instance.Config.PickedUpBroadcast.Duration);
        }

        /// <summary>
        /// Called when a player is given the item directly via a command or plugin.
        /// </summary>
        /// <param name="player">The <see cref="Player"/> who received the item.</param>
        protected virtual void ItemGiven(Player player)
        {
            // Needs to be changed into an event
        }
    }
}
