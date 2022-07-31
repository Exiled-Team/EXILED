// -----------------------------------------------------------------------
// <copyright file="ChangingRadioPresetEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Player
{
    using Exiled.API.Features;
    using Exiled.Events.EventArgs.Interfaces;
    using Exiled.Events.EventArgs.Interfaces.Item;

    /// <summary>
    ///     Contains all information before radio preset is changed.
    /// </summary>
    public class ChangingRadioPresetEventArgs : IPlayerEvent, IDeniableEvent
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="ChangingRadioPresetEventArgs" /> class.
        /// </summary>
        /// <param name="player">
        ///     <inheritdoc cref="Player" />
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
        public ChangingRadioPresetEventArgs(Player player, byte oldValue, byte newValue, bool isAllowed = true)
        {
            Player = player;
            OldValue = oldValue;
            NewValue = newValue;
            IsAllowed = isAllowed;
        }

        /// <summary>
        ///     Gets the old radio preset value.
        /// </summary>
        public byte OldValue { get; }

        /// <summary>
        ///     Gets or sets the new radio preset value.
        ///     <remarks>Client radio graphics won't sync with this value.</remarks>
        /// </summary>
        public byte NewValue { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether the radio preset can be changed or not.
        ///     <remarks>Client radio graphics won't sync with <see cref="OldValue" />.</remarks>
        /// </summary>
        public bool IsAllowed { get; set; }

        /// <summary>
        ///     Gets the player who's using the radio.
        /// </summary>
        public Player Player { get; }
    }
}
