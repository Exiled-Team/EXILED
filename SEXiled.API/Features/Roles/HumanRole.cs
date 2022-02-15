// -----------------------------------------------------------------------
// <copyright file="HumanRole.cs" company="SEXiled Team">
// Copyright (c) SEXiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace SEXiled.API.Features.Roles
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
        /// <param name="type">The RoleType.</param>
        internal HumanRole(Player player, RoleType type)
        {
            Owner = player;
            RoleType = type;
        }

        /// <inheritdoc/>
        public override Player Owner { get; }

        /// <inheritdoc/>
        internal override RoleType RoleType { get; }
    }
}
