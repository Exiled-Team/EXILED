// -----------------------------------------------------------------------
// <copyright file="Radio.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Items {
    using Exiled.API.Enums;
    using Exiled.API.Structs;

    using InventorySystem;
    using InventorySystem.Items;
    using InventorySystem.Items.Radio;

    /// <summary>
    /// A wrapper class for <see cref="InventorySystem.Items.Radio.RadioItem"/>.
    /// </summary>
    public class Radio : Item {
        /// <summary>
        /// Initializes a new instance of the <see cref="Radio"/> class.
        /// </summary>
        /// <param name="itemBase"><inheritdoc cref="Base"/></param>
        public Radio(RadioItem itemBase)
            : base(itemBase) {
            Base = itemBase;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Radio"/> class.
        /// </summary>
        /// <param name="type"><inheritdoc cref="Base"/></param>
        public Radio(ItemType type)
            : this((RadioItem)Server.Host.Inventory.CreateItemInstance(type, false)) {
        }

        /// <inheritdoc cref="Item.Base"/>
        public new RadioItem Base { get; }

        /// <summary>
        /// Gets or sets the percentage of the radio's battery.
        /// </summary>
        public byte BatteryLevel {
            get => Base.BatteryPercent;
            set => Base.BatteryPercent = value;
        }

        /// <summary>
        /// Gets or sets the current <see cref="RadioRange"/>.
        /// </summary>
        public RadioRange Range {
            get => (RadioRange)Base.CurRange;
            set => Base.CurRange = (int)value;
        }

        /// <summary>
        /// Gets or sets the <see cref="RadioRangeSettings"/> for the current <see cref="Range"/>.
        /// </summary>
        public RadioRangeSettings RangeSettings {
            get =>
                new RadioRangeSettings {
                    IdleUsage = Base.Ranges[(int)Range].MinuteCostWhenIdle,
                    TalkingUsage = Base.Ranges[(int)Range].MinuteCostWhenTalking,
                    MaxRange = Base.Ranges[(int)Range].MaximumRange,
                };
            set =>
                Base.Ranges[(int)Range] = new RadioRangeMode {
                    MaximumRange = value.MaxRange,
                    MinuteCostWhenIdle = value.IdleUsage,
                    MinuteCostWhenTalking = value.TalkingUsage,
                };
        }

        /// <summary>
        /// Turns off the radio.
        /// </summary>
        public void Disable() => Base._radio.ForceDisableRadio();

        /// <inheritdoc/>
        public override string ToString() {
            return $"{Type} ({Serial}) [{Weight}] *{Scale}* |{Range}| -{BatteryLevel}-";
        }
    }
}
