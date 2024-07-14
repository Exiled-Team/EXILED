// -----------------------------------------------------------------------
// <copyright file="ChangingAttachmentsEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Item
{
    using System.Collections.Generic;
    using System.Linq;

    using API.Features;
    using API.Features.Items;
    using API.Structs;

    using Exiled.API.Extensions;

    using Interfaces;

    using InventorySystem.Items.Firearms.Attachments;

    /// <summary>
    /// Contains all information before <see cref="Exiled.API.Features.Items.Firearm"/> attachments are changed.
    /// </summary>
    public class ChangingAttachmentsEventArgs : IPlayerEvent, IDeniableEvent, IFirearmEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ChangingAttachmentsEventArgs" /> class.
        /// </summary>
        /// <param name="firearm">
        /// <inheritdoc cref="Firearm" />
        /// </param>
        /// <param name="code">The attachments code.</param>
        /// <param name="isAllowed">
        /// <inheritdoc cref="IsAllowed" />
        /// </param>
        public ChangingAttachmentsEventArgs(Firearm firearm, uint code, bool isAllowed = true)
        {
            Firearm = firearm;
            Player = Firearm.Owner;
            IsAllowed = isAllowed;
            OldAttachmentIdentifiers = firearm.AttachmentIdentifiers;
            OldAttachmentsCode = firearm.Base.GetCurrentAttachmentsCode();
            NewAttachmentsCode = code;
        }

        /// <summary>
        /// Gets the old <see cref="AttachmentIdentifier" /> list.
        /// </summary>
        public IEnumerable<AttachmentIdentifier> OldAttachmentIdentifiers { get; }

        /// <summary>
        /// Gets or sets the new <see cref="AttachmentIdentifier" /> list.
        /// </summary>
        public List<AttachmentIdentifier> NewAttachmentIdentifiers { get; set; }

        /// <summary>
        /// Gets the <see cref="OldAttachmentIdentifiers" /> code.
        /// </summary>
        public uint OldAttachmentsCode { get; }

        /// <summary>
        /// Gets or sets the <see cref="NewAttachmentIdentifiers" /> code.
        /// </summary>
        public uint NewAttachmentsCode
        {
            get => NewAttachmentIdentifiers.GetAttachmentsCode();
            set => NewAttachmentIdentifiers = Firearm.FirearmType.GetAttachmentIdentifiers(value).ToList();
        }

        /// <summary>
        /// Gets or sets a value indicating whether the attachments can be changed.
        /// </summary>
        public bool IsAllowed { get; set; }

        /// <summary>
        /// Gets the <see cref="API.Features.Items.Firearm" /> which is being modified.
        /// </summary>
        public Firearm Firearm { get; }

        /// <inheritdoc/>
        public Item Item => Firearm;

        /// <summary>
        /// Gets the <see cref="API.Features.Player" /> who's changing attachments.
        /// </summary>
        public Player Player { get; }
    }
}