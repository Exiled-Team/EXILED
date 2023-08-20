// -----------------------------------------------------------------------
// <copyright file="CustomRoleAttribute.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Attributes
{
    using System;

    using PlayerRoles;

    /// <summary>
    /// An attribute to easily manage CustomRole initialization.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public sealed class CustomRoleAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CustomRoleAttribute"/> class.
        /// </summary>
        /// <param name="type">The <see cref="PlayerRoles.RoleTypeId"/> to serialize.</param>
        /// <param name="teamId">To witch team this custom role belong to.</param>
        public CustomRoleAttribute(RoleTypeId type, uint teamId)
        {
            RoleTypeId = type;
            TeamId = teamId;
        }

        /// <inheritdoc cref="CustomRoleAttribute"/>
        /// <param name="type">The <see cref="PlayerRoles.RoleTypeId"/> to serialize.</param>
        public CustomRoleAttribute(RoleTypeId type)
        {
            RoleTypeId = type;
            TeamId = null;
        }

        /// <summary>
        /// Gets the attribute's <see cref="PlayerRoles.RoleTypeId"/>.
        /// </summary>
        public RoleTypeId RoleTypeId { get; }

        /// <summary>
        /// Gets the linked Team Id.
        /// </summary>
        public uint? TeamId { get; }
    }
}