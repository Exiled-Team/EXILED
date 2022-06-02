// -----------------------------------------------------------------------
// <copyright file="RadioPickup.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Pickups
{
    using Exiled.API.Enums;

    using BaseRadio = InventorySystem.Items.Radio.RadioPickup;

    /// <summary>
    /// A wrapper class for Radio.
    /// </summary>
    public class RadioPickup : Pickup
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RadioPickup"/> class.
        /// </summary>
        /// <param name="itemBase">The base <see cref="BaseRadio"/> class.</param>
        internal RadioPickup(BaseRadio itemBase)
            : base(itemBase)
        {
            Base = pickupBase;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RadioPickup"/> class.
        /// </summary>
        internal RadioPickup()
            : base(ItemType.Radio)
        {
            Base = (BaseRadio)((Pickup)this).Base;
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
        /// Gets or sets the <see cref="RadioRange"/>.
        /// </summary>
        public RadioRange Range
        {
            get => (RadioRange)Base.SavedRange;
            set => Base.SavedRange = (int)value;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the radio is active.
        /// </summary>
        public bool IsActive
        {
            get => Base.SavedEnabled;
            set => Base.SavedEnabled = value;
        }

        /// <summary>
        /// Returns the RadioPickup in a human readable format.
        /// </summary>
        /// <returns>A string containing RadioPickup related data.</returns>
        public override string ToString() => $"{Type} ({Serial}) [{Weight}] *{Scale}* |{Energy}| -{Range}- /{IsActive}/";
    }
}
