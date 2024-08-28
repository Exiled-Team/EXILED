// -----------------------------------------------------------------------
// <copyright file="TrackerBase.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomModules.API.Features.Generic
{
    using System.Collections.Generic;
    using System.Linq;

    using Exiled.API.Extensions;
    using Exiled.API.Features;
    using Exiled.API.Features.Attributes;
    using Exiled.API.Features.Core;
    using Exiled.API.Features.DynamicEvents;
    using Exiled.API.Features.Items;
    using Exiled.API.Features.Pickups;
    using Exiled.CustomModules.API.Interfaces;
    using Exiled.CustomModules.Events.EventArgs.Tracking;
    using Exiled.Events.EventArgs.Map;
    using Exiled.Events.EventArgs.Player;
    using UnityEngine;

    /// <summary>
    /// The actor which handles all tracking-related tasks for items.
    /// </summary>
    /// <typeparam name="T">The type of the <see cref="ITrackable"/>.</typeparam>
    public class TrackerBase<T> : StaticActor
        where T : ITrackable
    {
        /// <summary>
        /// Gets or sets the <see cref="TDynamicEventDispatcher{T}"/> which handles all the delegates fired when an item is added.
        /// </summary>
        [DynamicEventDispatcher]
        public TDynamicEventDispatcher<Item> ItemAddedDispatcher { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="TDynamicEventDispatcher{T}"/> which handles all the delegates fired when an item is removed.
        /// </summary>
        [DynamicEventDispatcher]
        public TDynamicEventDispatcher<ushort> ItemRemovedDispatcher { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="TDynamicEventDispatcher{T}"/> which handles all the delegates fired when an item is restored.
        /// </summary>
        [DynamicEventDispatcher]
        public TDynamicEventDispatcher<Item> ItemRestoredDispatcher { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="TDynamicEventDispatcher{T}"/> which handles all the delegates fired when an item tracking is modified.
        /// </summary>
        [DynamicEventDispatcher]
        public TDynamicEventDispatcher<ItemTrackingModifiedEventArgs> ItemTrackingModifiedDispatcher { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="TDynamicEventDispatcher{T}"/> which handles all the delegates fired when a pickup is added.
        /// </summary>
        [DynamicEventDispatcher]
        public TDynamicEventDispatcher<Pickup> PickupAddedDispatcher { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="TDynamicEventDispatcher{T}"/> which handles all the delegates fired when a pickup is removed.
        /// </summary>
        [DynamicEventDispatcher]
        public TDynamicEventDispatcher<ushort> PickupRemovedDispatcher { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="TDynamicEventDispatcher{T}"/> which handles all the delegates fired when a pickup is restored.
        /// </summary>
        [DynamicEventDispatcher]
        public TDynamicEventDispatcher<Pickup> PickupRestoredDispatcher { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="TDynamicEventDispatcher{T}"/> which handles all the delegates fired when a pickup tracking is modified.
        /// </summary>
        [DynamicEventDispatcher]
        public TDynamicEventDispatcher<PickupTrackingModifiedEventArgs> PickupTrackingModifiedDispatcher { get; set; }

        /// <summary>
        /// Gets a <see cref="Dictionary{TKey, TValue}"/> containing all serials and their corresponding <typeparamref name="T"/> items.
        /// </summary>
        private Dictionary<ushort, HashSet<T>> TrackedItemSerials { get; } = new();

        /// <summary>
        /// Gets a <see cref="Dictionary{TKey, TValue}"/> containing all serials and their corresponding <typeparamref name="T"/> items.
        /// </summary>
        private Dictionary<ushort, HashSet<T>> TrackedPickupSerials { get; } = new();

        /// <summary>
        /// Adds or tracks the trackables of an item based on its serial.
        /// </summary>
        /// <param name="item">The <see cref="Item"/> whose trackables are to be added or tracked.</param>
        /// <returns><see langword="true"/> if the item was added or tracked successfully; otherwise, <see langword="false"/>.</returns>
        public virtual bool AddOrTrack(Item item)
        {
            if (!item)
                return false;

            IEnumerable<T> trackableBehaviours = item.GetComponents<T>();
            if (trackableBehaviours.IsEmpty())
                return false;

            if (TrackedItemSerials.ContainsKey(item.Serial))
            {
                IEnumerable<T> previousTrackableItems = TrackedItemSerials[item.Serial];
                TrackedItemSerials[item.Serial].AddRange(trackableBehaviours.Cast<T>());
                ItemTrackingModifiedEventArgs ev = new(item, previousTrackableItems.Cast<ITrackable>(), TrackedItemSerials[item.Serial].Cast<ITrackable>());
                ItemTrackingModifiedDispatcher.InvokeAll(ev);
                Log.WarnWithContext($"Item with serial {item.Serial} of type {item.Type} was added or tracked.");
                return true;
            }

            TrackedItemSerials.Add(item.Serial, new HashSet<T>());
            TrackedItemSerials[item.Serial].AddRange(trackableBehaviours.Cast<T>());

            ItemAddedDispatcher.InvokeAll(item);
            Log.WarnWithContext($"Item with serial {item.Serial} of type {item.Type} was added.");

            return true;
        }

        /// <summary>
        /// Adds or tracks the trackables of a pickup based on its serial.
        /// </summary>
        /// <param name="pickup">The <see cref="Pickup"/> whose trackables are to be added or tracked.</param>
        /// <returns><see langword="true"/> if the pickup was added or tracked successfully; otherwise, <see langword="false"/>.</returns>
        public virtual bool AddOrTrack(Pickup pickup)
        {
            if (!pickup)
                return false;

            IEnumerable<ITrackable> trackableBehaviours = pickup.GetComponents<ITrackable>();
            if (trackableBehaviours.IsEmpty())
                return false;

            if (TrackedPickupSerials.ContainsKey(pickup.Serial))
            {
                IEnumerable<T> previousTrackableItems = TrackedPickupSerials[pickup.Serial];
                TrackedPickupSerials[pickup.Serial].AddRange(trackableBehaviours.Cast<T>());
                PickupTrackingModifiedEventArgs ev = new(pickup, previousTrackableItems.Cast<ITrackable>(), TrackedPickupSerials[pickup.Serial].Cast<ITrackable>());
                PickupTrackingModifiedDispatcher.InvokeAll(ev);
                Log.WarnWithContext($"Pickup with serial {pickup.Serial} of type {pickup.Type} was added or tracked.");

                return true;
            }

            TrackedPickupSerials.Add(pickup.Serial, new HashSet<T>());
            TrackedPickupSerials[pickup.Serial].AddRange(trackableBehaviours.Cast<T>());

            PickupAddedDispatcher.InvokeAll(pickup);
            Log.WarnWithContext($"Pickup with serial {pickup.Serial} of type {pickup.Type} was added.");

            return true;
        }

        /// <summary>
        /// Adds or tracks the trackables of both pickups and items based on their serial.
        /// </summary>
        /// <param name="objects">The objects whose trackables are to be added or tracked.</param>
        public virtual void AddOrTrack(params object[] objects)
        {
            foreach (object @object in objects)
            {
                if (@object is Pickup pickup)
                {
                    AddOrTrack(pickup);
                    continue;
                }

                if (@object is Item item)
                    AddOrTrack(item);
            }
        }

        /// <summary>
        /// Restores all trackables from a pickup which is being tracked.
        /// </summary>
        /// <param name="serial">The serial to restore all trackables from.</param>
        /// <param name="item">The item to reapply the trackables to.</param>
        /// <returns><see langword="true"/> if the pickup was restored successfully; otherwise, <see langword="false"/>.</returns>
        public virtual bool Restore(ushort serial, Item item)
        {
            if (!item || !TrackedItemSerials.TryGetValue(serial, out HashSet<T> itemSerial))
                return false;

            foreach (T behaviour in itemSerial)
            {
                if (behaviour is not EActor component)
                    continue;

                item.AddComponent(component);
            }

            ItemRestoredDispatcher.InvokeAll(item);
            Log.WarnWithContext($"Item with serial {serial} was restored to item with serial {item.Serial} of type {item.Type}.");

            return true;
        }

        /// <summary>
        /// Restores all trackables from a pickup which is being tracked.
        /// </summary>
        /// <param name="pickup">The <see cref="Pickup"/> whose trackables are to be restored.</param>
        /// <returns><see langword="true"/> if the pickup was restored successfully; otherwise, <see langword="false"/>.</returns>
        public virtual bool Restore(Pickup pickup)
        {
            if (!pickup || !TrackedPickupSerials.TryGetValue(pickup.Serial, out HashSet<T> serial))
                return false;

            foreach (T behaviour in serial)
            {
                if (behaviour is not EActor component)
                    continue;

                pickup.AddComponent(component);
            }

            PickupRestoredDispatcher.InvokeAll(pickup);
            Log.WarnWithContext($"Pickup with serial {serial} was restored.");

            return true;
        }

        /// <summary>
        /// Restores all trackables from a pickup which is being tracked.
        /// </summary>
        /// <param name="pickup">The <see cref="Pickup"/> whose trackables are to be restored.</param>
        /// <param name="item">The <see cref="Item"/> whose trackables are to be transfered.</param>
        /// <returns><see langword="true"/> if the pickup was restored successfully; otherwise, <see langword="false"/>.</returns>
        public virtual bool Restore(Pickup pickup, Item item)
        {
            if (!pickup || !item || !TrackedPickupSerials.ContainsKey(pickup.Serial) ||
                !TrackedItemSerials.TryGetValue(item.Serial, out HashSet<T> serial))
                return false;

            foreach (T behaviour in serial)
            {
                if (behaviour is not EActor component)
                    continue;

                pickup.AddComponent(component);
            }

            PickupRestoredDispatcher.InvokeAll(pickup);
            Log.WarnWithContext($"Pickup with serial {pickup.Serial} was restored to item with serial {item.Serial} of type {item.Type}.");

            return true;
        }

        /// <summary>
        /// Restores all trackables from a item which is being tracked.
        /// </summary>
        /// <param name="item">The <see cref="Item"/> whose trackables are to be restored.</param>
        /// <param name="pickup">The <see cref="Pickup"/> whose trackables are to be transfered.</param>
        /// <returns><see langword="true"/> if the item was restored successfully; otherwise, <see langword="false"/>.</returns>
        public virtual bool Restore(Item item, Pickup pickup)
        {
            if (!pickup || !item || !TrackedPickupSerials.ContainsKey(pickup.Serial) ||
                !TrackedItemSerials.ContainsKey(item.Serial))
                return false;

            foreach (T behaviour in TrackedPickupSerials[pickup.Serial])
            {
                if (behaviour is not EActor component)
                    continue;

                pickup.AddComponent(component);
            }

            PickupRestoredDispatcher.InvokeAll(pickup);
            Log.WarnWithContext($"Item with serial {item.Serial} was restored to pickup with serial {pickup.Serial} of type {pickup.Type}.");

            return true;
        }

        /// <summary>
        /// Removes an item and all its trackables from the tracking.
        /// </summary>
        /// <param name="item">The item to be removed.</param>
        public virtual void Remove(Item item)
        {
            Log.WarnWithContext($"Item with serial {item.Serial} of type {item.Type} is being untracked.");
            if (TrackedItemSerials.ContainsKey(item.Serial))
            {
                TrackedItemSerials.Remove(item.Serial);

                item.GetComponents<T>().ForEach(c =>
                {
                    if (c is EActor actor)
                        actor.Destroy();
                });

                ItemRemovedDispatcher.InvokeAll(item.Serial);
                Log.WarnWithContext($"Item with serial {item.Serial} of type {item.Type} was untracked.");

            }
        }

        /// <summary>
        /// Removes an ability from the tracking.
        /// </summary>
        /// <param name="item">The item owning the ability.</param>
        /// <param name="behaviour">The <see cref="ITrackable"/> to be removed.</param>
        public virtual void Remove(Item item, T behaviour)
        {
            Log.WarnWithContext($"Item with serial {item.Serial} of type {item.Type} is being untracked ({behaviour.GetType().Name}).");

            if (TrackedItemSerials.ContainsKey(item.Serial))
            {
                IEnumerable<T> previousTrackedItems = TrackedItemSerials[item.Serial];
                TrackedItemSerials[item.Serial].Remove(behaviour);
                item.GetComponent(behaviour.GetType()).Destroy();
                ItemTrackingModifiedEventArgs ev = new(item, previousTrackedItems.Cast<ITrackable>(), TrackedItemSerials[item.Serial].Cast<ITrackable>());
                ItemTrackingModifiedDispatcher.InvokeAll(ev);
                Log.WarnWithContext($"Item with serial {item.Serial} of type {item.Type} was untracked: ({behaviour.GetType().Name}).");

            }
        }

        /// <summary>
        /// Removes a pickup and all its trackables from the tracking.
        /// </summary>
        /// <param name="pickup">The pickup to be removed.</param>
        public virtual void Remove(Pickup pickup)
        {
            Log.WarnWithContext($"Pickup with serial {pickup.Serial} of type {pickup.Type} is being untracked.");

            if (TrackedPickupSerials.ContainsKey(pickup.Serial))
            {
                TrackedPickupSerials.Remove(pickup.Serial);

                pickup.GetComponents<T>().ForEach(c =>
                {
                    if (c is EActor actor)
                        actor.Destroy();
                });

                PickupRemovedDispatcher.InvokeAll(pickup.Serial);
                Log.WarnWithContext($"Pickup with serial {pickup.Serial} of type {pickup.Type} was untracked.");

            }
        }

        /// <summary>
        /// Removes an ability from the tracking.
        /// </summary>
        /// <param name="pickup">The pickup owning the ability.</param>
        /// <param name="behaviour">The <typeparamref name="T"/> to be removed.</param>
        public virtual void Remove(Pickup pickup, T behaviour)
        {
            Log.WarnWithContext($"Pickup with serial {pickup.Serial} of type {pickup.Type} is being untracked: ({behaviour.GetType().Name}).");

            if (TrackedPickupSerials.ContainsKey(pickup.Serial))
            {
                IEnumerable<T> previousTrackableItems = TrackedPickupSerials[pickup.Serial].Cast<T>();
                TrackedPickupSerials[pickup.Serial].Remove(behaviour);
                pickup.GetComponent(behaviour.GetType()).Destroy();
                PickupTrackingModifiedEventArgs ev = new(pickup, previousTrackableItems.Cast<ITrackable>(), TrackedPickupSerials[pickup.Serial].Cast<ITrackable>());
                PickupTrackingModifiedDispatcher.InvokeAll(ev);
                Log.WarnWithContext($"Pickup with serial {pickup.Serial} of type {pickup.Type} was untracked: ({behaviour.GetType().Name}).");

            }
        }

        /// <summary>
        /// Removes an item or pickup with the specified serial number from the tracking.
        /// </summary>
        /// <param name="serial">The serial number of the item or pickup to be removed.</param>
        /// <param name="behaviours">The <see cref="IEnumerable{T}"/> of <typeparamref name="T"/> containing all <see cref="ITrackable"/> items to be removed.</param>
        public virtual void Remove(ushort serial, IEnumerable<T> behaviours)
        {
            if (TrackedItemSerials.ContainsKey(serial))
            {
                IEnumerable<T> previousTrackableItems = TrackedItemSerials[serial].Cast<T>();

                foreach (T behaviour in behaviours)
                    TrackedItemSerials[serial].Remove(behaviour);

                ItemTrackingModifiedEventArgs ev = new(Item.Get(serial), previousTrackableItems.Cast<ITrackable>(), TrackedItemSerials[serial].Cast<ITrackable>());
                ItemTrackingModifiedDispatcher.InvokeAll(ev);
                Log.WarnWithContext($"Item with serial {serial} was untracked.");
            }

            if (TrackedPickupSerials.ContainsKey(serial))
            {
                IEnumerable<T> previousTrackableItems = TrackedPickupSerials[serial].Cast<T>();

                foreach (T behaviour in behaviours)
                    TrackedPickupSerials[serial].Remove(behaviour);

                PickupTrackingModifiedEventArgs ev = new(Pickup.Get(serial), previousTrackableItems.Cast<ITrackable>(), TrackedPickupSerials[serial].Cast<ITrackable>());
                PickupTrackingModifiedDispatcher.InvokeAll(ev);
                Log.WarnWithContext($"Pickup with serial {serial} was untracked.");
            }
        }

        /// <summary>
        /// Checks if an item is being tracked.
        /// </summary>
        /// <param name="item">The <see cref="Item"/> to check.</param>
        /// <returns><see langword="true"/> if the item is being tracked; otherwise, <see langword="false"/>.</returns>
        public virtual bool IsTracked(Item item) => TrackedItemSerials.ContainsKey(item.Serial);

        /// <summary>
        /// Checks if a pickup is being tracked.
        /// </summary>
        /// <param name="pickup">The <see cref="Pickup"/> to check.</param>
        /// <returns><see langword="true"/> if the pickup is being tracked; otherwise, <see langword="false"/>.</returns>
        public virtual bool IsTracked(Pickup pickup) => TrackedPickupSerials.ContainsKey(pickup.Serial);

        /// <summary>
        /// Gets the tracked values associated with the specified item.
        /// </summary>
        /// <param name="item">The <see cref="Item"/> to retrieve tracked values from.</param>
        /// <returns>
        /// An <see cref="IEnumerable{T}"/> containing the tracked values associated with the item.
        /// If the item is not tracked, returns an empty collection.
        /// </returns>
        public virtual IEnumerable<T> GetTrackedValues(Item item) => !IsTracked(item) ? Enumerable.Empty<T>() : TrackedItemSerials[item.Serial];

        /// <summary>
        /// Gets the tracked values associated with the specified pickup.
        /// </summary>
        /// <param name="pickup">The <see cref="Pickup"/> to retrieve tracked values from.</param>
        /// <returns>
        /// An <see cref="IEnumerable{T}"/> containing the tracked values associated with the pickup.
        /// If the pickup is not tracked, returns an empty collection.
        /// </returns>
        public virtual IEnumerable<T> GetTrackedValues(Pickup pickup) => !IsTracked(pickup) ? Enumerable.Empty<T>() : TrackedPickupSerials[pickup.Serial];

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