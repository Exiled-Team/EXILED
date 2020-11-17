// -----------------------------------------------------------------------
// <copyright file="Item.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Handlers
{
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
        public static event CustomEventHandler<ChangingAttributesEventArgs> ChangingDurability;

        /// <summary>
        /// Invoked before item attachments are changed.
        /// </summary>
        public static event CustomEventHandler<ChangingAttributesEventArgs> ChangingAttachments;

        /// <summary>
        /// Called before the durability of an item is changed.
        /// </summary>
        /// <param name="ev">The <see cref="ChangingAttributesEventArgs"/> instance.</param>
        public static void OnChangingDurability(ChangingAttributesEventArgs ev) => ChangingDurability.InvokeSafely(ev);

        /// <summary>
        /// Called before item attachments are changed.
        /// </summary>
        /// <param name="ev">The <see cref="ChangingAttributesEventArgs"/> instance.</param>
        public static void OnChangingAttachments(ChangingAttributesEventArgs ev) => ChangingAttachments.InvokeSafely(ev);
    }
}
