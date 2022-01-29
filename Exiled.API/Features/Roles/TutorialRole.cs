// -----------------------------------------------------------------------
// <copyright file="TutorialRole.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Roles
{

    /// <summary>
    /// Defines a role that represents a tutorial class.
    /// </summary>
    public class TutorialRole : Role
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TutorialRole"/> class.
        /// </summary>
        /// <param name="player">The encapsulated player.</param>
        internal TutorialRole(Player player)
        {
            Owner = player;
        }

        /// <inheritdoc/>
        public override Player Owner { get; }

        /// <inheritdoc/>
        public override RoleType Type => RoleType.Tutorial;
    }
}
