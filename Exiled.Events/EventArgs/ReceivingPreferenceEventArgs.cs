// -----------------------------------------------------------------------
// <copyright file="ReceivingPreferenceEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs
{
    using System.Collections.Generic;

    using Exiled.API.Features;
    using Exiled.API.Structs;

    /// <summary>
    /// Contains all informations before receiving a preference.
    /// </summary>
    public class ReceivingPreferenceEventArgs : System.EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReceivingPreferenceEventArgs"/> class.
        /// </summary>
        /// <param name="player"><inheritdoc cref="Player"/></param>
        /// <param name="itemType"><inheritdoc cref="Item"/></param>
        /// <param name="oldIdentifiers"><inheritdoc cref="OldAttachmentIdentifiers"/></param>
        /// <param name="newIdentifiers"><inheritdoc cref="NewAttachmentIdentifiers"/></param>
        /// <param name="isAllowed"><inheritdoc cref="IsAllowed"/></param>
        public ReceivingPreferenceEventArgs(
            Player player,
            ItemType itemType,
            AttachmentIdentifier[] oldIdentifiers,
            List<AttachmentIdentifier> newIdentifiers,
            bool isAllowed = true)
        {
            Player = player;
            Item = itemType;
            OldAttachmentIdentifiers = oldIdentifiers;
            NewAttachmentIdentifiers = newIdentifiers;
            IsAllowed = isAllowed;
        }

        /// <summary>
        /// Gets the <see cref="API.Features.Player"/> who's changing attachments.
        /// </summary>
        public Player Player { get; }

        /// <summary>
        /// Gets or sets the <see cref="ItemType"/> which is being modified.
        /// </summary>
        public ItemType Item { get; set; }

        /// <summary>
        /// Gets the old <see cref="AttachmentIdentifier"/>[].
        /// </summary>
        public AttachmentIdentifier[] OldAttachmentIdentifiers { get; }

        /// <summary>
        /// Gets or sets the new <see cref="List{T}"/> of <see cref="AttachmentIdentifier"/>.
        /// </summary>
        public List<AttachmentIdentifier> NewAttachmentIdentifiers { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether or not the attachments preference can be executed.
        /// </summary>
        public bool IsAllowed { get; set; }
    }
}
