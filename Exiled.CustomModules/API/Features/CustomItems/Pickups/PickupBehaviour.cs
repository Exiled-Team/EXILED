// -----------------------------------------------------------------------
// <copyright file="PickupBehaviour.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomModules.API.Features.CustomItems.Pickups
{
    using System;
    using System.Linq;
    using System.Reflection;

    using Exiled.API.Features;
    using Exiled.API.Features.Attributes;
    using Exiled.API.Features.Core;
    using Exiled.API.Features.Core.Behaviours;
    using Exiled.API.Features.Core.Interfaces;
    using Exiled.API.Features.DynamicEvents;
    using Exiled.API.Features.Pickups;
    using Exiled.CustomModules.API.Features.CustomItems.Items;
    using Exiled.Events.EventArgs.Player;
    using Exiled.Events.EventArgs.Scp914;

    /// <summary>
    /// Represents the base class for custom pickup behaviors.
    /// </summary>
    /// <remarks>
    /// This class extends <see cref="EPickupBehaviour"/> and implements <see cref="IPickupBehaviour"/> and <see cref="IAdditiveSettings{T}"/>.
    /// <br/>It provides a foundation for creating custom behaviors associated with in-game pickup.
    /// </remarks>
    public abstract class PickupBehaviour : EPickupBehaviour, IPickupBehaviour, IAdditiveSettings<PickupSettings>
    {
        /// <summary>
        /// Gets or sets the <see cref="TDynamicEventDispatcher{T}"/> which handles all the delegates fired before the pickup is gets picked up.
        /// </summary>
        [DynamicEventDispatcher]
        public TDynamicEventDispatcher<PickingUpItemEventArgs> PickingUpItemDispatcher { get; set; }

        /// <inheritdoc/>
        public override bool DisposeOnNullOwner { get; protected set; } = false;

        /// <summary>
        /// Gets the relative <see cref="CustomItems.CustomItem"/>.
        /// </summary>
        public CustomItem CustomItem { get; private set; }

        /// <inheritdoc/>
        public PickupSettings Settings { get; set; }

        /// <summary>
        /// Gets or sets the item's configs.
        /// </summary>
        public virtual EConfig Config { get; set; }

        /// <inheritdoc/>
        public virtual void AdjustAdditivePipe()
        {
            if (Config is not null)
            {
                foreach (PropertyInfo propertyInfo in Config.GetType().GetProperties())
                {
                    PropertyInfo targetInfo = Config.GetType().GetProperty(propertyInfo.Name);
                    targetInfo?.SetValue(Settings, propertyInfo.GetValue(Config, null));
                }
            }

            if (CustomItem.TryGet(GetType(), out CustomItem customItem) && customItem.Settings is PickupSettings pickupSettings)
            {
                CustomItem = customItem;
                Settings = pickupSettings;
            }

            if (CustomItem is null || Settings is null)
            {
                Log.Error($"Custom pickup ({GetType().Name}) has invalid configuration.");
                Destroy();
            }
        }

        /// <summary>
        /// Checks if the specified pickup is being tracked and associated with this item.
        /// </summary>
        /// <param name="pickup">The <see cref="Pickup"/> to check.</param>
        /// <returns>
        /// <see langword="true"/> if the specified pickup is being tracked and associated with this item; otherwise, <see langword="false"/>.
        /// </returns>
        /// <remarks>
        /// This method ensures that the provided pickup is being tracked by the <see cref="ItemTracker"/>
        /// and the tracked values associated with the pickup contain this item instance.
        /// </remarks>
        protected override bool Check(Pickup pickup) =>
            base.Check(pickup) && StaticActor.Get<PickupTracker>() is PickupTracker pickupTracker &&
            pickupTracker.IsTracked(pickup) && pickupTracker.GetTrackedValues(pickup).Contains(this);

        /// <inheritdoc/>
        protected override void PostInitialize()
        {
            base.PostInitialize();

            AdjustAdditivePipe();
        }

        /// <inheritdoc/>
        protected override void SubscribeEvents()
        {
            base.SubscribeEvents();

            Exiled.Events.Handlers.Player.PickingUpItem += OnInternalPickingUp;
            Exiled.Events.Handlers.Player.AddingItem += OnInternalAddingItem;
            Exiled.Events.Handlers.Scp914.UpgradingPickup += OnInternalUpgradingPickup;
        }

        /// <inheritdoc/>
        protected override void UnsubscribeEvents()
        {
            base.UnsubscribeEvents();

            Exiled.Events.Handlers.Player.PickingUpItem -= OnInternalPickingUp;
            Exiled.Events.Handlers.Player.AddingItem -= OnInternalAddingItem;
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
            if (!Check(ev.Pickup) || ev.Player.Items.Count >= 8)
                return;

            OnPickingUp(ev);

            if (!ev.IsAllowed)
                return;
        }

        private void OnInternalAddingItem(AddingItemEventArgs ev)
        {
            if (!Check(ev.Pickup))
                return;

            ev.IsAllowed = false;
            OnAcquired(ev.Player, true);
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