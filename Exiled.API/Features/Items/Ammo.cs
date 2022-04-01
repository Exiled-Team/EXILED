// -----------------------------------------------------------------------
// <copyright file="Ammo.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Items {
    using Exiled.API.Enums;

    using InventorySystem.Items.Firearms.Ammo;

    /// <summary>
    /// A wrapper class for <see cref="AmmoItem"/>.
    /// </summary>
    public class Ammo : Item {
        /// <summary>
        /// Initializes a new instance of the <see cref="Ammo"/> class.
        /// </summary>
        /// <param name="itemBase"><inheritdoc cref="Base"/></param>
        public Ammo(AmmoItem itemBase)
            : base(itemBase) {
            Base = itemBase;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Ammo"/> class.
        /// </summary>
        /// <param name="type"><inheritdoc cref="Item.Type"/></param>
        public Ammo(ItemType type)
            : this((AmmoItem)Server.Host.Inventory.CreateItemInstance(type, false)) {
        }

        /// <inheritdoc cref="Item.Base"/>
        public new AmmoItem Base { get; }
    }
}
