// -----------------------------------------------------------------------
// <copyright file="ISubroutinedScpRole.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Roles
{
    using PlayerRoles.Subroutines;

    /// <summary>
    /// Defines a interface that represents a <see cref="SubroutineManagerModule"/> in player's role.
    /// </summary>
    public interface ISubroutinedScpRole
    {
        /// <summary>
        /// Gets the SCP <see cref="SubroutineManagerModule"/>.
        /// </summary>
        SubroutineManagerModule SubroutineModule { get; }
    }
}