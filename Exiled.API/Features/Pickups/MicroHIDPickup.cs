// -----------------------------------------------------------------------
// <copyright file="MicroHIDPickup.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Pickups
{
    using Exiled.API.Interfaces;
    using InventorySystem;
    using UnityEngine;

    using BaseMicroHID = InventorySystem.Items.MicroHID.MicroHIDPickup;

    /// <summary>
    /// A wrapper class for a MicroHID pickup.
    /// </summary>
    public class MicroHIDPickup : Pickup, IWrapper<BaseMicroHID>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MicroHIDPickup"/> class.
        /// </summary>
        /// <param name="pickupBase">The base <see cref="BaseMicroHID"/> class.</param>
        internal MicroHIDPickup(BaseMicroHID pickupBase)
            : base(pickupBase)
        {
            Base = pickupBase;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MicroHIDPickup"/> class.
        /// </summary>
        internal MicroHIDPickup()
            : this((BaseMicroHID)Object.Instantiate(InventoryItemLoader.AvailableItems[ItemType.MicroHID].PickupDropModel))
        {
        }

        /// <summary>
        /// Gets the <see cref="BaseMicroHID"/> that this class is encapsulating.
        /// </summary>
        public new BaseMicroHID Base { get; }

        /// <summary>
        /// Gets or sets the MicroHID Energy Level.
        /// </summary>
        public float Energy
        {
            get => Base.NetworkEnergy;
            set => Base.NetworkEnergy = value;
        }

        /// <summary>
        /// Returns the MicroHIDPickup in a human readable format.
        /// </summary>
        /// <returns>A string containing MicroHIDPickup related data.</returns>
        public override string ToString() => $"{Type} ({Serial}) [{Weight}] *{Scale}* |{Energy}|";
    }
}
