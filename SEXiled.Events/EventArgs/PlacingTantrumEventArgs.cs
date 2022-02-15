// -----------------------------------------------------------------------
// <copyright file="PlacingTantrumEventArgs.cs" company="SEXiled Team">
// Copyright (c) SEXiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace SEXiled.Events.EventArgs
{
    using System;

    using SEXiled.API.Features;

    using UnityEngine;

    /// <summary>
    /// Contains all information before the tantrum is placed.
    /// </summary>
    public class PlacingTantrumEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PlacingTantrumEventArgs"/> class.
        /// </summary>
        /// <param name="scp173"><inheritdoc cref="Scp173"/></param>
        /// <param name="player"><inheritdoc cref="Player"/></param>
        /// <param name="gameObject"><inheritdoc cref="GameObject"/></param>
        /// <param name="cooldown"><inheritdoc cref="Cooldown"/></param>
        /// <param name="isAllowed"><inheritdoc cref="IsAllowed"/></param>
        public PlacingTantrumEventArgs(PlayableScps.Scp173 scp173, Player player, GameObject gameObject, float cooldown, bool isAllowed = true)
        {
            Scp173 = scp173;
            Player = player;
            GameObject = gameObject;
            Cooldown = cooldown;
            IsAllowed = isAllowed;
        }

        /// <summary>
        /// Gets the player's <see cref="PlayableScps.Scp173"/> instance.
        /// </summary>
        public PlayableScps.Scp173 Scp173 { get; }

        /// <summary>
        /// Gets the player who's placing the tantrum.
        /// </summary>
        public Player Player { get; }

        /// <summary>
        /// Gets the tantrum <see cref="UnityEngine.GameObject"/>.
        /// </summary>
        public GameObject GameObject { get; }

        /// <summary>
        /// Gets or sets the tantrum cooldown.
        /// </summary>
        public float Cooldown { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether or not the tantrum can be placed.
        /// </summary>
        public bool IsAllowed { get; set; }
    }
}
