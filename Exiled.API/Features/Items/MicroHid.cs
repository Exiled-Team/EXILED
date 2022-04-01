// -----------------------------------------------------------------------
// <copyright file="MicroHid.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Items {
    using Exiled.API.Enums;

    using InventorySystem.Items.MicroHID;

    /// <summary>
    /// A wrapper class for <see cref="InventorySystem.Items.MicroHID.MicroHIDItem"/>.
    /// </summary>
    public class MicroHid : Item {
        /// <summary>
        /// Initializes a new instance of the <see cref="MicroHid"/> class.
        /// </summary>
        /// <param name="itemBase"><inheritdoc cref="Base"/></param>
        public MicroHid(MicroHIDItem itemBase)
            : base(itemBase) {
            Base = itemBase;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MicroHid"/> class.
        /// </summary>
        /// <param name="type"><inheritdoc cref="Base"/></param>
        public MicroHid(ItemType type)
            : this((MicroHIDItem)Server.Host.Inventory.CreateItemInstance(type, false)) {
        }

        /// <summary>
        /// Gets or sets the remaining energy in the MicroHID.
        /// </summary>
        public float Energy {
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
        public HidState State {
            get => Base.State;
            set => Base.State = value;
        }

        /// <summary>
        /// Starts firing the MicroHID.
        /// </summary>
        public void Fire() {
            Base.UserInput = HidUserInput.Fire;
            State = HidState.Firing;
        }

        /// <inheritdoc/>
        public override string ToString() {
            return $"{Type} ({Serial}) [{Weight}] *{Scale}* |{Energy}| -{State}-";
        }
    }
}
