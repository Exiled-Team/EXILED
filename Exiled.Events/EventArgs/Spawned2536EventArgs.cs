// -----------------------------------------------------------------------
// <copyright file="Spawned2536EventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs
{
    using System;
    using System.Collections.Generic;
    using Exiled.API.Features;

    /// <summary>
    /// Contains all informations after SCP-2536 spawns.
    /// </summary>
    public class Spawned2536EventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Spawned2536EventArgs"/> class.
        /// </summary>
        /// <param name="player">The player who SCP-2536 spawned for.</param>
        /// <param name="spawn">The selected <see cref="SCP2536_Spawn_Location"/>.</param>
        /// <param name="presents">The list of <see cref="SCP2536_Present"/> instances.</param>
        public Spawned2536EventArgs(Player player, SCP2536_Spawn_Location spawn, List<SCP2536_Present> presents)
        {
            Player = player;
            Spawn = spawn;
            Presents = presents;
        }

        /// <summary>
        /// Gets the targeted player.
        /// </summary>
        public Player Player { get; }

        /// <summary>
        /// Gets the spawn position of SCP-2536.
        /// </summary>
        public SCP2536_Spawn_Location Spawn { get; }

        /// <summary>
        /// Gets a List of <see cref="SCP2536_Present"/>.
        /// </summary>
        public List<SCP2536_Present> Presents { get; }
    }
}
