// -----------------------------------------------------------------------
// <copyright file="Ammo.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Items
{
    using InventorySystem.Items.Firearms.Ammo;

    /// <summary>
    /// A wrapper class for <see cref="AmmoItem"/>.
    /// </summary>
    public class Ammo : Item
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Ammo"/> class.
        /// </summary>
        /// <param name="itemBase"><inheritdoc cref="Base"/></param>
        public Ammo(AmmoItem itemBase)
            : base(itemBase)
        {
            Base = itemBase;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Ammo"/> class.
        /// </summary>
        /// <param name="type"><inheritdoc cref="Item.Type"/></param>
        internal Ammo(ItemType type)
            : this((AmmoItem)Server.Host.Inventory.CreateItemInstance(type, false))
        {
        }

        /// <summary>
        /// Gets the absolute maximum amount of ammo that may be held at one time, if ammo is forcefully given to the player (regardless of worn armor or server configuration).
        /// <para>
        /// For accessing the maximum amount of ammo that may be held based on worn armor and server settings, see <see cref="Player.GetAmmoLimit(Enums.AmmoType)"/>.
        /// </para>
        /// </summary>
        public static ushort AmmoLimit => ushort.MaxValue;

        /// <inheritdoc cref="Item.Base"/>
        public new AmmoItem Base { get; }
    }
}
