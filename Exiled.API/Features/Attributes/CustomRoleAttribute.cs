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
        public CustomRoleAttribute(RoleTypeId type)
        {
            RoleTypeId = type;
        }

        /// <summary>
        /// Gets the attribute's <see cref="PlayerRoles.RoleTypeId"/>.
        /// </summary>
        public RoleTypeId RoleTypeId { get; }

        /// <summary>
        /// Gets or sets the linked Team Id.
        /// </summary>
        public uint? TeamId { get; set; } = null;
    }
}