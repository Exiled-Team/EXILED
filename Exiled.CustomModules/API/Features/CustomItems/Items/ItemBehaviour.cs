// -----------------------------------------------------------------------
// <copyright file="ItemBehaviour.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomModules.API.Features.CustomItems.Items
{
    using System.Linq;
    using System.Reflection;

    using Exiled.API.Enums;
    using Exiled.API.Extensions;
    using Exiled.API.Features;
    using Exiled.API.Features.Attributes;
    using Exiled.API.Features.Core;
    using Exiled.API.Features.Core.Behaviours;
    using Exiled.API.Features.Core.Interfaces;
    using Exiled.API.Features.DynamicEvents;
    using Exiled.API.Features.Items;
    using Exiled.API.Features.Pickups;
    using Exiled.CustomModules.Events.EventArgs.CustomItems;
    using Exiled.Events.EventArgs.Player;
    using Exiled.Events.EventArgs.Scp914;

    using MEC;

    using PlayerRoles;

    /// <summary>
    /// Represents the base class for custom item behaviors.
    /// </summary>
    /// <remarks>
    /// This class extends <see cref="EItemBehaviour"/> and implements <see cref="IItemBehaviour"/> and <see cref="IAdditiveSettings{T}"/>.
    /// <br/>It provides a foundation for creating custom behaviors associated with in-game items.
    /// </remarks>
    public abstract class ItemBehaviour : EItemBehaviour, IItemBehaviour, IAdditiveSettings<ItemSettings>
    {
        /// <summary>
        /// Gets or sets the <see cref="TDynamicEventDispatcher{T}"/> which handles all the delegates fired before owner of the item changes role.
        /// </summary>
        [DynamicEventDispatcher]
        public TDynamicEventDispatcher<OwnerChangingRoleEventArgs> OwnerChangingRoleDispatcher { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="TDynamicEventDispatcher{T}"/> which handles all the delegates fired before owner of the item dies.
        /// </summary>
        [DynamicEventDispatcher]
        public TDynamicEventDispatcher<OwnerDyingEventArgs> OwnerDyingDispatcher { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="TDynamicEventDispatcher{T}"/> which handles all the delegates fired before owner of the item escapes.
        /// </summary>
        [DynamicEventDispatcher]
        public TDynamicEventDispatcher<OwnerEscapingEventArgs> OwnerEscapingDispatcher { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="TDynamicEventDispatcher{T}"/> which handles all the delegates fired before owner of the item gets handcuffed.
        /// </summary>
        [DynamicEventDispatcher]
        public TDynamicEventDispatcher<OwnerHandcuffingEventArgs> OwnerHandcuffingDispatcher { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="TDynamicEventDispatcher{T}"/> which handles all the delegates fired before owner of the item drops it.
        /// </summary>
        [DynamicEventDispatcher]
        public TDynamicEventDispatcher<DroppingItemEventArgs> DroppingItemDispatcher { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="TDynamicEventDispatcher{T}"/> which handles all the delegates fired before owner of the item picks it up.
        /// </summary>
        [DynamicEventDispatcher]
        public TDynamicEventDispatcher<PickingUpItemEventArgs> PickingUpItemDispatcher { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="TDynamicEventDispatcher{T}"/> which handles all the delegates fired before owner of the item changes it.
        /// </summary>
        [DynamicEventDispatcher]
        public TDynamicEventDispatcher<ChangingItemEventArgs> ChangingItemDispatcher { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="TDynamicEventDispatcher{T}"/> which handles all the delegates fired before owner of the item upgrades it.
        /// </summary>
        [DynamicEventDispatcher]
        public TDynamicEventDispatcher<UpgradingEventArgs> UpgradingPickupDispatcher { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="TDynamicEventDispatcher{T}"/> which handles all the delegates fired before owner of the item upgrades it through his inventory.
        /// </summary>
        [DynamicEventDispatcher]
        public TDynamicEventDispatcher<UpgradingItemEventArgs> UpgradingItemDispatcher { get; set; }

        /// <inheritdoc/>
        public override bool DisposeOnNullOwner { get; protected set; } = false;

        /// <summary>
        /// Gets the relative <see cref="CustomItems.CustomItem"/>.
        /// </summary>
        public CustomItem CustomItem { get; private set; }

        /// <inheritdoc/>
        public ItemSettings Settings { get; set; }

        /// <summary>
        /// Gets or sets the item's configs.
        /// </summary>
        public virtual EConfig Config { get; set; }

        /// <summary>
        /// Gets the item's owner.
        /// </summary>
        public Player ItemOwner => Owner.Owner;

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

            if (CustomItem.TryGet(GetType(), out CustomItem customItem) && customItem.Settings is ItemSettings itemSettings)
            {
                CustomItem = customItem;
                Settings = itemSettings;
            }

            if (CustomItem is null || Settings is null)
            {
                Log.Error($"Custom item ({GetType().Name}) has invalid configuration.");
                Destroy();
            }
        }

        /// <summary>
        /// Checks if the specified owner is not null and matches the owner of the item.
        /// </summary>
        /// <param name="owner">The player who owns the item.</param>
        /// /// <returns><see langword="true"/> if the specified owner is not null and matches the owner of the item; otherwise, <see langword="false"/>.</returns>
        /// <remarks>
        /// This method verifies if the provided owner is not null and matches the owner of the item.
        /// <br/>It is typically used to ensure that the owner being checked is valid and corresponds to the expected owner for the current context.
        /// </remarks>
        protected virtual bool Check(Player owner) => owner && owner == ItemOwner;

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
        protected virtual bool Check(Pickup pickup) =>
            pickup && StaticActor.Get<ItemTracker>() is ItemTracker itemTracker &&
            itemTracker.IsTracked(pickup) && itemTracker.GetTrackedValues(pickup).Contains(this);

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

            Exiled.Events.Handlers.Player.Dying += OnInternalOwnerDying;
            Exiled.Events.Handlers.Player.DroppingItem += OnInternalDropping;
            Exiled.Events.Handlers.Player.ChangingItem += OnInternalChanging;
            Exiled.Events.Handlers.Player.Escaping += OnInternalOwnerEscaping;
            Exiled.Events.Handlers.Player.PickingUpItem += OnInternalPickingUp;
            Exiled.Events.Handlers.Player.ItemAdded += OnInternalItemAdded;
            Exiled.Events.Handlers.Scp914.UpgradingPickup += OnInternalUpgradingPickup;
            Exiled.Events.Handlers.Player.Handcuffing += OnInternalOwnerHandcuffing;
            Exiled.Events.Handlers.Player.ChangingRole += OnInternalOwnerChangingRole;
            Exiled.Events.Handlers.Scp914.UpgradingInventoryItem += OnInternalUpgradingInventoryItem;
        }

        /// <inheritdoc/>
        protected override void UnsubscribeEvents()
        {
            base.UnsubscribeEvents();

            Exiled.Events.Handlers.Player.Dying -= OnInternalOwnerDying;
            Exiled.Events.Handlers.Player.DroppingItem -= OnInternalDropping;
            Exiled.Events.Handlers.Player.ChangingItem -= OnInternalChanging;
            Exiled.Events.Handlers.Player.Escaping -= OnInternalOwnerEscaping;
            Exiled.Events.Handlers.Player.PickingUpItem -= OnInternalPickingUp;
            Exiled.Events.Handlers.Player.ItemAdded -= OnInternalItemAdded;
            Exiled.Events.Handlers.Scp914.UpgradingPickup -= OnInternalUpgradingPickup;
            Exiled.Events.Handlers.Player.Handcuffing -= OnInternalOwnerHandcuffing;
            Exiled.Events.Handlers.Player.ChangingRole -= OnInternalOwnerChangingRole;
            Exiled.Events.Handlers.Scp914.UpgradingInventoryItem -= OnInternalUpgradingInventoryItem;
        }

        /// <summary>
        /// Handles tracking items when they a player changes their role.
        /// </summary>
        /// <param name="ev"><see cref="OwnerChangingRoleEventArgs"/>.</param>
        protected virtual void OnOwnerChangingRole(OwnerChangingRoleEventArgs ev) => OwnerChangingRoleDispatcher.InvokeAll(ev);

        /// <summary>
        /// Handles making sure custom items are not "lost" when a player dies.
        /// </summary>
        /// <param name="ev"><see cref="OwnerDyingEventArgs"/>.</param>
        protected virtual void OnOwnerDying(OwnerDyingEventArgs ev) => OwnerDyingDispatcher.InvokeAll(ev);

        /// <summary>
        /// Handles making sure custom items are not "lost" when a player escapes.
        /// </summary>
        /// <param name="ev"><see cref="OwnerEscapingEventArgs"/>.</param>
        protected virtual void OnOwnerEscaping(OwnerEscapingEventArgs ev) => OwnerEscapingDispatcher.InvokeAll(ev);

        /// <summary>
        /// Handles making sure custom items are not "lost" when being handcuffed.
        /// </summary>
        /// <param name="ev"><see cref="OwnerHandcuffingEventArgs"/>.</param>
        protected virtual void OnOwnerHandcuffing(OwnerHandcuffingEventArgs ev) => OwnerHandcuffingDispatcher.InvokeAll(ev);

        /// <summary>
        /// Handles tracking items when they are dropped by a player.
        /// </summary>
        /// <param name="ev"><see cref="DroppingItemEventArgs"/>.</param>
        protected virtual void OnDropping(DroppingItemEventArgs ev) => DroppingItemDispatcher.InvokeAll(ev);

        /// <summary>
        /// Handles tracking items when they are picked up by a player.
        /// </summary>
        /// <param name="ev"><see cref="PickingUpItemEventArgs"/>.</param>
        protected virtual void OnPickingUp(PickingUpItemEventArgs ev) => PickingUpItemDispatcher.InvokeAll(ev);

        /// <summary>
        /// Handles tracking items when they are selected in the player's inventory.
        /// </summary>
        /// <param name="ev"><see cref="ChangingItemEventArgs"/>.</param>
        protected virtual void OnChangingItem(ChangingItemEventArgs ev)
        {
            if (Settings.ShouldMessageOnGban)
            {
                foreach (Player player in Player.Get(RoleTypeId.Spectator))
                {
                    Timing.CallDelayed(0.5f, () => player.SendFakeSyncVar(
                        ev.Player.NetworkIdentity, typeof(NicknameSync), nameof(NicknameSync.Network_displayName), $"{ev.Player.Nickname} (Custom Item: {Name})"));
                }
            }

            ShowSelectedMessage(ev.Player);
            ChangingItemDispatcher.InvokeAll(ev);
        }

        /// <summary>
        /// Prevents custom items from being affected by SCP-914.
        /// </summary>
        /// <param name="ev"><see cref="UpgradingEventArgs"/>.</param>
        protected virtual void OnUpgradingPickup(UpgradingEventArgs ev) => UpgradingPickupDispatcher.InvokeAll(ev);

        /// <inheritdoc cref="OnUpgradingPickup(UpgradingEventArgs)"/>
        protected virtual void OnUpgradingItem(UpgradingItemEventArgs ev) => UpgradingItemDispatcher.InvokeAll(ev);

        /// <summary>
        /// Called anytime the item enters a player's inventory by any means.
        /// </summary>
        /// <param name="player">The <see cref="Player"/> acquiring the item.</param>
        /// <param name="item">The <see cref="Item"/> being acquired.</param>
        /// <param name="displayMessage">Whether the pickup hint should be displayed.</param>
        protected virtual void OnAcquired(Player player, Item item, bool displayMessage = true)
        {
            if (displayMessage)
                ShowPickedUpMessage(player);
        }

        /// <summary>
        /// Shows a message to the player upon picking up a custom item.
        /// </summary>
        /// <param name="player">The <see cref="Player"/> who will be shown the message.</param>
        protected virtual void ShowPickedUpMessage(Player player) => player.ShowTextDisplay(Settings.PickedUpText);

        /// <summary>
        /// Shows a message to the player upon selecting a custom item.
        /// </summary>
        /// <param name="player">The <see cref="Player"/> who will be shown the message.</param>
        protected virtual void ShowSelectedMessage(Player player)
        {
            string text = Settings.SelectedText.Content.Replace("{item}", CustomItem.Name).Replace("{description}", CustomItem.Description);
            TextDisplay textDisplay = new(text, Settings.SelectedText.Duration, Settings.SelectedText.CanBeDisplayed, Settings.SelectedText.Channel);
            player.ShowTextDisplay(textDisplay);
        }

        private void OnInternalOwnerChangingRole(ChangingRoleEventArgs ev)
        {
            if (ev.Reason.Equals(SpawnReason.Escaped))
                return;

            foreach (Item item in ev.Player.Items.ToList())
            {
                if (!Check(item))
                    continue;

                OnOwnerChangingRole(new(item, CustomItem, this, ev));

                CustomItem.Spawn(ev.Player, item, ev.Player);
                ev.Player.RemoveItem(item);
            }

            MirrorExtensions.ResyncSyncVar(ev.Player.NetworkIdentity, typeof(NicknameSync), nameof(NicknameSync.Network_myNickSync));
        }

        private void OnInternalOwnerDying(DyingEventArgs ev)
        {
            if (ItemOwner is null || ev.Player != ItemOwner)
                return;

            foreach (Item item in ev.Player.Items.ToList())
            {
                if (!Check(item))
                    continue;

                OnOwnerDying(new(item, CustomItem, this, ev));

                if (!ev.IsAllowed)
                    continue;

                CustomItem.Spawn(ev.Player, item, ev.Player);
                ev.Player.RemoveItem(item);

                MirrorExtensions.ResyncSyncVar(ev.Player.NetworkIdentity, typeof(NicknameSync), nameof(NicknameSync.Network_myNickSync));
            }

            MirrorExtensions.ResyncSyncVar(ev.Player.NetworkIdentity, typeof(NicknameSync), nameof(NicknameSync.Network_myNickSync));
        }

        private void OnInternalOwnerEscaping(EscapingEventArgs ev)
        {
            if (ItemOwner is null || ev.Player != ItemOwner)
                return;

            foreach (Item item in ev.Player.Items.ToList())
            {
                if (!Check(item))
                    continue;

                OnOwnerEscaping(new(item, CustomItem, this, ev));

                if (!ev.IsAllowed)
                    continue;

                ev.Player.RemoveItem(item, false);
                Timing.CallDelayed(1.5f, () => CustomItem.Spawn(ev.Player.Position, item, null));

                MirrorExtensions.ResyncSyncVar(ev.Player.NetworkIdentity, typeof(NicknameSync), nameof(NicknameSync.Network_myNickSync));
            }

            MirrorExtensions.ResyncSyncVar(ev.Player.NetworkIdentity, typeof(NicknameSync), nameof(NicknameSync.Network_myNickSync));
        }

        private void OnInternalOwnerHandcuffing(HandcuffingEventArgs ev)
        {
            foreach (Item item in ev.Target.Items.ToList())
            {
                if (!Check(item))
                    continue;

                OnOwnerHandcuffing(new(item, CustomItem, this, ev));

                if (!ev.IsAllowed)
                    continue;

                CustomItem.Spawn(ev.Target, item, ev.Target);
                ev.Target.RemoveItem(item);
            }
        }

        private void OnInternalDropping(DroppingItemEventArgs ev)
        {
            if (!Check(ev.Item) || !Check(Owner))
                return;

            OnDropping(ev);
        }

        private void OnInternalPickingUp(PickingUpItemEventArgs ev)
        {
            if (!Check(ev.Pickup) || ev.Player.Items.Count >= 8)
                return;

            OnPickingUp(ev);
        }

        private void OnInternalItemAdded(ItemAddedEventArgs ev)
        {
            if (!Check(ev.Pickup))
                return;

            OnAcquired(ev.Player, ev.Item, true);
        }

        private void OnInternalChanging(ChangingItemEventArgs ev)
        {
            if (!Check(ev.Item))
            {
                MirrorExtensions.ResyncSyncVar(ev.Player.NetworkIdentity, typeof(NicknameSync), nameof(NicknameSync.Network_displayName));
                return;
            }

            OnChangingItem(ev);
        }

        private void OnInternalUpgradingInventoryItem(UpgradingInventoryItemEventArgs ev)
        {
            if (!Check(ev.Item))
                return;

            ev.IsAllowed = false;

            OnUpgradingItem(new(ev.Player, ev.Item.Base, CustomItem, this, ev.KnobSetting));
        }

        private void OnInternalUpgradingPickup(UpgradingPickupEventArgs ev)
        {
            if (!Check(ev.Pickup))
                return;

            ev.IsAllowed = false;

            Timing.CallDelayed(3.5f, () =>
            {
                ev.Pickup.Position = ev.OutputPosition;
                OnUpgradingPickup(new(ev.Pickup, CustomItem, this, ev.OutputPosition, ev.KnobSetting));
            });
        }
    }
}