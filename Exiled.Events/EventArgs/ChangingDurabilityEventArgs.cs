// -----------------------------------------------------------------------
// <copyright file="ChangingDurabilityEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs {
    using InventorySystem.Items.Firearms;

    /// <summary>
    /// Contains all informations before changing item durability.
    /// </summary>
    public class ChangingDurabilityEventArgs : ChangingAttributesEventArgs {
        /// <summary>
        /// Initializes a new instance of the <see cref="ChangingDurabilityEventArgs"/> class.
        /// </summary>
        /// <param name="item"><inheritdoc cref="ChangingAttributesEventArgs.OldItem"/></param>
        /// <param name="newDurability"><inheritdoc cref="NewDurability"/></param>
        /// <param name="isAllowed"><inheritdoc cref="ChangingAttributesEventArgs.IsAllowed"/></param>
        public ChangingDurabilityEventArgs(Firearm item, float newDurability, bool isAllowed = true)
            : base(item, item, isAllowed) {
            Item = (API.Features.Items.Firearm)API.Features.Items.Item.Get(item);
            NewDurability = newDurability;
        }

        /// <inheritdoc cref="Item"/>
        public API.Features.Items.Firearm Item { get; set; }

        /// <summary>
        /// Gets or sets the new durability to be used by the weapon.
        /// </summary>
        public float NewDurability { get; set; }
    }
}
