// -----------------------------------------------------------------------
// <copyright file="IHumeShieldRole.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Roles
{
    using PlayerRoles.PlayableScps.HumeShield;

    /// <summary>
    /// Represents a role that supports a hume shield.
    /// </summary>
    public interface IHumeShieldRole
    {
        /// <summary>
        /// Gets a reference to the role's <see cref="HumeShieldModuleBase"/>.
        /// </summary>
        HumeShieldModuleBase HumeShieldModule { get; }
    }
}
