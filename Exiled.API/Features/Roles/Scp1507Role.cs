// -----------------------------------------------------------------------
// <copyright file="Scp1507Role.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Roles
{
    using Exiled.API.Enums;
    using Exiled.API.Interfaces;
    using PlayerRoles;

    using BaseRole = PlayerRoles.PlayableScps.Scp1507.Scp1507Role;

    /// <summary>
    /// A wrapper for <see cref="BaseRole"/>.
    /// </summary>
    public class Scp1507Role : Role, IWrapper<BaseRole>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Scp1507Role"/> class.
        /// </summary>
        /// <param name="baseRole">the base <see cref="PlayerRoleBase"/>.</param>
        public Scp1507Role(BaseRole baseRole)
            : base(baseRole)
        {
            Base = baseRole;
        }

        /// <inheritdoc/>
        public override RoleTypeId Type => RoleTypeId.Flamingo;

        /// <inheritdoc/>
        public new BaseRole Base { get; }

        /// <summary>
        /// Gets or sets sync spawn reason for role.
        /// </summary>
        public SpawnReason SyncSpawnReason
        {
            get => (SpawnReason)Base._syncSpawnReason;
            set => Base._syncSpawnReason = (RoleChangeReason)value;
        }
    }
}