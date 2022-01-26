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
            Owner = player;
            Type = player.ReferenceHub.characterClassManager.NetworkCurClass;
        }

        /// <inheritdoc/>
        public override Player Owner { get; }

        /// <inheritdoc/>
        public override RoleType Type { get; }
    }
}
