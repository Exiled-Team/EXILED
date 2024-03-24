// -----------------------------------------------------------------------
// <copyright file="UUCustomItemType.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomModules.API.Enums
{
    using Exiled.API.Features.Core.Generic;

    /// <summary>
    /// Represents the base enum class for all available custom items.
    /// </summary>
    public class UUCustomItemType : UniqueUnmanagedEnumClass<uint, UUCustomItemType>
    {
        /// <summary>
        /// Represents an invalid custom item.
        /// </summary>
        public static readonly UUCustomItemType None = new();
    }
}
