// -----------------------------------------------------------------------
// <copyright file="Item.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Handlers
{
    using Exiled.Events.EventArgs;
    using Exiled.Events.Features;

    /// <summary>
    /// Item related events.
    /// </summary>
    public static class Item
    {
        /// <summary>
        /// Gets or sets the event invoked before the durability of an item is changed.
        /// </summary>
        public static Event<ChangingDurabilityEventArgs> ChangingDurability { get; set; } = new();

        /// <summary>
        /// Gets or sets the event invoked before item attachments are changed.
        /// </summary>
        public static Event<ChangingAttachmentsEventArgs> ChangingAttachments { get; set; } = new();

        /// <summary>
        /// Gets or sets the event invoked before receiving a preference.
        /// </summary>
        public static Event<ReceivingPreferenceEventArgs> ReceivingPreference { get; set; } = new();

        /// <summary>
        /// Invoked before a keycard interacts with a door.
        /// </summary>
        public static event CustomEventHandler<KeycardInteractingEventArgs> KeycardInteracting;

        /// <summary>
        /// Called before the durability of an item is changed.
        /// </summary>
        /// <param name="ev">The <see cref="ChangingDurabilityEventArgs"/> instance.</param>
        public static void OnChangingDurability(ChangingDurabilityEventArgs ev) => ChangingDurability.InvokeSafely(ev);

        /// <summary>
        /// Called before item attachments are changed.
        /// </summary>
        /// <param name="ev">The <see cref="ChangingAttachmentsEventArgs"/> instance.</param>
        public static void OnChangingAttachments(ChangingAttachmentsEventArgs ev) => ChangingAttachments.InvokeSafely(ev);

        /// <summary>
        /// Called before receiving a preference.
        /// </summary>
        /// <param name="ev">The <see cref="ReceivingPreferenceEventArgs"/> instance.</param>
        public static void OnReceivingPreference(ReceivingPreferenceEventArgs ev) => ReceivingPreference.InvokeSafely(ev);

        /// <summary>
        /// Called before keycard interacts with a door.
        /// </summary>
        /// <param name="ev">The <see cref="KeycardInteractingEventArgs"/> instance.</param>
        public static void OnKeycardInteracting(KeycardInteractingEventArgs ev) => KeycardInteracting.InvokeSafely(ev);
    }
}
