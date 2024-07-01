// -----------------------------------------------------------------------
// <copyright file="FiringMode.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomModules.API.Enums
{
    /// <summary>
    /// All available firing modes.
    /// </summary>
    public enum FiringMode
    {
        /// <summary>
        /// Shooting is not allowed.
        /// </summary>
        None,

        /// <summary>
        /// Semi-automatic firing mode.
        /// </summary>
        SemiAutomatic,

        /// <summary>
        /// Burst firing mode.
        /// <para/>
        /// Only automatic firearms are supported.
        /// </summary>
        Burst,

        /// <summary>
        /// Automatic firing mode.
        /// <para/>
        /// Only automatic firearms are supported.
        /// </summary>
        Automatic,
    }
}