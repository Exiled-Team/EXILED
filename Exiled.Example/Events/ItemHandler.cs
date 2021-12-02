// -----------------------------------------------------------------------
// <copyright file="ItemHandler.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Example.Events
{
    using System.Collections.Generic;
    using System.Linq;

    using Exiled.API.Extensions;
    using Exiled.API.Features;
    using Exiled.API.Structs;
    using Exiled.Events.EventArgs;

    using InventorySystem.Items.Firearms.Attachments;

    /// <summary>
    /// Handles Map events.
    /// </summary>
    internal sealed class ItemHandler
    {
        /// <inheritdoc cref="Exiled.Events.Handlers.Item.OnChangingDurability(ChangingDurabilityEventArgs)"/>
        public void OnChangingDurability(ChangingDurabilityEventArgs ev)
        {
            Log.Info($"Durability of {ev.Firearm.Type} {$"({ev.OldDurability})"} is changing. New durability: {ev.NewDurability}");
        }

        /// <inheritdoc cref="Exiled.Events.Handlers.Item.OnChangingAttachments(ChangingAttachmentsEventArgs)"/>
        public void OnChangingAttachments(ChangingAttachmentsEventArgs ev)
        {
            Log.Info($"Item {ev.Firearm.Type} attachments are changing. Old attachment: {ev.OldAttachmentIdentifier.Name} - New Attachment: {ev.NewAttachmentIdentifier.Name}");
        }

        /// <inheritdoc cref="Exiled.Events.Handlers.Item.OnReceivingPreference(ReceivingPreferenceEventArgs)"/>
        public void OnReceivingPreference(ReceivingPreferenceEventArgs ev)
        {
            Log.Info($"Receiving a preference from {ev.Player.Nickname} - Item: {ev.Item}");
        }
    }
}
