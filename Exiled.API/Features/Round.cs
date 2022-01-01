// -----------------------------------------------------------------------
// <copyright file="Round.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features
{
    using System;

    using GameCore;

    using MEC;

    using PlayerStatsSystem;

    using RoundRestarting;

    /// <summary>
    /// A set of tools to handle the round more easily.
    /// </summary>
    public static class Round
    {
        /// <summary>
        /// Gets the time elapsed from the start of the round.
        /// </summary>
        public static TimeSpan ElapsedTime => RoundStart.RoundLength;

        /// <summary>
        /// Gets the start time of the round.
        /// </summary>
        public static DateTime StartedTime => DateTime.Now - ElapsedTime;

        /// <summary>
        /// Gets a value indicating whether the round is started or not.
        /// </summary>
        public static bool IsStarted => RoundSummary.RoundInProgress();

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
        /// Gets the number of players who have escaped as <see cref="RoleType.ClassD"/>.
        /// </summary>
        public static int EscapedDClasses => RoundSummary.EscapedClassD;

        /// <summary>
        /// Gets the number of players who have escaped as <see cref="RoleType.Scientist"/>.
        /// </summary>
        public static int EscapedScientists => RoundSummary.EscapedScientists;

        /// <summary>
        /// Gets the number of kills.
        /// </summary>
        public static int Kills => RoundSummary.Kills;

        /// <summary>
        /// Gets the number of kills made by SCPs.
        /// </summary>
        public static int KillsByScp => RoundSummary.KilledBySCPs;

        /// <summary>
        /// Gets the number of players who have been turned into zombies.
        /// </summary>
        public static int ChangedIntoZombies => RoundSummary.ChangedIntoZombies;

        /// <summary>
        /// Gets the number of rounds since the server started.
        /// </summary>
        public static int UptimeRounds => RoundRestart.UptimeRounds;

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
        /// Restarts the round silently after a given delay has passed.
        /// </summary>
        /// <param name="delay">Delay in seconds.</param>
        public static void RestartSilenty(float delay) => Timing.CallDelayed(delay, RestartSilently);

        /// <summary>
        /// Forces the round to end, regardless of which factions are alive.
        /// </summary>
        /// <returns>A <see cref="bool"/> describing whether or not the round was successfully ended.</returns>
        public static bool ForceEnd()
        {
            if (RoundSummary.singleton._keepRoundOnOne && Player.Dictionary.Count < 2)
            {
                return false;
            }

            if (IsStarted && !IsLocked)
            {
                RoundSummary.singleton.ForceEnd();
                return true;
            }

            return false;
        }

        /// <summary>
        /// Forces the round to end after a given delay has passed, regardless of which factions are alive.
        /// </summary>
        /// <remarks>This method does not return any value regarding whether or not the request to end the round was successful. If this value is required, please use ForceEnd() with a coroutine instead of this method.</remarks>
        /// <param name="delay">Delay in seconds.</param>
        public static void ForceEnd(float delay) => Timing.CallDelayed(delay, () => ForceEnd());

        /// <summary>
        /// Start the round.
        /// </summary>
        public static void Start() => CharacterClassManager.ForceRoundStart();

        /// <summary>
        /// Start the round after a given delay has passed.
        /// </summary>
        /// <param name="delay">Delay in seconds.</param>
        public static void Start(float delay) => Timing.CallDelayed(delay, Start);
    }
}
