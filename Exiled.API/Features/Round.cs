// -----------------------------------------------------------------------
// <copyright file="Round.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features
{
    using System;
    using System.Collections.Generic;

    using Enums;

    using GameCore;

    using PlayerRoles;

    using RoundRestarting;

    /// <summary>
    /// A set of tools to handle the round more easily.
    /// </summary>
    public static class Round
    {
        /// <summary>
        /// Gets a list of players who will be ignored from determining round end.
        /// </summary>
        public static HashSet<Player> IgnoredPlayers { get; } = new(20);

        /// <summary>
        /// Gets the time elapsed from the start of the round.
        /// </summary>
        /// <seealso cref="StartedTime"/>
        public static TimeSpan ElapsedTime => RoundStart.RoundLength;

        /// <summary>
        /// Gets the start time of the round.
        /// </summary>
        /// <seealso cref="ElapsedTime"/>
        /// <seealso cref="IsStarted"/>
        public static DateTime StartedTime => DateTime.Now - ElapsedTime;

        /// <summary>
        /// Gets a value indicating whether the round is started or not.
        /// </summary>
        public static bool IsStarted => ReferenceHub.LocalHub && ReferenceHub.LocalHub.characterClassManager.RoundStarted;

        /// <summary>
        /// Gets a value indicating whether the round in progress or not.
        /// </summary>
        public static bool InProgress => ReferenceHub.LocalHub is not null && RoundSummary.RoundInProgress();

        /// <summary>
        /// Gets a value indicating whether the round is ended or not.
        /// </summary>
        public static bool IsEnded => RoundSummary.singleton._roundEnded;

        /// <summary>
        /// Gets a value indicating whether the round is lobby or not.
        /// </summary>
        public static bool IsLobby => !(IsEnded || IsStarted);

        /// <summary>
        /// Gets or sets a value indicating the amount of Chaos Targets remaining.
        /// </summary>
        public static int ChaosTargetCount
        {
            get => RoundSummary.singleton.Network_chaosTargetCount;
            set => RoundSummary.singleton.Network_chaosTargetCount = value;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the round is locked or not.
        /// </summary>
        public static bool IsLocked
        {
            get => RoundSummary.RoundLock;
            set => RoundSummary.RoundLock = value;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the lobby is locked or not.
        /// </summary>
        public static bool IsLobbyLocked
        {
            get => RoundStart.LobbyLock;
            set => RoundStart.LobbyLock = value;
        }

        /// <summary>
        /// Gets or sets the number of players who have escaped as <see cref="RoleTypeId.ClassD"/>.
        /// </summary>
        public static int EscapedDClasses
        {
            get => RoundSummary.EscapedClassD;
            set => RoundSummary.EscapedClassD = value;
        }

        /// <summary>
        /// Gets or sets the number of players who have escaped as <see cref="RoleTypeId.Scientist"/>.
        /// </summary>
        public static int EscapedScientists
        {
            get => RoundSummary.EscapedScientists;
            set => RoundSummary.EscapedScientists = value;
        }

        /// <summary>
        /// Gets or sets the number of kills.
        /// </summary>
        public static int Kills
        {
            get => RoundSummary.Kills;
            set => RoundSummary.Kills = value;
        }

        /// <summary>
        /// Gets or sets the number of surviving SCPs.
        /// </summary>
        public static int SurvivingSCPs
        {
            get => RoundSummary.SurvivingSCPs;
            set => RoundSummary.SurvivingSCPs = value;
        }

        /// <summary>
        /// Gets or sets the number of kills made by SCPs.
        /// </summary>
        public static int KillsByScp
        {
            get => RoundSummary.KilledBySCPs;
            set => RoundSummary.KilledBySCPs = value;
        }

        /// <summary>
        /// Gets or sets the number of players who have been turned into zombies.
        /// </summary>
        public static int ChangedIntoZombies
        {
            get => RoundSummary.ChangedIntoZombies;
            set => RoundSummary.ChangedIntoZombies = value;
        }

        /// <summary>
        /// Gets or sets the timer for waiting players in lobby.
        /// </summary>
        public static short LobbyWaitingTime
        {
            get => RoundStart.singleton.NetworkTimer;
            set => RoundStart.singleton.NetworkTimer = value;
        }

        /// <summary>
        /// Gets or sets the action to do at round end.
        /// </summary>
        public static ServerStatic.NextRoundAction NextRoundAction
        {
            get => ServerStatic.StopNextRound;
            set => ServerStatic.StopNextRound = value;
        }

        /// <summary>
        /// Gets the number of rounds since the server started.
        /// </summary>
        public static int UptimeRounds => RoundRestart.UptimeRounds;

        /// <summary>
        /// Gets a <see cref="IEnumerable{T}"/> indicating the sides that are currently alive.
        /// </summary>
        public static IEnumerable<Side> AliveSides
        {
            get
            {
                List<Side> sides = new(4);
                foreach (Player player in Player.Get(ply => ply.IsAlive))
                {
                    if (!sides.Contains(player.Role.Side))
                        sides.Add(player.Role.Side);
                }

                return sides;
            }
        }

        /// <summary>
        /// Restarts the round with custom settings.
        /// </summary>
        /// <param name="fastRestart">
        /// Indicates whether or not it'll be a fast restart.
        /// If it's a fast restart, then players won't be reconnected from
        /// the server; otherwise, they will.
        /// </param>
        /// <param name="overrideRestartAction">
        /// Overrides a value of <see cref="ServerStatic.NextRoundAction"/>.
        /// Makes sense if someone used a command to set another action.
        /// </param>
        /// <param name="restartAction">
        /// The <see cref="ServerStatic.NextRoundAction"/>.
        /// <para>
        /// <see cref="ServerStatic.NextRoundAction.DoNothing"/> - does nothing, just restarts the round silently.
        /// <see cref="ServerStatic.NextRoundAction.Restart"/> - restarts the server, reconnects all players.
        /// <see cref="ServerStatic.NextRoundAction.Shutdown"/> - shutdowns the server, also disconnects all players.
        /// </para>
        /// </param>
        public static void Restart(bool fastRestart = true, bool overrideRestartAction = false, ServerStatic.NextRoundAction restartAction = ServerStatic.NextRoundAction.DoNothing)
        {
            if (overrideRestartAction)
                ServerStatic.StopNextRound = restartAction;

            // Great hack since the game has no
            // hard dependency on 'CustomNetworkManager.EnableFastRestart'
            bool oldValue = CustomNetworkManager.EnableFastRestart;
            CustomNetworkManager.EnableFastRestart = fastRestart;

            RoundRestart.InitiateRoundRestart();

            CustomNetworkManager.EnableFastRestart = oldValue;
        }

        /// <summary>
        /// Restarts the round silently.
        /// </summary>
        public static void RestartSilently() => Restart(true, true, ServerStatic.NextRoundAction.DoNothing);

        /// <summary>
        /// Forces the round to end, regardless of which factions are alive.
        /// </summary>
        /// <param name="forceEnd">
        /// Indicates whether or not it'll force the restart with no check if it's locked.
        /// </param>
        /// <returns>A <see cref="bool"/> describing whether or not the round was successfully ended.</returns>
        public static bool EndRound(bool forceEnd = false)
        {
            if (RoundSummary.singleton.KeepRoundOnOne && Player.Dictionary.Count < 2 && !forceEnd)
                return false;

            if ((IsStarted && !IsLocked) || forceEnd)
            {
                RoundSummary.singleton.ForceEnd();
                return true;
            }

            return false;
        }

        /// <summary>
        /// Start the round.
        /// </summary>
        public static void Start() => CharacterClassManager.ForceRoundStart();
    }
}
