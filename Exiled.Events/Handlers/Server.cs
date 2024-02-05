// -----------------------------------------------------------------------
// <copyright file="Server.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Handlers
{
    using System.Collections.Generic;

    using Respawning;

#pragma warning disable SA1623 // Property summary documentation should match accessors

    using Exiled.Events.EventArgs.Player;
    using Exiled.Events.EventArgs.Server;
    using Exiled.Events.Features;

    /// <summary>
    /// Server related events.
    /// </summary>
    public static class Server
    {
        /// <summary>
        /// Invoked before waiting for players.
        /// </summary>
        public static Event WaitingForPlayers { get; set; } = new();

        /// <summary>
        /// Invoked after the start of a new round.
        /// </summary>
        public static Event RoundStarted { get; set; } = new();

        /// <summary>
        /// Invoked before ending a round.
        /// </summary>
        public static Event<EndingRoundEventArgs> EndingRound { get; set; } = new();

        /// <summary>
        /// Invoked after the end of a round.
        /// </summary>
        public static Event<RoundEndedEventArgs> RoundEnded { get; set; } = new();

        /// <summary>
        /// Invoked before the restart of a round.
        /// </summary>
        public static Event RestartingRound { get; set; } = new();

        /// <summary>
        /// Invoked when a player reports a cheater.
        /// </summary>
        public static Event<ReportingCheaterEventArgs> ReportingCheater { get; set; } = new();

        /// <summary>
        /// Invoked before setting up the environment for respawning a wave of Chaos Insurgency or NTF.
        /// </summary>
        public static Event<PreRespawningTeamEventArgs> PreRespawningTeam { get; set; } = new();

        /// <summary>
        /// Invoked before respawning a wave of Chaos Insurgency or NTF.
        /// </summary>
        public static Event<RespawningTeamEventArgs> RespawningTeam { get; set; } = new();

        /// <summary>
        /// Invoked before adding an unit name.
        /// </summary>
        public static Event<AddingUnitNameEventArgs> AddingUnitName { get; set; } = new();

        /// <summary>
        /// Invoked when sending a complaint about a player to the local server administrators.
        /// </summary>
        public static Event<LocalReportingEventArgs> LocalReporting { get; set; } = new();

        /// <summary>
        /// Invoked before choosing the team to be assigned to a player.
        /// </summary>
        public static Event<ChoosingStartTeamQueueEventArgs> ChoosingStartTeamQueue { get; set; } = new();

        /// <summary>
        /// Invoked before selecting the team that will respawn.
        /// </summary>
        public static Event<SelectingRespawnTeamEventArgs> SelectingRespawnTeam { get; set; } = new();

        /// <summary>
        /// Invoked after the "reload configs" command is ran.
        /// </summary>
        public static Event ReloadedConfigs { get; set; } = new();

        /// <summary>
        /// Invoked after the "reload translations" command is ran.
        /// </summary>
        public static Event ReloadedTranslations { get; set; } = new();

        /// <summary>
        /// Invoked after the "reload gameplay" command is ran.
        /// </summary>
        public static Event ReloadedGameplay { get; set; } = new();

        /// <summary>
        /// Invoked after the "reload remoteadminconfigs" command is ran.
        /// </summary>
        public static Event ReloadedRA { get; set; } = new();

        /// <summary>
        /// Invoked after the "reload plugins" command is ran.
        /// </summary>
        public static Event ReloadedPlugins { get; set; } = new();

        /// <summary>
        /// Invoked after the "reload permissions" command is ran.
        /// </summary>
        public static Event ReloadedPermissions { get; set; } = new();

        /// <summary>
        /// Invoked after a team has spawned.
        /// </summary>
        public static Event<RespawnedTeamEventArgs> RespawnedTeam { get; set; } = new();

        /// <summary>
        /// Invoked before setting up the environment for the assignment of human roles.
        /// </summary>
        public static Event<PreAssigningHumanRolesEventArgs> PreAssigningHumanRoles { get; set; } = new();

        /// <summary>
        /// Invoked before setting up the environment for the assignment of SCP roles.
        /// </summary>
        public static Event<PreAssigningScpRolesEventArgs> PreAssigningScpRoles { get; set; } = new();

        /// <summary>
        /// Invoked before assigning human roles.
        /// </summary>
        public static Event<AssigningHumanRolesEventArgs> AssigningHumanRoles { get; set; } = new();

        /// <summary>
        /// Invoked before assigning SCP roles.
        /// </summary>
        public static Event<AssigningScpRolesEventArgs> AssigningScpRoles { get; set; } = new();

        /// <summary>
        /// Invoked before deploying a SCP role.
        /// </summary>
        public static Event<DeployingScpRoleEventArgs> DeployingScpRole { get; set; } = new();

        /// <summary>
        /// Invoked before deploying a human role.
        /// </summary>
        public static Event<DeployingHumanRoleEventArgs> DeployingHumanRole { get; set; } = new();

        /// <summary>
        /// Invoked before deploying a team role.
        /// </summary>
        public static Event<DeployingTeamRoleEventArgs> DeployingTeamRole { get; set; } = new();

        /// <summary>
        /// Invoked after a new respawn sequence has been restarted.
        /// </summary>
        public static Event<RestartedRespawnSequenceEventArgs> RestartedRespawnSequence { get; set; } = new();

        /// <summary>
        /// Called before waiting for players.
        /// </summary>
        public static void OnWaitingForPlayers() => WaitingForPlayers.InvokeSafely();

        /// <summary>
        /// Called after the start of a new round.
        /// </summary>
        public static void OnRoundStarted() => RoundStarted.InvokeSafely();

        /// <summary>
        /// Called before ending a round.
        /// </summary>
        /// <param name="ev">The <see cref="EndingRoundEventArgs"/> instance.</param>
        public static void OnEndingRound(EndingRoundEventArgs ev) => EndingRound.InvokeSafely(ev);

        /// <summary>
        /// Called after the end of a round.
        /// </summary>
        /// <param name="ev">The <see cref="RoundEndedEventArgs"/> instance.</param>
        public static void OnRoundEnded(RoundEndedEventArgs ev) => RoundEnded.InvokeSafely(ev);

        /// <summary>
        /// Called before restarting a round.
        /// </summary>
        public static void OnRestartingRound() => RestartingRound.InvokeSafely();

        /// <summary>
        /// Called when a player reports a cheater.
        /// </summary>
        /// <param name="ev">The <see cref="ReportingCheaterEventArgs"/> instance.</param>
        public static void OnReportingCheater(ReportingCheaterEventArgs ev) => ReportingCheater.InvokeSafely(ev);

        /// <summary>
        /// Called before selecting the team that will respawn next.
        /// </summary>
        /// <param name="ev">The <see cref="SelectingRespawnTeamEventArgs"/> instance.</param>
        public static void OnSelectingRespawnTeam(SelectingRespawnTeamEventArgs ev) => SelectingRespawnTeam.InvokeSafely(ev);

        /// <summary>
        /// Called before setting up the environment for respawning a wave of Chaos Insurgency or NTF.
        /// </summary>
        /// <param name="ev">The <see cref="PreRespawningTeamEventArgs"/> instance.</param>
        public static void OnPreRespawningTeam(PreRespawningTeamEventArgs ev) => PreRespawningTeam.InvokeSafely(ev);

        /// <summary>
        /// Called before respawning a wave of Chaos Insurgency or NTF.
        /// </summary>
        /// <param name="ev">The <see cref="RespawningTeamEventArgs"/> instance.</param>
        public static void OnRespawningTeam(RespawningTeamEventArgs ev) => RespawningTeam.InvokeSafely(ev);

        /// <summary>
        /// Called after a team has spawned.
        /// </summary>
        /// <param name="teamType"><inheritdoc cref="RespawnedTeamEventArgs.Team"/></param>
        /// <param name="hubs"><inheritdoc cref="RespawnedTeamEventArgs.Players"/></param>
        public static void OnRespawnedTeam(SpawnableTeamType teamType, List<ReferenceHub> hubs) => RespawnedTeam.InvokeSafely(new RespawnedTeamEventArgs(teamType, hubs));

        /// <summary>
        /// Called before adding an unit name.
        /// </summary>
        /// <param name="ev">The <see cref="AddingUnitNameEventArgs"/> instance.</param>
        public static void OnAddingUnitName(AddingUnitNameEventArgs ev) => AddingUnitName.InvokeSafely(ev);

        /// <summary>
        /// Called when sending a complaint about a player to the local server administrators.
        /// </summary>
        /// <param name="ev">The <see cref="LocalReportingEventArgs"/> instance.</param>
        public static void OnLocalReporting(LocalReportingEventArgs ev) => LocalReporting.InvokeSafely(ev);

        /// <summary>
        /// Called before choosing the team to be assigned to a player.
        /// </summary>
        /// <param name="ev">The <see cref="ChoosingStartTeamQueueEventArgs"/> instance.</param>
        public static void OnChoosingStartTeam(ChoosingStartTeamQueueEventArgs ev) => ChoosingStartTeamQueue.InvokeSafely(ev);

        /// <summary>
        /// Called after the "reload configs" command is ran.
        /// </summary>
        public static void OnReloadedConfigs() => ReloadedConfigs.InvokeSafely();

        /// <summary>
        /// Called after the "reload translations" command is ran.
        /// </summary>
        public static void OnReloadedTranslations() => ReloadedTranslations.InvokeSafely();

        /// <summary>
        /// Called after the "reload gameplay" command is ran.
        /// </summary>
        public static void OnReloadedGameplay() => ReloadedGameplay.InvokeSafely();

        /// <summary>
        /// Called after the "reload remoteadminconfigs" command is ran.
        /// </summary>
        public static void OnReloadedRA() => ReloadedRA.InvokeSafely();

        /// <summary>
        /// Called after the "reload plugins" command is ran.
        /// </summary>
        public static void OnReloadedPlugins() => ReloadedPlugins.InvokeSafely();

        /// <summary>
        /// Called after the "reload permissions" command is ran.
        /// </summary>
        public static void OnReloadedPermissions() => ReloadedPermissions.InvokeSafely();

        /// <summary>
        /// Called before setting up the environment for the assignment of human roles.
        /// </summary>
        /// <param name="ev">The <see cref="PreAssigningHumanRolesEventArgs"/> instance.</param>
        public static void OnPreAssigningHumanRoles(PreAssigningHumanRolesEventArgs ev) => PreAssigningHumanRoles.InvokeSafely(ev);

        /// <summary>
        /// Called before setting up the environment for the assignment of SCP roles.
        /// </summary>
        /// <param name="ev">The <see cref="PreAssigningScpRolesEventArgs"/> instance.</param>
        public static void OnPreAssigningScpRoles(PreAssigningScpRolesEventArgs ev) => PreAssigningScpRoles.InvokeSafely(ev);

        /// <summary>
        /// Called before assigning human roles.
        /// </summary>
        /// <param name="ev">The <see cref="AssigningHumanRolesEventArgs"/> instance.</param>
        public static void OnAssigningHumanRoles(AssigningHumanRolesEventArgs ev) => AssigningHumanRoles.InvokeSafely(ev);

        /// <summary>
        /// Called before assigning SCP roles.
        /// </summary>
        /// <param name="ev">The <see cref="AssigningScpRolesEventArgs"/> instance.</param>
        public static void OnAssigningScpRoles(AssigningScpRolesEventArgs ev) => AssigningScpRoles.InvokeSafely(ev);

        /// <summary>
        /// Called before deploying a SCP role.
        /// </summary>
        /// <param name="ev">The <see cref="DeployingScpRoleEventArgs"/> instance.</param>
        public static void OnDeployingScpRole(DeployingScpRoleEventArgs ev) => DeployingScpRole.InvokeSafely(ev);

        /// <summary>
        /// Called before deploying a human role.
        /// </summary>
        /// <param name="ev">The <see cref="DeployingHumanRoleEventArgs"/> instance.</param>
        public static void OnDeployingHumanRole(DeployingHumanRoleEventArgs ev) => DeployingHumanRole.InvokeSafely(ev);

        /// <summary>
        /// Called before deploying a team role.
        /// </summary>
        /// <param name="ev">The <see cref="DeployingTeamRoleEventArgs"/> instance.</param>
        public static void OnDeployingTeamRole(DeployingTeamRoleEventArgs ev) => DeployingTeamRole.InvokeSafely(ev);

        /// <summary>
        /// Called after a new respawn sequence has been restarted.
        /// </summary>
        /// <param name="ev">The <see cref="RestartedRespawnSequenceEventArgs"/> instance.</param>
        public static void OnRestartedRespawnSequence(RestartedRespawnSequenceEventArgs ev) => RestartedRespawnSequence.InvokeSafely(ev);
    }
}