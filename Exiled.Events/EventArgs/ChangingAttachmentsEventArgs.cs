// -----------------------------------------------------------------------
// <copyright file="ChangingAttachmentsEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs {
    using Exiled.API.Enums;
    using Exiled.API.Features.Items;

    using Firearm = InventorySystem.Items.Firearms.Firearm;

    /// <summary>
    /// Contains all informations before changing item attachments.
    /// </summary>
    public class ChangingAttachmentsEventArgs : ChangingAttributesEventArgs {
        /// <summary>
        /// Initializes a new instance of the <see cref="ChangingAttachmentsEventArgs"/> class.
        /// </summary>
        /// <param name="item"><inheritdoc cref="ChangingAttributesEventArgs.OldItem"/></param>
        /// <param name="newSight"><inheritdoc cref="NewSight"/></param>
        /// <param name="newBarrel"><inheritdoc cref="NewBarrel"/></param>
        /// <param name="newOther"><inheritdoc cref="NewOther"/></param>
        /// <param name="isAllowed"><inheritdoc cref="ChangingAttributesEventArgs.IsAllowed"/></param>
        public ChangingAttachmentsEventArgs(Firearm item, SightType newSight, BarrelType newBarrel, OtherType newOther, bool isAllowed = true)
            : base(item, item, isAllowed) {
            NewSight = newSight;
            NewBarrel = newBarrel;
            NewOther = newOther;
            OldItem = (API.Features.Items.Firearm)Item.Get(item);
        }

        /// <inheritdoc cref="ChangingAttributesEventArgs.OldItem"/>
        public new API.Features.Items.Firearm OldItem { get; }

        /// <summary>
        /// Gets the new item sight attachment.
        /// </summary>
        public SightType NewSight { get; }

        /// <summary>
        /// Gets the new item barrel attachment.
        /// </summary>
        public BarrelType NewBarrel { get; }

        /// <summary>
        /// Gets the new item other attachment.
        /// </summary>
        public OtherType NewOther { get; }
    }
}
