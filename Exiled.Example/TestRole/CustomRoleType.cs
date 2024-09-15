// -----------------------------------------------------------------------
// <copyright file="CustomRoleType.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Example.TestRole
{
    using Exiled.CustomModules.API.Enums;

    /// <summary>
    /// The custom role type.
    /// </summary>
    public class CustomRoleType : UUCustomRoleType
    {
        /// <summary>
        /// Initializes a new custom role id.
        /// </summary>
        public static readonly CustomRoleType TestRole = new();
    }
}