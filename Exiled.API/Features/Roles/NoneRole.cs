// -----------------------------------------------------------------------
// <copyright file="NoneRole.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Roles
{
    using System.Diagnostics;

    using PlayerRoles;

    using NoneGameRole = PlayerRoles.NoneRole;

    /// <summary>
    /// Defines a role that represents players with no role.
    /// </summary>
    [DebuggerDisplay("None")]
    public class NoneRole : Role
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NoneRole"/> class.
        /// </summary>
        /// <param name="baseRole">the base <see cref="NoneGameRole"/>.</param>
        internal NoneRole(PlayerRoleBase baseRole)
            : base(baseRole)
        {
        }

        /// <inheritdoc/>
        public override RoleTypeId Type { get; } = RoleTypeId.None;
    }
}