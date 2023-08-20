// -----------------------------------------------------------------------
// <copyright file="Server.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Handlers
{
    using Exiled.Events.EventArgs.Player;
    using Exiled.Events.EventArgs.Server;

    using Extensions;

    using static Events;

    /// <summary>
    /// Server related events.
    /// </summary>
    public static class Server
    {
        /// <summary>
        /// Invoked before waiting for players.
        /// </summary>
        public static event CustomEventHandler WaitingForPlayers;

        /// <summary>
        /// Invoked after the start of a new round.
        /// </summary>
        public static event CustomEventHandler RoundStarted;

        /// <summary>
        /// Invoked before ending a round.
        /// </summary>
        public static event CustomEventHandler<EndingRoundEventArgs> EndingRound;

        /// <summary>
        /// Invoked after the end of a round.
        /// </summary>
        public static event CustomEventHandler<RoundEndedEventArgs> RoundEnded;

        /// <summary>
        /// Invoked before the restart of a round.
        /// </summary>
        public static event CustomEventHandler RestartingRound;

        /// <summary>
        /// Invoked when a player reports a cheater.
        /// </summary>
        public static event CustomEventHandler<ReportingCheaterEventArgs> ReportingCheater;

        /// <summary>
        /// Invoked before respawning a wave of Chaos Insurgency or NTF.
        /// </summary>
        public static event CustomEventHandler<RespawningTeamEventArgs> RespawningTeam;

        /// <summary>
        /// Invoked before adding an unit name.
        /// </summary>
        public static event CustomEventHandler<AddingUnitNameEventArgs> AddingUnitName;

        /// <summary>
        /// Invoked when sending a complaint about a player to the local server administrators.
        /// </summary>
        public static event CustomEventHandler<LocalReportingEventArgs> LocalReporting;

        /// <summary>
        /// Invoked before choosing the Team than player will get.
        /// </summary>
        public static event CustomEventHandler<ChoosingStartTeamQueueEventArgs> ChoosingStartTeamQueue;

        /// <summary>
        /// Invoked after the "reload configs" command is ran.
        /// </summary>
        public static event CustomEventHandler ReloadedConfigs;

        /// <summary>
        /// Invoked after the "reload translations" command is ran.
        /// </summary>
        public static event CustomEventHandler ReloadedTranslations;

        /// <summary>
        /// Invoked after the "reload gameplay" command is ran.
        /// </summary>
        public static event CustomEventHandler ReloadedGameplay;

        /// <summary>
        /// Invoked after the "reload remoteadminconfigs" command is ran.
        /// </summary>
        public static event CustomEventHandler ReloadedRA;

        /// <summary>
        /// Invoked after the "reload plugins" command is ran.
        /// </summary>
        public static event CustomEventHandler ReloadedPlugins;

        /// <summary>
        /// Invoked after the "reload permissions" command is ran.
        /// </summary>
        public static event CustomEventHandler ReloadedPermissions;

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
        /// Called before respawning a wave of Chaos Insurgency or NTF.
        /// </summary>
        /// <param name="ev">The <see cref="RespawningTeamEventArgs"/> instance.</param>
        public static void OnRespawningTeam(RespawningTeamEventArgs ev) => RespawningTeam.InvokeSafely(ev);

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
        /// Called before a <see cref="Player"/>'s custom display name is changed.
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
    }
}