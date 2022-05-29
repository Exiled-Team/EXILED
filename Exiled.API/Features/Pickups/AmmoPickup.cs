// -----------------------------------------------------------------------
// <copyright file="AmmoPickup.cs" company="Exiled Team">
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

    using NWAmmo = InventorySystem.Items.Firearms.Ammo.AmmoPickup;

    /// <summary>
    /// A wrapper class for SCP-330 bags.
    /// </summary>
    public class AmmoPickup : Pickup
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AmmoPickup"/> class.
        /// </summary>
        /// <param name="itemBase">The base <see cref="NWAmmo"/> class.</param>
        public AmmoPickup(NWAmmo itemBase)
            : base(itemBase)
        {
            Base = itemBase;
        }

        /// <summary>
        /// Gets the <see cref="NWAmmo"/> that this class is encapsulating.
        /// </summary>
        public new NWAmmo Base { get; }
    }
}
