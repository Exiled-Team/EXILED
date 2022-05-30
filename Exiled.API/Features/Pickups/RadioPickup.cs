// -----------------------------------------------------------------------
// <copyright file="RadioPickup.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Pickups
{
    using Exiled.API.Enums;
    using Exiled.API.Features.Items;

    using BaseRadio = InventorySystem.Items.Radio.RadioPickup;

    /// <summary>
    /// A wrapper class for SCP-330 bags.
    /// </summary>
    public class RadioPickup : Pickup
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RadioPickup"/> class.
        /// </summary>
        /// <param name="itemBase">The base <see cref="BaseRadio"/> class.</param>
        public RadioPickup(BaseRadio itemBase)
            : base(itemBase)
        {
            Base = itemBase;
        }

        /// <summary>
        /// Gets the <see cref="BaseRadio"/> that this class is encapsulating.
        /// </summary>
        public new BaseRadio Base { get; }

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

        /// <summary>
        /// Returns the AmmoPickup in a human readable format.
        /// </summary>
        /// <returns>A string containing AmmoPickup related data.</returns>
        public override string ToString() => $"{Type} ({Serial}) [{Weight}] *{Scale}* |{Energy}| -{Range}- /{IsActive}/";
    }
}
