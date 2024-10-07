// -----------------------------------------------------------------------
// <copyright file="TestTeam.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Example.TestTeam
{
    using Exiled.CustomModules.API.Features.Attributes;
    using Exiled.CustomModules.API.Features.CustomRoles;
    using PlayerRoles;
    using Respawning;
    using UnityEngine;

    /// <inheritdoc />
    [ModuleIdentifier]
    public class TestTeam : CustomTeam
    {
        /// <inheritdoc />
        public override uint Id { get; set; } = CustomTeamType.TestTeam;

        /// <inheritdoc />
        public override string Name { get; set; } = "TestTeam";

        /// <inheritdoc />
        public override bool IsEnabled { get; set; } = true;

        /// <inheritdoc />
        public override string DisplayName { get; set; } = "ECustomTeam";

        /// <inheritdoc />
        public override int Probability { get; set; } = 100;

        /// <inheritdoc />
        public override string DisplayColor { get; set; } = Color.cyan.ToHex();

        /// <inheritdoc />
        public override SpawnableTeamType[] SpawnableFromTeams { get; set; } = { SpawnableTeamType.ChaosInsurgency };

        /// <inheritdoc />
        public override Team[] TeamsOwnership { get; set; } = { Team.FoundationForces };

        /// <inheritdoc />
        public override bool UseTickets { get; set; } = false;
    }
}