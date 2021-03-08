// -----------------------------------------------------------------------
// <copyright file="UsingRadioBatteryEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs
{
    using System;

    using Exiled.API.Features;

    /// <summary>
    /// Contains all informations before radio battery charge is changed.
    /// </summary>
    public class UsingRadioBatteryEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UsingRadioBatteryEventArgs"/> class.
        /// </summary>
        /// <param name="player"><inheritdoc cref="Player"/></param>
        /// <param name="charge"><inheritdoc cref="Charge"/></param>
        /// <param name="isAllowed"><inheritdoc cref="IsAllowed"/></param>
        public UsingRadioBatteryEventArgs(Player player, float charge, bool isAllowed = true)
        {
            Player = player;
            Charge = charge;
            IsAllowed = isAllowed;
        }

        /// <summary>
        /// Gets the player who's using the radio.
        /// </summary>
        public Player Player { get; }

        /// <summary>
        /// Gets or sets the radio battery charge.
        /// </summary>
        public float Charge { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the radio battery charge can be changed or not.
        /// </summary>
        public bool IsAllowed { get; set; }
    }
}
