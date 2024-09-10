// -----------------------------------------------------------------------
// <copyright file="TrackerBase.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomModules.API.Features
{
    using System.Collections.Generic;
    using System.Linq;

    using Exiled.API.Extensions;
    using Exiled.API.Features.Attributes;
    using Exiled.API.Features.Core;
    using Exiled.API.Features.DynamicEvents;
    using Exiled.API.Features.Items;
    using Exiled.API.Features.Pickups;
    using Exiled.CustomModules.API.Features.CustomAbilities;
    using Exiled.CustomModules.API.Features.CustomItems.Items;
    using Exiled.CustomModules.API.Features.CustomItems.Pickups;
    using Exiled.CustomModules.API.Interfaces;
    using Exiled.CustomModules.Events.EventArgs.Tracking;
    using Exiled.Events.EventArgs.Map;
    using Exiled.Events.EventArgs.Player;

    /// <summary>
    /// The actor which handles all tracking-related tasks for items.
    /// </summary>
    public class TrackerBase : StaticActor
    {
        /// <summary>
        /// Gets or sets the <see cref="TDynamicEventDispatcher{T}"/> which handles all the delegates fired when an item is added.
        /// </summary>
        [DynamicEventDispatcher]
        public TDynamicEventDispatcher<Item> ItemAddedDispatcher { get; set; } = new();

        /// <summary>
        /// Gets or sets the <see cref="TDynamicEventDispatcher{T}"/> which handles all the delegates fired when an item is removed.
        /// </summary>
        [DynamicEventDispatcher]
        public TDynamicEventDispatcher<ushort> ItemRemovedDispatcher { get; set; } = new();

        /// <summary>
        /// Gets or sets the <see cref="TDynamicEventDispatcher{T}"/> which handles all the delegates fired when an item is restored.
        /// </summary>
        [DynamicEventDispatcher]
        public TDynamicEventDispatcher<Item> ItemRestoredDispatcher { get; set; } = new();

        /// <summary>
        /// Gets or sets the <see cref="TDynamicEventDispatcher{T}"/> which handles all the delegates fired when an item tracking is modified.
        /// </summary>
        [DynamicEventDispatcher]
        public TDynamicEventDispatcher<ItemTrackingModifiedEventArgs> ItemTrackingModifiedDispatcher { get; set; } = new();

        /// <summary>
        /// Gets or sets the <see cref="TDynamicEventDispatcher{T}"/> which handles all the delegates fired when a pickup is added.
        /// </summary>
        [DynamicEventDispatcher]
        public TDynamicEventDispatcher<Pickup> PickupAddedDispatcher { get; set; } = new();

        /// <summary>
        /// Gets or sets the <see cref="TDynamicEventDispatcher{T}"/> which handles all the delegates fired when a pickup is removed.
        /// </summary>
        [DynamicEventDispatcher]
        public TDynamicEventDispatcher<ushort> PickupRemovedDispatcher { get; set; } = new();

        /// <summary>
        /// Gets or sets the <see cref="TDynamicEventDispatcher{T}"/> which handles all the delegates fired when a pickup is restored.
        /// </summary>
        [DynamicEventDispatcher]
        public TDynamicEventDispatcher<Pickup> PickupRestoredDispatcher { get; set; } = new();

        /// <summary>
        /// Gets or sets the <see cref="TDynamicEventDispatcher{T}"/> which handles all the delegates fired when a pickup tracking is modified.
        /// </summary>
        [DynamicEventDispatcher]
        public TDynamicEventDispatcher<PickupTrackingModifiedEventArgs> PickupTrackingModifiedDispatcher { get; set; } = new();

        /// <summary>
        /// Gets a <see cref="Dictionary{TKey, TValue}"/> containing all serials and their corresponding <see cref="ITrackable"/> items.
        /// </summary>
        private Dictionary<ushort, HashSet<ITrackable>> TrackedSerials { get; } = new();

        /// <summary>
        /// Adds or tracks the trackables of an item based on its serial.
        /// </summary>
        /// <param name="item">The <see cref="Item"/> whose trackables are to be added or tracked.</param>
        /// <returns><see langword="true"/> if the item was added or tracked successfully; otherwise, <see langword="false"/>.</returns>
        public virtual bool AddOrTrack(Item item)
        {
            if (!item)
                return false;

            IEnumerable<ITrackable> trackableBehaviours = item.GetComponents<ITrackable>() ?? Enumerable.Empty<ITrackable>();
            if (trackableBehaviours.IsEmpty())
                return false;

            if (TrackedSerials.ContainsKey(item.Serial))
            {
                IEnumerable<ITrackable> previousTrackableItems = TrackedSerials[item.Serial];
                TrackedSerials[item.Serial].AddRange(trackableBehaviours.Cast<ITrackable>());
                ItemTrackingModifiedEventArgs ev = new(item, previousTrackableItems.Cast<ITrackable>(), TrackedSerials[item.Serial].Cast<ITrackable>());
                ItemTrackingModifiedDispatcher.InvokeAll(ev);
                return true;
            }

            TrackedSerials.Add(item.Serial, new HashSet<ITrackable>());
            TrackedSerials[item.Serial].AddRange(trackableBehaviours.Cast<ITrackable>());

            ItemAddedDispatcher.InvokeAll(item);

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

            if (TrackedSerials.ContainsKey(pickup.Serial))
            {
                IEnumerable<ITrackable> previousTrackableItems = TrackedSerials[pickup.Serial];
                TrackedSerials[pickup.Serial].AddRange(trackableBehaviours.Cast<ITrackable>());
                PickupTrackingModifiedEventArgs ev = new(pickup, previousTrackableItems.Cast<ITrackable>(), TrackedSerials[pickup.Serial].Cast<ITrackable>());
                PickupTrackingModifiedDispatcher.InvokeAll(ev);
                return true;
            }

            TrackedSerials.Add(pickup.Serial, new HashSet<ITrackable>());
            TrackedSerials[pickup.Serial].AddRange(trackableBehaviours.Cast<ITrackable>());

            PickupAddedDispatcher.InvokeAll(pickup);

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
            if (!item || !TrackedSerials.TryGetValue(serial, out HashSet<ITrackable> components))
                return false;

            bool wasRestored = false;
            foreach (ITrackable behaviour in components)
            {
                if (behaviour is not(IItemBehaviour and IAbilityBehaviour and EActor component))
                    continue;

                wasRestored = true;
                item.AddComponent(component);
            }

            if (wasRestored)
                ItemRestoredDispatcher.InvokeAll(item);

            return wasRestored;
        }

        /// <summary>
        /// Restores all trackables from a pickup which is being tracked.
        /// </summary>
        /// <param name="pickup">The <see cref="Pickup"/> whose trackables are to be restored.</param>
        /// <returns><see langword="true"/> if the pickup was restored successfully; otherwise, <see langword="false"/>.</returns>
        public virtual bool Restore(Pickup pickup)
        {
            if (!pickup || !TrackedSerials.TryGetValue(pickup.Serial, out HashSet<ITrackable> components))
                return false;

            bool wasRestored = false;
            foreach (ITrackable behaviour in components)
            {
                if (behaviour is not(IPickupBehaviour and IAbilityBehaviour and EActor component))
                    continue;

                wasRestored = true;
                pickup.AddComponent(component);
            }

            if (wasRestored)
                PickupRestoredDispatcher.InvokeAll(pickup);

            return wasRestored;
        }

        /// <summary>
        /// Restores all trackables from a pickup which is being tracked.
        /// </summary>
        /// <param name="pickup">The <see cref="Pickup"/> whose trackables are to be restored.</param>
        /// <param name="item">The <see cref="Item"/> whose trackables are to be transfered.</param>
        /// <returns><see langword="true"/> if the pickup was restored successfully; otherwise, <see langword="false"/>.</returns>
        public virtual bool Restore(Pickup pickup, Item item)
        {
            if (!IsTracked(pickup) || !IsTracked(item))
                return false;

            bool wasRestored = false;
            foreach (ITrackable behaviour in TrackedSerials[pickup.Serial].ToList())
            {
                if (behaviour is not(IItemBehaviour and IAbilityBehaviour and EActor component))
                {
                    pickup.RemoveComponent(behaviour as EActor);
                    continue;
                }

                wasRestored = true;
                item.AddComponent(component);
            }

            if (wasRestored)
                ItemRestoredDispatcher.InvokeAll(item);

            return wasRestored;
        }

        /// <summary>
        /// Restores all trackables from a item which is being tracked.
        /// </summary>
        /// <param name="item">The <see cref="Item"/> whose trackables are to be restored.</param>
        /// <param name="pickup">The <see cref="Pickup"/> whose trackables are to be transfered.</param>
        /// <returns><see langword="true"/> if the item was restored successfully; otherwise, <see langword="false"/>.</returns>
        public virtual bool Restore(Item item, Pickup pickup)
        {
            if (!IsTracked(pickup) || !IsTracked(item))
                return false;

            bool wasRestored = false;
            foreach (ITrackable behaviour in TrackedSerials[item.Serial].ToList())
            {
                if (behaviour is not(IPickupBehaviour and IAbilityBehaviour and EActor component))
                {
                    item.RemoveComponent(behaviour as EActor);
                    continue;
                }

                wasRestored = true;
                pickup.AddComponent(component);
            }

            if (wasRestored)
                PickupRestoredDispatcher.InvokeAll(pickup);

            return wasRestored;
        }

        /// <summary>
        /// Removes an item and all its trackables from the tracking.
        /// </summary>
        /// <param name="item">The item to be removed.</param>
        /// <param name="destroy">A value indicating whether the behaviours should be destroyed.</param>
        public virtual void Remove(Item item, bool destroy = false)
        {
            if (TrackedSerials.ContainsKey(item.Serial))
            {
                TrackedSerials.Remove(item.Serial);

                if (destroy)
                {
                    item.GetComponents<ITrackable>().ForEach(c =>
                    {
                        if (c is EActor actor)
                            actor.Destroy();
                    });
                }
                else
                {
                    item.GetComponents<ITrackable>().ForEach(c =>
                    {
                        if (c is EActor actor)
                            item.RemoveComponent(actor);
                    });
                }

                ItemRemovedDispatcher.InvokeAll(item.Serial);
            }
        }

        /// <summary>
        /// Removes an item and their relative trackable from the tracking.
        /// </summary>
        /// <param name="item">The item to be removed.</param>
        /// <param name="behaviour">The <see cref="ITrackable"/> to be removed.</param>
        /// <param name="destroy">A value indicating whether the behaviour should be destroyed.</param>
        public virtual void Remove(Item item, ITrackable behaviour, bool destroy = false)
        {
            if (TrackedSerials.ContainsKey(item.Serial))
            {
                IEnumerable<ITrackable> previousTrackedItems = TrackedSerials[item.Serial];
                TrackedSerials[item.Serial].Remove(behaviour);

                if (destroy)
                    item.GetComponent(behaviour.GetType()).Destroy();
                else
                    item.RemoveComponent(behaviour.GetType());

                ItemTrackingModifiedEventArgs ev = new(item, previousTrackedItems.Cast<ITrackable>(), TrackedSerials[item.Serial].Cast<ITrackable>());
                ItemTrackingModifiedDispatcher.InvokeAll(ev);
            }
        }

        /// <summary>
        /// Removes a pickup and all its trackables from the tracking.
        /// </summary>
        /// <param name="pickup">The pickup to be removed.</param>
        /// <param name="destroy">A value indicating whether the behaviours should be destroyed.</param>
        public virtual void Remove(Pickup pickup, bool destroy = false)
        {
            if (TrackedSerials.ContainsKey(pickup.Serial))
            {
                TrackedSerials.Remove(pickup.Serial);

                if (destroy)
                {
                    pickup.GetComponents<ITrackable>().ForEach(c =>
                    {
                        if (c is EActor actor)
                            actor.Destroy();
                    });
                }
                else
                {
                    pickup.GetComponents<ITrackable>().ForEach(c =>
                    {
                        if (c is EActor actor)
                            pickup.RemoveComponent(actor);
                    });
                }

                PickupRemovedDispatcher.InvokeAll(pickup.Serial);
            }
        }

        /// <summary>
        /// Removes an ability from the tracking.
        /// </summary>
        /// <param name="pickup">The pickup owning the ability.</param>
        /// <param name="behaviour">The <see cref="ITrackable"/> to be removed.</param>
        /// <param name="destroy">A value indicating whether the behaviour should be destroyed.</param>
        public virtual void Remove(Pickup pickup, ITrackable behaviour, bool destroy = false)
        {
            if (TrackedSerials.ContainsKey(pickup.Serial))
            {
                IEnumerable<ITrackable> previousTrackableItems = TrackedSerials[pickup.Serial].Cast<ITrackable>();
                TrackedSerials[pickup.Serial].Remove(behaviour);

                if (destroy)
                    (behaviour as EActor).Destroy();
                else
                    pickup.RemoveComponent(behaviour.GetType());

                PickupTrackingModifiedEventArgs ev = new(pickup, previousTrackableItems.Cast<ITrackable>(), TrackedSerials[pickup.Serial].Cast<ITrackable>());
                PickupTrackingModifiedDispatcher.InvokeAll(ev);
            }
        }

        /// <summary>
        /// Removes an item or pickup with the specified serial number from the tracking.
        /// </summary>
        /// <param name="serial">The serial number of the item or pickup to be removed.</param>
        /// <param name="behaviours">The <see cref="IEnumerable{T}"/> containing all <see cref="ITrackable"/> items to be removed.</param>
        public virtual void Remove(ushort serial, IEnumerable<ITrackable> behaviours)
        {
            if (TrackedSerials.ContainsKey(serial))
            {
                IEnumerable<ITrackable> previousTrackableItems = TrackedSerials[serial].Cast<ITrackable>();

                foreach (ITrackable behaviour in behaviours)
                    TrackedSerials[serial].Remove(behaviour);

                ItemTrackingModifiedEventArgs ev = new(Item.Get(serial), previousTrackableItems.Cast<ITrackable>(), TrackedSerials[serial].Cast<ITrackable>());
                ItemTrackingModifiedDispatcher.InvokeAll(ev);
            }

            if (TrackedSerials.ContainsKey(serial))
            {
                IEnumerable<ITrackable> previousTrackableItems = TrackedSerials[serial].Cast<ITrackable>();

                foreach (ITrackable behaviour in behaviours)
                    TrackedSerials[serial].Remove(behaviour);

                PickupTrackingModifiedEventArgs ev = new(Pickup.Get(serial), previousTrackableItems.Cast<ITrackable>(), TrackedSerials[serial].Cast<ITrackable>());
                PickupTrackingModifiedDispatcher.InvokeAll(ev);
            }
        }

        /// <summary>
        /// Checks if an item is being tracked.
        /// </summary>
        /// <param name="item">The <see cref="Item"/> to check.</param>
        /// <returns><see langword="true"/> if the item is being tracked; otherwise, <see langword="false"/>.</returns>
        public virtual bool IsTracked(Item item) => item && TrackedSerials.ContainsKey(item.Serial);

        /// <summary>
        /// Checks if a pickup is being tracked.
        /// </summary>
        /// <param name="pickup">The <see cref="Pickup"/> to check.</param>
        /// <returns><see langword="true"/> if the pickup is being tracked; otherwise, <see langword="false"/>.</returns>
        public virtual bool IsTracked(Pickup pickup) => pickup && TrackedSerials.ContainsKey(pickup.Serial);

        /// <summary>
        /// Gets the tracked values associated with the specified item.
        /// </summary>
        /// <param name="item">The <see cref="Item"/> to retrieve tracked values from.</param>
        /// <returns>
        /// An <see cref="IEnumerable{T}"/> containing the tracked values associated with the item.
        /// If the item is not tracked, returns an empty collection.
        /// </returns>
        public virtual IEnumerable<ITrackable> GetTrackedValues(Item item) => !IsTracked(item) ? Enumerable.Empty<ITrackable>() : TrackedSerials[item.Serial];

        /// <summary>
        /// Gets the tracked values associated with the specified pickup.
        /// </summary>
        /// <param name="pickup">The <see cref="Pickup"/> to retrieve tracked values from.</param>
        /// <returns>
        /// An <see cref="IEnumerable{T}"/> containing the tracked values associated with the pickup.
        /// If the pickup is not tracked, returns an empty collection.
        /// </returns>
        public virtual IEnumerable<ITrackable> GetTrackedValues(Pickup pickup) => !IsTracked(pickup) ? Enumerable.Empty<ITrackable>() : TrackedSerials[pickup.Serial];

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
            if (ev.Pickup is not null)
            {
                Restore(ev.Item, ev.Pickup);
                return;
            }

            Remove(ev.Item, true);
        }

        /// <inheritdoc/>
        protected override void SubscribeEvents()
        {
            base.SubscribeEvents();

            Exiled.Events.Handlers.Player.ItemAdded += OnItemAdded;
            Exiled.Events.Handlers.Player.ItemRemoved += OnItemRemoved;
        }

        /// <inheritdoc/>
        protected override void UnsubscribeEvents()
        {
            base.UnsubscribeEvents();

            Exiled.Events.Handlers.Player.ItemAdded -= OnItemAdded;
            Exiled.Events.Handlers.Player.ItemRemoved -= OnItemRemoved;
        }
    }
}