// -----------------------------------------------------------------------
// <copyright file="TestGamemode.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Example.TestGamemode
{
    using System;

    using Exiled.CustomModules.API.Features.Attributes;
    using Exiled.CustomModules.API.Features.CustomGameModes;
    using PlayerRoles;

    /// <inheritdoc />
    [ModuleIdentifier]
    public class TestGamemode : CustomGameMode
    {
        /// <inheritdoc />
        public override uint Id { get; set; } = CustomGamemodeType.TestGamemode;

        /// <inheritdoc />
        public override string Name { get; set; } = "Test Gamemode";

        /// <inheritdoc />
        public override bool IsEnabled { get; set; } = true;

        /// <inheritdoc />
        public override Type[] BehaviourComponents { get; } = { typeof(TestGamemodeGameState), typeof(TestGamemodePlayerState) };

        /// <inheritdoc />
        public override GameModeSettings Settings { get; set; } = new()
        {
            Automatic = false,
            MinimumPlayers = 1,
            MaximumPlayers = 5,
            RejectExceedingPlayers = false,
            IsRespawnEnabled = false,
            IsTeamRespawnEnabled = false,
            RestartRoundOnEnd = true,
            IsDecontaminationEnabled = false,
            IsWarheadEnabled = false,
            IsWarheadInteractable = false,
            SpawnableRoles = new[] { RoleTypeId.ClassD, RoleTypeId.Scientist },
        };
    }
}