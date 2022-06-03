// -----------------------------------------------------------------------
// <copyright file="Scp106Container.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features
{
    using System.Collections.Generic;

    using UnityEngine;

    /// <summary>
    /// A wrapper class for <see cref="LureSubjectContainer"/>.
    /// </summary>
    public static class Scp106Container
    {
        /// <summary>
        /// Gets or sets a <see cref="HashSet{T}"/> of <see cref="Player"/> which contains all the players ignored by LureSubjectContainer.
        /// </summary>
        public static HashSet<Player> IgnoredPlayers { get; set; } = new();

        /// <summary>
        /// Gets or sets a <see cref="HashSet{T}"/> of <see cref="RoleType"/> which contains all the roles ignored by LureSubjectContainer.
        /// </summary>
        public static List<RoleType> IgnoredRoles { get; set; } = new() { RoleType.Spectator };

        /// <summary>
        /// Gets or sets a <see cref="HashSet{T}"/> of <see cref="Team"/> which contains all the teams ignored by LureSubjectContainer.
        /// </summary>
        public static List<Team> IgnoredTeams { get; set; } = new() { Team.SCP };

        /// <summary>
        /// Gets the base <see cref="LureSubjectContainer"/>.
        /// </summary>
        public static LureSubjectContainer Base { get; internal set; }

        /// <summary>
        /// Gets the LureSubjectContainer <see cref="UnityEngine.GameObject"/>.
        /// </summary>
        public static GameObject GameObject => Base.gameObject;

        /// <summary>
        /// Gets the LureSubjectContainer <see cref="UnityEngine.Transform"/>.
        /// </summary>
        public static Transform Transform => Base.transform;

        /// <summary>
        /// Gets the LureSubjectContainer position.
        /// </summary>
        public static Vector3 Position => Base._position;

        /// <summary>
        /// Gets the LureSubjectContainer rotation.
        /// </summary>
        public static Quaternion Rotation => Quaternion.Euler(Base._rotation);

        /// <summary>
        /// Gets the <see cref="UnityEngine.BoxCollider"/> of the LureSubjectContainer.
        /// </summary>
        public static BoxCollider BoxCollider { get; internal set; }

        /// <summary>
        /// Gets or sets a value indicating whether or not the SCP-106 container has been used.
        /// </summary>
        public static bool Scp106FemurUsed
        {
            get => OneOhSixContainer.used;
            set => OneOhSixContainer.used = value;
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not the SCP-106 container can be used.
        /// </summary>
        public static bool AllowContain
        {
            get => Base.NetworkallowContain;
            set => Base.NetworkallowContain = value;
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="Player"/> is in the range of the KillZone.
        /// </summary>
        /// <param name="player">The <see cref="Player"/> to check.</param>
        /// <returns><see langword="true"/> if the given <see cref="Player"/> is in the range of the KillZone; otherwise, <see langword="false"/>.</returns>
        public static bool InTheKillZone(Player player) => (player.Position - Position).sqrMagnitude <= 1.97f * 1.97f;

        /// <summary>
        /// Gets a value indicating whether the <see cref="Player"/> can be killed.
        /// </summary>
        /// <param name="player">The <see cref="Player"/> to check.</param>
        /// <returns><see langword="true"/> if the given <see cref="Player"/> can be killed; otherwise, <see langword="false"/>.</returns>
        public static bool CanBeKilled(Player player) => !player.IsGodModeEnabled && !IgnoredPlayers.Contains(player) && !IgnoredRoles.Contains(player.Role) && !IgnoredTeams.Contains(player.Role.Team) && InTheKillZone(player);
    }
}
