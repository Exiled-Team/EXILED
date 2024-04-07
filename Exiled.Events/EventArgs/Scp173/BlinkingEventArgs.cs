// -----------------------------------------------------------------------
// <copyright file="BlinkingEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Scp173
{
    using System.Collections.Generic;

    using API.Features;

    using Interfaces;

    using PlayerRoles.PlayableScps.Scp173;

    using UnityEngine;

    using Scp173Role = API.Features.Roles.Scp173Role;

    /// <summary>
    /// Contains all information before a players blink near SCP-173.
    /// </summary>
    public class BlinkingEventArgs : IScp173Event, IDeniableEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BlinkingEventArgs" /> class.
        /// </summary>
        /// <param name="player">
        /// <inheritdoc cref="Player" />
        /// </param>
        /// <param name="targets">
        /// <inheritdoc cref="Targets" />
        /// </param>
        /// <param name="blinkPos">
        /// <inheritdoc cref="BlinkPosition" />
        /// </param>
        public BlinkingEventArgs(Player player, List<Player> targets, Vector3 blinkPos)
        {
            Player = player;
            Scp173 = player.Role.As<Scp173Role>();
            BlinkPosition = blinkPos;
            Targets = targets;
            BlinkCooldown = Scp173BlinkTimer.CooldownBaseline;
        }

        /// <summary>
        /// Gets or sets the location the player is blinking to.
        /// </summary>
        public Vector3 BlinkPosition { get; set; }

        /// <summary>
        /// Gets or sets how long the blink cooldown will last.
        /// </summary>
        public float BlinkCooldown { get; set; }

        /// <summary>
        /// Gets a <see cref="IEnumerable{T}" /> of players who have triggered SCP-173.
        /// </summary>
        public IReadOnlyList<Player> Targets { get; }

        /// <summary>
        /// Gets or sets a value indicating whether or not the player is allowed to blink.
        /// </summary>
        public bool IsAllowed { get; set; } = true;

        /// <summary>
        /// Gets the player who controlling SCP-173.
        /// </summary>
        public Player Player { get; }

        /// <inheritdoc/>
        public Scp173Role Scp173 { get; }
    }
}
