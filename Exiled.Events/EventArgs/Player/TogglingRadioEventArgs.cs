// -----------------------------------------------------------------------
// <copyright file="TogglingRadioEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Player
{
    using API.Features;

    using Exiled.API.Features.Items;
    using Interfaces;
    using InventorySystem.Items.Radio;

    /// <summary>
    ///     Contains all information before radio preset is changed.
    /// </summary>
    public class TogglingRadioEventArgs : IPlayerEvent, IDeniableEvent, IItemEvent
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="TogglingRadioEventArgs" /> class.
        /// </summary>
        /// <param name="player">
        ///     <inheritdoc cref="Player" />
        /// </param>
        /// <param name="radio">
        ///     <inheritdoc cref="Radio" />
        /// </param>
        /// <param name="newState">
        ///     <inheritdoc cref="NewState" />
        /// </param>
        /// <param name="isAllowed">
        ///     <inheritdoc cref="IsAllowed" />
        /// </param>
        public TogglingRadioEventArgs(Player player, RadioItem radio, bool newState, bool isAllowed = true)
        {
            Player = player;
            Radio = (Radio)Item.Get(radio);
            NewState = newState;
            IsAllowed = isAllowed;
        }

        /// <summary>
        ///     Gets the <see cref="API.Features.Items.Radio" /> which is being used.
        /// </summary>
        public Radio Radio { get; }

        /// <inheritdoc/>
        public Item Item => Radio;

        /// <summary>
        ///     Gets or sets a value indicating whether new state of the radio.
        /// </summary>
        public bool NewState { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether the radio preset can be changed or not.
        /// </summary>
        public bool IsAllowed { get; set; }

        /// <summary>
        ///     Gets the player who's using the radio.
        /// </summary>
        public Player Player { get; }
    }
}
