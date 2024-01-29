// -----------------------------------------------------------------------
// <copyright file="UsingMicroHIDEnergyEventArgs.cs" company="Exiled Team">
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
    ///     Contains all information before MicroHID energy is changed.
    /// </summary>
    public class UsingMicroHIDEnergyEventArgs : IPlayerEvent, IDeniableEvent, IItemEvent
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="UsingMicroHIDEnergyEventArgs" /> class.
        /// </summary>
        /// <param name="player">
        ///     <inheritdoc cref="Player" />
        /// </param>
        /// <param name="microHIDitem">
        ///     <inheritdoc cref="MicroHID" />
        /// </param>
        /// <param name="currentState">
        ///     <inheritdoc cref="CurrentState" />
        /// </param>
        /// <param name="drain">
        ///     <inheritdoc cref="Drain" />
        /// </param>
        /// <param name="isAllowed">
        ///     <inheritdoc cref="IsAllowed" />
        /// </param>
        public UsingMicroHIDEnergyEventArgs(Player player, MicroHIDItem microHIDitem, HidState currentState, float drain, bool isAllowed = true)
        {
            Player = player;
            MicroHID = (MicroHid)Item.Get(microHIDitem);
            CurrentState = currentState;
            Drain = drain;
            IsAllowed = isAllowed;
        }

        /// <summary>
        ///     Gets the MicroHID instance.
        /// </summary>
        public MicroHid MicroHID { get; }

        /// <inheritdoc/>
        public Item Item => MicroHID;

        /// <summary>
        ///     Gets the current state of the MicroHID.
        /// </summary>
        public HidState CurrentState { get; }

        /// <summary>
        ///     Gets or sets the MicroHID energy drain.
        /// </summary>
        public float Drain { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether the MicroHID energy can be changed or not.
        /// </summary>
        public bool IsAllowed { get; set; }

        /// <summary>
        ///     Gets the player who's using the MicroHID.
        /// </summary>
        public Player Player { get; }
    }
}