// -----------------------------------------------------------------------
// <copyright file="KillingPlayerEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------
namespace Exiled.Events.EventArgs
{
    using System;

    using Exiled.API.Features;

    using PlayerStatsSystem;

    /// <summary>
    /// Contains all informations before player data to kill player is sent.
    /// </summary>
    public class KillingPlayerEventArgs : EventArgs
    {
        /// <summary>
        ///  Initializes a new instance of the <see cref="KillingPlayerEventArgs"/> class.
        /// </summary>
        /// <param name="player"> Current player. </param>
        /// <param name="handler"> DamageHandler instance. </param>
        public KillingPlayerEventArgs(Player player, ref PlayerStatsSystem.DamageHandlerBase handler)
        {
            this.Player = player;
            this.Handler = handler;
        }

        /// <summary>
        /// Gets or sets current player.
        /// </summary>
        public Player Player { get; set; }

        /// <summary>
        /// Gets or sets current Damage Handler.
        /// </summary>
        public DamageHandlerBase Handler { get; set; }
    }
}
