// -----------------------------------------------------------------------
// <copyright file="Item.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Handlers
{
    using Exiled.Events.EventArgs.Item;
    using Exiled.Events.Extensions;

    using static Events;

    /// <summary>
    ///     Item related events.
    /// </summary>
    public static class Item
    {
        /// <summary>
        ///     Invoked before the durability of an item is changed.
        /// </summary>
        public static event CustomEventHandler<ChangingDurabilityEventArgs> ChangingDurability;

        /// <summary>
        ///     Invoked before item attachments are changed.
        /// </summary>
        public static event CustomEventHandler<ChangingAttachmentsEventArgs> ChangingAttachments;

        /// <summary>
        ///     Invoked before receiving a preference.
        /// </summary>
        public static event CustomEventHandler<ReceivingPreferenceEventArgs> ReceivingPreference;

        /// <summary>
        ///     Invoked before using an <see cref="API.Features.Items.Item" />.
        /// </summary>
        public static event CustomEventHandler<UsingItemEventArgs> UsingItem;


        /// <summary>
        ///     Invoked after a <see cref="API.Features.Player" /> uses an <see cref="API.Features.Items.Item" />.
        /// </summary>
        /// <remarks>
        ///     Invoked after <see cref="UsedItem" />, if a player's class has
        ///     changed during their health increase, won't fire.
        /// </remarks>
        public static event CustomEventHandler<UsedItemEventArgs> UsedItem;

        /// <summary>
        ///     Invoked after a <see cref="API.Features.Player" /> has stopped the use of a
        ///     <see cref="API.Features.Items.Usable" />.
        /// </summary>
        public static event CustomEventHandler<CancellingItemUseEventArgs> CancellingItemUse;


        /// <summary>
        ///     Invoked before throwing an <see cref="API.Features.Items.Item" />.
        /// </summary>
        public static event CustomEventHandler<ThrowingItemEventArgs> ThrowingItem;

        /// <summary>
        ///     Invoked before dropping an <see cref="API.Features.Items.Item" />.
        /// </summary>
        public static event CustomEventHandler<DroppingItemEventArgs> DroppingItem;

        /// <summary>
        ///     Invoked before dropping a null <see cref="API.Features.Items.Item" />.
        /// </summary>
        public static event CustomEventHandler<DroppingNothingEventArgs> DroppingNull;

        /// <summary>
        ///     Invoked before picking up ammo.
        /// </summary>
        public static event CustomEventHandler<PickingUpAmmoEventArgs> PickingUpAmmo;

        /// <summary>
        ///     Invoked before picking up armor.
        /// </summary>
        public static event CustomEventHandler<PickingUpArmorEventArgs> PickingUpArmor;

        /// <summary>
        ///     Invoked after a <see cref="API.Features.Player" /> gets shot.
        /// </summary>
        public static event CustomEventHandler<ShotEventArgs> Shot;

        /// <summary>
        ///     Invoked before a <see cref="API.Features.Player" /> shoots a weapon.
        /// </summary>
        public static event CustomEventHandler<ShootingEventArgs> Shooting;

        /// <summary>
        ///     Invoked before picking up an <see cref="API.Features.Items.Item" />.
        /// </summary>
        public static event CustomEventHandler<PickingUpItemEventArgs> PickingUpItem;


        /// <summary>
        ///     Invoked before a user's radio battery charge is changed.
        /// </summary>
        public static event CustomEventHandler<UsingRadioBatteryEventArgs> UsingRadioBattery;

        /// <summary>
        ///     Invoked before a user's radio preset is changed.
        /// </summary>
        public static event CustomEventHandler<ChangingRadioPresetEventArgs> ChangingRadioPreset;

        /// <summary>
        ///     Invoked before a <see cref="API.Features.Player" /> MicroHID state is changed.
        /// </summary>
        public static event CustomEventHandler<ChangingMicroHIDStateEventArgs> ChangingMicroHIDState;

        /// <summary>
        ///     Invoked before a <see cref="API.Features.Player" /> MicroHID energy is changed.
        /// </summary>
        public static event CustomEventHandler<UsingMicroHIDEnergyEventArgs> UsingMicroHIDEnergy;

        /// <summary>
        ///     Called before processing a hotkey.
        /// </summary>
        public static event CustomEventHandler<ProcessingHotkeyEventArgs> ProcessingHotkey;

        /// <summary>
        ///     Invoked before dropping ammo.
        /// </summary>
        public static event CustomEventHandler<DroppingAmmoEventArgs> DroppingAmmo;

        /// <summary>
        ///     Invoked before a <see cref="API.Features.Player" /> flips a coin.
        /// </summary>
        public static event CustomEventHandler<FlippingCoinEventArgs> FlippingCoin;

        /// <summary>
        ///     Invoked before a <see cref="API.Features.Player" /> toggles the flashlight.
        /// </summary>
        public static event CustomEventHandler<TogglingFlashlightEventArgs> TogglingFlashlight;

        /// <summary>
        ///     Invoked before a <see cref="API.Features.Player" /> toggles the weapon's flashlight.
        /// </summary>
        public static event CustomEventHandler<TogglingWeaponFlashlightEventArgs> TogglingWeaponFlashlight;

        /// <summary>
        ///     Invoked before a <see cref="API.Features.Player" /> unloads a weapon.
        /// </summary>
        public static event CustomEventHandler<UnloadingWeaponEventArgs> UnloadingWeapon;

        /// <summary>
        ///     Invoked before a <see cref="API.Features.Player" /> dryfires a weapon.
        /// </summary>
        public static event CustomEventHandler<DryfiringWeaponEventArgs> DryfiringWeapon;

        /// <summary>
        ///     Invoked before a <see cref="API.Features.Player" /> triggers an aim action.
        /// </summary>
        public static event CustomEventHandler<AimingDownSightEventArgs> AimingDownSight;

        /// <summary>
        ///     Invoked before a <see cref="API.Features.Player" /> held <see cref="API.Features.Items.Item" /> changes.
        /// </summary>
        public static event CustomEventHandler<ChangingItemEventArgs> ChangingItem;

        /// <summary>
        ///     Invoked before a <see cref="API.Features.Player" /> searches a Pickup.
        /// </summary>
        public static event CustomEventHandler<SearchingPickupEventArgs> SearchingPickup;

        /// <summary>
        ///     Invoked after a <see cref="T:Exiled.API.Features.Player" /> has an item added to their inventory.
        /// </summary>
        public static event CustomEventHandler<ItemAddedEventArgs> ItemAdded;

        /// <summary>
        ///     Called before the durability of an item is changed.
        /// </summary>
        /// <param name="ev">The <see cref="ChangingDurabilityEventArgs" /> instance.</param>
        public static void OnChangingDurability(ChangingDurabilityEventArgs ev)
        {
            ChangingDurability.InvokeSafely(ev);
        }

        /// <summary>
        ///     Called before item attachments are changed.
        /// </summary>
        /// <param name="ev">The <see cref="ChangingAttachmentsEventArgs" /> instance.</param>
        public static void OnChangingAttachments(ChangingAttachmentsEventArgs ev)
        {
            ChangingAttachments.InvokeSafely(ev);
        }

        /// <summary>
        ///     Called before receiving a preference.
        /// </summary>
        /// <param name="ev">The <see cref="ReceivingPreferenceEventArgs" /> instance.</param>
        public static void OnReceivingPreference(ReceivingPreferenceEventArgs ev)
        {
            ReceivingPreference.InvokeSafely(ev);
        }

        /// <summary>
        ///     Called after a <see cref="API.Features.Player" /> used a medical item.
        /// </summary>
        /// <param name="ev">The <see cref="UsedItemEventArgs" /> instance.</param>
        public static void OnUsedItem(UsedItemEventArgs ev)
        {
            UsedItem.InvokeSafely(ev);
        }

        /// <summary>
        ///     Called after a <see cref="API.Features.Player" /> has stopped the use of a medical item.
        /// </summary>
        /// <param name="ev">The <see cref="CancellingItemUseEventArgs" /> instance.</param>
        public static void OnCancellingItemUse(CancellingItemUseEventArgs ev)
        {
            CancellingItemUse.InvokeSafely(ev);
        }

        /// <summary>
        ///     Called before using a usable item.
        /// </summary>
        /// <param name="ev">The <see cref="UsingItemEventArgs" /> instance.</param>
        public static void OnUsingItem(UsingItemEventArgs ev)
        {
            UsingItem.InvokeSafely(ev);
        }

        /// <summary>
        ///     Called before throwing a grenade.
        /// </summary>
        /// <param name="ev">The <see cref="ThrowingItemEventArgs" /> instance.</param>
        public static void OnThrowingItem(ThrowingItemEventArgs ev)
        {
            ThrowingItem.InvokeSafely(ev);
        }

        /// <summary>
        ///     Called before dropping an item.
        /// </summary>
        /// <param name="ev">The <see cref="DroppingItemEventArgs" /> instance.</param>
        public static void OnDroppingItem(DroppingItemEventArgs ev)
        {
            DroppingItem.InvokeSafely(ev);
        }

        /// <summary>
        ///     Called before dropping a null item.
        /// </summary>
        /// <param name="ev">The <see cref="DroppingNothingEventArgs" /> instance.</param>
        public static void OnDroppingNull(DroppingNothingEventArgs ev)
        {
            DroppingNull.InvokeSafely(ev);
        }

        /// <summary>
        ///     Called before a <see cref="API.Features.Player" /> picks up ammo.
        /// </summary>
        /// <param name="ev">The <see cref="PickingUpAmmoEventArgs" /> instance.</param>
        public static void OnPickingUpAmmo(PickingUpAmmoEventArgs ev)
        {
            PickingUpAmmo.InvokeSafely(ev);
        }

        /// <summary>
        ///     Called before a <see cref="API.Features.Player" /> picks up armor.
        /// </summary>
        /// <param name="ev">The <see cref="PickingUpArmorEventArgs" /> instance.</param>
        public static void OnPickingUpArmor(PickingUpArmorEventArgs ev)
        {
            PickingUpArmor.InvokeSafely(ev);
        }

        /// <summary>
        ///     Called before a <see cref="API.Features.Player" /> picks up an item.
        /// </summary>
        /// <param name="ev">The <see cref="PickingUpItemEventArgs" /> instance.</param>
        public static void OnPickingUpItem(PickingUpItemEventArgs ev)
        {
            PickingUpItem.InvokeSafely(ev);
        }

        /// <summary>
        ///     Called after a <see cref="API.Features.Player" /> shoots a weapon.
        /// </summary>
        /// <param name="ev">The <see cref="ShotEventArgs" /> instance.</param>
        public static void OnShot(ShotEventArgs ev)
        {
            Shot.InvokeSafely(ev);
        }

        /// <summary>
        ///     Called before a <see cref="API.Features.Player" /> shoots a weapon.
        /// </summary>
        /// <param name="ev">The <see cref="ShootingEventArgs" /> instance.</param>
        public static void OnShooting(ShootingEventArgs ev)
        {
            Shooting.InvokeSafely(ev);
        }

        /// <summary>
        ///     Called before a user's radio battery charge is changed.
        /// </summary>
        /// <param name="ev">The <see cref="UsingRadioBatteryEventArgs" /> instance.</param>
        public static void OnUsingRadioBattery(UsingRadioBatteryEventArgs ev)
        {
            UsingRadioBattery.InvokeSafely(ev);
        }

        /// <summary>
        ///     Called before a user's radio preset is changed.
        /// </summary>
        /// <param name="ev">The <see cref="ChangingRadioPresetEventArgs" /> instance.</param>
        public static void OnChangingRadioPreset(ChangingRadioPresetEventArgs ev)
        {
            ChangingRadioPreset.InvokeSafely(ev);
        }

        /// <summary>
        ///     Called before a <see cref="API.Features.Player" /> MicroHID state is changed.
        /// </summary>
        /// <param name="ev">The <see cref="ChangingRadioPresetEventArgs" /> instance.</param>
        public static void OnChangingMicroHIDState(ChangingMicroHIDStateEventArgs ev)
        {
            ChangingMicroHIDState.InvokeSafely(ev);
        }

        /// <summary>
        ///     Called before a <see cref="API.Features.Player" /> MicroHID energy is changed.
        /// </summary>
        /// <param name="ev">The <see cref="UsingMicroHIDEnergyEventArgs" /> instance.</param>
        public static void OnUsingMicroHIDEnergy(UsingMicroHIDEnergyEventArgs ev)
        {
            UsingMicroHIDEnergy.InvokeSafely(ev);
        }

        /// <summary>
        ///     Called before processing a hotkey.
        /// </summary>
        /// <param name="ev">The <see cref="ProcessingHotkeyEventArgs" /> instance.</param>
        public static void OnProcessingHotkey(ProcessingHotkeyEventArgs ev)
        {
            ProcessingHotkey.InvokeSafely(ev);
        }

        /// <summary>
        ///     Called before dropping ammo.
        /// </summary>
        /// <param name="ev">The <see cref="DroppingAmmoEventArgs" /> instance.</param>
        public static void OnDroppingAmmo(DroppingAmmoEventArgs ev)
        {
            DroppingAmmo.InvokeSafely(ev);
        }

        /// <summary>
        ///     Called before a <see cref="API.Features.Player" /> flips a coin.
        /// </summary>
        /// <param name="ev">The <see cref="FlippingCoinEventArgs" /> instance.</param>
        public static void OnFlippingCoin(FlippingCoinEventArgs ev)
        {
            FlippingCoin.InvokeSafely(ev);
        }

        /// <summary>
        ///     Called before a <see cref="API.Features.Player" /> held item changes.
        /// </summary>
        /// <param name="ev">The <see cref="ChangingItemEventArgs" /> instance.</param>
        public static void OnChangingItem(ChangingItemEventArgs ev)
        {
            ChangingItem.InvokeSafely(ev);
        }

        /// <summary>
        ///     Called before a <see cref="API.Features.Player" /> toggles the flashlight.
        /// </summary>
        /// <param name="ev">The <see cref="TogglingFlashlightEventArgs" /> instance.</param>
        public static void OnTogglingFlashlight(TogglingFlashlightEventArgs ev)
        {
            TogglingFlashlight.InvokeSafely(ev);
        }

        /// <summary>
        ///     Called before a <see cref="API.Features.Player" /> unloads a weapon.
        /// </summary>
        /// <param name="ev">The <see cref="UnloadingWeaponEventArgs" /> instance.</param>
        public static void OnUnloadingWeapon(UnloadingWeaponEventArgs ev)
        {
            UnloadingWeapon.InvokeSafely(ev);
        }

        /// <summary>
        ///     Called before a <see cref="API.Features.Player" /> triggers an aim action.
        /// </summary>
        /// <param name="ev">The <see cref="AimingDownSightEventArgs" /> instance.</param>
        public static void OnAimingDownSight(AimingDownSightEventArgs ev)
        {
            AimingDownSight.InvokeSafely(ev);
        }

        /// <summary>
        ///     Called before a <see cref="API.Features.Player" /> toggles the weapon's flashlight.
        /// </summary>
        /// <param name="ev">The <see cref="TogglingWeaponFlashlightEventArgs" /> instance.</param>
        public static void OnTogglingWeaponFlashlight(TogglingWeaponFlashlightEventArgs ev)
        {
            TogglingWeaponFlashlight.InvokeSafely(ev);
        }

        /// <summary>
        ///     Called before a <see cref="API.Features.Player" /> dryfires a weapon.
        /// </summary>
        /// <param name="ev">The <see cref="DryfiringWeaponEventArgs" /> instance.</param>
        public static void OnDryfiringWeapon(DryfiringWeaponEventArgs ev)
        {
            DryfiringWeapon.InvokeSafely(ev);
        }

        /// <summary>
        ///     Called before a <see cref="API.Features.Player" /> searches a Pickup.
        /// </summary>
        /// <param name="ev">The <see cref="SearchingPickupEventArgs" /> instance.</param>
        public static void OnSearchPickupRequest(SearchingPickupEventArgs ev)
        {
            SearchingPickup.InvokeSafely(ev);
        }

        /// <summary>
        /// Called after a <see cref="T:Exiled.API.Features.Player" /> has an item added to their inventory.
        /// </summary>
        /// <param name="inventory">The <see cref="InventorySystem.Inventory"/> the item was added to.</param>
        /// <param name="itemBase">The added <see cref="InventorySystem.Items.ItemBase"/>.</param>
        /// <param name="pickupBase">The <see cref="InventorySystem.Items.Pickups.ItemPickupBase"/> the <see cref="InventorySystem.Items.ItemBase"/> originated from, or <see langword="null"/> if the item was not picked up.</param>
        public static void OnItemAdded(InventorySystem.Inventory inventory, InventorySystem.Items.ItemBase itemBase, InventorySystem.Items.Pickups.ItemPickupBase pickupBase) => ItemAdded.InvokeSafely(new ItemAddedEventArgs(inventory, itemBase, pickupBase));
    }
}
