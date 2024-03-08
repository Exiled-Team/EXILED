// -----------------------------------------------------------------------
// <copyright file="RoleExtensions.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Enums;
    using Exiled.API.Features.Spawn;
    using InventorySystem;
    using InventorySystem.Configs;
    using PlayerRoles;
    using PlayerRoles.FirstPersonControl;

    using UnityEngine;

    using Team = PlayerRoles.Team;

    /// <summary>
    /// A set of extensions for <see cref="RoleTypeId"/>.
    /// </summary>
    public static class RoleExtensions
    {
        /// <summary>
        /// Gets a <see cref="RoleTypeId">role's</see> <see cref="Color"/>.
        /// </summary>
        /// <param name="roleType">The <see cref="RoleTypeId"/> to get the color of.</param>
        /// <returns>The <see cref="Color"/> of the role.</returns>
        public static Color GetColor(this RoleTypeId roleType) => roleType == RoleTypeId.None ? Color.white : roleType.GetRoleBase().RoleColor;

        /// <summary>
        /// Gets a <see cref="RoleTypeId">role's</see> <see cref="Side"/>.
        /// </summary>
        /// <param name="roleType">The <see cref="RoleTypeId"/> to check the side of.</param>
        /// <returns><see cref="Side"/>.</returns>
        public static Side GetSide(this RoleTypeId roleType) => PlayerRolesUtils.GetTeam(roleType).GetSide();

        /// <summary>
        /// Gets a <see cref="Team">team's</see> <see cref="Side"/>.
        /// </summary>
        /// <param name="team">The <see cref="Team"/> to get the <see cref="Side"/> of.</param>
        /// <returns><see cref="Side"/>.</returns>.
        public static Side GetSide(this Team team) => team switch
        {
            Team.SCPs => Side.Scp,
            Team.FoundationForces or Team.Scientists => Side.Mtf,
            Team.ChaosInsurgency or Team.ClassD => Side.ChaosInsurgency,
            Team.OtherAlive => Side.Tutorial,
            _ => Side.None,
        };

        /// <summary>
        /// Gets the <see cref="Team"/> of the given <see cref="RoleTypeId"/>.
        /// </summary>
        /// <param name="roleType">The <see cref="RoleTypeId"/>.</param>
        /// <returns><see cref="Team"/>.</returns>
        public static Team GetTeam(this RoleTypeId roleType) => roleType switch
        {
            RoleTypeId.ChaosConscript or RoleTypeId.ChaosMarauder or RoleTypeId.ChaosRepressor or RoleTypeId.ChaosRifleman => Team.ChaosInsurgency,
            RoleTypeId.Scientist => Team.Scientists,
            RoleTypeId.ClassD => Team.ClassD,
            RoleTypeId.Scp049 or RoleTypeId.Scp939 or RoleTypeId.Scp0492 or RoleTypeId.Scp079 or RoleTypeId.Scp096 or RoleTypeId.Scp106 or RoleTypeId.Scp173 or RoleTypeId.Scp3114 => Team.SCPs,
            RoleTypeId.FacilityGuard or RoleTypeId.NtfCaptain or RoleTypeId.NtfPrivate or RoleTypeId.NtfSergeant or RoleTypeId.NtfSpecialist => Team.FoundationForces,
            RoleTypeId.Tutorial => Team.OtherAlive,
            _ => Team.Dead,
        };

        /// <summary>
        /// Gets the full name of the given <see cref="RoleTypeId"/>.
        /// </summary>
        /// <param name="typeId">The <see cref="RoleTypeId"/>.</param>
        /// <returns>The full name.</returns>
        public static string GetFullName(this RoleTypeId typeId) => typeId.GetRoleBase().RoleName;

        /// <summary>
        /// Gets the base <see cref="PlayerRoleBase"/> of the given <see cref="RoleTypeId"/>.
        /// </summary>
        /// <param name="roleType">The <see cref="RoleTypeId"/>.</param>
        /// <returns>The <see cref="PlayerRoleBase"/>.</returns>
        public static PlayerRoleBase GetRoleBase(this RoleTypeId roleType) => roleType.TryGetRoleBase(out PlayerRoleBase roleBase) ? roleBase : null;

        /// <summary>
        /// Tries to get the base <see cref="PlayerRoleBase"/> of the given <see cref="RoleTypeId"/>.
        /// </summary>
        /// <param name="roleType">The <see cref="RoleTypeId"/>.</param>
        /// <param name="roleBase">The <see cref="PlayerRoleBase"/> to return.</param>
        /// <returns>The <see cref="PlayerRoleBase"/>.</returns>
        public static bool TryGetRoleBase(this RoleTypeId roleType, out PlayerRoleBase roleBase) => PlayerRoleLoader.TryGetRoleTemplate(roleType, out roleBase);

        /// <summary>
        /// Gets the <see cref="LeadingTeam"/>.
        /// </summary>
        /// <param name="team">Team.</param>
        /// <returns><see cref="LeadingTeam"/>.</returns>
        public static LeadingTeam GetLeadingTeam(this Team team) => team switch
        {
            Team.ClassD or Team.ChaosInsurgency => LeadingTeam.ChaosInsurgency,
            Team.FoundationForces or Team.Scientists => LeadingTeam.FacilityForces,
            Team.SCPs => LeadingTeam.Anomalies,
            _ => LeadingTeam.Draw,
        };

        /// <summary>
        /// Checks whether a <see cref="RoleTypeId"/> is an <see cref="IFpcRole"/> or not.
        /// </summary>
        /// <param name="roleType">The <see cref="RoleTypeId"/>.</param>
        /// <returns>Returns whether <paramref name="roleType"/> is an <see cref="IFpcRole"/> or not.</returns>
        public static bool IsFpcRole(this RoleTypeId roleType) => roleType.GetRoleBase() is IFpcRole;

        /// <summary>
        /// Gets a random spawn point of a <see cref="RoleTypeId"/>.
        /// </summary>
        /// <param name="roleType">The <see cref="RoleTypeId"/> to get the spawn point from.</param>
        /// <returns>Returns a <see cref="SpawnLocation"/> representing the spawn, or <see langword="null"/> if no spawns were found.</returns>
        public static SpawnLocation GetRandomSpawnLocation(this RoleTypeId roleType)
        {
            if (roleType.GetRoleBase() is IFpcRole fpcRole &&
                fpcRole.SpawnpointHandler != null &&
                fpcRole.SpawnpointHandler.TryGetSpawnpoint(out Vector3 position, out float horizontalRotation))
            {
                return new SpawnLocation(roleType, position, horizontalRotation);
            }

            return null;
        }

        /// <summary>
        /// Gets the starting items of a <see cref="RoleTypeId"/>.
        /// </summary>
        /// <param name="roleType">The <see cref="RoleTypeId"/>.</param>
        /// <returns>An <see cref="Array"/> of <see cref="ItemType"/> that the role receives on spawn. Will be empty for classes that do not spawn with items.</returns>
        public static ItemType[] GetStartingInventory(this RoleTypeId roleType)
        {
            if (StartingInventories.DefinedInventories.TryGetValue(roleType, out InventoryRoleInfo info))
                return info.Items;

            return Array.Empty<ItemType>();
        }

        /// <summary>
        /// Gets the starting ammo of a <see cref="RoleTypeId"/>.
        /// </summary>
        /// <param name="roleType">The <see cref="RoleTypeId"/>.</param>
        /// <returns>An <see cref="Array"/> of <see cref="ItemType"/> that the role receives on spawn. Will be empty for classes that do not spawn with ammo.</returns>
        public static Dictionary<AmmoType, ushort> GetStartingAmmo(this RoleTypeId roleType)
        {
            if (StartingInventories.DefinedInventories.TryGetValue(roleType, out InventoryRoleInfo info))
                return info.Ammo.ToDictionary(kvp => kvp.Key.GetAmmoType(), kvp => kvp.Value);

            return new();
        }
    }
}