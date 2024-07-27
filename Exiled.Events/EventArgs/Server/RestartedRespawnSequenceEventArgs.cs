// -----------------------------------------------------------------------
// <copyright file="RestartedRespawnSequenceEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Server
{
    using System.Diagnostics;
    using Interfaces;
    using Respawning;

    /// <summary>
    /// Contains all information after a new respawn sequence has been restarted.
    /// </summary>
    public class RestartedRespawnSequenceEventArgs : IExiledEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RestartedRespawnSequenceEventArgs"/> class.
        /// </summary>
        /// <param name="respawnManager"><inheritdoc cref="RespawnManager"/></param>
        public RestartedRespawnSequenceEventArgs(RespawnManager respawnManager) => RespawnManager = respawnManager;

        /// <summary>
        /// Gets the <see cref="RespawnManager"/> instance.
        /// </summary>
        public RespawnManager RespawnManager { get; }

        /// <summary>
        /// Gets the sequence's timer.
        /// </summary>
        public Stopwatch Timer => RespawnManager._stopwatch;

        /// <summary>
        /// Gets or sets the current <see cref="RespawnManager.RespawnSequencePhase"/>.
        /// </summary>
        public RespawnManager.RespawnSequencePhase CurrentRespawnSequencePhase
        {
            get => RespawnManager._curSequence;
            set => RespawnManager._curSequence = value;
        }

        /// <summary>
        /// Gets or sets the time for the next sequence.
        /// </summary>
        public float TimeForNextSequence
        {
            get => RespawnManager.TimeTillRespawn;
            set
            {
                RespawnManager._timeForNextSequence = value;
                Timer.Restart();
            }
        }
    }
}