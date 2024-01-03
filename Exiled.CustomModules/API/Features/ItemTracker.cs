// -----------------------------------------------------------------------
// <copyright file="ItemTracker.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomModules.API.Features
{
    using System.Collections.Generic;

    using Exiled.API.Extensions;
    using Exiled.API.Features.Attributes;
    using Exiled.API.Features.Core;
    using Exiled.API.Features.DynamicEvents;
    using Exiled.API.Features.Items;
    using Exiled.API.Features.Pickups;
    using Exiled.CustomModules.API.Features.CustomAbilities;
    using Exiled.CustomModules.Events.EventArgs.Tracking;
    using Exiled.Events.EventArgs.Map;
    using Exiled.Events.EventArgs.Player;

    /// <summary>
    /// The actor which handles all tracking-related tasks for items.
    /// </summary>
    public class ItemTracker : StaticActor
    {
        /// <summary>
        /// Gets the <see cref="TDynamicEventDispatcher{T}"/> which handles all the delegates fired when an item is added.
        /// </summary>
        [DynamicEventDispatcher]
        public TDynamicEventDispatcher<Item> ItemAddedDispatcher { get; private set; }

        /// <summary>
        /// Gets the <see cref="TDynamicEventDispatcher{T}"/> which handles all the delegates fired when an item is removed.
        /// </summary>
        [DynamicEventDispatcher]
        public TDynamicEventDispatcher<ushort> ItemRemovedDispatcher { get; private set; }

        /// <summary>
        /// Gets the <see cref="TDynamicEventDispatcher{T}"/> which handles all the delegates fired when an item is restored.
        /// </summary>
        [DynamicEventDispatcher]
        public TDynamicEventDispatcher<Item> ItemRestoredDispatcher { get; private set; }

        /// <summary>
        /// Gets the <see cref="TDynamicEventDispatcher{T}"/> which handles all the delegates fired when an item tracking is modified.
        /// </summary>
        [DynamicEventDispatcher]
        public TDynamicEventDispatcher<ItemTrackingModifiedEventArgs> ItemTrackingModifiedDispatcher { get; private set; }

        /// <summary>
        /// Gets the <see cref="TDynamicEventDispatcher{T}"/> which handles all the delegates fired when a pickup is added.
        /// </summary>
        [DynamicEventDispatcher]
        public TDynamicEventDispatcher<Pickup> PickupAddedDispatcher { get; private set; }

        /// <summary>
        /// Gets the <see cref="TDynamicEventDispatcher{T}"/> which handles all the delegates fired when a pickup is removed.
        /// </summary>
        [DynamicEventDispatcher]
        public TDynamicEventDispatcher<ushort> PickupRemovedDispatcher { get; private set; }

        /// <summary>
        /// Gets the <see cref="TDynamicEventDispatcher{T}"/> which handles all the delegates fired when a pickup is restored.
        /// </summary>
        [DynamicEventDispatcher]
        public TDynamicEventDispatcher<Pickup> PickupRestoredDispatcher { get; private set; }

        /// <summary>
        /// Gets the <see cref="TDynamicEventDispatcher{T}"/> which handles all the delegates fired when a pickup tracking is modified.
        /// </summary>
        [DynamicEventDispatcher]
        public TDynamicEventDispatcher<PickupTrackingModifiedEventArgs> PickupTrackingModifiedDispatcher { get; private set; }

        /// <summary>
        /// Gets a <see cref="Dictionary{TKey, TValue}"/> containing all serials and their corresponding abilities.
        /// </summary>
        internal Dictionary<ushort, HashSet<IAbilityBehaviour>> TrackedItemSerials { get; } = new();

        /// <summary>
        /// Gets a <see cref="Dictionary{TKey, TValue}"/> containing all serials and their corresponding abilities.
        /// </summary>
        internal Dictionary<ushort, HashSet<IAbilityBehaviour>> TrackedPickupSerials { get; } = new();

        /// <summary>
        /// Adds or tracks the abilities of an item based on its serial.
        /// </summary>
        /// <param name="item">The <see cref="Item"/> whose abilities are to be added or tracked.</param>
        /// <returns><see langword="true"/> if the item was added or tracked successfully; otherwise, <see langword="false"/>.</returns>
        public bool AddOrTrack(Item item)
        {
            if (!item)
                return false;

            IEnumerable<IAbilityBehaviour> abilityBehaviours = item.GetComponents<IAbilityBehaviour>();
            if (abilityBehaviours.IsEmpty())
                return false;

            if (TrackedItemSerials.ContainsKey(item.Serial))
            {
                IEnumerable<IAbilityBehaviour> previousAbilities = TrackedItemSerials[item.Serial];
                TrackedItemSerials[item.Serial].AddRange(abilityBehaviours);
                ItemTrackingModifiedEventArgs ev = new(item, previousAbilities, TrackedItemSerials[item.Serial]);
                ItemTrackingModifiedDispatcher.InvokeAll(ev);

                return true;
            }

            TrackedItemSerials.Add(item.Serial, new HashSet<IAbilityBehaviour>());
            TrackedItemSerials[item.Serial].AddRange(abilityBehaviours);

            ItemAddedDispatcher.InvokeAll(item);

            return true;
        }

        /// <summary>
        /// Adds or tracks the abilities of a pickup based on its serial.
        /// </summary>
        /// <param name="pickup">The <see cref="Pickup"/> whose abilities are to be added or tracked.</param>
        /// <returns><see langword="true"/> if the pickup was added or tracked successfully; otherwise, <see langword="false"/>.</returns>
        public bool AddOrTrack(Pickup pickup)
        {
            if (!pickup)
                return false;

            IEnumerable<IAbilityBehaviour> abilityBehaviours = pickup.GetComponents<IAbilityBehaviour>();
            if (abilityBehaviours.IsEmpty())
                return false;

            if (TrackedItemSerials.ContainsKey(pickup.Serial))
            {
                IEnumerable<IAbilityBehaviour> previousAbilities = TrackedPickupSerials[pickup.Serial];
                TrackedPickupSerials[pickup.Serial].AddRange(abilityBehaviours);
                PickupTrackingModifiedEventArgs ev = new(pickup, previousAbilities, TrackedPickupSerials[pickup.Serial]);
                PickupTrackingModifiedDispatcher.InvokeAll(ev);

                return true;
            }

            TrackedPickupSerials.Add(pickup.Serial, new HashSet<IAbilityBehaviour>());
            TrackedPickupSerials[pickup.Serial].AddRange(abilityBehaviours);

            PickupAddedDispatcher.InvokeAll(pickup);

            return true;
        }

        /// <summary>
        /// Restores all abilities from a pickup which is being tracked.
        /// </summary>
        /// <param name="pickup">The pickup to restore all abilities from.</param>
        /// <param name="item">The item to reapply the abilities to.</param>
        /// <returns><see langword="true"/> if the pickup was restored successfully; otherwise, <see langword="false"/>.</returns>
        public bool Restore(Pickup pickup, Item item)
        {
            if (!pickup || !item || !TrackedItemSerials.ContainsKey(pickup.Serial))
                return false;

            foreach (IAbilityBehaviour behaviour in TrackedItemSerials[pickup.Serial])
            {
                if (behaviour is not EActor component)
                    continue;

                item.AddComponent(component);
            }

            ItemRestoredDispatcher.InvokeAll(item);

            return true;
        }

        /// <summary>
        /// Restores all abilities from a pickup which is being tracked.
        /// </summary>
        /// <param name="pickup">The <see cref="Pickup"/> whose abilities are to be restored.</param>
        /// <returns><see langword="true"/> if the pickup was restored successfully; otherwise, <see langword="false"/>.</returns>
        public bool Restore(Pickup pickup)
        {
            if (!pickup || !TrackedPickupSerials.ContainsKey(pickup.Serial))
                return false;

            foreach (IAbilityBehaviour behaviour in TrackedPickupSerials[pickup.Serial])
            {
                if (behaviour is not EActor component)
                    continue;

                pickup.AddComponent(component);
            }

            PickupRestoredDispatcher.InvokeAll(pickup);

            return true;
        }

        /// <summary>
        /// Removes an item and all its abilities from the tracking.
        /// </summary>
        /// <param name="item">The item to be removed.</param>
        public void Remove(Item item)
        {
            if (TrackedItemSerials.ContainsKey(item.Serial))
            {
                TrackedItemSerials.Remove(item.Serial);
                ItemRemovedDispatcher.InvokeAll(item.Serial);
            }
        }

        /// <summary>
        /// Removes an ability from the tracking.
        /// </summary>
        /// <param name="item">The item owning the ability.</param>
        /// <param name="behaviour">The <see cref="IAbilityBehaviour"/> to be removed.</param>
        public void Remove(Item item, IAbilityBehaviour behaviour)
        {
            if (TrackedItemSerials.ContainsKey(item.Serial))
            {
                IEnumerable<IAbilityBehaviour> previousAbilities = TrackedItemSerials[item.Serial];
                TrackedItemSerials[item.Serial].Remove(behaviour);
                ItemTrackingModifiedEventArgs ev = new(item, previousAbilities, TrackedItemSerials[item.Serial]);
                ItemTrackingModifiedDispatcher.InvokeAll(ev);
            }
        }

        /// <summary>
        /// Removes a pickup and all its abilities from the tracking.
        /// </summary>
        /// <param name="pickup">The pickup to be removed.</param>
        public void Remove(Pickup pickup)
        {
            if (TrackedPickupSerials.ContainsKey(pickup.Serial))
            {
                TrackedPickupSerials.Remove(pickup.Serial);
                PickupRemovedDispatcher.InvokeAll(pickup.Serial);
            }
        }

        /// <summary>
        /// Removes an ability from the tracking.
        /// </summary>
        /// <param name="pickup">The pickup owning the ability.</param>
        /// <param name="behaviour">The <see cref="IAbilityBehaviour"/> to be removed.</param>
        public void Remove(Pickup pickup, IAbilityBehaviour behaviour)
        {
            if (TrackedPickupSerials.ContainsKey(pickup.Serial))
            {
                IEnumerable<IAbilityBehaviour> previousAbilities = TrackedPickupSerials[pickup.Serial];
                TrackedPickupSerials[pickup.Serial].Remove(behaviour);
                PickupTrackingModifiedEventArgs ev = new(pickup, previousAbilities, TrackedPickupSerials[pickup.Serial]);
                PickupTrackingModifiedDispatcher.InvokeAll(ev);
            }
        }

        /// <summary>
        /// Removes an item or pickup with the specified serial number from the tracking.
        /// </summary>
        /// <param name="serial">The serial number of the item or pickup to be removed.</param>
        /// <param name="behaviours">The <see cref="IEnumerable{T}"/> of <see cref="IAbilityBehaviour"/> containing all abilities to be removed.</param>
        public void Remove(ushort serial, IEnumerable<IAbilityBehaviour> behaviours)
        {
            if (TrackedItemSerials.ContainsKey(serial))
            {
                IEnumerable<IAbilityBehaviour> previousAbilities = TrackedItemSerials[serial];

                foreach (IAbilityBehaviour behaviour in behaviours)
                    TrackedItemSerials[serial].Remove(behaviour);

                ItemTrackingModifiedEventArgs ev = new(Item.Get(serial), previousAbilities, TrackedItemSerials[serial]);
                ItemTrackingModifiedDispatcher.InvokeAll(ev);
            }

            if (TrackedPickupSerials.ContainsKey(serial))
            {
                IEnumerable<IAbilityBehaviour> previousAbilities = TrackedPickupSerials[serial];

                foreach (IAbilityBehaviour behaviour in behaviours)
                    TrackedPickupSerials[serial].Remove(behaviour);

                PickupTrackingModifiedEventArgs ev = new(Pickup.Get(serial), previousAbilities, TrackedPickupSerials[serial]);
                PickupTrackingModifiedDispatcher.InvokeAll(ev);
            }
        }

        /// <summary>
        /// Checks if an item is being tracked.
        /// </summary>
        /// <param name="item">The <see cref="Item"/> to check.</param>
        /// <returns>Returns <see langword="true"/> if the item is being tracked; otherwise, <see langword="false"/>.</returns>
        public bool IsTracked(Item item) => TrackedItemSerials.ContainsKey(item.Serial);

        /// <summary>
        /// Checks if a pickup is being tracked.
        /// </summary>
        /// <param name="pickup">The <see cref="Pickup"/> to check.</param>
        /// <returns>Returns <see langword="true"/> if the pickup is being tracked; otherwise, <see langword="false"/>.</returns>
        public bool IsTracked(Pickup pickup) => TrackedPickupSerials.ContainsKey(pickup.Serial);

        /// <summary>
        /// Handles the event when a player is dropping an item.
        /// </summary>
        /// <param name="ev">The <see cref="DroppingItemEventArgs"/> containing information about the dropping item.</param>
        internal void OnDroppingItem(DroppingItemEventArgs ev) => AddOrTrack(ev.Item);

        /// <summary>
        /// Handles the event when an item is dropped.
        /// </summary>
        /// <param name="ev">The <see cref="DroppedItemEventArgs"/> containing information about the dropped item.</param>
        internal void OnDroppedItem(DroppedItemEventArgs ev) => Restore(ev.Pickup);

        /// <summary>
        /// Handles the event when an item is added.
        /// </summary>
        /// <param name="ev">The <see cref="ItemAddedEventArgs"/> containing information about the added item.</param>
        internal void OnItemAdded(ItemAddedEventArgs ev) => Restore(ev.Pickup, ev.Item);

        /// <summary>
        /// Handles the event when a tracked item or pickup is removed from a player's inventory.
        /// </summary>
        /// <param name="ev">The <see cref="PickupDestroyedEventArgs"/> containing information about the removed item.</param>
        internal void OnItemRemoved(ItemRemovedEventArgs ev)
        {
            if (ev.Pickup)
                return;

            Remove(ev.Item);
        }

        /// <summary>
        /// Handles the event when a tracked item or pickup is destroyed.
        /// </summary>
        /// <param name="ev">The <see cref="PickupDestroyedEventArgs"/> containing information about the destroyed pickup.</param>
        internal void OnPickupDestroyed(PickupDestroyedEventArgs ev) => Remove(ev.Pickup);

        /// <inheritdoc/>
        protected override void SubscribeEvents()
        {
            base.SubscribeEvents();

            Exiled.Events.Handlers.Player.DroppingItem += OnDroppingItem;
            Exiled.Events.Handlers.Player.DroppedItem += OnDroppedItem;
            Exiled.Events.Handlers.Player.ItemAdded += OnItemAdded;
            Exiled.Events.Handlers.Player.ItemRemoved += OnItemRemoved;
            Exiled.Events.Handlers.Map.PickupDestroyed += OnPickupDestroyed;
        }

        /// <inheritdoc/>
        protected override void UnsubscribeEvents()
        {
            base.UnsubscribeEvents();

            Exiled.Events.Handlers.Player.DroppingItem -= OnDroppingItem;
            Exiled.Events.Handlers.Player.DroppedItem -= OnDroppedItem;
            Exiled.Events.Handlers.Player.ItemAdded -= OnItemAdded;
            Exiled.Events.Handlers.Player.ItemRemoved -= OnItemRemoved;
            Exiled.Events.Handlers.Map.PickupDestroyed -= OnPickupDestroyed;
        }
    }
}