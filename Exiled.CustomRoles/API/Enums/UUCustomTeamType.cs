// -----------------------------------------------------------------------
// <copyright file="UUCustomTeamType.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomModules.API.Enums
{
    using Exiled.API.Features.Core.Generic;

    /// <summary>
    /// Represents the base enum class for all available custom roles.
    /// </summary>
    public class UUCustomTeamType : UniqueUnmanagedEnumClass<uint, UUCustomTeamType>
    {
        /// <summary>
        /// Represents an invalid custom role.
        /// </summary>
        public static readonly UUCustomTeamType None = new();
    }
}