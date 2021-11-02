// -----------------------------------------------------------------------
// <copyright file="PunchingEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs
{
    using System;
    using System.Collections.Generic;

    using Exiled.API.Features;

    /// <summary>
    /// Contains all information before a player punches.
    /// </summary>
    public class PunchingEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PunchingEventArgs"/> class.
        /// </summary>
        /// <param name="attacker"><inheritdoc cref="Attacker"/></param>
        /// <param name="targets"><inheritdoc cref="Targets"/></param>
        /// <param name="isAllowed"><inheritdoc cref="IsAllowed"/></param>
        public PunchingEventArgs(Player attacker, List<Player> targets, bool isAllowed = true)
        {
            Attacker = attacker;
            Targets = targets;
            IsAllowed = isAllowed;
        }

        /// <summary>
        /// Gets the player who's punching.
        /// </summary>
        public Player Attacker { get; }

        /// <summary>
        /// Gets the list of targets.
        /// </summary>
        public List<Player> Targets { get; }

        /// <summary>
        /// Gets or sets a value indicating whether or not the attacker is able to punch.
        /// </summary>
        public bool IsAllowed { get; set; }
    }
}
