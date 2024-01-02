// -----------------------------------------------------------------------
// <copyright file="FindingPositionEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Scp2536
{
    using Christmas.Scp2536;
    using Exiled.API.Features;
    using Exiled.Events.EventArgs.Interfaces;

    /// <summary>
    /// Contains all information before SCP-2536 chooses target for spawning.
    /// </summary>
    public class FindingPositionEventArgs : IDeniableEvent, IPlayerEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FindingPositionEventArgs"/> class.
        /// </summary>
        /// <param name="player"><inheritdoc cref="Player"/></param>
        /// <param name="spawnpoint"><inheritdoc cref="Spawnpoint"/></param>
        /// <param name="isAllowed"><inheritdoc cref="IsAllowed"/></param>
        public FindingPositionEventArgs(Player player, Scp2536Spawnpoint spawnpoint, bool isAllowed = true)
        {
            Player = player;
            Spawnpoint = spawnpoint;
            IsAllowed = isAllowed;
        }

        /// <inheritdoc/>
        public bool IsAllowed { get; set; }

        /// <summary>
        /// Gets the player near whom SCP-2536 will spawn.
        /// </summary>
        public Player Player { get; }

        /// <summary>
        /// Gets or sets the spawn point where SCP will spawn.
        /// </summary>
        public Scp2536Spawnpoint Spawnpoint { get; set; }
    }
}