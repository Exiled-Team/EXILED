// -----------------------------------------------------------------------
// <copyright file="UUModuleType.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomModules.API.Enums
{
    using Exiled.API.Features.Core.Generic;

    /// <summary>
    /// Enumerates the available module types.
    /// </summary>
    public class UUModuleType : UniqueUnmanagedEnumClass<uint, UUCustomRoleType>
    {
        /// <summary>
        /// Custom items module.
        /// </summary>
        public static readonly UUModuleType CustomItems = new();

        /// <summary>
        /// Custom abilities module.
        /// </summary>
        public static readonly UUModuleType CustomAbilities = new();

        /// <summary>
        /// Custom escapes module.
        /// </summary>
        public static readonly UUModuleType CustomEscapes = new();

        /// <summary>
        /// Custom roles module.
        /// </summary>
        public static readonly UUModuleType CustomRoles = new();

        /// <summary>
        /// Custom teams module.
        /// </summary>
        public static readonly UUModuleType CustomTeams = new();

        /// <summary>
        /// Custom game modes module.
        /// </summary>
        public static readonly UUModuleType CustomGameModes = new();
    }
}