// -----------------------------------------------------------------------
// <copyright file="Item.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Handlers
{
#pragma warning disable SA1623 // Property summary documentation should match accessors

    using System.Collections.Generic;

    using Exiled.API.Features.Pickups;
    using Exiled.API.Structs;
    using Exiled.Events.EventArgs.Item;
    using Exiled.Events.Features;

    /// <summary>
    /// Item related events.
    /// </summary>
    public static class Item
    {
        /// <summary>
        /// Invoked before firearm ammo count is changed.
        /// </summary>
        public static Event<ChangingAmmoEventArgs> ChangingAmmo { get; set; } = new ();

        /// <summary>
        /// Invoked before firearm attachments are changed.
        /// </summary>
        public static Event<ChangingAttachmentsEventArgs> ChangingAttachments { get; set; } = new();

        /// <summary>
        /// Invoked after firearm attachments were changed.
        /// </summary>
        public static Event<ChangedAttachmentsEventArgs> ChangedAttachments { get; set; } = new();

        /// <summary>
        /// Invoked before receiving firearm attachment preferences from a player.
        /// Preferences are used as a default value when player receives a newly created firearm, for example, when spawning.
        /// </summary>
        public static Event<ReceivingPreferenceEventArgs> ReceivingPreference { get; set; } = new();

        /// <summary>
        /// Invoked before a thrown <see cref="KeycardPickup"/> interacts with a door.
        /// </summary>
        public static Event<KeycardInteractingEventArgs> KeycardInteracting { get; set; } = new();

        /// <summary>
        /// Invoked before a melee item is swung.
        /// </summary>
        public static Event<SwingingEventArgs> Swinging { get; set; } = new();

        /// <summary>
        /// Invoked before a <see cref="API.Features.Items.Jailbird"/> charged attack.
        /// </summary>
        public static Event<ChargingJailbirdEventArgs> ChargingJailbird { get; set; } = new();

        /// <summary>
        /// Invoked before draining a <see cref="RadioPickup"/>'s battery.
        /// </summary>
        public static Event<UsingRadioPickupBatteryEventArgs> UsingRadioPickupBattery { get; set; } = new();

        /// <summary>
        /// Invoked before a jailbird breaks.
        /// </summary>
        public static Event<BreakingJailbirdEventArgs> BreakingJailbird { get; set; } = new();

        /// <summary>
        /// Called before firearm ammo count is changed.
        /// </summary>
        /// <param name="ev">The <see cref="ChangingAmmoEventArgs"/> instance.</param>
        public static void OnChangingAmmo(ChangingAmmoEventArgs ev) => ChangingAmmo.InvokeSafely(ev);

        /// <summary>
        /// Called before firearm attachments are changed.
        /// </summary>
        /// <param name="ev">The <see cref="ChangingAttachmentsEventArgs" /> instance.</param>
        public static void OnChangingAttachments(ChangingAttachmentsEventArgs ev) => ChangingAttachments.InvokeSafely(ev);

        /// <summary>
        /// Invoked after firearm attachments were changed.
        /// </summary>
        /// <param name="ev">The <see cref="ChangingAttachmentsEventArgs" /> instance.</param>
        public static void OnChangedAttachments(ChangedAttachmentsEventArgs ev) => ChangedAttachments.InvokeSafely(ev);

        /// <summary>
        /// Invoked before receiving firearm attachment preferences from a player.
        /// Preferences are used as a default value when player receives a newly created firearm, for example, when spawning.
        /// </summary>
        /// <param name="ev">The <see cref="ReceivingPreferenceEventArgs" /> instance.</param>
        public static void OnReceivingPreference(ReceivingPreferenceEventArgs ev) => ReceivingPreference.InvokeSafely(ev);

        /// <summary>
        /// Invoked before a thrown <see cref="KeycardPickup"/> interacts with a door.
        /// </summary>
        /// <param name="ev">The <see cref="KeycardInteractingEventArgs"/> instance.</param>
        public static void OnKeycardInteracting(KeycardInteractingEventArgs ev) => KeycardInteracting.InvokeSafely(ev);

        /// <summary>
        /// Called before a melee item is swung.
        /// </summary>
        /// <param name="ev">The <see cref="SwingingEventArgs"/> instance.</param>
        public static void OnSwinging(SwingingEventArgs ev) => Swinging.InvokeSafely(ev);

        /// <summary>
        /// Invoked before a <see cref="API.Features.Items.Jailbird"/> charged attack.
        /// </summary>
        /// <param name="ev">The <see cref="ChargingJailbirdEventArgs"/> instance.</param>
        public static void OnChargingJailbird(ChargingJailbirdEventArgs ev) => ChargingJailbird.InvokeSafely(ev);

        /// <summary>
        /// Invoked before draining a <see cref="RadioPickup"/>'s battery.
        /// </summary>
        /// <param name="ev">The <see cref="UsingRadioPickupBatteryEventArgs"/> instance.</param>
        public static void OnUsingRadioPickupBattery(UsingRadioPickupBatteryEventArgs ev) => UsingRadioPickupBattery.InvokeSafely(ev);

        /// <summary>
        /// Invoked before a jailbird breaks.
        /// </summary>
        /// <param name="ev">The <see cref="BreakingJailbirdEventArgs"/> instance.</param>
        public static void OnBreakingJailbird(BreakingJailbirdEventArgs ev) => BreakingJailbird.InvokeSafely(ev);
    }
}