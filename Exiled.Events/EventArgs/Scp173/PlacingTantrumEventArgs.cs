// -----------------------------------------------------------------------
// <copyright file="PlacingTantrumEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Scp173
{
    using Exiled.API.Features;
    using Exiled.Events.EventArgs.Interfaces;

    using UnityEngine;

    using Scp173 = PlayableScps.Scp173;

    /// <summary>
    ///     Contains all information before the tantrum is placed.
    /// </summary>
    public class PlacingTantrumEventArgs : IPlayerEvent, IDeniableEvent, IEnvironmentalHazardEvent
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="PlacingTantrumEventArgs" /> class.
        /// </summary>
        /// <param name="scp173">
        ///     <inheritdoc cref="Scp173" />
        /// </param>
        /// <param name="player">
        ///     <inheritdoc cref="Player" />
        /// </param>
        /// <param name="gameObject">
        ///     <inheritdoc cref="EnvironmentalHazard" />
        /// </param>
        /// <param name="cooldown">
        ///     <inheritdoc cref="Cooldown" />
        /// </param>
        /// <param name="isAllowed">
        ///     <inheritdoc cref="IsAllowed" />
        /// </param>
        public PlacingTantrumEventArgs(Scp173 scp173, Player player, GameObject gameObject, float cooldown, bool isAllowed = true)
        {
            Scp173 = scp173;
            Player = player;
            EnvironmentalHazard = gameObject.GetComponent<EnvironmentalHazard>();
            Cooldown = cooldown;
            IsAllowed = isAllowed;
        }

        /// <summary>
        ///     Gets the player's <see cref="PlayableScps.Scp173" /> instance.
        /// </summary>
        public Scp173 Scp173 { get; }

        /// <summary>
        ///     Gets the tantrum <see cref="EnvironmentalHazard" />.
        /// </summary>
        public EnvironmentalHazard EnvironmentalHazard { get; }

        /// <summary>
        ///     Gets or sets the tantrum cooldown.
        /// </summary>
        public float Cooldown { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether or not the tantrum can be placed.
        /// </summary>
        public bool IsAllowed { get; set; }

        /// <summary>
        ///     Gets the player who's placing the tantrum.
        /// </summary>
        public Player Player { get; }
    }
}
