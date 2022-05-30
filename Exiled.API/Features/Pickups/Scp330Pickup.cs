// -----------------------------------------------------------------------
// <copyright file="Scp330Pickup.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Pickups
{
#pragma warning disable CS1591 // Commentaire XML manquant pour le type ou le membre visible publiquement
#pragma warning disable SA1600 // Commentaire XML manquant pour le type ou le membre visible publiquement

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

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
        /// <param name="itemBase">The base <see cref="BaseScp330"/> class.</param>
        public Scp330Pickup(BaseScp330 itemBase)
            : base(itemBase)
        {
            Base = itemBase;
        }

        /// <summary>
        /// Gets the <see cref="BaseScp330"/> that this class is encapsulating.
        /// </summary>
        public new BaseScp330 Base { get; }

        /// <summary>
        /// Gets or sets the exposed type.
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
        /// <returns>A string containing AmmoPickup related data.</returns>
        public override string ToString() => $"{Type} ({Serial}) [{Weight}] *{Scale}* |{ExposedCandy}| -{Candies}-";
    }
}
