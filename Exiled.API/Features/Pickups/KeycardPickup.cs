// -----------------------------------------------------------------------
// <copyright file="KeycardPickup.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Pickups
{
    using Exiled.API.Enums;
    using Exiled.API.Extensions;
    using Exiled.API.Features.Core.Attributes;
    using Exiled.API.Features.Items;
    using Exiled.API.Interfaces;
    using InventorySystem;
    using InventorySystem.Items;
    using InventorySystem.Items.Keycards;
    using UnityEngine;

    using BaseKeycard = InventorySystem.Items.Keycards.KeycardPickup;

    /// <summary>
    /// A wrapper class for a Keycard pickup.
    /// </summary>
    public class KeycardPickup : Pickup, IWrapper<BaseKeycard>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="KeycardPickup"/> class.
        /// </summary>
        /// <param name="pickupBase">The base <see cref="BaseKeycard"/> class.</param>
        internal KeycardPickup(BaseKeycard pickupBase)
            : base(pickupBase)
        {
            Base = pickupBase;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="KeycardPickup"/> class.
        /// </summary>
        /// <param name="type">The <see cref="ItemType"/> of the pickup.</param>
        internal KeycardPickup(ItemType type)
            : this((BaseKeycard)type.GetItemBase().ServerDropItem())
        {
        }

        /// <summary>
        /// Gets or sets the <see cref="KeycardPermissions"/> of the keycard.
        /// </summary>
        [EProperty(category: nameof(KeycardPickup))]
        public KeycardPermissions Permissions { get; set; }

        /// <summary>
        /// Gets the <see cref="BaseKeycard"/> that this class is encapsulating.
        /// </summary>
        public new BaseKeycard Base { get; }

        /// <inheritdoc/>
        internal override void ReadItemInfo(Item item)
        {
            base.ReadItemInfo(item);
            if (item is Keycard keycarditem)
            {
                Permissions = keycarditem.Permissions;
            }
        }

        /// <inheritdoc/>
        protected override void InitializeProperties(ItemBase itemBase)
        {
            base.InitializeProperties(itemBase);
            if (itemBase is KeycardItem keycardItem)
            {
                Permissions = (KeycardPermissions)keycardItem.Permissions;
            }
        }
    }
}
