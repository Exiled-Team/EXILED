// -----------------------------------------------------------------------
// <copyright file="EscapingEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomModules.Events
{
    using System;

    using Exiled.API.Features;
    using Exiled.CustomModules.API.Enums;
    using Exiled.CustomModules.API.Features;

    using PlayerRoles;

    /// <summary>
    /// Contains all informations before escaping.
    /// </summary>
    public sealed class EscapingEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EscapingEventArgs"/> class.
        /// </summary>
        /// <param name="player"><inheritdoc cref="Player"/></param>
        /// <param name="newRole"><inheritdoc cref="NewRole"/></param>
        /// <param name="newCustomRole"><inheritdoc cref="NewCustomRole"/></param>
        /// <param name="scenarioType"><inheritdoc cref="ScenarioType"/></param>
        /// <param name="hint"><inheritdoc cref="Hint"/></param>
        /// <param name="isAllowed"><inheritdoc cref="IsAllowed"/></param>
        public EscapingEventArgs(
            Pawn player,
            RoleTypeId newRole,
            CustomRole newCustomRole,
            UUEscapeScenarioType scenarioType,
            Hint hint,
            bool isAllowed = true)
        {
            Player = player;
            NewRole = newRole;
            NewCustomRole = newCustomRole;
            ScenarioType = scenarioType;
            Hint = hint;
            IsAllowed = isAllowed;
        }

        /// <summary>
        /// Gets the player who's escaping.
        /// </summary>
        public Pawn Player { get; }

        /// <summary>
        /// Gets or sets the new <see cref="RoleTypeId"/>.
        /// </summary>
        public RoleTypeId NewRole { get; set; }

        /// <summary>
        /// Gets or sets the new <see cref="CustomRole"/>.
        /// </summary>
        public CustomRole NewCustomRole { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="UUEscapeScenarioType"/>.
        /// </summary>
        public UUEscapeScenarioType ScenarioType { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="Exiled.API.Features.Hint"/> to be displayed.
        /// </summary>
        public Hint Hint { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the player can escape.
        /// </summary>
        public bool IsAllowed { get; set; }
    }
}