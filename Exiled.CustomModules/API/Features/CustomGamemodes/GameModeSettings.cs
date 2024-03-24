// -----------------------------------------------------------------------
// <copyright file="GameModeSettings.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomModules.API.Features.CustomGameModes
{
    using System;

    using Exiled.API.Enums;
    using Exiled.API.Features.Core;
    using Exiled.API.Features.Core.Interfaces;
    using Exiled.CustomModules.API.Enums;
    using PlayerRoles;
    using Respawning;

    /// <summary>
    /// A tool to easily setup game modes.
    /// </summary>
    public class GameModeSettings : TypeCastObject<GameModeSettings>, IAdditiveProperty
    {
        /// <summary>
        /// Gets or sets a value indicating whether the game mode operates in automatic mode.
        /// </summary>
        /// <remarks>
        /// In automatic mode, the behavior of the game mode is managed automatically by the <see cref="World"/>.
        /// <br/>
        /// The game mode will start automatically if the specified probability condition is met and the minimum player requirement (<see cref="MinimumPlayers"/>) is satisfied.
        /// </remarks>
        public virtual bool Automatic { get; set; }

        /// <summary>
        /// Gets or sets the probability condition for automatic game mode activation.
        /// </summary>
        /// <remarks>
        /// This property specifies the probability condition that determines whether the game mode will start automatically.
        /// <br/>
        /// If the specified probability condition is met and the minimum player requirement (<see cref="MinimumPlayers"/>) is satisfied, the game mode will activate automatically.
        /// </remarks>
        public virtual float AutomaticProbability { get; set; }

        /// <summary>
        /// Gets or sets the minimum amount of players to start the game mode.
        /// </summary>
        public virtual uint MinimumPlayers { get; set; }

        /// <summary>
        /// Gets or sets the maximum allowed amount of players managed by the game mode.
        /// </summary>
        public virtual uint MaximumPlayers { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the exceeding players should be rejected.
        /// </summary>
        public virtual bool RejectExceedingPlayers { get; set; }

        /// <summary>
        /// Gets or sets the message to be displayed when a player is rejected due to exceeding amount of players.
        /// </summary>
        public virtual string RejectExceedingMessage { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the players can respawn.
        /// </summary>
        public virtual bool IsRespawnEnabled { get; set; }

        /// <summary>
        /// Gets or sets the respawn time for individual players.
        /// </summary>
        public virtual float RespawnTime { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether teams can regularly respawn.
        /// </summary>
        public virtual bool IsTeamRespawnEnabled { get; set; }

        /// <summary>
        /// Gets or sets the respawn time for individual teams.
        /// </summary>
        public virtual int TeamRespawnTime { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether custom ending conditions should be used over predefined conditions.
        /// </summary>
        public virtual bool UseCustomEndingConditions { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether server should be restarted when the game mode ends.
        /// </summary>
        public virtual bool RestartRoundOnEnd { get; set; }

        /// <summary>
        /// Gets or sets the amount of time to await before restarting the server.
        /// </summary>
        public virtual float RestartWindupTime { get; set; }

        /// <summary>
        /// Gets or sets a <see cref="ZoneType"/>[] containing all zones that should be permanently locked.
        /// </summary>
        public virtual ZoneType[] LockedZones { get; set; }

        /// <summary>
        /// Gets or sets a <see cref="DoorType"/>[] containing all doors that should be permanently locked.
        /// </summary>
        public virtual DoorType[] LockedDoors { get; set; }

        /// <summary>
        /// Gets or sets a <see cref="ElevatorType"/>[] containing all elevators that should be permanently locked.
        /// </summary>
        public virtual ElevatorType[] LockedElevators { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the decontamination should be enabled.
        /// </summary>
        public virtual bool IsDecontaminationEnabled { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the Alpha Warhead is enabled.
        /// </summary>
        public virtual bool IsWarheadEnabled { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the Alpha Warhead interactions are allowed.
        /// </summary>
        public virtual bool IsWarheadInteractable { get; set; }

        /// <summary>
        /// Gets or sets the amount of time, expressed in seconds, after which the Alpha Warhead will be automatically started.
        /// <para/>
        /// <see cref="IsWarheadEnabled"/> must be set to <see langword="true"/>.
        /// </summary>
        public virtual float AutoWarheadTime { get; set; }

        /// <summary>
        /// Gets or sets a <see cref="RoleTypeId"/>[] containing all spawnable roles.
        /// <para/>
        /// If not empty, all undefined roles won't be able to spawn.
        /// <br/>
        /// It's highly recommended to not use it along with <see cref="NonSpawnableRoles"/>.
        /// </summary>
        public virtual RoleTypeId[] SpawnableRoles { get; set; }

        /// <summary>
        /// Gets or sets a <see cref="UUCustomRoleType"/>[] containing all spawnable custom roles.
        /// <para/>
        /// If not empty, all undefined custom roles won't be able to spawn.
        /// <br/>
        /// It's highly recommended to not use it along with <see cref="NonSpawnableCustomRoles"/>.
        /// </summary>
        public virtual uint[] SpawnableCustomRoles { get; set; }

        /// <summary>
        /// Gets or sets a <see cref="SpawnableTeamType"/>[] containing all spawnable teams.
        /// <para/>
        /// If not empty, all undefined teams won't be able to spawn.
        /// <br/>
        /// It's highly recommended to not use it along with <see cref="NonSpawnableTeams"/>.
        /// </summary>
        public virtual SpawnableTeamType[] SpawnableTeams { get; set; }

        /// <summary>
        /// Gets or sets a <see cref="UUCustomTeamType"/>[] containing all spawnable custom teams.
        /// <para/>
        /// If not empty, all undefined custom teams won't be able to spawn.
        /// <br/>
        /// It's highly recommended to not use it along with <see cref="NonSpawnableCustomTeams"/>.
        /// </summary>
        public virtual uint[] SpawnableCustomTeams { get; set; }

        /// <summary>
        /// Gets or sets a <see cref="RoleTypeId"/>[] containing all non spawnable roles.
        /// <para/>
        /// If not empty, all undefined roles will be able to spawn.
        /// <br/>
        /// It's highly recommended to not use it along with <see cref="SpawnableRoles"/>.
        /// </summary>
        public virtual RoleTypeId[] NonSpawnableRoles { get; set; }

        /// <summary>
        /// Gets or sets a <see cref="UUCustomRoleType"/>[] containing all non spawnable custom roles.
        /// <para/>
        /// If not empty, all undefined custom roles will be able to spawn.
        /// <br/>
        /// It's highly recommended to not use it along with <see cref="SpawnableCustomRoles"/>.
        /// </summary>
        public virtual uint[] NonSpawnableCustomRoles { get; set; }

        /// <summary>
        /// Gets or sets a <see cref="UUCustomTeamType"/>[] containing all non spawnable custom teams.
        /// <para/>
        /// If not empty, all undefined teams will be able to spawn.
        /// <br/>
        /// It's highly recommended to not use it along with <see cref="SpawnableTeams"/>.
        /// </summary>
        public virtual SpawnableTeamType[] NonSpawnableTeams { get; set; }

        /// <summary>
        /// Gets or sets a <see cref="UUCustomTeamType"/>[] containing all non spawnable custom teams.
        /// <para/>
        /// If not empty, all undefined custom teams will be able to spawn.
        /// <br/>
        /// It's highly recommended to not use it along with <see cref="SpawnableCustomTeams"/>.
        /// </summary>
        public virtual uint[] NonSpawnableCustomTeams { get; set; }
    }
}