// -----------------------------------------------------------------------
// <copyright file="RoleExtensions.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Extensions {
    using System;

    using Exiled.API.Enums;

    using UnityEngine;

    /// <summary>
    /// A set of extensions for <see cref="RoleType"/>.
    /// </summary>
    public static class RoleExtensions {
        /// <summary>
        /// Get a <see cref="RoleType">role's</see> <see cref="Color"/>.
        /// </summary>
        /// <param name="role">The <see cref="RoleType"/> to get the color of.</param>
        /// <returns>The <see cref="Color"/> of the role.</returns>
        public static Color GetColor(this RoleType role) => role == RoleType.None ? Color.white : CharacterClassManager._staticClasses.Get(role).classColor;

        /// <summary>
        /// Get a <see cref="RoleType">role's</see> <see cref="Side"/>.
        /// </summary>
        /// <param name="role">The <see cref="RoleType"/> to check the side of.</param>
        /// <returns><see cref="Side"/>.</returns>
        public static Side GetSide(this RoleType role) => role.GetTeam().GetSide();

        /// <summary>
        /// Get a <see cref="Team">team's</see> <see cref="Side"/>.
        /// </summary>
        /// <param name="team">The <see cref="Team"/> to get the <see cref="Side"/> of.</param>
        /// <returns><see cref="Side"/>.</returns>.
        public static Side GetSide(this Team team) {
            switch (team) {
                case Team.SCP:
                    return Side.Scp;
                case Team.MTF:
                case Team.RSC:
                    return Side.Mtf;
                case Team.CHI:
                case Team.CDP:
                    return Side.ChaosInsurgency;
                case Team.TUT:
                    return Side.Tutorial;
                default:
                    return Side.None;
            }
        }

        /// <summary>
        /// Get the <see cref="Team"/> of the given <see cref="RoleType"/>.
        /// </summary>
        /// <param name="roleType">Role.</param>
        /// <returns><see cref="Team"/>.</returns>
        public static Team GetTeam(this RoleType roleType) {
            switch (roleType) {
                case RoleType.ChaosConscript:
                case RoleType.ChaosMarauder:
                case RoleType.ChaosRepressor:
                case RoleType.ChaosRifleman:
                    return Team.CHI;
                case RoleType.Scientist:
                    return Team.RSC;
                case RoleType.ClassD:
                    return Team.CDP;
                case RoleType.Scp049:
                case RoleType.Scp93953:
                case RoleType.Scp93989:
                case RoleType.Scp0492:
                case RoleType.Scp079:
                case RoleType.Scp096:
                case RoleType.Scp106:
                case RoleType.Scp173:
                    return Team.SCP;
                case RoleType.Spectator:
                    return Team.RIP;
                case RoleType.FacilityGuard:
                case RoleType.NtfCaptain:
                case RoleType.NtfPrivate:
                case RoleType.NtfSergeant:
                case RoleType.NtfSpecialist:
                    return Team.MTF;
                case RoleType.Tutorial:
                    return Team.TUT;
                default:
                    return Team.RIP;
            }
        }

        /// <summary>
        /// Get the <see cref="LeadingTeam"/>.
        /// </summary>
        /// <param name="team">Team.</param>
        /// <returns><see cref="LeadingTeam"/>.</returns>
        public static LeadingTeam GetLeadingTeam(this Team team) {
            switch (team) {
                case Team.CDP:
                case Team.CHI:
                    return LeadingTeam.ChaosInsurgency;
                case Team.MTF:
                case Team.RSC:
                    return LeadingTeam.FacilityForces;
                case Team.SCP:
                    return LeadingTeam.Anomalies;
                default:
                    return LeadingTeam.Draw;
            }
        }

        /// <summary>
        /// Gets a random spawn point of a <see cref="RoleType"/>.
        /// </summary>
        /// <param name="roleType">The <see cref="RoleType"/> to get the spawn point from.</param>
        /// <returns>Returns the spawn point <see cref="Vector3"/> and rotation <see cref="float"/>.</returns>
        public static Tuple<Vector3, float> GetRandomSpawnProperties(this RoleType roleType) {
            GameObject randomPosition = SpawnpointManager.GetRandomPosition(roleType);

            return randomPosition == null ? new Tuple<Vector3, float>(Vector3.zero, 0f) : new Tuple<Vector3, float>(randomPosition.transform.position, randomPosition.transform.rotation.eulerAngles.y);
        }
    }
}
