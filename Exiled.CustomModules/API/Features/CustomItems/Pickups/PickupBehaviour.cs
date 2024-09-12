// -----------------------------------------------------------------------
// <copyright file="PickupBehaviour.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomModules.API.Features.CustomItems.Pickups
{
    using System.Linq;
    using System.Reflection;

    using Exiled.API.Features;
    using Exiled.API.Features.Attributes;
    using Exiled.API.Features.Core.Interfaces;
    using Exiled.API.Features.DynamicEvents;
    using Exiled.API.Features.Pickups;
    using Exiled.CustomModules.API.Features.Generic;
    using Exiled.Events.EventArgs.Player;
    using Exiled.Events.EventArgs.Scp914;

    /// <summary>
    /// Represents the base class for custom pickup behaviors.
    /// </summary>
    /// <remarks>
    /// This class extends <see cref="ModuleBehaviour{T}"/> and implements <see cref="IPickupBehaviour"/> and <see cref="IAdditiveSettings{T}"/>.
    /// <br/>It provides a foundation for creating custom behaviors associated with in-game pickup.
    /// </remarks>
    public abstract class PickupBehaviour : ModuleBehaviour<Pickup>, IPickupBehaviour, IAdditiveSettings<SettingsBase>
    {
        private static TrackerBase tracker;
        private ushort serial;

        /// <summary>
        /// Gets or sets the <see cref="TDynamicEventDispatcher{T}"/> which handles all the delegates fired before the pickup is gets picked up.
        /// </summary>
        [DynamicEventDispatcher]
        public TDynamicEventDispatcher<PickingUpItemEventArgs> PickingUpItemDispatcher { get; set; } = new();

        /// <inheritdoc/>
        public override bool DisposeOnNullOwner { get; protected set; } = false;

        /// <summary>
        /// Gets the relative <see cref="CustomItems.CustomItem"/>.
        /// </summary>
        public CustomItem CustomItem { get; private set; }

        /// <inheritdoc/>
        public SettingsBase Settings { get; set; }

        /// <summary>
        /// Gets the <see cref="TrackerBase"/>.
        /// </summary>
        protected static TrackerBase Tracker => tracker ??= TrackerBase.Get();

        /// <inheritdoc/>
        public virtual void AdjustAdditivePipe()
        {
            ImplementConfigs();

            if (CustomItem.TryGet(GetType(), out CustomItem customItem) && customItem.Settings is SettingsBase settings)
            {
                CustomItem = customItem;
                Settings = settings;
            }

            if (CustomItem is null || Settings is null)
            {
                Log.Error($"Custom pickup ({GetType().Name}) has invalid configuration.");
                Destroy();
            }
        }

        /// <inheritdoc/>
        protected override void ApplyConfig(PropertyInfo propertyInfo, PropertyInfo targetInfo)
        {
            targetInfo?.SetValue(
                typeof(SettingsBase).IsAssignableFrom(targetInfo.DeclaringType) ? Settings : this,
                propertyInfo.GetValue(Config, null));
        }

        /// <summary>
        /// Checks if the specified pickup is being tracked and associated with this item.
        /// </summary>
        /// <param name="pickup">The <see cref="Pickup"/> to check.</param>
        /// <returns>
        /// <see langword="true"/> if the specified pickup is being tracked and associated with this item; otherwise, <see langword="false"/>.
        /// </returns>
        /// <remarks>
        /// This method ensures that the provided pickup is being tracked by the <see cref="TrackerBase"/>
        /// and the tracked values associated with the pickup contain this item instance.
        /// </remarks>
        protected override bool Check(Pickup pickup)
        {
            bool isTracked = pickup is not null;
            isTracked = isTracked && serial == pickup.Serial;
            isTracked = isTracked && Tracker.IsTracked(pickup);
            isTracked = isTracked && Tracker.GetTrackedValues(pickup).Any(c => c.GetHashCode() == GetHashCode());
            return isTracked;
        }

        /// <inheritdoc/>
        protected override void PostInitialize()
        {
            base.PostInitialize();

            AdjustAdditivePipe();

            FixedTickRate = 1f;
            CanEverTick = true;
        }

        /// <inheritdoc/>
        protected override void OnBeginPlay()
        {
            base.OnBeginPlay();

            SubscribeEvents();

            serial = Owner.Serial;
        }

        /// <inheritdoc/>
        protected override void SubscribeEvents()
        {
            base.SubscribeEvents();

            Exiled.Events.Handlers.Player.PickingUpItem += OnInternalPickingUp;
            Exiled.Events.Handlers.Player.ItemAdded += OnInternalItemAdded;
            Exiled.Events.Handlers.Player.ItemRemoved += OnInternalItemRemoved;
            Exiled.Events.Handlers.Scp914.UpgradingPickup += OnInternalUpgradingPickup;
        }

        /// <inheritdoc/>
        protected override void UnsubscribeEvents()
        {
            base.UnsubscribeEvents();

            Exiled.Events.Handlers.Player.PickingUpItem -= OnInternalPickingUp;
            Exiled.Events.Handlers.Player.ItemAdded -= OnInternalItemAdded;
            Exiled.Events.Handlers.Player.ItemRemoved -= OnInternalItemRemoved;
            Exiled.Events.Handlers.Scp914.UpgradingPickup -= OnInternalUpgradingPickup;
        }

        /// <summary>
        /// Handles tracking pickups when they are picked up by a player.
        /// </summary>
        /// <param name="ev"><see cref="PickingUpItemEventArgs"/>.</param>
        protected virtual void OnPickingUp(PickingUpItemEventArgs ev) => PickingUpItemDispatcher.InvokeAll(ev);

        /// <summary>
        /// Called anytime the pickup enters a player's inventory by any means.
        /// </summary>
        /// <param name="player">The <see cref="Player"/> acquiring the pickup.</param>
        /// <param name="displayMessage">Whether the pickup hint should be displayed.</param>
        protected virtual void OnAcquired(Player player, bool displayMessage = true)
        {
            if (displayMessage)
                ShowPickedUpMessage(player);
        }

        /// <summary>
        /// Shows a message to the player upon picking up a custom item.
        /// </summary>
        /// <param name="player">The <see cref="Player"/> who will be shown the message.</param>
        protected virtual void ShowPickedUpMessage(Player player) => player.ShowTextDisplay(Settings.PickedUpText);

        private void OnInternalPickingUp(PickingUpItemEventArgs ev)
        {
            if (!Check(ev.Pickup))
                return;

            OnPickingUp(ev);
        }

        private void OnInternalItemAdded(ItemAddedEventArgs ev)
        {
            if (!Check(ev.Pickup))
                return;

            Owner = null;
            OnAcquired(ev.Player);
        }

        private void OnInternalItemRemoved(ItemRemovedEventArgs ev)
        {
            if (!Check(ev.Pickup))
                return;

            Owner = ev.Pickup;
        }

        private void OnInternalUpgradingPickup(UpgradingPickupEventArgs ev)
        {
            if (!Check(ev.Pickup))
                return;

            ev.IsAllowed = false;
            ev.OutputPosition = ev.Pickup.Position;
        }
    }
}