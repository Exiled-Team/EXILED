// -----------------------------------------------------------------------
// <copyright file="ItemHandler.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Example.Events
{
    using System.Linq;

    using Exiled.API.Features;
    using Exiled.Events.EventArgs.Item;

    /// <summary>
    /// Handles Item events.
    /// </summary>
    internal sealed class ItemHandler
    {
        /// <inheritdoc cref="Exiled.Events.Handlers.Item.OnChangingAmmo(ChangingAmmoEventArgs)"/>
        public void OnChangingAmmo(ChangingAmmoEventArgs ev)
        {
            Log.Info($"Durability of {ev.Firearm.Type} ({ev.OldAmmo}) is changing. New durability: {ev.NewAmmo}");
        }

        /// <inheritdoc cref="Exiled.Events.Handlers.Item.OnChangingAttachments(ChangingAttachmentsEventArgs)"/>
        public void OnChangingAttachments(ChangingAttachmentsEventArgs ev)
        {
            string oldAttachments = ev.CurrentAttachmentIdentifiers.Aggregate(string.Empty, (current, attachmentIdentifier) => current + $"{attachmentIdentifier.Name}\n");
            string newAttachments = ev.NewAttachmentIdentifiers.Aggregate(string.Empty, (current, attachmentIdentifier) => current + $"{attachmentIdentifier.Name}\n");

            Log.Info($"Item {ev.Firearm.Type} attachments are changing. Old attachments:\n{oldAttachments} - New Attachments:\n{newAttachments}");
        }

        /// <inheritdoc cref="Exiled.Events.Handlers.Item.OnReceivingPreference(ReceivingPreferenceEventArgs)"/>
        public void OnReceivingPreference(ReceivingPreferenceEventArgs ev)
        {
            Log.Info($"Receiving a preference from {ev.Player.Nickname} - Item: {ev.Item}");
        }
    }
}