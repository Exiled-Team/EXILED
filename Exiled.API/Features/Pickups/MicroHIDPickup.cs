// -----------------------------------------------------------------------
// <copyright file="MicroHIDPickup.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Pickups
{
    using Exiled.API.Features.Items;

    using BaseMicroHID = InventorySystem.Items.MicroHID.MicroHIDPickup;

    /// <summary>
    /// A wrapper class for SCP-330 bags.
    /// </summary>
    public class MicroHIDPickup : Pickup
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MicroHIDPickup"/> class.
        /// </summary>
        /// <param name="itemBase">The base <see cref="BaseMicroHID"/> class.</param>
        public MicroHIDPickup(BaseMicroHID itemBase)
            : base(itemBase)
        {
            Base = itemBase;
        }

        /// <summary>
        /// Gets the <see cref="BaseMicroHID"/> that this class is encapsulating.
        /// </summary>
        public new BaseMicroHID Base { get; }

        /// <summary>
        /// Gets or sets the MicroHID Energy.
        /// </summary>
        public float Energy
        {
            get => Base.NetworkEnergy;
            set => Base.NetworkEnergy = value;
        }

        /// <summary>
        /// Returns the AmmoPickup in a human readable format.
        /// </summary>
        /// <returns>A string containing AmmoPickup related data.</returns>
        public override string ToString() => $"{Type} ({Serial}) [{Weight}] *{Scale}* |{Energy}|";
    }
}
