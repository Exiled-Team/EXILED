// -----------------------------------------------------------------------
// <copyright file="RoleExtensions.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Extensions
{
    using System;

    using Exiled.API.Enums;

    using UnityEngine;

    /// <summary>
    /// A set of extensions for <see cref="RoleType"/>.
    /// </summary>
    public static class RoleExtensions
    {
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
        public static Side GetSide(this Team team) => team switch
        {
            Team.SCP => Side.Scp,
            Team.MTF or Team.RSC => Side.Mtf,
            Team.CHI or Team.CDP => Side.ChaosInsurgency,
            Team.TUT => Side.Tutorial,
            _ => Side.None,
        };

        /// <summary>
        /// Get the <see cref="Team"/> of the given <see cref="RoleType"/>.
        /// </summary>
        /// <param name="roleType">Role.</param>
        /// <returns><see cref="Team"/>.</returns>
        public static Team GetTeam(this RoleType roleType) => roleType switch
        {
            RoleType.ChaosConscript or RoleType.ChaosMarauder or RoleType.ChaosRepressor or RoleType.ChaosRifleman => Team.CHI,
            RoleType.Scientist => Team.RSC,
            RoleType.ClassD => Team.CDP,
            RoleType.Scp049 or RoleType.Scp93953 or RoleType.Scp93989 or RoleType.Scp0492 or RoleType.Scp079 or RoleType.Scp096 or RoleType.Scp106 or RoleType.Scp173 => Team.SCP,
            RoleType.Spectator => Team.RIP,
            RoleType.FacilityGuard or RoleType.NtfCaptain or RoleType.NtfPrivate or RoleType.NtfSergeant or RoleType.NtfSpecialist => Team.MTF,
            RoleType.Tutorial => Team.TUT,
            _ => Team.RIP,
        };

        /// <summary>
        /// Gets the full name of the given <see cref="RoleType"/>.
        /// </summary>
        /// <param name="roleType">Role.</param>
        /// <returns>The full name.</returns>
        public static string GetFullName(this RoleType roleType) => CharacterClassManager._staticClasses.SafeGet(roleType).fullName;

        /// <summary>
        /// Get the <see cref="LeadingTeam"/>.
        /// </summary>
        /// <param name="team">Team.</param>
        /// <returns><see cref="LeadingTeam"/>.</returns>
        public static LeadingTeam GetLeadingTeam(this Team team) => team switch
        {
            Team.CDP or Team.CHI => LeadingTeam.ChaosInsurgency,
            Team.MTF or Team.RSC => LeadingTeam.FacilityForces,
            Team.SCP => LeadingTeam.Anomalies,
            _ => LeadingTeam.Draw,
        };

        /// <summary>
        /// Gets a random spawn point of a <see cref="RoleType"/>.
        /// </summary>
        /// <param name="roleType">The <see cref="RoleType"/> to get the spawn point from.</param>
        /// <returns>Returns the spawn point <see cref="Vector3"/> and rotation <see cref="float"/>.</returns>
        public static Tuple<Vector3, float> GetRandomSpawnProperties(this RoleType roleType)
        {
            GameObject randomPosition = SpawnpointManager.GetRandomPosition(roleType);

            return randomPosition is null ? new Tuple<Vector3, float>(Vector3.zero, 0f) : new Tuple<Vector3, float>(randomPosition.transform.position, randomPosition.transform.rotation.eulerAngles.y);
        }
    }
}
