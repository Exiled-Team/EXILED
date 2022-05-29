// -----------------------------------------------------------------------
// <copyright file="Scp244Pickup.cs" company="Exiled Team">
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

    using InventorySystem.Items.Usables.Scp244;

    /// <summary>
    /// A wrapper class for SCP-330 bags.
    /// </summary>
    public class Scp244Pickup : Pickup
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Scp244Pickup"/> class.
        /// </summary>
        /// <param name="itemBase">The base <see cref="Scp244DeployablePickup"/> class.</param>
        public Scp244Pickup(Scp244DeployablePickup itemBase)
            : base(itemBase)
        {
            Base = itemBase;
        }

        /// <summary>
        /// Gets the <see cref="Scp244DeployablePickup"/> that this class is encapsulating.
        /// </summary>
        public new Scp244DeployablePickup Base { get; }
    }
}
