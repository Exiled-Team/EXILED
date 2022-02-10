// -----------------------------------------------------------------------
// <copyright file="CustomRoleAttribute.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Attributes
{
    using System;

    /// <summary>
    /// An attribute to easily manage CustomRole initialization.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public sealed class CustomRoleAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CustomRoleAttribute"/> class.
        /// </summary>
        /// <param name="type">The <see cref="global::RoleType"/> to serialize.</param>
        public CustomRoleAttribute(RoleType type) => RoleType = type;

        /// <summary>
        /// Gets the attribute's <see cref="global::RoleType"/>.
        /// </summary>
        public RoleType RoleType { get; }
    }
}
