// -----------------------------------------------------------------------
// <copyright file="Scp244.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Items
{
    using System.Collections.Generic;

    using InventorySystem.Items.Pickups;
    using InventorySystem.Items.Usables.Scp244;

    using Mirror;

    using UnityEngine;

    using Object = UnityEngine.Object;

    /// <summary>
    /// A wrapper class for SCP-244.
    /// </summary>
    public class Scp244 : Usable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Scp244"/> class.
        /// </summary>
        /// <param name="itemBase"><inheritdoc cref="Base"/></param>
        public Scp244(Scp244Item itemBase)
            : base(itemBase)
        {
            Base = itemBase;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Scp244"/> class, as well as a new SCP-244 item.
        /// </summary>
        /// <param name="scp244Type">The type of SCP-244, either <see cref="ItemType.SCP244a"/> or <see cref="ItemType.SCP244b"/>.</param>
        internal Scp244(ItemType scp244Type)
            : this((Scp244Item)Server.Host.Inventory.CreateItemInstance(scp244Type, false))
        {
        }

        /// <inheritdoc cref="Item.Base"/>
        public new Scp244Item Base { get; }

        /// <summary>
        /// Gets or sets a value indicating whether or not SCP-244 will spawn primed.
        /// </summary>
        public bool Primed
        {
            get => Base._primed;
            set => Base._primed = value;
        }
    }
}
