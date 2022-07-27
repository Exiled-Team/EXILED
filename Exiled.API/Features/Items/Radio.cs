// -----------------------------------------------------------------------
// <copyright file="Radio.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Items
{
    using Exiled.API.Enums;
    using Exiled.API.Structs;

    using InventorySystem.Items.Radio;

    using MEC;

    /// <summary>
    /// A wrapper class for <see cref="RadioItem"/>.
    /// </summary>
    public class Radio : Item
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Radio"/> class.
        /// </summary>
        /// <param name="itemBase">The base <see cref="RadioItem"/> class.</param>
        public Radio(RadioItem itemBase)
            : base(itemBase)
        {
            Base = itemBase;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Radio"/> class, as well as a new Radio item.
        /// </summary>
        internal Radio()
            : this((RadioItem)Server.Host.Inventory.CreateItemInstance(ItemType.Radio, false))
        {
        }

        /// <summary>
        /// Gets the <see cref="RadioItem"/> that this class is encapsulating.
        /// </summary>
        public new RadioItem Base { get; }

        /// <summary>
        /// Gets or sets the percentage of the radio's battery.
        /// </summary>
        public byte BatteryLevel
        {
            get => Base.BatteryPercent;
            set => Base.BatteryPercent = value;
        }

        /// <summary>
        /// Gets or sets the current <see cref="RadioRange"/>.
        /// </summary>
        public RadioRange Range
        {
            get => (RadioRange)Base.CurRange;
            set => Base.CurRange = (int)value;
        }

        /// <summary>
        /// Gets or sets the <see cref="RadioRangeSettings"/> for the current <see cref="Range"/>.
        /// </summary>
        public RadioRangeSettings RangeSettings
        {
            get =>
                new()
                {
                    IdleUsage = Base.Ranges[(int)Range].MinuteCostWhenIdle,
                    TalkingUsage = Base.Ranges[(int)Range].MinuteCostWhenTalking,
                    MaxRange = Base.Ranges[(int)Range].MaximumRange,
                };
            set =>
                Base.Ranges[(int)Range] = new RadioRangeMode
                {
                    MaximumRange = value.MaxRange,
                    MinuteCostWhenIdle = value.IdleUsage,
                    MinuteCostWhenTalking = value.TalkingUsage,
                };
        }

        /// <summary>
        /// Turns off the radio.
        /// </summary>
        public void Disable() => Base._radio.ForceDisableRadio();

        /// <summary>
        /// Clones current <see cref="Radio"/> object.
        /// </summary>
        /// <returns> New <see cref="Radio"/> object. </returns>
        public override Item Clone()
        {
            Radio radio = new();

            Timing.CallDelayed(1f, () =>
            {
                radio.BatteryLevel = BatteryLevel;
                radio.Range = Range;
                radio.RangeSettings = RangeSettings;
            });
            return radio;
        }

        /// <summary>
        /// Returns the Radio in a human readable format.
        /// </summary>
        /// <returns>A string containing Radio-related data.</returns>
        public override string ToString() => $"{Type} ({Serial}) [{Weight}] *{Scale}* |{Range}| -{BatteryLevel}-";
    }
}
