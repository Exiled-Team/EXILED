// -----------------------------------------------------------------------
// <copyright file="Item.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Handlers
{
    using Exiled.Events.EventArgs.Item;

    using Extensions;

    using static Events;

    /// <summary>
    ///     Item related events.
    /// </summary>
    public static class Item
    {
        /// <summary>
        ///     Invoked before the ammo of an firearm are changed.
        /// </summary>
        public static event CustomEventHandler<ChangingAmmoEventArgs> ChangingAmmo;

        /// <summary>
        ///     Invoked before item attachments are changed.
        /// </summary>
        public static event CustomEventHandler<ChangingAttachmentsEventArgs> ChangingAttachments;

        /// <summary>
        ///     Invoked before receiving a preference.
        /// </summary>
        public static event CustomEventHandler<ReceivingPreferenceEventArgs> ReceivingPreference;

        /// <summary>
        /// Invoked before a keycard interacts with a door.
        /// </summary>
        public static event CustomEventHandler<KeycardInteractingEventArgs> KeycardInteracting;

        /// <summary>
        /// Invoked before a melee item is swung.
        /// </summary>
        public static event CustomEventHandler<SwingingEventArgs> Swinging;

        /// <summary>
        /// Invoked before a <see cref="API.Features.Items.Jailbird"/> is charged.
        /// </summary>
        public static event CustomEventHandler<ChargingJailbirdEventArgs> ChargingJailbird;

        /// <summary>
        /// Called before the ammo of an firearm is changed.
        /// </summary>
        /// <param name="ev">The <see cref="ChangingAmmoEventArgs"/> instance.</param>
        public static void OnChangingAmmo(ChangingAmmoEventArgs ev) => ChangingAmmo.InvokeSafely(ev);

        /// <summary>
        ///     Called before item attachments are changed.
        /// </summary>
        /// <param name="ev">The <see cref="ChangingAttachmentsEventArgs" /> instance.</param>
        public static void OnChangingAttachments(ChangingAttachmentsEventArgs ev) => ChangingAttachments.InvokeSafely(ev);

        /// <summary>
        ///     Called before receiving a preference.
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
    }
}