// -----------------------------------------------------------------------
// <copyright file="ChangedAttachmentsEventArgs.cs" company="Exiled Team">
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
    using InventorySystem.Items.Firearms;
    using InventorySystem.Items.Firearms.Attachments;

    using BaseFirearm = InventorySystem.Items.Firearms.Firearm;
    using Firearm = API.Features.Items.Firearm;

    /// <summary>
    ///     Contains all information after changing item attachments.
    /// </summary>
    public class ChangedAttachmentsEventArgs : IPlayerEvent, IFirearmEvent
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="ChangedAttachmentsEventArgs" /> class.
        /// </summary>
        /// <param name="basefirearm"><inheritdoc cref="Firearm"/></param>
        /// <param name="code"><inheritdoc cref="OldCode"/></param>
        public ChangedAttachmentsEventArgs(BaseFirearm basefirearm, uint code)
        {
            Firearm = Item.Get(basefirearm).As<Firearm>();
            Player = Firearm.Owner;
            OldAttachmentIdentifiers = Firearm.AttachmentIdentifiers;
            NewAttachmentIdentifiers = Firearm.FirearmType.GetAttachmentIdentifiers(code).ToList();
            OldCode = code;
            NewCode = basefirearm.GetCurrentAttachmentsCode();
        }

        /// <summary>
        ///     Gets the old <see cref="AttachmentIdentifier" />.
        /// </summary>
        public IEnumerable<AttachmentIdentifier> OldAttachmentIdentifiers { get; }

        /// <summary>
        ///     Gets or sets the new <see cref="AttachmentIdentifier" />.
        /// </summary>
        public List<AttachmentIdentifier> NewAttachmentIdentifiers { get; set; }

        /// <summary>
        ///     Gets the <see cref="OldAttachmentIdentifiers" /> code.
        /// </summary>
        public uint OldCode { get; }

        /// <summary>
        ///     Gets the <see cref="NewAttachmentIdentifiers" /> code.
        /// </summary>
        public uint NewCode { get; }

        /// <summary>
        ///     Gets the <see cref="API.Features.Items.Firearm" /> which as been modified.
        /// </summary>
        public Firearm Firearm { get; }

        /// <inheritdoc/>
        public Item Item => Firearm;

        /// <summary>
        ///     Gets the <see cref="API.Features.Player" /> who's changed attachments.
        /// </summary>
        public Player Player { get; }
    }
}