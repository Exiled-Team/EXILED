// -----------------------------------------------------------------------
// <copyright file="Scp244.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Items
{
    using InventorySystem.Items.Usables.Scp244;

    /// <summary>
    /// A wrapper class for SCP-244.
    /// </summary>
    public class Scp244 : Usable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Scp244"/> class.
        /// </summary>
        /// <param name="itemBase">The base <see cref="Scp244Item"/> class.</param>
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

        /// <summary>
        /// Gets the <see cref="Scp244Item"/> that this class is encapsulating.
        /// </summary>
        public new Scp244Item Base { get; }

        /// <summary>
        /// Gets or sets a value indicating whether or not SCP-244 will spawn primed.
        /// </summary>
        public bool Primed
        {
            get => Base._primed;
            set => Base._primed = value;
        }

        /// <summary>
        /// Returns the SCP-244 in a human readable format.
        /// </summary>
        /// <returns>A string containing SCP-244 related data.</returns>
        public override string ToString() => $"{Type} ({Serial}) [{Weight}] *{Scale}* -{Primed}-";

        /// <summary>
        /// Clones current <see cref="Scp244"/> object.
        /// </summary>
        /// <returns> New <see cref="Scp244"/> object. </returns>
        public override Item Clone()
        {
            Scp244 cloneableItem = new(Type);

            cloneableItem.Primed = Primed;

            return cloneableItem;
        }
    }
}
