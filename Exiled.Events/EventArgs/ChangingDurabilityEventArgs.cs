// -----------------------------------------------------------------------
// <copyright file="ChangingDurabilityEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs
{
    /// <summary>
    /// Contains all informations before changing item durability.
    /// </summary>
    public class ChangingDurabilityEventArgs : ChangingAttributesEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ChangingDurabilityEventArgs"/> class.
        /// </summary>
        /// <param name="item"><inheritdoc cref="ChangingAttributesEventArgs.OldItem"/></param>
        /// <param name="newDurability"><inheritdoc cref="ChangingAttributesEventArgs.NewDurability"/></param>
        /// <param name="isAllowed"><inheritdoc cref="ChangingAttributesEventArgs.IsAllowed"/></param>
        public ChangingDurabilityEventArgs(Inventory.SyncItemInfo item, float newDurability, bool isAllowed = true)
            : base(item, item, isAllowed)
        {
            NewDurability = newDurability;
        }
    }
}
