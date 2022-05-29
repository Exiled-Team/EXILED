// -----------------------------------------------------------------------
// <copyright file="FirearmPickup.cs" company="Exiled Team">
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

    using NWFirearm = InventorySystem.Items.Firearms.FirearmPickup;

    /// <summary>
    /// A wrapper class for SCP-330 bags.
    /// </summary>
    public class FirearmPickup : Pickup
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FirearmPickup"/> class.
        /// </summary>
        /// <param name="itemBase">The base <see cref="NWFirearm"/> class.</param>
        public FirearmPickup(NWFirearm itemBase)
            : base(itemBase)
        {
            Base = itemBase;
        }

        /// <summary>
        /// Gets the <see cref="NWFirearm"/> that this class is encapsulating.
        /// </summary>
        public new NWFirearm Base { get; }
    }
}
