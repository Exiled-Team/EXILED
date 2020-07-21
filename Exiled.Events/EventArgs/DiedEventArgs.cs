// -----------------------------------------------------------------------
// <copyright file="DiedEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs
{
    using System;

    using Exiled.API.Features;

    /// <summary>
    /// Contains all informations after a player dies.
    /// </summary>
    public class DiedEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DiedEventArgs"/> class.
        /// </summary>
        /// <param name="killer"><inheritdoc cref="Killer"/></param>
        /// <param name="target"><inheritdoc cref="Target"/></param>
        /// <param name="hitInformations"><inheritdoc cref="HitInformations"/></param>
        public DiedEventArgs(Player killer, Player target, PlayerStats.HitInfo hitInformations)
        {
            Killer = killer;
            Target = target;
            HitInformations = hitInformations;
        }

        /// <summary>
        /// Gets the killer player.
        /// </summary>
        public Player Killer { get; }

        /// <summary>
        /// Gets the killed player.
        /// </summary>
        public Player Target { get; }

        /// <summary>
        /// Gets or sets the hit informations.
        /// </summary>
        public PlayerStats.HitInfo HitInformations { get; set; }
    }
}
