// -----------------------------------------------------------------------
// <copyright file="EscapingEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Player
{
    using System.Collections.Generic;

    using API.Features;
    using Exiled.API.Enums;
    using Interfaces;

    using PlayerRoles;

    using Respawning;

    using static Escape;

    /// <summary>
    /// Contains all information before a player escapes.
    /// </summary>
    public class EscapingEventArgs : IPlayerEvent, IDeniableEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EscapingEventArgs" /> class.
        /// </summary>
        /// <param name="player">
        /// <inheritdoc cref="Player" />
        /// </param>
        /// <param name="newRole">
        /// <inheritdoc cref="NewRole" />
        /// </param>
        /// <param name="escapeScenario">
        /// <inheritdoc cref="EscapeScenario" />
        /// </param>
        public EscapingEventArgs(Player player, RoleTypeId newRole, EscapeScenarioType escapeScenario)
        {
            Player = player;
            NewRole = newRole;
            EscapeScenario = (EscapeScenario)escapeScenario;
            IsAllowed = escapeScenario != EscapeScenario.CustomEscape;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EscapingEventArgs" /> class.
        /// </summary>
        /// <param name="player">
        /// <inheritdoc cref="Player" />
        /// </param>
        /// <param name="newRole">
        /// <inheritdoc cref="NewRole" />
        /// </param>
        /// <param name="escapeScenario">
        /// <inheritdoc cref="EscapeScenario" />
        /// </param>
        /// <param name="respawnTickets">
        /// <inheritdoc cref="RespawnTickets"/>
        /// </param>
        public EscapingEventArgs(Player player, RoleTypeId newRole, EscapeScenarioType escapeScenario, KeyValuePair<SpawnableTeamType, float> respawnTickets)
            : this(player, newRole, escapeScenario)
        {
            RespawnTickets = respawnTickets;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EscapingEventArgs" /> class.
        /// </summary>
        /// <param name="player">
        /// <inheritdoc cref="Player" />
        /// </param>
        /// <param name="newRole">
        /// <inheritdoc cref="NewRole" />
        /// </param>
        /// <param name="escapeScenario">
        /// <inheritdoc cref="EscapeScenario" />
        /// </param>
        /// <param name="teamToGrantTickets">
        /// A <see cref="SpawnableTeamType"/> that <see cref="RespawnTickets"/> will be initialized with.
        /// </param>
        /// <param name="ticketsToGrant">
        /// A <see langword="float"/> that <see cref="RespawnTickets"/> will be initialized with.
        /// </param>
        public EscapingEventArgs(Player player, RoleTypeId newRole, EscapeScenarioType escapeScenario, SpawnableTeamType teamToGrantTickets, float ticketsToGrant)
            : this(player, newRole, escapeScenario)
        {
            if (teamToGrantTickets != SpawnableTeamType.None)
                RespawnTickets = new KeyValuePair<SpawnableTeamType, float>(teamToGrantTickets, ticketsToGrant);
        }

        /// <summary>
        /// Gets the player who's escaping.
        /// </summary>
        public Player Player { get; }

        /// <summary>
        /// Gets or sets the role that will be assigned when the player escapes.
        /// </summary>
        public RoleTypeId NewRole { get; set; }

        /// <summary>
        /// Gets or sets the EscapeScenario that will represent for this player.
        /// </summary>
        public EscapeScenario EscapeScenario { get; set; }

        /// <summary>
        /// Gets or sets the RespawnTickets that will represent the amount of tickets granted to a specific <see cref="SpawnableTeamType"/> after the player escapes.
        /// </summary>
        /// <seealso cref="RespawnTokensManager"/>
        public KeyValuePair<SpawnableTeamType, float> RespawnTickets { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the player can escape.
        /// </summary>
        public bool IsAllowed { get; set; }
    }
}
