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
        /// <param name="roleChangeReason">the player's spawned with <see cref="RoleChangeReason" />.</param>
        /// <param name="roleSpawnFlags">the player's spawned with <see cref="RoleSpawnFlags" />.</param>
        public SpawnedEventArgs(ReferenceHub player, PlayerRoleBase oldRole, RoleChangeReason roleChangeReason, RoleSpawnFlags roleSpawnFlags)
        {
            Player = Player.Get(player);
            OldRole = Role.Create(oldRole);
            Reason = (SpawnReason)roleChangeReason;
            SpawnFlags = roleSpawnFlags;
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
