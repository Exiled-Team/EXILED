// -----------------------------------------------------------------------
// <copyright file="TriggeringTeslaEventArgs.cs" company="SEXiled Team">
// Copyright (c) SEXiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace SEXiled.Events.EventArgs
{
    using System;

    using SEXiled.API.Features;

    /// <summary>
    /// Contains all informations before triggering a tesla.
    /// </summary>
    public class TriggeringTeslaEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TriggeringTeslaEventArgs"/> class.
        /// </summary>
        /// <param name="player"><inheritdoc cref="Player"/></param>
        /// <param name="teslaGate"><inheritdoc cref="Tesla"/></param>
        /// <param name="isInHurtingRange"><inheritdoc cref="IsInHurtingRange"/></param>
        /// <param name="isTriggerable"><inheritdoc cref="IsTriggerable"/></param>
        public TriggeringTeslaEventArgs(Player player, TeslaGate teslaGate, bool isInHurtingRange, bool isTriggerable)
        {
            Player = player;
            Tesla = teslaGate;
            IsInHurtingRange = isInHurtingRange;
            IsTriggerable = isTriggerable;
        }

        /// <summary>
        /// Gets the player who triggered the tesla.
        /// </summary>
        public Player Player { get; }

        /// <summary>
        /// Gets the Tesla.
        /// </summary>
        public TeslaGate Tesla { get; }

        /// <summary>
        /// Gets or sets a value indicating whether or not the player is in hurting range.
        /// </summary>
        public bool IsInHurtingRange { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether or not the tesla is going to be activated.
        /// </summary>
        public bool IsTriggerable { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether or not the is going to be idle.
        /// </summary>
        public bool IsInIdleRange { get; set; } = true;
    }
}
