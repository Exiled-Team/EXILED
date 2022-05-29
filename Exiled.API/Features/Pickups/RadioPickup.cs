// -----------------------------------------------------------------------
// <copyright file="RadioPickup.cs" company="Exiled Team">
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

    using NWRadio = InventorySystem.Items.Radio.RadioPickup;

    /// <summary>
    /// A wrapper class for SCP-330 bags.
    /// </summary>
    public class RadioPickup : Pickup
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RadioPickup"/> class.
        /// </summary>
        /// <param name="itemBase">The base <see cref="NWRadio"/> class.</param>
        public RadioPickup(NWRadio itemBase)
            : base(itemBase)
        {
            Base = itemBase;
        }

        /// <summary>
        /// Gets the <see cref="NWRadio"/> that this class is encapsulating.
        /// </summary>
        public new NWRadio Base { get; }
    }
}
