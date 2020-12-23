// -----------------------------------------------------------------------
// <copyright file="Spawning2536EventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs
{
    using System;
    using Exiled.API.Features;

    /// <summary>
    /// Contains all informations before SCP-2536 spawns.
    /// </summary>
    public class Spawning2536EventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Spawning2536EventArgs"/> class.
        /// </summary>
        /// <param name="player">The player who SCP-2536 spawned for.</param>
        /// <param name="spawn">The selected <see cref="SCP2536_Spawn_Location"/>.</param>
        /// <param name="isAllowed">Indicates whether or not SCP-2536 can spawn.</param>
        public Spawning2536EventArgs(Player player, SCP2536_Spawn_Location spawn, bool isAllowed = true)
        {
            Player = player;
            Spawn = spawn;
            IsAllowed = isAllowed;
        }

        /// <summary>
        /// Gets or sets the targeted player.
        /// </summary>
        public Player Player { get; set; }

        /// <summary>
        /// Gets the spawn position of SCP-2536.
        /// </summary>
        public SCP2536_Spawn_Location Spawn { get; }

        /// <summary>
        /// Gets or sets a value indicating whether or not SCP-2536 can spawn.
        /// </summary>
        public bool IsAllowed { get; set; }
    }
}
