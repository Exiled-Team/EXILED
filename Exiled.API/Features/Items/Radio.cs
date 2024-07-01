// -----------------------------------------------------------------------
// <copyright file="Radio.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Items
{
    using Enums;
    using Exiled.API.Features.Core;
    using Exiled.API.Features.Core.Attributes;
    using Exiled.API.Interfaces;
    using InventorySystem.Items.Radio;
    using Structs;
    using VoiceChat.Playbacks;

    /// <summary>
    /// A wrapper class for <see cref="RadioItem"/>.
    /// </summary>
    public class Radio : Item, IWrapper<RadioItem>
    {
        private readonly ConstProperty<float> drainMultiplier = new(RadioItem.DrainMultiplier, new[] { typeof(RadioItem) });

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
            : this((RadioItem)Server.Host.Inventory.CreateItemInstance(new(ItemType.Radio, 0), false))
        {
        }

        /// <summary>
        /// Gets the <see cref="RadioItem"/> that this class is encapsulating.
        /// </summary>
        public new RadioItem Base { get; }

        /// <summary>
        /// Gets or sets the percentage of the radio's battery, between <c>0-100</c>.
        /// </summary>
        [EProperty(category: nameof(Radio))]
        public byte BatteryLevel
        {
            get => Base.BatteryPercent;
            set => Base.BatteryPercent = value;
        }

        /// <summary>
        /// Gets or sets the current <see cref="RadioRange"/>.
        /// </summary>
        [EProperty(category: nameof(Radio))]
        public RadioRange Range
        {
            get => (RadioRange)Base._rangeId;
            set => Base._rangeId = (byte)value;
        }

        /// <summary>
        /// Gets or sets the <see cref="RadioRangeSettings"/> for the current <see cref="Range"/>.
        /// </summary>
        [EProperty(category: nameof(Radio))]
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
        /// Gets or sets a value indicating whether the radio is enabled or not.
        /// </summary>
        [EProperty(category: nameof(Radio))]
        public bool IsEnabled
        {
            get => Base._enabled;
            set => Base._enabled = value;
        }

        /// <summary>
        /// Gets a value indicating whether or not the radio is transmitting.
        /// </summary>
        [EProperty(readOnly: true, category: nameof(Radio))]
        public bool IsTransmitting => PersonalRadioPlayback.IsTransmitting(Owner.ReferenceHub);

        /// <summary>
        /// Gets or sets a multiplier for draining radio.
        /// </summary>
        public float DrainMultiplier
        {
            get => drainMultiplier;
            set => drainMultiplier.Value = value;
        }

        /// <summary>
        /// Sets the <see cref="RadioRangeSettings"/> of the given <paramref name="range"/>.
        /// </summary>
        /// <param name="range">The <see cref="RadioRange"/> to modify.</param>
        /// <param name="settings">The new settings of the specified range.</param>
        public void SetRangeSettings(RadioRange range, RadioRangeSettings settings)
        {
            Base.Ranges[(int)range] = new RadioRangeMode
            {
                MaximumRange = settings.MaxRange,
                MinuteCostWhenIdle = settings.IdleUsage,
                MinuteCostWhenTalking = settings.TalkingUsage,
            };
        }

        /// <summary>
        /// Clones current <see cref="Radio"/> object.
        /// </summary>
        /// <returns> New <see cref="Radio"/> object. </returns>
        public override Item Clone() => new Radio()
        {
            BatteryLevel = BatteryLevel,
            Range = Range,
            RangeSettings = RangeSettings,
        };

        /// <summary>
        /// Returns the Radio in a human readable format.
        /// </summary>
        /// <returns>A string containing Radio-related data.</returns>
        public override string ToString() => $"{Type} ({Serial}) [{Weight}] *{Scale}* |{Range}| -{BatteryLevel}-";

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="oldOwner">old <see cref="Item"/> owner.</param>
        /// <param name="newOwner">new <see cref="Item"/> owner.</param>
        internal override void ChangeOwner(Player oldOwner, Player newOwner) => Base.Owner = newOwner.ReferenceHub;
    }
}