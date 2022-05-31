// -----------------------------------------------------------------------
// <copyright file="Scp330Pickup.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Pickups
{
    using System.Collections.Generic;

    using Exiled.API.Features.Items;

    using InventorySystem.Items.Usables.Scp330;

    using BaseScp330 = InventorySystem.Items.Usables.Scp330.Scp330Pickup;

    /// <summary>
    /// A wrapper class for SCP-330 bags.
    /// </summary>
    public class Scp330Pickup : Pickup
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Scp330Pickup"/> class.
        /// </summary>
        /// <param name="pickupBase">The base <see cref="BaseScp330"/> class.</param>
        public Scp330Pickup(BaseScp330 pickupBase)
            : base(pickupBase)
        {
            Base = pickupBase;
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
        /// Returns the AmmoPickup in a human readable format.
        /// </summary>
        /// <returns>A string containing Scp330Pickup related data.</returns>
        public override string ToString() => $"{Type} ({Serial}) [{Weight}] *{Scale}* |{ExposedCandy}| -{Candies}-";
    }
}
