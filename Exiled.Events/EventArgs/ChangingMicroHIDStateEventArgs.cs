// -----------------------------------------------------------------------
// <copyright file="ChangingMicroHIDStateEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs
{
    using System;

    using Exiled.API.Features;

    /// <summary>
    /// Contains all information before MicroHID state is changed.
    /// </summary>
    public class ChangingMicroHIDStateEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ChangingMicroHIDStateEventArgs"/> class.
        /// </summary>
        /// <param name="player"><inheritdoc cref="Player"/></param>
        /// <param name="oldState"><inheritdoc cref="OldState"/></param>
        /// <param name="newState"><inheritdoc cref="NewState"/></param>
        /// <param name="isAllowed"><inheritdoc cref="IsAllowed"/></param>
        public ChangingMicroHIDStateEventArgs(Player player, MicroHID.MicroHidState oldState, MicroHID.MicroHidState newState, bool isAllowed = true)
        {
            Player = player;
            OldState = oldState;
            NewState = newState;
            IsAllowed = isAllowed;
        }

        /// <summary>
        /// Gets the player who's using the MicroHID.
        /// </summary>
        public Player Player { get; }

        /// <summary>
        /// Gets the old MicroHID state.
        /// </summary>
        public MicroHID.MicroHidState OldState { get; }

        /// <summary>
        /// Gets or sets the new MicroHID state.
        /// </summary>
        public MicroHID.MicroHidState NewState { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the MicroHID state can be changed or not.
        /// </summary>
        public bool IsAllowed { get; set; }
    }
}
