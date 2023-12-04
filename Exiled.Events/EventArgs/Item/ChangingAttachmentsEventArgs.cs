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
    ///     Contains all information before changing item attachments.
    /// </summary>
    public class ChangingAttachmentsEventArgs : IPlayerEvent, IDeniableEvent, IFirearmEvent
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="ChangingAttachmentsEventArgs" /> class.
        /// </summary>
        /// <param name="player">
        ///     <inheritdoc cref="Player" />
        /// </param>
        /// <param name="firearm">
        ///     <inheritdoc cref="Firearm" />
        /// </param>
        /// <param name="code">The attachments code.</param>
        /// <param name="isAllowed">
        ///     <inheritdoc cref="IsAllowed" />
        /// </param>
        public ChangingAttachmentsEventArgs(Player player, Firearm firearm, uint code, bool isAllowed = true)
        {
            Player = player;
            Firearm = firearm;
            CurrentAttachmentIdentifiers = firearm.AttachmentIdentifiers;
            NewAttachmentIdentifiers = firearm.FirearmType.GetAttachmentIdentifiers(code).ToList();
            CurrentCode = firearm.Base.GetCurrentAttachmentsCode();
            NewCode = code;
            IsAllowed = isAllowed;
        }

        /// <summary>
        ///     Gets the old <see cref="AttachmentIdentifier" />.
        /// </summary>
        public IEnumerable<AttachmentIdentifier> CurrentAttachmentIdentifiers { get; }

        /// <summary>
        ///     Gets or sets the new <see cref="AttachmentIdentifier" />.
        /// </summary>
        public List<AttachmentIdentifier> NewAttachmentIdentifiers { get; set; }

        /// <summary>
        ///     Gets the <see cref="CurrentAttachmentIdentifiers" /> code.
        /// </summary>
        public uint CurrentCode { get; }

        /// <summary>
        ///     Gets the <see cref="NewAttachmentIdentifiers" /> code.
        /// </summary>
        public uint NewCode { get; }

        /// <summary>
        ///     Gets or sets a value indicating whether or not the attachments can be changed.
        /// </summary>
        public bool IsAllowed { get; set; }

        /// <summary>
        ///     Gets the <see cref="API.Features.Items.Firearm" /> which is being modified.
        /// </summary>
        public Firearm Firearm { get; }

        /// <inheritdoc/>
        public Item Item => Firearm;

        /// <summary>
        ///     Gets the <see cref="API.Features.Player" /> who's changing attachments.
        /// </summary>
        public Player Player { get; }
    }
}