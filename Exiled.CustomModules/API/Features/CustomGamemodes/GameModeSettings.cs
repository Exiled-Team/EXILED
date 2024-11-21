// -----------------------------------------------------------------------
// <copyright file="GameModeSettings.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomModules.API.Features.CustomGameModes
{
    using System.ComponentModel;

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
        [Description("Indicates whether the game mode operates automatically, managed by the World.")]
        public virtual bool Automatic { get; set; }

        /// <summary>
        /// Gets or sets the probability condition for automatic game mode activation.
        /// </summary>
        /// <remarks>
        /// This property specifies the probability condition that determines whether the game mode will start automatically.
        /// <br/>
        /// If the specified probability condition is met and the minimum player requirement (<see cref="MinimumPlayers"/>) is satisfied, the game mode will activate automatically.
        /// </remarks>
        [Description("The probability condition for automatic activation of the game mode.")]
        public virtual float AutomaticProbability { get; set; }

        /// <summary>
        /// Gets or sets the minimum amount of players to start the game mode.
        /// </summary>
        [Description("The minimum number of players required to start the game mode.")]
        public virtual uint MinimumPlayers { get; set; }

        /// <summary>
        /// Gets or sets the maximum allowed amount of players managed by the game mode.
        /// </summary>
        [Description("The maximum number of players that the game mode can manage.")]
        public virtual uint MaximumPlayers { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the exceeding players should be rejected.
        /// </summary>
        [Description("Indicates whether players exceeding the maximum allowed number should be rejected.")]
        public virtual bool RejectExceedingPlayers { get; set; }

        /// <summary>
        /// Gets or sets the message to be displayed when a player is rejected due to exceeding amount of players.
        /// </summary>
        [Description("The message displayed when a player is rejected due to exceeding the allowed player count.")]
        public virtual string RejectExceedingMessage { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the players can respawn.
        /// </summary>
        [Description("Indicates whether players are allowed to respawn.")]
        public virtual bool IsRespawnEnabled { get; set; }

        /// <summary>
        /// Gets or sets the respawn time for individual players.
        /// </summary>
        [Description("The respawn time for individual players.")]
        public virtual float RespawnTime { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether teams can regularly respawn.
        /// </summary>
        [Description("Indicates whether teams are allowed to respawn regularly.")]
        public virtual bool IsTeamRespawnEnabled { get; set; }

        /// <summary>
        /// Gets or sets the respawn time for individual teams.
        /// </summary>
        [Description("The respawn time for individual teams.")]
        public virtual int TeamRespawnTime { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether custom ending conditions should be used over predefined conditions.
        /// </summary>
        [Description("Indicates whether custom ending conditions should override predefined conditions.")]
        public virtual bool UseCustomEndingConditions { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the server should be restarted when the game mode ends.
        /// </summary>
        [Description("Indicates whether the server should be restarted when the game mode ends.")]
        public virtual bool RestartRoundOnEnd { get; set; }

        /// <summary>
        /// Gets or sets the amount of time to await before restarting the server.
        /// </summary>
        [Description("The time to wait before restarting the server.")]
        public virtual float RestartWindupTime { get; set; }

        /// <summary>
        /// Gets or sets a <see cref="ZoneType"/>[] containing all zones that should be permanently locked.
        /// </summary>
        [Description("An array of zones that should be permanently locked.")]
        public virtual ZoneType[] LockedZones { get; set; }

        /// <summary>
        /// Gets or sets a <see cref="DoorType"/>[] containing all doors that should be permanently locked.
        /// </summary>
        [Description("An array of doors that should be permanently locked.")]
        public virtual DoorType[] LockedDoors { get; set; }

        /// <summary>
        /// Gets or sets a <see cref="ElevatorType"/>[] containing all elevators that should be permanently locked.
        /// </summary>
        [Description("An array of elevators that should be permanently locked.")]
        public virtual ElevatorType[] LockedElevators { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the decontamination should be enabled.
        /// </summary>
        [Description("Indicates whether decontamination should be enabled.")]
        public virtual bool IsDecontaminationEnabled { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the Alpha Warhead is enabled.
        /// </summary>
        [Description("Indicates whether the Alpha Warhead is enabled.")]
        public virtual bool IsWarheadEnabled { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the Alpha Warhead interactions are allowed.
        /// </summary>
        [Description("Indicates whether interactions with the Alpha Warhead are allowed.")]
        public virtual bool IsWarheadInteractable { get; set; }

        /// <summary>
        /// Gets or sets the amount of time, expressed in seconds, after which the Alpha Warhead will be automatically started.
        /// <para/>
        /// <see cref="IsWarheadEnabled"/> must be set to <see langword="true"/>.
        /// </summary>
        [Description("The time in seconds after which the Alpha Warhead will automatically start, if enabled.")]
        public virtual float AutoWarheadTime { get; set; }

        /// <summary>
        /// Gets or sets a <see cref="RoleTypeId"/>[] containing all spawnable roles.
        /// <para/>
        /// If not empty, all undefined roles won't be able to spawn.
        /// <br/>
        /// It's highly recommended to not use it along with <see cref="NonSpawnableRoles"/>.
        /// </summary>
        [Description("An array of spawnable roles. If specified, only these roles can spawn.")]
        public virtual RoleTypeId[] SpawnableRoles { get; set; }

        /// <summary>
        /// Gets or sets a <see cref="UUCustomRoleType"/>[] containing all spawnable custom roles.
        /// <para/>
        /// If not empty, all undefined custom roles won't be able to spawn.
        /// <br/>
        /// It's highly recommended to not use it along with <see cref="NonSpawnableCustomRoles"/>.
        /// </summary>
        [Description("An array of spawnable custom roles. If specified, only these custom roles can spawn.")]
        public virtual uint[] SpawnableCustomRoles { get; set; }

        /// <summary>
        /// Gets or sets a <see cref="SpawnableTeamType"/>[] containing all spawnable teams.
        /// <para/>
        /// If not empty, all undefined teams won't be able to spawn.
        /// <br/>
        /// It's highly recommended to not use it along with <see cref="NonSpawnableTeams"/>.
        /// </summary>
        [Description("An array of spawnable teams. If specified, only these teams can spawn.")]
        public virtual SpawnableTeamType[] SpawnableTeams { get; set; }

        /// <summary>
        /// Gets or sets a <see cref="UUCustomTeamType"/>[] containing all spawnable custom teams.
        /// <para/>
        /// If not empty, all undefined custom teams won't be able to spawn.
        /// <br/>
        /// It's highly recommended to not use it along with <see cref="NonSpawnableCustomTeams"/>.
        /// </summary>
        [Description("An array of spawnable custom teams. If specified, only these custom teams can spawn.")]
        public virtual uint[] SpawnableCustomTeams { get; set; }

        /// <summary>
        /// Gets or sets a <see cref="RoleTypeId"/>[] containing all non spawnable roles.
        /// <para/>
        /// If not empty, all undefined roles will be able to spawn.
        /// <br/>
        /// It's highly recommended to not use it along with <see cref="SpawnableRoles"/>.
        /// </summary>
        [Description("An array of non-spawnable roles. If specified, these roles cannot spawn.")]
        public virtual RoleTypeId[] NonSpawnableRoles { get; set; }

        /// <summary>
        /// Gets or sets a <see cref="UUCustomRoleType"/>[] containing all non spawnable custom roles.
        /// <para/>
        /// If not empty, all undefined custom roles will be able to spawn.
        /// <br/>
        /// It's highly recommended to not use it along with <see cref="SpawnableCustomRoles"/>.
        /// </summary>
        [Description("An array of non-spawnable custom roles. If specified, these custom roles cannot spawn.")]
        public virtual uint[] NonSpawnableCustomRoles { get; set; }

        /// <summary>
        /// Gets or sets a <see cref="SpawnableTeamType"/>[] containing all non spawnable custom teams.
        /// <para/>
        /// If not empty, all undefined teams will be able to spawn.
        /// <br/>
        /// It's highly recommended to not use it along with <see cref="SpawnableTeams"/>.
        /// </summary>
        [Description("An array of non-spawnable custom teams. If specified, these custom teams cannot spawn.")]
        public virtual SpawnableTeamType[] NonSpawnableTeams { get; set; }

        /// <summary>
        /// Gets or sets a <see cref="UUCustomTeamType"/>[] containing all non spawnable custom teams.
        /// <para/>
        /// If not empty, all undefined custom teams will be able to spawn.
        /// <br/>
        /// It's highly recommended to not use it along with <see cref="SpawnableCustomTeams"/>.
        /// </summary>
        [Description("An array of non-spawnable custom teams. If specified, these custom teams cannot spawn.")]
        public virtual uint[] NonSpawnableCustomTeams { get; set; }
    }
}