// -----------------------------------------------------------------------
// <copyright file="ChangingMicroHIDStateEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Player
{
    using API.Features;
    using API.Features.Items;

    using Interfaces;

    using InventorySystem.Items.MicroHID;

    /// <summary>
    ///     Contains all information before MicroHID state is changed.
    /// </summary>
    public class ChangingMicroHIDStateEventArgs : IPlayerEvent, IDeniableEvent
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="ChangingMicroHIDStateEventArgs" /> class.
        /// </summary>
        /// <param name="player">
        ///     <inheritdoc cref="Player" />
        /// </param>
        /// <param name="microHID">
        ///     <inheritdoc cref="MicroHID" />
        /// </param>
        /// <param name="oldState">
        ///     <inheritdoc cref="OldState" />
        /// </param>
        /// <param name="newState">
        ///     <inheritdoc cref="NewState" />
        /// </param>
        /// <param name="isAllowed">
        ///     <inheritdoc cref="IsAllowed" />
        /// </param>
        public ChangingMicroHIDStateEventArgs(Player player, MicroHIDItem microHID, HidState oldState, HidState newState, bool isAllowed = true)
        {
            Player = player;
            MicroHID = (MicroHid)Item.Get(microHID);
            OldState = oldState;
            NewState = newState;
            IsAllowed = isAllowed;
        }

        /// <summary>
        ///     Gets the MicroHID instance.
        /// </summary>
        public MicroHid MicroHID { get; }

        /// <summary>
        ///     Gets the old MicroHID state.
        /// </summary>
        public HidState OldState { get; }

        /// <summary>
        ///     Gets or sets the new MicroHID state.
        /// </summary>
        public HidState NewState { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether the MicroHID state can be changed or not.
        /// </summary>
        public bool IsAllowed { get; set; }

        /// <summary>
        ///     Gets the player who's using the MicroHID.
        /// </summary>
        public Player Player { get; }
    }
}