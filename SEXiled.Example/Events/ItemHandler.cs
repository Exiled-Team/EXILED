// -----------------------------------------------------------------------
// <copyright file="ItemHandler.cs" company="SEXiled Team">
// Copyright (c) SEXiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace SEXiled.Example.Events
{
    using System.Linq;

    using SEXiled.API.Features;
    using SEXiled.Events.EventArgs;

    /// <summary>
    /// Handles Map events.
    /// </summary>
    internal sealed class ItemHandler
    {
        /// <inheritdoc cref="SEXiled.Events.Handlers.Item.OnChangingDurability(ChangingDurabilityEventArgs)"/>
        public void OnChangingDurability(ChangingDurabilityEventArgs ev)
        {
            Log.Info($"Durability of {ev.Firearm.Type} ({ev.OldDurability}) is changing. New durability: {ev.NewDurability}");
        }

        /// <inheritdoc cref="SEXiled.Events.Handlers.Item.OnChangingAttachments(ChangingAttachmentsEventArgs)"/>
        public void OnChangingAttachments(ChangingAttachmentsEventArgs ev)
        {
            string oldAttachments = ev.CurrentAttachmentIdentifiers.Aggregate(string.Empty, (current, attachmentIdentifier) => current + $"{attachmentIdentifier.Name}\n");
            string newAttachments = ev.NewAttachmentIdentifiers.Aggregate(string.Empty, (current, attachmentIdentifier) => current + $"{attachmentIdentifier.Name}\n");

            Log.Info($"Item {ev.Firearm.Type} attachments are changing. Old attachments:\n{oldAttachments} - New Attachments:\n{newAttachments}");
        }

        /// <inheritdoc cref="SEXiled.Events.Handlers.Item.OnReceivingPreference(ReceivingPreferenceEventArgs)"/>
        public void OnReceivingPreference(ReceivingPreferenceEventArgs ev)
        {
            Log.Info($"Receiving a preference from {ev.Player.Nickname} - Item: {ev.Item}");
        }
    }
}
