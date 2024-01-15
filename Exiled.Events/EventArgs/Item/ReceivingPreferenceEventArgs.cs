// -----------------------------------------------------------------------
// <copyright file="ReceivingPreferenceEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Item
{
    using System.Collections.Generic;
    using System.Linq;

    using API.Features;
    using API.Structs;

    using Exiled.API.Enums;
    using Exiled.API.Extensions;

    using Interfaces;

    /// <summary>
    /// Contains all information before receiving a preference.
    /// </summary>
    public class ReceivingPreferenceEventArgs : IPlayerEvent, IDeniableEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReceivingPreferenceEventArgs" /> class.
        /// </summary>
        /// <param name="player">
        /// <inheritdoc cref="Player" />
        /// </param>
        /// <param name="itemType">
        /// <inheritdoc cref="Item" />
        /// </param>
        /// <param name="currentCode">
        /// <inheritdoc cref="CurrentCode" />
        /// </param>
        /// <param name="newCode">
        /// <inheritdoc cref="NewCode" />
        /// </param>
        /// <param name="isAllowed">
        /// <inheritdoc cref="IsAllowed" />
        /// </param>
        public ReceivingPreferenceEventArgs(Player player, ItemType itemType, uint currentCode, uint newCode, bool isAllowed = true)
        {
            Player = player;
            Item = itemType.GetFirearmType();
            CurrentAttachmentIdentifiers = Item.GetAttachmentIdentifiers(currentCode);
            NewAttachmentIdentifiers = Item.GetAttachmentIdentifiers(newCode).ToList();
            CurrentCode = currentCode;
            IsAllowed = isAllowed;
        }

        /// <summary>
        /// Gets the <see cref="FirearmType" /> which is being modified.
        /// </summary>
        public FirearmType Item { get; }

        /// <summary>
        /// Gets the old <see cref="AttachmentIdentifier" />[].
        /// </summary>
        public IEnumerable<AttachmentIdentifier> CurrentAttachmentIdentifiers { get; }

        /// <summary>
        /// Gets or sets the new <see cref="List{T}" /> of <see cref="AttachmentIdentifier" />.
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
        /// Gets or sets a value indicating whether or not the attachments preference is allowed.
        /// </summary>
        public bool IsAllowed { get; set; }

        /// <summary>
        /// Gets the <see cref="API.Features.Player" /> who's changing attachments.
        /// </summary>
        public Player Player { get; }
    }
}