// -----------------------------------------------------------------------
// <copyright file="ChangingAttachmentsEventArgs.cs" company="SEXiled Team">
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
    using SEXiled.API.Features.Items;
    using SEXiled.API.Structs;

    using InventorySystem.Items.Firearms.Attachments;

    /// <summary>
    /// Contains all informations before changing item attachments.
    /// </summary>
    public class ChangingAttachmentsEventArgs : System.EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ChangingAttachmentsEventArgs"/> class.
        /// </summary>
        /// <param name="player"><inheritdoc cref="Player"/></param>
        /// <param name="firearm"><inheritdoc cref="Firearm"/></param>
        /// <param name="code">The attachments code.</param>
        /// <param name="isAllowed"><inheritdoc cref="IsAllowed"/></param>
        public ChangingAttachmentsEventArgs(
            Player player,
            Firearm firearm,
            uint code,
            bool isAllowed = true)
        {
            Player = player;
            Firearm = firearm;
            CurrentAttachmentIdentifiers = firearm.AttachmentIdentifiers;
            NewAttachmentIdentifiers = firearm.Type.GetAttachmentIdentifiers(code).ToList();
            CurrentCode = firearm.Base.GetCurrentAttachmentsCode();
            NewCode = code;
            IsAllowed = isAllowed;
        }

        /// <summary>
        /// Gets the <see cref="API.Features.Player"/> who's changing attachments.
        /// </summary>
        public Player Player { get; }

        /// <summary>
        /// Gets the <see cref="API.Features.Items.Firearm"/> which is being modified.
        /// </summary>
        public Firearm Firearm { get; }

        /// <summary>
        /// Gets the old <see cref="AttachmentIdentifier"/>.
        /// </summary>
        public IEnumerable<AttachmentIdentifier> CurrentAttachmentIdentifiers { get; }

        /// <summary>
        /// Gets or sets the new <see cref="AttachmentIdentifier"/>.
        /// </summary>
        public List<AttachmentIdentifier> NewAttachmentIdentifiers { get; set; }

        /// <summary>
        /// Gets the <see cref="CurrentAttachmentIdentifiers"/> code.
        /// </summary>
        public uint CurrentCode { get; }

        /// <summary>
        /// Gets the <see cref="NewAttachmentIdentifiers"/> code.
        /// </summary>
        public uint NewCode { get; }

        /// <summary>
        /// Gets or sets a value indicating whether or not the attachments can be changed.
        /// </summary>
        public bool IsAllowed { get; set; }
    }
}
