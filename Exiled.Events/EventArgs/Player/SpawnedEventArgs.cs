// -----------------------------------------------------------------------
// <copyright file="SpawnedEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Player
{
    using API.Features;
    using Exiled.API.Enums;
    using Exiled.API.Features.Roles;

    using Interfaces;

    using PlayerRoles;

    /// <summary>
    ///     Contains all information after spawning a player.
    /// </summary>
    public class SpawnedEventArgs : IPlayerEvent
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="SpawnedEventArgs" /> class.
        /// </summary>
        /// <param name="player">the spawned player.</param>
        /// <param name="oldRole">the spawned player's old <see cref="PlayerRoleBase">role</see>.</param>
        public SpawnedEventArgs(Player player, PlayerRoleBase oldRole)
        {
            Player = player;
            OldRole = Role.Create(oldRole);
            Reason = (SpawnReason)Player.Role.SpawnReason;
            SpawnFlags = Player.Role.SpawnFlags;
        }

        /// <summary>
        ///     Gets the <see cref="API.Features.Player" /> who spawned.
        /// </summary>
        public Player Player { get; }

        /// <summary>
        ///     Gets the player's old <see cref="PlayerRoleBase">role</see>.
        /// </summary>
        public Role OldRole { get; }

        /// <summary>
        ///     Gets the reason for their class change.
        /// </summary>
        public SpawnReason Reason { get; }

        /// <summary>
        ///     Gets the spawn flags for their class change.
        /// </summary>
        public RoleSpawnFlags SpawnFlags { get; }
    }
}