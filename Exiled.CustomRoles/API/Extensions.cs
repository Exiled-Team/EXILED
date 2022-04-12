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
    using Exiled.API.Features.Core;
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

            foreach (EActor component in player.Components)
            {
                if (component.Cast(out CustomRole customRole))
                    roles.Add(customRole);
            }

            return roles.AsReadOnly();
        }

        /// <summary>
        /// Registers an <see cref="IEnumerable{T}"/> of <see cref="CustomRoleBlueprint"/>s.
        /// </summary>
        /// <param name="blueprints"><see cref="CustomRoleBlueprint"/>s to be registered.</param>
        public static void Register(this IEnumerable<CustomRoleBlueprint> blueprints)
        {
            if (blueprints is null)
                throw new ArgumentNullException(nameof(blueprints));

            foreach (CustomRoleBlueprint blueprint in blueprints)
                blueprint.TryRegister();
        }

        /// <summary>
        /// Registers a <see cref="CustomRoleBlueprint"/>.
        /// </summary>
        /// <param name="blueprint"><see cref="CustomRoleBlueprint"/> to be registered.</param>
        public static void Register(this CustomRoleBlueprint blueprint) => blueprint.TryRegister();

        /// <summary>
        /// Registers a <see cref="CustomAbility"/>.
        /// </summary>
        /// <param name="ability">The <see cref="CustomAbility"/> to be registered.</param>
        public static void Register(this CustomAbility ability) => ability.TryRegister();

        /// <summary>
        /// Unregisters an <see cref="IEnumerable{T}"/> of <see cref="CustomRoleBlueprint"/>s.
        /// </summary>
        /// <param name="blueprints"><see cref="CustomRoleBlueprint"/>s to be unregistered.</param>
        public static void Unregister(this IEnumerable<CustomRoleBlueprint> blueprints)
        {
            if (blueprints is null)
                throw new ArgumentNullException(nameof(blueprints));

            foreach (CustomRoleBlueprint blueprint in blueprints)
                blueprint.TryUnregister();
        }

        /// <summary>
        /// Unregisters a <see cref="CustomRoleBlueprint"/>.
        /// </summary>
        /// <param name="blueprint"><see cref="CustomRoleBlueprint"/> to be unregistered.</param>
        public static void Unregister(this CustomRoleBlueprint blueprint) => blueprint.TryUnregister();

        /// <summary>
        /// Unregisters a <see cref="CustomAbility"/>.
        /// </summary>
        /// <param name="ability">The <see cref="CustomAbility"/> to be unregistered.</param>
        public static void Unregister(this CustomAbility ability) => ability.TryUnregister();
    }
}
