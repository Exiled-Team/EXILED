// -----------------------------------------------------------------------
// <copyright file="MicroHid.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Items
{
    using InventorySystem;
    using InventorySystem.Items;
    using InventorySystem.Items.MicroHID;

    using HidState = Exiled.API.Enums.HidState;

    /// <summary>
    /// A wrapper class for <see cref="InventorySystem.Items.MicroHID.MicroHIDItem"/>.
    /// </summary>
    public class MicroHid : Item
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MicroHid"/> class.
        /// </summary>
        /// <param name="itemBase"><inheritdoc cref="Base"/></param>
        public MicroHid(ItemBase itemBase)
            : base(itemBase)
        {
            Base = (MicroHIDItem)itemBase;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MicroHid"/> class.
        /// </summary>
        /// <param name="type"><inheritdoc cref="Base"/></param>
        public MicroHid(ItemType type)
            : base(type)
        {
            if (!InventoryItemLoader.AvailableItems.TryGetValue(type, out ItemBase itemBase))
                return;

            Base = (MicroHIDItem)itemBase;
        }

        /// <summary>
        /// Gets or sets the remaining energy in the MicroHID.
        /// </summary>
        public float Energy
        {
            get => Base.RemainingEnergy;
            set => Base.RemainingEnergy = value;
        }

        /// <summary>
        /// Gets the <see cref="MicroHIDItem"/> base of the item.
        /// </summary>
        public new MicroHIDItem Base { get; }

        /// <summary>
        /// Gets or sets the <see cref="HidState"/>.
        /// </summary>
        public HidState State
        {
            get => (HidState)Base.State;
            set => Base.State = (InventorySystem.Items.MicroHID.HidState)value;
        }

        /// <summary>
        /// Starts firing the MicroHID.
        /// </summary>
        public void Fire()
        {
            Base.UserInput = HidUserInput.Fire;
            State = HidState.Firing;
        }
    }
}
