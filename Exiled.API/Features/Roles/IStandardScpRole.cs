// -----------------------------------------------------------------------
// <copyright file="IStandardScpRole.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Roles
{
    /// <summary>
    /// Represents a Scp Role.
    /// </summary>
    public interface IStandardScpRole
    {
        /// <summary>
        /// Trigger the attack ability of a Scp Role.
        /// </summary>
        /// <param name="player">The player to attack.</param>
        public void Attack(Player player);
    }
}