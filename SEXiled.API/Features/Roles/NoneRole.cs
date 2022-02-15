// -----------------------------------------------------------------------
// <copyright file="NoneRole.cs" company="SEXiled Team">
// Copyright (c) SEXiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace SEXiled.API.Features.Roles
{
    /// <summary>
    /// Defines a role that represents players with no role.
    /// </summary>
    public class NoneRole : Role
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NoneRole"/> class.
        /// </summary>
        /// <param name="player">The encapsulated player.</param>
        internal NoneRole(Player player) => Owner = player;

        /// <inheritdoc/>
        public override Player Owner { get; }

        /// <inheritdoc/>
        internal override RoleType RoleType { get; } = RoleType.None;
    }
}
