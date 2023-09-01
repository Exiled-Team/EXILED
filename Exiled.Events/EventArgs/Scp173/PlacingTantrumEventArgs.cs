// -----------------------------------------------------------------------
// <copyright file="PlacingTantrumEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Scp173
{
    using Exiled.API.Features;
    using Exiled.API.Features.Roles;
    using Exiled.Events.EventArgs.Interfaces;

    using Hazards;

    using PlayerRoles.PlayableScps.Scp173;
    using PlayerRoles.PlayableScps.Subroutines;

    using Scp173Role = API.Features.Roles.Scp173Role;

    /// <summary>
    ///     Contains all information before the tantrum is placed.
    /// </summary>
    public class PlacingTantrumEventArgs : IScp173Event, IDeniableEvent
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="PlacingTantrumEventArgs" /> class.
        /// </summary>
        /// <param name="player">
        ///     <inheritdoc cref="Player" />
        /// </param>
        /// <param name="tantrumHazard">
        ///     <inheritdoc cref="TantrumHazard" />
        /// </param>
        /// <param name="cooldown">
        ///     <inheritdoc cref="Cooldown" />
        /// </param>
        /// <param name="isAllowed">
        ///     <inheritdoc cref="IsAllowed" />
        /// </param>
        public PlacingTantrumEventArgs(Player player, TantrumEnvironmentalHazard tantrumHazard, AbilityCooldown cooldown, bool isAllowed = true)
        {
            Player = player;
            Scp173 = Player.Role.As<Scp173Role>();
            TantrumHazard = tantrumHazard;
            Cooldown = cooldown;
            IsAllowed = isAllowed;
        }

        /// <summary>
        ///     Gets the player's <see cref="Scp173Role" /> instance.
        /// </summary>
        public Scp173Role Scp173 { get; }

        /// <summary>
        ///     Gets the <see cref="TantrumEnvironmentalHazard" />.
        /// </summary>
        public TantrumEnvironmentalHazard TantrumHazard { get; }

        /// <summary>
        ///     Gets the tantrum <see cref="AbilityCooldown"/>.
        /// </summary>
        public AbilityCooldown Cooldown { get; }

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