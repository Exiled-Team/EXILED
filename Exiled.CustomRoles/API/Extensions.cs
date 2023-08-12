// -----------------------------------------------------------------------
// <copyright file="Extensions.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomRoles.API
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    using Exiled.API.Features;
    using Exiled.CustomRoles.API.Features;

    /// <summary>
    /// A collection of API methods.
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Gets a <see cref="ReadOnlyCollection{T}"/> of the player's current custom roles.
        /// </summary>
        /// <param name="player">The <see cref="Player"/> to check for roles.</param>
        /// <returns>A <see cref="ReadOnlyCollection{T}"/> of all current custom roles.</returns>
        public static ReadOnlyCollection<CustomRole> GetCustomRoles(this Player player)
        {
            List<CustomRole> roles = new();

            foreach (CustomRole customRole in CustomRole.Registered)
            {
                if (customRole.Check(player))
                    roles.Add(customRole);
            }

            return roles.AsReadOnly();
        }

        /// <summary>
        /// Gets a <see cref="ReadOnlyCollection{T}"/> of the player's current custom teams.
        /// </summary>
        /// <param name="player">The <see cref="Player"/> to check for teams.</param>
        /// <returns>A <see cref="ReadOnlyCollection{T}"/> of all current custom teams.</returns>
        public static ReadOnlyCollection<CustomTeam> GetCustomTeams(this Player player)
        {
            List<CustomTeam> teams = new();

            foreach (CustomRole customRole in CustomRole.Registered)
            {
                if (customRole.Check(player) && customRole.CustomTeam != null)
                    teams.Add(customRole.CustomTeam);
            }

            return teams.AsReadOnly();
        }

        /// <summary>
        /// Registers an <see cref="IEnumerable{T}"/> of <see cref="CustomRole"/>s.
        /// </summary>
        /// <param name="customRoles"><see cref="CustomRole"/>s to be registered.</param>
        public static void Register(this IEnumerable<CustomRole> customRoles)
        {
            if (customRoles is null)
                throw new ArgumentNullException(nameof(customRoles));

            foreach (CustomRole customRole in customRoles)
                customRole.TryRegister();
        }

        /// <summary>
        /// Registers an <see cref="IEnumerable{T}"/> of <see cref="CustomTeam"/>s.
        /// </summary>
        /// <param name="customTeams"><see cref="CustomRole"/>s to be registered.</param>
        public static void Register(this IEnumerable<CustomTeam> customTeams)
        {
            if (customTeams is null)
                throw new ArgumentNullException(nameof(customTeams));

            foreach (CustomTeam customTeam in customTeams)
                customTeam.TryRegister();
        }

        /// <summary>
        /// Registers a <see cref="CustomRole"/>.
        /// </summary>
        /// <param name="role"><see cref="CustomRole"/> to be registered.</param>
        public static void Register(this CustomRole role) => role.TryRegister();

        /// <summary>
        /// Registers a <see cref="CustomTeam"/>.
        /// </summary>
        /// <param name="team"><see cref="CustomTeam"/> to be registered.</param>
        public static void Register(this CustomTeam team) => team.TryRegister();

        /// <summary>
        /// Registers a <see cref="CustomAbility"/>.
        /// </summary>
        /// <param name="ability">The <see cref="CustomAbility"/> to be registered.</param>
        public static void Register(this CustomAbility ability) => ability.TryRegister();

        /// <summary>
        /// Unregisters an <see cref="IEnumerable{T}"/> of <see cref="CustomRole"/>s.
        /// </summary>
        /// <param name="customRoles"><see cref="CustomRole"/>s to be unregistered.</param>
        public static void Unregister(this IEnumerable<CustomRole> customRoles)
        {
            if (customRoles is null)
                throw new ArgumentNullException(nameof(customRoles));

            foreach (CustomRole customRole in customRoles)
                customRole.TryUnregister();
        }

        /// <summary>
        /// Unregisters an <see cref="IEnumerable{T}"/> of <see cref="CustomTeam"/>s.
        /// </summary>
        /// <param name="customTeams"><see cref="CustomTeam"/>s to be unregistered.</param>
        public static void Unregister(this IEnumerable<CustomTeam> customTeams)
        {
            if (customTeams is null)
                throw new ArgumentNullException(nameof(customTeams));

            foreach (CustomTeam customTeam in customTeams)
                customTeam.TryUnregister();
        }

        /// <summary>
        /// Unregisters a <see cref="CustomRole"/>.
        /// </summary>
        /// <param name="role"><see cref="CustomRole"/> to be unregistered.</param>
        public static void Unregister(this CustomRole role) => role.TryUnregister();

        /// <summary>
        /// Unregisters a <see cref="CustomTeam"/>.
        /// </summary>
        /// <param name="role"><see cref="CustomTeam"/> to be unregistered.</param>
        public static void Unregister(this CustomTeam role) => role.TryUnregister();

        /// <summary>
        /// Unregisters a <see cref="CustomAbility"/>.
        /// </summary>
        /// <param name="ability">The <see cref="CustomAbility"/> to be unregistered.</param>
        public static void Unregister(this CustomAbility ability) => ability.TryUnregister();
    }
}