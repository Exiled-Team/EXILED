// -----------------------------------------------------------------------
// <copyright file="Ammo.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Items
{
    using Enums;
    using Exiled.API.Interfaces;

    using InventorySystem.Items.Firearms.Ammo;

    /// <summary>
    /// A wrapper class for <see cref="AmmoItem"/>.
    /// </summary>
    public class Ammo : Item, IWrapper<AmmoItem>
    {
        /// <summary>
        /// Gets the absolute maximum amount of ammo that may be held at one time, if ammo is forcefully given to the player (regardless of worn armor or server configuration).
        /// <para>
        /// For accessing the maximum amount of ammo that may be held based on worn armor and server settings, see <see cref="Player.GetAmmoLimit(AmmoType, bool)"/>.
        /// </para>
        /// </summary>
        public const ushort AmmoLimit = ushort.MaxValue;

        /// <summary>
        /// Initializes a new instance of the <see cref="Ammo"/> class.
        /// </summary>
        /// <param name="itemBase">The base <see cref="AmmoItem"/> class.</param>
        public Ammo(AmmoItem itemBase)
            : base(itemBase)
        {
            Base = itemBase;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Ammo"/> class.
        /// </summary>
        /// <param name="type">The <see cref="ItemType"/> of the ammo.</param>
        internal Ammo(ItemType type)
            : this((AmmoItem)Server.Host.Inventory.CreateItemInstance(new(type, 0), false))
        {
        }

        /// <summary>
        /// Gets the <see cref="AmmoItem"/> that this class is encapsulating.
        /// </summary>
        public new AmmoItem Base { get; }

        /// <summary>
        /// Clones current <see cref="Ammo"/> object.
        /// </summary>
        /// <returns> New <see cref="Ammo"/> object. </returns>
        public override Item Clone() => new(Type);
    }
}