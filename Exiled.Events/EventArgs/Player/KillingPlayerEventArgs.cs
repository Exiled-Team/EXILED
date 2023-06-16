// -----------------------------------------------------------------------
// <copyright file="KillingPlayerEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Player
{
    using Exiled.API.Features.DamageHandlers;
    using Interfaces;

    using PlayerStatsSystem;

    /// <summary>
    /// Contains all information before player data to kill player is sent.
    /// </summary>
    public class KillingPlayerEventArgs : IPlayerEvent, IDamageEvent
    {
        /// <summary>
        ///  Initializes a new instance of the <see cref="KillingPlayerEventArgs"/> class.
        /// </summary>
        /// <param name="player"> Current player. </param>
        /// <param name="handler"> DamageHandler instance. </param>
        public KillingPlayerEventArgs(API.Features.Player player, PlayerStatsSystem.DamageHandlerBase handler)
        {
            Player = player;
            DamageHandler = new CustomDamageHandler(Player, handler);
        }

        /// <inheritdoc />
        public API.Features.Player Player { get; set; }

        /// <inheritdoc />
        public CustomDamageHandler DamageHandler { get; set; }
    }
}