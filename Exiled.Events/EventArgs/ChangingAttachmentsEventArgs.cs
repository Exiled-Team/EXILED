// -----------------------------------------------------------------------
// <copyright file="ChangingAttachmentsEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs
{
    using Exiled.API.Enums;

    /// <summary>
    /// Contains all informations before changing item attachments.
    /// </summary>
    public class ChangingAttachmentsEventArgs : ChangingAttributesEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ChangingAttachmentsEventArgs"/> class.
        /// </summary>
        /// <param name="item"><inheritdoc cref="ChangingAttributesEventArgs.OldItem"/></param>
        /// <param name="newSight"><inheritdoc cref="ChangingAttributesEventArgs.NewSight"/></param>
        /// <param name="newBarrel"><inheritdoc cref="ChangingAttributesEventArgs.NewBarrel"/></param>
        /// <param name="newOther"><inheritdoc cref="ChangingAttributesEventArgs.NewOther"/></param>
        /// <param name="isAllowed"><inheritdoc cref="ChangingAttributesEventArgs.IsAllowed"/></param>
        public ChangingAttachmentsEventArgs(Inventory.SyncItemInfo item, SightType newSight, BarrelType newBarrel, OtherType newOther, bool isAllowed = true)
            : base(item, item, isAllowed)
        {
            NewSight = newSight;
            NewBarrel = newBarrel;
            NewOther = newOther;
        }
    }
}
