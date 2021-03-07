// -----------------------------------------------------------------------
// <copyright file="ItemHandler.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Example.Events
{
    using Exiled.API.Features;
    using Exiled.Events.EventArgs;

    /// <summary>
    /// Handles Map events.
    /// </summary>
    internal sealed class ItemHandler
    {
        /// <inheritdoc cref="Exiled.Events.Handlers.Item.OnChangingDurability(ChangingDurabilityEventArgs)"/>
        public void OnChangingDurability(ChangingDurabilityEventArgs ev)
        {
            Log.Info($"Item {ev.OldItem.id} durability of {ev.OldItem.durability} is changing");
        }

        /// <inheritdoc cref="Exiled.Events.Handlers.Item.OnChangingAttachments(ChangingAttachmentsEventArgs)"/>
        public void OnChangingAttachments(ChangingAttachmentsEventArgs ev)
        {
            Log.Info($"Item {ev.NewItem.id} attachments are changing, old ones:\n[SIGHT ({ev.OldItem.modSight})] [BARREL ({ev.OldItem.modBarrel})] [OTHER ({ev.OldItem.modOther})]");
        }
    }
}
