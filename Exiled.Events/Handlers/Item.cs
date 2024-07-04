// -----------------------------------------------------------------------
// <copyright file="Item.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Handlers
{
#pragma warning disable SA1623 // Property summary documentation should match accessors

    using Exiled.Events.EventArgs.Item;

    using Exiled.Events.Features;

    /// <summary>
    /// Item related events.
    /// </summary>
    public static class Item
    {
        /// <summary>
        /// Invoked before the ammo of an firearm are changed.
        /// </summary>
        public static Event<ChangingAmmoEventArgs> ChangingAmmo { get; set; } = new ();

        /// <summary>
        /// Invoked before item attachments are changed.
        /// </summary>
        public static Event<ChangingAttachmentsEventArgs> ChangingAttachments { get; set; } = new();

        /// <summary>
        /// Invoked after item attachments are changed.
        /// </summary>
        public static Event<ChangedAttachmentsEventArgs> ChangedAttachments { get; set; } = new();

        /// <summary>
        /// Invoked before receiving a preference.
        /// </summary>
        public static Event<ReceivingPreferenceEventArgs> ReceivingPreference { get; set; } = new();

        /// <summary>
        /// Invoked before a keycard interacts with a door.
        /// </summary>
        public static Event<KeycardInteractingEventArgs> KeycardInteracting { get; set; } = new();

        /// <summary>
        /// Invoked before a melee item is swung.
        /// </summary>
        public static Event<SwingingEventArgs> Swinging { get; set; } = new();

        /// <summary>
        /// Invoked before a <see cref="API.Features.Items.Jailbird"/> is charged.
        /// </summary>
        public static Event<ChargingJailbirdEventArgs> ChargingJailbird { get; set; } = new();

        /// <summary>
        /// Invoked before a radio pickup is draining battery.
        /// </summary>
        public static Event<UsingRadioPickupBatteryEventArgs> UsingRadioPickupBattery { get; set; } = new();

        /// <summary>
        /// Invoked before jailbird breaks.
        /// </summary>
        public static Event<BreakingJailbirdEventArgs> BreakingJailbird { get; set; } = new();

        /// <summary>
        /// Called before the ammo of an firearm is changed.
        /// </summary>
        /// <param name="ev">The <see cref="ChangingAmmoEventArgs"/> instance.</param>
        public static void OnChangingAmmo(ChangingAmmoEventArgs ev) => ChangingAmmo.InvokeSafely(ev);

        /// <summary>
        /// Called before item attachments are changed.
        /// </summary>
        /// <param name="ev">The <see cref="ChangingAttachmentsEventArgs" /> instance.</param>
        public static void OnChangingAttachments(ChangingAttachmentsEventArgs ev) => ChangingAttachments.InvokeSafely(ev);

        /// <summary>
        /// Called after item attachments are changed.
        /// </summary>
        /// <param name="ev">The <see cref="ChangingAttachmentsEventArgs" /> instance.</param>
        public static void OnChangedAttachments(ChangedAttachmentsEventArgs ev) => ChangedAttachments.InvokeSafely(ev);

        /// <summary>
        /// Called before receiving a preference.
        /// </summary>
        /// <param name="ev">The <see cref="ReceivingPreferenceEventArgs" /> instance.</param>
        public static void OnReceivingPreference(ReceivingPreferenceEventArgs ev) => ReceivingPreference.InvokeSafely(ev);

        /// <summary>
        /// Called before keycard interacts with a door.
        /// </summary>
        /// <param name="ev">The <see cref="KeycardInteractingEventArgs"/> instance.</param>
        public static void OnKeycardInteracting(KeycardInteractingEventArgs ev) => KeycardInteracting.InvokeSafely(ev);

        /// <summary>
        /// Called before a melee item is swung.
        /// </summary>
        /// <param name="ev">The <see cref="SwingingEventArgs"/> instance.</param>
        public static void OnSwinging(SwingingEventArgs ev) => Swinging.InvokeSafely(ev);

        /// <summary>
        /// Called before a <see cref="API.Features.Items.Jailbird"/> is charged.
        /// </summary>
        /// <param name="ev">The <see cref="ChargingJailbirdEventArgs"/> instance.</param>
        public static void OnChargingJailbird(ChargingJailbirdEventArgs ev) => ChargingJailbird.InvokeSafely(ev);

        /// <summary>
        /// Called before radio pickup is draining battery.
        /// </summary>
        /// <param name="ev">The <see cref="UsingRadioPickupBatteryEventArgs"/> instance.</param>
        public static void OnUsingRadioPickupBattery(UsingRadioPickupBatteryEventArgs ev) => UsingRadioPickupBattery.InvokeSafely(ev);

        /// <summary>
        /// Called before jailbird breaks.
        /// </summary>
        /// <param name="ev">The <see cref="BreakingJailbirdEventArgs"/> instance.</param>
        public static void OnBreakingJailbird(BreakingJailbirdEventArgs ev) => BreakingJailbird.InvokeSafely(ev);
    }
}