// -----------------------------------------------------------------------
// <copyright file="UsingRadioBatteryEventArgs.cs" company="SEXiled Team">
// Copyright (c) SEXiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace SEXiled.Events.EventArgs
{
    using System;

    using SEXiled.API.Features;
    using SEXiled.API.Features.Items;

    using InventorySystem.Items.Radio;

    /// <summary>
    /// Contains all informations before radio battery charge is changed.
    /// </summary>
    public class UsingRadioBatteryEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UsingRadioBatteryEventArgs"/> class.
        /// </summary>
        /// <param name="radio"><inheritdoc cref="Radio"/></param>
        /// <param name="player"><inheritdoc cref="Player"/></param>
        /// <param name="drain"><inheritdoc cref="Drain"/></param>
        /// <param name="isAllowed"><inheritdoc cref="IsAllowed"/></param>
        public UsingRadioBatteryEventArgs(RadioItem radio, Player player, float drain, bool isAllowed = true)
        {
            Radio = (Radio)Item.Get(radio);
            Player = player;
            Drain = drain;
            IsAllowed = isAllowed;
        }

        /// <summary>
        /// Gets the <see cref="API.Features.Items.Radio"/> which is being used.
        /// </summary>
        public Radio Radio { get; }

        /// <summary>
        /// Gets the player who's using the radio.
        /// </summary>
        public Player Player { get; }

        /// <summary>
        /// Gets or sets the radio battery drain per second.
        /// </summary>
        public float Drain { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the radio battery charge can be changed or not.
        /// </summary>
        public bool IsAllowed { get; set; }
    }
}
