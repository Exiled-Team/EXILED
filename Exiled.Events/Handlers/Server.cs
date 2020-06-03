// -----------------------------------------------------------------------
// <copyright file="Server.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Handlers
{
    using System;
    using System.IO;
    using Exiled.API.Extensions;
    using Exiled.API.Features;
    using Exiled.Events.Handlers.EventArgs;

    /// <summary>
    /// Server related events.
    /// </summary>
    public class Server
    {
        /// <summary>
        /// Invoked before waiting for players.
        /// </summary>
        public static event EventHandler WaitingForPlayers;

        /// <summary>
        /// Invoked after the start of a new round.
        /// </summary>
        public static event EventHandler RoundStarted;

        /// <summary>
        /// Invoked before ending a round
        /// </summary>
        public static event EventHandler<EndingRoundEventArgs> EndingRound;

        /// <summary>
        /// Invoked after the end of a round.
        /// </summary>
        public static event EventHandler<RoundEndedEventArgs> RoundEnded;

        /// <summary>
        /// Invoked before the restart of a round.
        /// </summary>
        public static event EventHandler RestartingRound;

        /// <summary>
        /// Invoked when a player reports a cheater.
        /// </summary>
        public static event EventHandler<ReportingCheaterEventArgs> ReportingCheater;

        /// <summary>
        /// Invoked before respawning a wave of Chaos Insurgency or NTF.
        /// </summary>
        public static event EventHandler<RespawningTeamEventArgs> RespawningTeam;

        /// <summary>
        /// Invoked when sending a command through the in-game console.
        /// </summary>
        public static event EventHandler<SendingConsoleCommandEventArgs> SendingConsoleCommand;

        /// <summary>
        /// Invoked when sending a command through the Remote Admin console.
        /// </summary>
        public static event EventHandler<SendingRemoteAdminCommandEventArgs> SendingRemoteAdminCommand;

        /// <summary>
        /// Called before waiting for players.
        /// </summary>
        public static void OnWaitingForPlayers() => WaitingForPlayers.InvokeSafely(null, System.EventArgs.Empty);

        /// <summary>
        /// Called after the start of a new round.
        /// </summary>
        public static void OnRoundStarted() => RoundStarted.InvokeSafely(null, System.EventArgs.Empty);

        /// <summary>
        /// Called before ending a round.
        /// </summary>
        /// <param name="ev">The <see cref="EndingRoundEventArgs"/> instance.</param>
        public static void OnEndingRound(EndingRoundEventArgs ev) => EndingRound.InvokeSafely(null, ev);

        /// <summary>
        /// Called after the end of a round.
        /// </summary>
        /// <param name="ev">The <see cref="RoundEndedEventArgs"/> instance.</param>
        public static void OnRoundEnded(RoundEndedEventArgs ev) => RoundEnded.InvokeSafely(null, ev);

        /// <summary>
        /// Called before restarting a round.
        /// </summary>
        public static void OnRestartingRound() => RestartingRound.InvokeSafely(null, System.EventArgs.Empty);

        /// <summary>
        /// Called when a player reports a cheater.
        /// </summary>
        /// <param name="ev">The <see cref="ReportingCheaterEventArgs"/> instance.</param>
        public static void OnReportingCheater(ReportingCheaterEventArgs ev) => ReportingCheater.InvokeSafely(null, ev);

        /// <summary>
        /// Called before respawning a wave of Chaso Insurgency or NTF.
        /// </summary>
        /// <param name="ev">The <see cref="RespawningTeamEventArgs"/> instance.</param>
        public static void OnRespawningTeam(RespawningTeamEventArgs ev) => RespawningTeam.InvokeSafely(null, ev);

        /// <summary>
        /// Called when sending a command through in-game console.
        /// </summary>
        /// <param name="ev">The <see cref="SendingConsoleCommandEventArgs"/> instance.</param>
        public static void OnSendingConsoleCommand(SendingConsoleCommandEventArgs ev) => SendingConsoleCommand.InvokeSafely(null, ev);

        /// <summary>
        /// Called when sending a command through the Remote Admin console.
        /// </summary>
        /// <param name="ev">The <see cref="SendingRemoteAdminCommandEventArgs"/> instance.</param>
        public static void OnSendingRemoteAdminCommand(SendingRemoteAdminCommandEventArgs ev)
        {
            SendingRemoteAdminCommand.InvokeSafely(null, ev);

            lock (ServerLogs.LockObject)
            {
                File.AppendAllText(Paths.Log, $"[{DateTime.Now}] {ev.Sender.Nickname} ({ev.Sender.UserId}) ran command: {ev.Name}. Command Permitted: {(ev.IsAllowed ? "[YES]" : "[NO]")}" + Environment.NewLine);
            }
        }
    }
}
