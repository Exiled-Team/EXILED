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
    /// Contains some info of the event.
    /// </summary>
    public class UsingRadioBatteryEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UsingRadioBatteryEventArgs"/> class.
        /// </summary>
        /// <param name="player"><inheritdoc cref="Player"/></param>
        /// <param name="radio"><inheritdoc cref="Radio"/></param>
        /// <param name="isTransmitting"><inheritdoc cref="IsTransmitting"/></param>
        /// <param name="isAllowed"><inheritdoc cref="IsAllowed"/></param>
        public UsingRadioBatteryEventArgs(Player player, Radio radio, bool isTransmitting, bool isAllowed = true)
        {
            Player = player;
            Radio = radio;
            IsTransmitting = isTransmitting;
            IsAllowed = isAllowed;
        }

        public Player Player { get; }

        public Radio Radio { get; }

        public bool IsTransmitting { get; }

        public bool IsAllowed { get; set; }

    }
}
