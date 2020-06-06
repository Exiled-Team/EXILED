// -----------------------------------------------------------------------
// <copyright file="TriggeringTeslaEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs
{
    using System;
    using Exiled.API.Features;

    /// <summary>
    /// Contains all informations before triggering a tesla.
    /// </summary>
    public class TriggeringTeslaEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TriggeringTeslaEventArgs"/> class.
        /// </summary>
        /// <param name="player"><inheritdoc cref="Player"/></param>
        /// <param name="isInHurtingRange"><inheritdoc cref="IsInHurtingRange"/></param>
        /// <param name="isTriggerable"><inheritdoc cref="IsTriggerable"/></param>
        public TriggeringTeslaEventArgs(Player player, bool isInHurtingRange, bool isTriggerable = true)
        {
            Player = player;
            IsInHurtingRange = isInHurtingRange;
            IsTriggerable = isTriggerable;
        }

        /// <summary>
        /// Gets the player who triggered the tesla.
        /// </summary>
        public Player Player { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether the player is in hurting range or not.
        /// </summary>
        public bool IsInHurtingRange { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the tesla is going to be triggered or not.
        /// </summary>
        public bool IsTriggerable { get; set; }
    }
}
