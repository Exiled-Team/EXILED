// -----------------------------------------------------------------------
// <copyright file="TriggeringTeslaEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs {
    using System;

    using Exiled.API.Features;

    /// <summary>
    /// Contains all informations before triggering a tesla.
    /// </summary>
    public class TriggeringTeslaEventArgs : EventArgs {
        /// <summary>
        /// Initializes a new instance of the <see cref="TriggeringTeslaEventArgs"/> class.
        /// </summary>
        /// <param name="player"><inheritdoc cref="Player"/></param>
        /// <param name="tesla"><inheritdoc cref="Tesla"/></param>
        /// <param name="isInHurtingRange"><inheritdoc cref="IsInHurtingRange"/></param>
        /// <param name="isTriggerable"><inheritdoc cref="IsTriggerable"/></param>
        /// <param name="isInIdleRange"><inheritdoc cref="IsInIdleRange"/></param>
        public TriggeringTeslaEventArgs(Player player, TeslaGate tesla, bool isInHurtingRange, bool isTriggerable, bool isInIdleRange = true) {
            Player = player;
            Tesla = tesla;
            IsInHurtingRange = isInHurtingRange;
            IsTriggerable = isTriggerable;
            IsInIdleRange = isInIdleRange;
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
        public bool IsInIdleRange { get; set; }
    }
}
