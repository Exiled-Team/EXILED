// -----------------------------------------------------------------------
// <copyright file="Item.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------


namespace Exiled.Events.Handlers
{
    using Exiled.API.Events;

    using Exiled.Events.EventArgs;

    using Exiled.Events.Extensions;

    using static Exiled.Events.Events;

    /// <summary>
    /// Item related events.
    /// </summary>
    public static class Item
    {
        /// <summary>
        /// Invoked before the durability of an item is changed.
        /// </summary>
        public static event CustomEventHandler<ChangingDurabilityEventArgs> ChangingDurability;

        /// <summary>
        /// Invoked before item attachments are changed.
        /// </summary>
        public static event CustomEventHandler<ChangingAttachmentsEventArgs> ChangingAttachments;

        /// <summary>
        /// Invoked before receiving a preference.
        /// </summary>
        public static event CustomEventHandler<ReceivingPreferenceEventArgs> ReceivingPreference;

        /// <summary>
        /// Called before the durability of an item is changed.
        /// </summary>
        /// <param name="ev">The <see cref="ChangingDurabilityEventArgs"/> instance.</param>
        public static void OnChangingDurability(ChangingDurabilityEventArgs ev) => EventManager.Instance.Invoke<ChangingDurabilityEventArgs>(ev);

        /// <summary>
        /// Automatically generated for backwards compatibility.
        /// </summary>
        /// <param name="ev">Input from the event system.</param>
        [Subscribe]
        public static void BackwardsCompatOnChangingDurability(ChangingDurabilityEventArgs ev) => ChangingDurability.InvokeSafely(ev);


        /// <summary>
        /// Called before item attachments are changed.
        /// </summary>
        /// <param name="ev">The <see cref="ChangingAttachmentsEventArgs"/> instance.</param>
        public static void OnChangingAttachments(ChangingAttachmentsEventArgs ev) => EventManager.Instance.Invoke<ChangingAttachmentsEventArgs>(ev);

        /// <summary>
        /// Automatically generated for backwards compatibility.
        /// </summary>
        /// <param name="ev">Input from the event system.</param>
        [Subscribe]
        public static void BackwardsCompatOnChangingAttachments(ChangingAttachmentsEventArgs ev) => ChangingAttachments.InvokeSafely(ev);

        /// <summary>
        /// Called before receiving a preference.
        /// </summary>
        /// <param name="ev">The <see cref="ReceivingPreferenceEventArgs"/> instance.</param>
        public static void OnReceivingPreference(ReceivingPreferenceEventArgs ev) => EventManager.Instance.Invoke<ReceivingPreferenceEventArgs>(ev);

        /// <summary>
        /// Automatically generated for backwards compatibility.
        /// </summary>
        /// <param name="ev">Input from the event system.</param>
        [Subscribe]
        public static void BackwardsCompatOnReceivingPreference(ReceivingPreferenceEventArgs ev) => ReceivingPreference.InvokeSafely(ev);


    }
}
