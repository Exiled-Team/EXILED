// -----------------------------------------------------------------------
// <copyright file="UUGameModeType.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomModules.API.Enums
{
    using Exiled.API.Features.Core.Generic;

    /// <summary>
    /// Represents the base enum class for all available custom teams.
    /// </summary>
    public class UUGameModeType : UniqueUnmanagedEnumClass<uint, UUGameModeType>
    {
        /// <summary>
        /// Represents an invalid custom game mode.
        /// </summary>
        public static readonly UUGameModeType None = new();
    }
}