// -----------------------------------------------------------------------
// <copyright file="SpawningTeamVehicleEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Map
{
    using Exiled.Events.EventArgs.Interfaces;
    using Respawning;

    /// <summary>
    ///     Contains all information before the server spawns a team's respawn vehicle.
    /// </summary>
    public class SpawningTeamVehicleEventArgs : IDeniableEvent
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="SpawningTeamVehicleEventArgs" /> class.
        /// </summary>
        /// <param name="team">
        ///     The team who the vehicle belongs to.
        /// </param>
        /// <param name="isAllowed">
        ///     <inheritdoc cref="IsAllowed" />
        /// </param>
        public SpawningTeamVehicleEventArgs(SpawnableTeamType team, bool isAllowed = true)
        {
            Team = team;
            IsAllowed = isAllowed;
        }

        /// <summary>
        ///     Gets or sets which vehicle should spawn.
        /// </summary>
        public SpawnableTeamType Team { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether or not the vehicle can be spawned.
        /// </summary>
        public bool IsAllowed { get; set; } = true;
    }
}