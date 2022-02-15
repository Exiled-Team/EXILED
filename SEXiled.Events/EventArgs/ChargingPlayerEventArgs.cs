// -----------------------------------------------------------------------
// <copyright file="ChargingPlayerEventArgs.cs" company="SEXiled Team">
// Copyright (c) SEXiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace SEXiled.Events.EventArgs
{
    using System;

    using SEXiled.API.Features;

    using Scp096 = PlayableScps.Scp096;

    /// <summary>
    /// Contains all informations before SCP-096 charges a player.
    /// </summary>
    public class ChargingPlayerEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ChargingPlayerEventArgs"/> class.
        /// </summary>
        /// <param name="scp096"><inheritdoc cref="Scp096"/></param>
        /// <param name="player"><inheritdoc cref="Player"/></param>
        /// <param name="victim"><inheritdoc cref="Victim"/></param>
        /// <param name="isTarget"><inheritdoc cref="IsTarget"/></param>
        /// <param name="damage"><inheritdoc cref="Damage"/></param>
        public ChargingPlayerEventArgs(Scp096 scp096, Player player, Player victim, bool isTarget, float damage)
        {
            Scp096 = scp096;
            Player = player;
            Victim = victim;
            IsTarget = isTarget;
            Damage = damage;
            EndCharge = IsTarget;
        }

        /// <summary>
        /// Gets the SCP-096 instance.
        /// </summary>
        public Scp096 Scp096 { get; }

        /// <summary>
        /// Gets the player who is controlling SCP-096.
        /// </summary>
        public Player Player { get; }

        /// <summary>
        /// Gets the player who SCP-096 is charging.
        /// </summary>
        public Player Victim { get; }

        /// <summary>
        /// Gets a value indicating whether the target is one of SCP-096's targets.
        /// </summary>
        public bool IsTarget { get; }

        /// <summary>
        /// Gets or sets the inflicted damage.
        /// </summary>
        public float Damage { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether SCP-096's charge should be ended next frame.
        /// </summary>
        public bool EndCharge { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether or not SCP-096 can hit the target.
        /// </summary>
        public bool IsAllowed { get; set; } = true;
    }
}
