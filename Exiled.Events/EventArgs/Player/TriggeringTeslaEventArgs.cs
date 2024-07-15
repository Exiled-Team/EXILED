// -----------------------------------------------------------------------
// <copyright file="TriggeringTeslaEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Player
{
    using API.Features;

    using Interfaces;

    /// <summary>
    /// Contains all information before triggering a tesla.
    /// </summary>
    public class TriggeringTeslaEventArgs : IPlayerEvent, ITeslaEvent, IDeniableEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TriggeringTeslaEventArgs" /> class.
        /// </summary>
        /// <param name="player">
        /// <inheritdoc cref="Player" />
        /// </param>
        /// <param name="teslaGate">
        /// <inheritdoc cref="Tesla" />
        /// </param>
        public TriggeringTeslaEventArgs(Player player, TeslaGate teslaGate)
        {
            Player = player;
            Tesla = teslaGate;
            IsInHurtingRange = Tesla.IsPlayerInHurtRange(player);
            IsTriggerable = Tesla.IsPlayerInTriggerRange(player);
        }

        /// <summary>
        /// Gets or sets a value indicating whether the player is in hurting range.
        /// </summary>
        public bool IsInHurtingRange { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the player will cause the tesla going to be idle.
        /// </summary>
        public bool IsInIdleRange { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether the player will cause the tesla going to be activated.
        /// </summary>
        public bool IsTriggerable { get; set; }

        /// <summary>
        /// Gets the player who triggered the tesla.
        /// </summary>
        public Player Player { get; }

        /// <summary>
        /// Gets the Tesla.
        /// </summary>
        public TeslaGate Tesla { get; }

        /// <summary>
        /// Gets or sets a value indicating whether the player will be detected by the tesla.
        /// </summary>
        public bool IsAllowed { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether the tesla will be deactivated.
        /// </summary>
        public bool DisableTesla { get; set; }
    }
}