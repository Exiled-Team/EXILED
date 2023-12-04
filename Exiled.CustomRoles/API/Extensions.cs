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
    using System.Linq;

    using Exiled.API.Features;
    using Exiled.CustomRoles.API.Features;
    using Exiled.CustomRoles.API.Features.Enums;

    using Utils.NonAllocLINQ;

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
        /// Registers a <see cref="CustomRole"/>.
        /// </summary>
        /// <param name="role"><see cref="CustomRole"/> to be registered.</param>
        public static void Register(this CustomRole role) => role.TryRegister();

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
        /// Unregisters a <see cref="CustomRole"/>.
        /// </summary>
        /// <param name="role"><see cref="CustomRole"/> to be unregistered.</param>
        public static void Unregister(this CustomRole role) => role.TryUnregister();

        /// <summary>
        /// Unregisters a <see cref="CustomAbility"/>.
        /// </summary>
        /// <param name="ability">The <see cref="CustomAbility"/> to be unregistered.</param>
        public static void Unregister(this CustomAbility ability) => ability.TryUnregister();

        /// <summary>
        /// Gets all <see cref="ActiveAbility"/>s a specific <see cref="Player"/> is able to use.
        /// </summary>
        /// <param name="player">The <see cref="Player"/> to get abilities for.</param>
        /// <returns>A <see cref="IEnumerable{T}"/> of their active abilities, or <see langword="null"/> if none.</returns>
        public static IEnumerable<ActiveAbility>? GetActiveAbilities(this Player player) => !ActiveAbility.AllActiveAbilities.TryGetValue(player, out HashSet<ActiveAbility> abilities) ? null : abilities;

        /// <summary>
        /// Gets the <see cref="Player"/>'s selected ability.
        /// </summary>
        /// <param name="player">The <see cref="Player"/> to check.</param>
        /// <returns>The <see cref="ActiveAbility"/> the <see cref="Player"/> has selected, or <see langword="null"/>.</returns>
        public static ActiveAbility? GetSelectedAbility(this Player player) => !ActiveAbility.AllActiveAbilities.TryGetValue(player, out HashSet<ActiveAbility> abilities) ? null : abilities.FirstOrDefault(a => a.Check(player, CheckType.Selected));

        /// <summary>
        /// Gets all <see cref="PassiveAbility"/>'s the <see cref="Player"/> currently has.
        /// </summary>
        /// <param name="player">The <see cref="Player"/> to get abilities for.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> of <see cref="PassiveAbility"/>s the player has.</returns>
        public static IEnumerable<PassiveAbility> GetPassiveAbilities(this Player player)
        {
            foreach (CustomAbility ability in CustomAbility.Registered)
            {
                if (ability is PassiveAbility passive && ability.Check(player))
                    yield return passive;
            }
        }

        /// <summary>
        /// Checks to see if a <see cref="Player"/> has a specific <see cref="CustomAbility"/>.
        /// </summary>
        /// <param name="player">The <see cref="Player"/> to check for the ability.</param>
        /// <typeparam name="T">The <see cref="Type"/> of the ability to look for.</typeparam>
        /// <returns><see langword="true"/> if the player has the ability, otherwise <see langword="false"/>.</returns>
        public static bool HasAbility<T>(Player player)
            where T : CustomAbility
        {
            CustomAbility? ability = CustomAbility.Get(typeof(T));
            return ability is ActiveAbility active ? active.Check(player, CheckType.Available) : ability?.Check(player) ?? false;
        }
    }
}