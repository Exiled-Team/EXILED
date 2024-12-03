// -----------------------------------------------------------------------
// <copyright file="DestroyedRole.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Roles
{
    using PlayerRoles;

    using BaseDestroyedRole = PlayerRoles.DestroyedRole;

    /// <summary>
    /// Defines a role that represents the Destroyed Role.
    /// </summary>
    public class DestroyedRole : Role
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DestroyedRole"/> class.
        /// </summary>
        /// <param name="baseRole">the base <see cref="DestroyedRole"/>.</param>
        internal DestroyedRole(BaseDestroyedRole baseRole)
            : base(baseRole)
        {
        }

        /// <inheritdoc/>
        public override RoleTypeId Type { get; } = RoleTypeId.Destroyed;
    }
}