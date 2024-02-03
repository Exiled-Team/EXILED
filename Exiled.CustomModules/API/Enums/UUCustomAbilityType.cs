// -----------------------------------------------------------------------
// <copyright file="UUCustomAbilityType.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomModules.API.Enums
{
    using Exiled.API.Features.Core.Generic;

    /// <summary>
    /// Represents the base enum class for all available custom abilities.
    /// </summary>
    public class UUCustomAbilityType : UniqueUnmanagedEnumClass<uint, UUCustomAbilityType>
    {
        /// <summary>
        /// Represents an invalid custom ability.
        /// </summary>
        public static readonly UUCustomAbilityType None = new();
    }
}
