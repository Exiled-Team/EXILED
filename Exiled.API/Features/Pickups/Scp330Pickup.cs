// -----------------------------------------------------------------------
// <copyright file="Scp330Pickup.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Pickups
{
    using System.Collections.Generic;

    using Exiled.API.Interfaces;
    using InventorySystem;
    using InventorySystem.Items.Usables.Scp330;
    using UnityEngine;

    using BaseScp330 = InventorySystem.Items.Usables.Scp330.Scp330Pickup;

    /// <summary>
    /// A wrapper class for dropped SCP-330 bags.
    /// </summary>
    public class Scp330Pickup : UsablePickup, IWrapper<BaseScp330>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Scp330Pickup"/> class.
        /// </summary>
        /// <param name="pickupBase">The base <see cref="BaseScp330"/> class.</param>
        internal Scp330Pickup(BaseScp330 pickupBase)
            : base(pickupBase)
        {
            Base = pickupBase;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Scp330Pickup"/> class.
        /// </summary>
        internal Scp330Pickup()
            : this((BaseScp330)Object.Instantiate(InventoryItemLoader.AvailableItems[ItemType.SCP330].PickupDropModel))
        {
            Base = (BaseScp330)((Pickup)this).Base;
        }

        /// <summary>
        /// Gets the <see cref="BaseScp330"/> that this class is encapsulating.
        /// </summary>
        public new BaseScp330 Base { get; }

        /// <summary>
        /// Gets or sets the exposed <see cref="CandyKindID"/>.
        /// </summary>
        public CandyKindID ExposedCandy
        {
            get => Base.NetworkExposedCandy;
            set => Base.NetworkExposedCandy = value;
        }

        /// <summary>
        /// Gets or sets the <see cref="CandyKindID"/>s held in this bag.
        /// </summary>
        public List<CandyKindID> Candies
        {
            get => Base.StoredCandies;
            set => Base.StoredCandies = value;
        }

        /// <summary>
        /// Returns the Scp330Pickup in a human readable format.
        /// </summary>
        /// <returns>A string containing Scp330Pickup related data.</returns>
        public override string ToString() => $"{Type} ({Serial}) [{Weight}] *{Scale}* |{ExposedCandy}| -{Candies}-";
    }
}
