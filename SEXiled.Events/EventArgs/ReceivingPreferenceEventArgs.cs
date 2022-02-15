// -----------------------------------------------------------------------
// <copyright file="ReceivingPreferenceEventArgs.cs" company="SEXiled Team">
// Copyright (c) SEXiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace SEXiled.Events.EventArgs
{
    using System.Collections.Generic;
    using System.Linq;

    using SEXiled.API.Extensions;
    using SEXiled.API.Features;
    using SEXiled.API.Structs;

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
        /// <param name="currentCode"><inheritdoc cref="CurrentCode"/></param>
        /// <param name="newCode"><inheritdoc cref="NewCode"/></param>
        /// <param name="isAllowed"><inheritdoc cref="IsAllowed"/></param>
        public ReceivingPreferenceEventArgs(
            Player player,
            ItemType itemType,
            uint currentCode,
            uint newCode,
            bool isAllowed = true)
        {
            Player = player;
            Item = itemType;
            CurrentAttachmentIdentifiers = Item.GetAttachmentIdentifiers(currentCode);
            NewAttachmentIdentifiers = Item.GetAttachmentIdentifiers(newCode).ToList();
            CurrentCode = currentCode;
            IsAllowed = isAllowed;
        }

        /// <summary>
        /// Gets the <see cref="API.Features.Player"/> who's changing attachments.
        /// </summary>
        public Player Player { get; }

        /// <summary>
        /// Gets the <see cref="ItemType"/> which is being modified.
        /// </summary>
        public ItemType Item { get; }

        /// <summary>
        /// Gets the old <see cref="AttachmentIdentifier"/>[].
        /// </summary>
        public IEnumerable<AttachmentIdentifier> CurrentAttachmentIdentifiers { get; }

        /// <summary>
        /// Gets or sets the new <see cref="List{T}"/> of <see cref="AttachmentIdentifier"/>.
        /// </summary>
        public List<AttachmentIdentifier> NewAttachmentIdentifiers { get; set; }

        /// <summary>
        /// Gets the current attachments code.
        /// </summary>
        public uint CurrentCode { get; }

        /// <summary>
        /// Gets or sets the new attachments code.
        /// </summary>
        public uint NewCode
        {
            get => NewAttachmentIdentifiers.GetAttachmentsCode();
            set => NewAttachmentIdentifiers = Item.GetAttachmentIdentifiers(value).ToList();
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not the attachments preference can be executed.
        /// </summary>
        public bool IsAllowed { get; set; }
    }
}
