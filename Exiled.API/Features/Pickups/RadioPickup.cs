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

    using Exiled.API.Enums;

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

        /// <summary>
        /// Gets or sets the Radio Energy.
        /// </summary>
        public float Energy
        {
            get => Base.SavedBattery;
            set => Base.SavedBattery = value;
        }

        /// <summary>
        /// Gets or sets the Radio Range.
        /// </summary>
        public RadioRange Range
        {
            get => (RadioRange)Base.SavedRange;
            set => Base.SavedRange = (int)value;
        }

        /// <summary>
        /// Gets or sets a value indicating whether is radio enable.
        /// </summary>
        public bool IsActive
        {
            get => Base.SavedEnabled;
            set => Base.SavedEnabled = value;
        }
    }
}
