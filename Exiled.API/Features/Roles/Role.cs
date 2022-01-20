// -----------------------------------------------------------------------
// <copyright file="Role.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Roles
{
    using Exiled.API.Extensions;

    /// <summary>
    /// Defines the class for role-related classes.
    /// </summary>
    public abstract class Role
    {
        /// <summary>
        /// Gets the player this role is referring to.
        /// </summary>
        public abstract Player Player { get; }

        /// <summary>
        /// Gets the <see cref="global::RoleType"/> of this role.
        /// </summary>
        public abstract RoleType RoleType { get; }

        /// <summary>
        /// Gets the <see cref="global::Team"/> of this role.
        /// </summary>
        public Team Team => RoleType.GetTeam();

        /// <summary>
        /// Gets the <see cref="Enums.Side"/> of this role.
        /// </summary>
        public Enums.Side Side => RoleType.GetSide();

        /// <summary>
        /// Gets the <see cref="UnityEngine.Color"/> of this role.
        /// </summary>
        public UnityEngine.Color Color => RoleType.GetColor();

        /// <summary>
        /// Gets a value indicating whether or not this role is still valid. This will only ever be <see langword="false"/> if the Role is stored and accessed at a later date.
        /// </summary>
        public bool IsValid => RoleType == Player.ReferenceHub.characterClassManager.NetworkCurClass;
    }
}
