// -----------------------------------------------------------------------
// <copyright file="HumanRole.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Roles
{

    /// <summary>
    /// Defines a role that represents a human class.
    /// </summary>
    public class HumanRole : Role
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HumanRole"/> class.
        /// </summary>
        /// <param name="player">The encapsulated player.</param>
        internal HumanRole(Player player)
        {
            Player = player;
            this.RoleType = player.RoleType;
        }

        /// <inheritdoc/>
        public override Player Player { get; }

        /// <inheritdoc/>
        public override RoleType RoleType { get; }
    }
}
