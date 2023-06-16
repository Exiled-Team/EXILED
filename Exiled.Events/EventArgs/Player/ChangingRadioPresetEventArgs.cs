// -----------------------------------------------------------------------
// <copyright file="ChangingRadioPresetEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Player
{
    using API.Features;

    using Exiled.API.Enums;
    using Exiled.API.Features.Items;
    using Interfaces;
    using InventorySystem.Items.Radio;

    using static InventorySystem.Items.Radio.RadioMessages;

    /// <summary>
    ///     Contains all information before radio preset is changed.
    /// </summary>
    public class ChangingRadioPresetEventArgs : IPlayerEvent, IItemEvent, IDeniableEvent
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="ChangingRadioPresetEventArgs" /> class.
        /// </summary>
        /// <param name="radioItem">
        ///     <inheritdoc cref="Radio" />
        /// </param>
        /// <param name="oldValue">
        ///     <inheritdoc cref="OldValue" />
        /// </param>
        /// <param name="newValue">
        ///     <inheritdoc cref="NewValue" />
        /// </param>
        /// <param name="isAllowed">
        ///     <inheritdoc cref="IsAllowed" />
        /// </param>
        public ChangingRadioPresetEventArgs(RadioItem radioItem, RadioRangeLevel oldValue, RadioRangeLevel newValue, bool isAllowed = true)
        {
            Radio = (Radio)Item.Get(radioItem);
            Player = Radio.Owner;
            OldValue = (RadioRange)oldValue;
            NewValue = (RadioRange)newValue;
            IsAllowed = isAllowed;
        }

        /// <summary>
        ///     Gets the Radio instance.
        /// </summary>
        public Radio Radio { get; }

        /// <inheritdoc />
        public Item Item => Radio;

        /// <summary>
        ///     Gets the old radio preset value.
        /// </summary>
        public RadioRange OldValue { get; }

        /// <summary>
        ///     Gets or sets the new radio preset value.
        ///     <remarks>Client radio graphics won't sync with this value.</remarks>
        /// </summary>
        public RadioRange NewValue { get; set; }

        /// <inheritdoc />
        /// <remarks>Client radio graphics won't sync with <see cref="OldValue" />.</remarks>
        public bool IsAllowed { get; set; }

        /// <inheritdoc />
        public Player Player { get; }
    }
}