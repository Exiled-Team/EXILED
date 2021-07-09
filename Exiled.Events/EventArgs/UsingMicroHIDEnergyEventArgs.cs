// -----------------------------------------------------------------------
// <copyright file="UsingMicroHIDEnergyEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs
{
    using System;

    using Exiled.API.Features;

    /// <summary>
    /// Contains all information before MicroHID energy is changed.
    /// </summary>
    public class UsingMicroHIDEnergyEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UsingMicroHIDEnergyEventArgs"/> class.
        /// </summary>
        /// <param name="player"><inheritdoc cref="Player"/></param>
        /// <param name="microHID"><inheritdoc cref="MicroHID"/></param>
        /// <param name="currentState"><inheritdoc cref="CurrentState"/></param>
        /// <param name="oldValue"><inheritdoc cref="OldValue"/></param>
        /// <param name="newValue"><inheritdoc cref="NewValue"/></param>
        /// <param name="isAllowed"><inheritdoc cref="IsAllowed"/></param>
        public UsingMicroHIDEnergyEventArgs(Player player, MicroHID microHID, MicroHID.MicroHidState currentState, float oldValue, float newValue, bool isAllowed = true)
        {
            Player = player;
            MicroHID = microHID;
            CurrentState = currentState;
            OldValue = oldValue;
            NewValue = newValue;
            IsAllowed = isAllowed;
        }

        /// <summary>
        /// Gets the player who's using the MicroHID.
        /// </summary>
        public Player Player { get; }

        /// <summary>
        /// Gets the MicroHID instance.
        /// </summary>
        public MicroHID MicroHID { get; }

        /// <summary>
        /// Gets the current state of the MicroHID.
        /// </summary>
        public MicroHID.MicroHidState CurrentState { get; }

        /// <summary>
        /// Gets the old MicroHID energy value.
        /// </summary>
        public float OldValue { get; }

        /// <summary>
        /// Gets or sets the new MicroHID energy value.
        /// </summary>
        public float NewValue { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the MicroHID energy can be changed or not.
        /// </summary>
        public bool IsAllowed { get; set; }
    }
}
