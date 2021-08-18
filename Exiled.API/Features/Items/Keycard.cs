// -----------------------------------------------------------------------
// <copyright file="Keycard.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Items
{
    using Interactables.Interobjects.DoorUtils;

    using InventorySystem;
    using InventorySystem.Items;
    using InventorySystem.Items.Keycards;

    /// <summary>
    /// A wrapper class for <see cref="KeycardItem"/>.
    /// </summary>
    public class Keycard : Item
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Keycard"/> class.
        /// </summary>
        /// <param name="itemBase"><inheritdoc cref="Base"/></param>
        public Keycard(ItemBase itemBase)
            : base(itemBase)
        {
            if (!(itemBase is KeycardItem key))
            {
                Log.Warn($"{nameof(Keycard)}.ctor: {itemBase} is not a valid Keycard! {nameof(itemBase)}");
                return;
            }

            Log.Warn($"{nameof(itemBase)}");
            Base = key;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Keycard"/> class.
        /// </summary>
        /// <param name="type"><inheritdoc cref="Item.Type"/></param>
        public Keycard(ItemType type)
            : this(Server.Host.Inventory.CreateItemInstance(type, false))
        {
        }

        /// <inheritdoc cref="Item.Base"/>
        public new KeycardItem Base { get; }

        /// <summary>
        /// Gets or sets the <see cref="KeycardPermissions"/> of the keycard.
        /// </summary>
        public Enums.KeycardPermissions Permissions
        {
            get => (Enums.KeycardPermissions)Base.Permissions;
            set => Base.Permissions = (KeycardPermissions)value;
        }
    }
}
