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
    using Exiled.API.Features;
    using Exiled.API.Features.Attributes;
    using Exiled.API.Features.Core;
    using Exiled.API.Features.Core.Generic;
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
    using MEC;

    /// <summary>
    /// The actor which handles all tracking-related tasks for items.
    /// </summary>
    public class TrackerBase : StaticActor<TrackerBase>
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

            if (TrackedSerials.TryGetValue(item.Serial, out HashSet<ITrackable> components))
            {
                IEnumerable<ITrackable> previousTrackableItems = components;
                TrackedSerials[item.Serial].AddRange(trackableBehaviours.Cast<ITrackable>());
                ItemTrackingModifiedEventArgs ev = new(item, previousTrackableItems.Cast<ITrackable>(), components.Cast<ITrackable>());
                ItemTrackingModifiedDispatcher.InvokeAll(ev);
                return true;
            }

            TrackedSerials.Add(item.Serial, new HashSet<ITrackable>(trackableBehaviours.Cast<ITrackable>()));
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

            if (TrackedSerials.TryGetValue(pickup.Serial, out HashSet<ITrackable> components))
            {
                IEnumerable<ITrackable> previousTrackableItems = components.ToList();
                components.AddRange(trackableBehaviours.Cast<ITrackable>());
                PickupTrackingModifiedEventArgs ev = new(pickup, previousTrackableItems.Cast<ITrackable>(), components.Cast<ITrackable>());
                PickupTrackingModifiedDispatcher.InvokeAll(ev);
                return true;
            }

            TrackedSerials.Add(pickup.Serial, new HashSet<ITrackable>(trackableBehaviours.Cast<ITrackable>()));
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
                if (behaviour is not(IItemBehaviour and IAbilityBehaviour))
                    continue;

                EActor component = behaviour as EActor;
                if (component is not null)
                {
                    wasRestored = true;
                    item.AddComponent(component);
                }
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
                if (behaviour is not(IPickupBehaviour and IAbilityBehaviour))
                    continue;

                EActor component = behaviour as EActor;
                if (component is not null)
                {
                    pickup.AddComponent(component);
                    wasRestored = true;
                }
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
            EActor component = null;
            foreach (ITrackable behaviour in TrackedSerials[pickup.Serial].ToList())
            {
                if (behaviour is not(IItemBehaviour and IAbilityBehaviour))
                {
                    component = behaviour as EActor;

                    if (component is not null)
                        pickup.RemoveComponent(component);

                    continue;
                }

                if (component is not null)
                {
                    item.AddComponent(component);
                    wasRestored = true;
                }
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
            EActor component = null;
            foreach (ITrackable behaviour in TrackedSerials[item.Serial].ToList())
            {
                if (behaviour is not(IPickupBehaviour and IAbilityBehaviour))
                {
                    component = behaviour as EActor;

                    if (component is not null)
                        item.RemoveComponent(behaviour as EActor);

                    continue;
                }

                if (component is not null)
                {
                    pickup.AddComponent(component);
                    wasRestored = true;
                }
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
            if (!TrackedSerials.TryGetValue(item.Serial, out HashSet<ITrackable> components))
                return;

            if (destroy)
            {
                Remove(item.Serial, true);
            }
            else
            {
                components.ForEach(c =>
                {
                    if (c is EActor actor)
                        item.RemoveComponent(actor);
                });

                TrackedSerials.Remove(item.Serial);
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
            if (!TrackedSerials.TryGetValue(item.Serial, out HashSet<ITrackable> components))
                return;

            IEnumerable<ITrackable> previousTrackedItems = components.ToList();
            components.Remove(behaviour);

            if (destroy)
                item.GetComponent(behaviour.GetType()).Destroy();
            else
                item.RemoveComponent(behaviour.GetType());

            ItemTrackingModifiedEventArgs ev = new(item, previousTrackedItems.Cast<ITrackable>(), components.Cast<ITrackable>());
            ItemTrackingModifiedDispatcher.InvokeAll(ev);
        }

        /// <summary>
        /// Removes a pickup and all its trackables from the tracking.
        /// </summary>
        /// <param name="pickup">The pickup to be removed.</param>
        /// <param name="destroy">A value indicating whether the behaviours should be destroyed.</param>
        public virtual void Remove(Pickup pickup, bool destroy = false)
        {
            ushort serial = pickup.Serial;

            if (!TrackedSerials.ContainsKey(serial))
                return;

            if (destroy)
            {
                Remove(serial, true);
            }
            else
            {
                TrackedSerials[serial].ForEach(c =>
                {
                    if (c is EActor actor)
                    {
                        pickup.RemoveComponent(actor);
                    }
                });

                TrackedSerials.Remove(serial);
                PickupRemovedDispatcher.InvokeAll(serial);
            }
        }

        /// <summary>
        /// Removes a serial and all its trackables from the tracking.
        /// </summary>
        /// <param name="serial">The serial to be removed.</param>
        /// <param name="destroy">A value indicating whether the behaviours should be destroyed.</param>
        public virtual void Remove(ushort serial, bool destroy = false)
        {
            if (!TrackedSerials.ContainsKey(serial))
                return;

            if (destroy)
            {
                TrackedSerials[serial].ForEach(c =>
                {
                    if (c is EActor actor)
                        actor.Destroy();
                });

                TrackedSerials.Remove(serial);
                PickupRemovedDispatcher.InvokeAll(serial);
            }
            else
            {
                Pickup pickup = Pickup.Get(serial);
                if (pickup is not null)
                {
                    Remove(pickup, destroy);
                    return;
                }

                Item item = Item.Get(serial);
                if (item is not null && item.Owner is not null && item.Owner != Server.Host)
                {
                    Remove(item, destroy);
                    return;
                }
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
            if (behaviour is not EActor component || !TrackedSerials.TryGetValue(pickup.Serial, out HashSet<ITrackable> components))
                return;

            IEnumerable<ITrackable> previousTrackableItems = components.ToList();
            components.Remove(behaviour);

            if (destroy)
                component.Destroy();
            else
                pickup.RemoveComponent(component.GetType());

            PickupTrackingModifiedEventArgs ev = new(pickup, previousTrackableItems.Cast<ITrackable>(), components);
            PickupTrackingModifiedDispatcher.InvokeAll(ev);
        }

        /// <summary>
        /// Removes an item or pickup with the specified serial number from the tracking.
        /// </summary>
        /// <param name="serial">The serial number of the item or pickup to be removed.</param>
        /// <param name="behaviours">The <see cref="IEnumerable{T}"/> containing all <see cref="ITrackable"/> items to be removed.</param>
        public virtual void Remove(ushort serial, IEnumerable<ITrackable> behaviours)
        {
            if (!TrackedSerials.TryGetValue(serial, out HashSet<ITrackable> components))
                return;

            IEnumerable<ITrackable> RemoveBehaviours()
            {
                IEnumerable<ITrackable> previousTrackableItems = components.ToList();

                foreach (ITrackable behaviour in behaviours)
                    components.Remove(behaviour);

                return previousTrackableItems;
            }

            Item item = Item.Get(serial);
            if (item is not null && item.Owner is not null && item.Owner != Server.Host)
            {
                IEnumerable<ITrackable> previousTrackableItems = RemoveBehaviours();
                ItemTrackingModifiedEventArgs ev = new(item, previousTrackableItems, components);
                ItemTrackingModifiedDispatcher.InvokeAll(ev);
                return;
            }

            Pickup pickup = Pickup.Get(serial);
            if (pickup is not null)
            {
                IEnumerable<ITrackable> previousTrackableItems = RemoveBehaviours();
                PickupTrackingModifiedEventArgs ev = new(Pickup.Get(serial), previousTrackableItems, components);
                PickupTrackingModifiedDispatcher.InvokeAll(ev);
            }
        }

        /// <summary>
        /// Checks if a serial is being tracked.
        /// </summary>
        /// <param name="serial">The serial to check.</param>
        /// <returns><see langword="true"/> if the serial is being tracked; otherwise, <see langword="false"/>.</returns>
        public virtual bool IsTracked(ushort serial) => TrackedSerials.ContainsKey(serial);

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
        /// Gets the tracked values associated with the specified serial.
        /// </summary>
        /// <param name="serial">The serial to retrieve tracked values from.</param>
        /// <returns>
        /// An <see cref="IEnumerable{T}"/> containing the tracked values associated with the serial.
        /// If the serial is not tracked, returns an empty collection.
        /// </returns>
        public virtual IEnumerable<ITrackable> GetTrackedValues(ushort serial) => !IsTracked(serial) ? Enumerable.Empty<ITrackable>() : TrackedSerials[serial];

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
        internal void OnItemAdded(ItemAddedEventArgs ev)
        {
            if (ev.Item.Serial == 0)
                return;

            TrackablesGC.Get().Free(ev.Item.Serial);
        }

        /// <summary>
        /// Handles the event when a tracked item or pickup is removed from a player's inventory.
        /// </summary>
        /// <param name="ev">The <see cref="ItemRemovedEventArgs"/> containing information about the removed item.</param>
        internal void OnItemRemoved(ItemRemovedEventArgs ev)
        {
            if (ev.Pickup is not null)
                return;

            Remove(ev.Item, true);
        }

        /// <summary>
        /// Handles the event when a tracked item or pickup is destroyed.
        /// </summary>
        /// <param name="ev">The <see cref="PickupDestroyedEventArgs"/> containing information about the destroyed pickup.</param>
        internal void OnPickupAdded(PickupAddedEventArgs ev)
        {
            if (ev.Pickup.Serial == 0)
                return;

            TrackablesGC.Get().Allocate(ev.Pickup.Serial);
        }

        /// <summary>
        /// Handles the event when a tracked item or pickup is destroyed.
        /// </summary>
        /// <param name="ev">The <see cref="PickupDestroyedEventArgs"/> containing information about the destroyed pickup.</param>
        internal void OnPickupDestroyed(PickupDestroyedEventArgs ev)
        {
            if (ev.Pickup.Serial == 0)
                return;

            Timing.CallDelayed(0.5f, () =>
            {
                if (TrackablesGC.Get().Collect(ev.Pickup.Serial))
                    Remove(ev.Pickup, true);
            });
        }

        /// <inheritdoc/>
        protected override void SubscribeEvents()
        {
            base.SubscribeEvents();

            Exiled.Events.Handlers.Player.ItemAdded += OnItemAdded;
            Exiled.Events.Handlers.Player.ItemRemoved += OnItemRemoved;
            Exiled.Events.Handlers.Map.PickupAdded += OnPickupAdded;
            Exiled.Events.Handlers.Map.PickupDestroyed += OnPickupDestroyed;
        }

        /// <inheritdoc/>
        protected override void UnsubscribeEvents()
        {
            base.UnsubscribeEvents();

            Exiled.Events.Handlers.Player.ItemAdded -= OnItemAdded;
            Exiled.Events.Handlers.Player.ItemRemoved -= OnItemRemoved;
            Exiled.Events.Handlers.Map.PickupAdded -= OnPickupAdded;
            Exiled.Events.Handlers.Map.PickupDestroyed -= OnPickupDestroyed;
        }
    }
}