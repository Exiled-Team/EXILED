// -----------------------------------------------------------------------
// <copyright file="Scp330Pickup.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Pickups
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    using NWScp330 = InventorySystem.Items.Usables.Scp330.Scp330Pickup;

    /// <summary>
    /// A wrapper class for SCP-330 bags.
    /// </summary>
    public class Scp330Pickup : Pickup
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Scp330Pickup"/> class.
        /// </summary>
        /// <param name="itemBase">The base <see cref="NWRadio"/> class.</param>
        public Scp330Pickup(NWScp330 itemBase)
            : base(itemBase)
        {
            Base = itemBase;
        }

        /// <summary>
        /// Gets the <see cref="NWScp330"/> that this class is encapsulating.
        /// </summary>
        public new NWScp330 Base { get; }
    }
}
